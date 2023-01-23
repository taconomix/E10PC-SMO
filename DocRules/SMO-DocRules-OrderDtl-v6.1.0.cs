/*== OrderDtl-v6.1.0 - DocRule Action ========================================

	Created: 07/18/2022 -Kevin Veldman
	Changed: 01/19/2023 -KV

	Info: Clean/Set defaults for missing data; Get calculated values; Set Order Line fields;
	File: SMO-DocRules-OrderDtl-v6.1.0.cs
============================================================================*/

	string sKey = OrderDtl.cDeviceCode_c;

	UDMethods.drSetFinalData(sKey); // Clean up data and set calculated values

	Func<string,string,string,string> sLkp = (t,c,r) => PCLookUp.DataLookup(t,c,r);
	Func<string,string> mySpec = c => sLkp("DefaultSpecs", c, sKey);

	Func<string,decimal,bool> kStrDec = (s,d) => decimal.TryParse(s, out d); 
	Func<string,int,    bool> kStrInt = (s,i) => int.TryParse(s, out i); 

	Func<decimal,string> sDec = d => d.ToString(); // works with Int32
	Func<string,decimal> dStr = s => kStrDec(s,0)? Convert.ToDecimal(s): 0;
	Func<string,int    > iStr = s => kStrInt(s,0)? Convert.ToInt32(s)  : 0;
	Func<string,bool   > kStr = s => ( s == "1" || s.ToLower() == "true");

	int custNum = Context.CustomerNumber;
	string mod   = OrderDtl.ModType_c;
	decimal dQty = OrderDtl.cSBLR_c=="B"? 2: 1;
	bool kTOP = (custNum == 1110), kHPO = (custNum == 1007);
	bool kRush = OrderDtl.kRush0D_c || OrderDtl.kRush1D_c;
	string basePN = ( OrderDtl.BasePartNum!=""   ) ? OrderDtl.BasePartNum :
	                ( OrderDtl.PartNum=="webSMO" ) ? "webSMO" : "cfgSMO";

	string[] ProdDates = UDMethods.drGetShopDates(sKey);
	string[] PriceDtl  = UDMethods.drGetPriceDtl(sKey);

	bool kRmkNC = UDMethods.drRemakeNC(OrderDtl.kRemake_c, kTOP);
	var dUnitPrice = kRmkNC? 0.00m: dStr(PriceDtl[0]);
	var dDiscount  = kRmkNC? 0.00m: dStr(PriceDtl[2]);
	var dDiscRate  = kRmkNC? 0.00m: dStr(PriceDtl[3]);

	StringBuilder dsc = new StringBuilder();
	var sMod = " from " + (mod=="C"? "Cast": mod=="S"? "Scan": "Measurements");
	dsc.Append(mySpec("DESC"));
	dsc.Append(kTOP? sMod: ", Custom Molded");
	dsc.Append(OrderDtl.kPrepped_c? ", Prepped": "");


// Set Product Group, Price, Quantity, Rush Status, Discount, PartNum.
	OrderDtl.BasePartNum     = basePN;
	OrderDtl.BaseRevisionNum = OrderDtl.RevisionNum;
	OrderDtl.PartNum         = UDMethods.getNewPart(sKey);
	OrderDtl.LineDesc        = dsc.ToString();

	OrderDtl.SellingQuantity = dQty;
	Pricing.OrderPrice       = dUnitPrice;
	OrderDtl.OrderComment    = dUnitPrice > 0? PriceDtl[1]: "";
	
	OrderDtl.DocDiscount     = dDiscount * dQty;
	
	if (dDiscRate > 0) 
		OrderDtl.DiscountPercent = dDiscRate;

	OrderDtl.LockDisc = true;

	OrderDtl.MktgEvntSeq = kRush? 2: 1;
	OrderDtl.RequestDate = DateTime.Parse(ProdDates[0]); //Turnaround
	OrderDtl.NeedByDate  = DateTime.Parse(ProdDates[1]); //Promised

	OrderDtl.ProdCode = sLkp("Standards", "Value", mySpec("BRAND").ToUpper());


// Set Tariff Code for International Orders only
	string[] nonIntl = PCLookUp.DataColumnList("dmstcGrps","Groups").Split('~');
	string custGrp = ( kTOP || kHPO )? "INC" : UDMethods.getCustGrp(custNum);
	
	if (Array.IndexOf(nonIntl, custGrp) < 0) OrderDtl.cIntlCustomsCode_c = "90211090";


// Set OrderHed Dates
	if (kRush) OrderHed.NeedByDate  = OrderDtl.NeedByDate;
	if (kRush) OrderHed.RequestDate = OrderDtl.RequestDate;
	
	if(OrderHed.NeedByDate==null || OrderHed.NeedByDate < OrderDtl.NeedByDate)
		OrderHed.NeedByDate = OrderDtl.NeedByDate;

	if(OrderHed.RequestDate==null || OrderHed.RequestDate<OrderDtl.RequestDate)
		OrderHed.RequestDate = OrderDtl.RequestDate;

// Set OrderDate, review-hold when generated w/out using client configurator
	if (!Inputs.kClientRules.Value) OrderDtl.kPartTrap_c = true;



/*== CHANGE LOG =============================================================

	09/15/2022: Online order rules: hold for review + wait for cast/scan;
	09/26/2022: TOP orders to include ModType in LineDesc; update BaseRevisionNum;
	11/30/2022: Refactor w/ combined UDMethods reduce # of queries executed;
	12/06/2022: Unblock Method Rules with client input. Prevents early execution errors;
	12/22/2022: New function returning string[] for dates. 
	01/02/2023: Fix syntax errors, ProdCode lookup;
	01/09/2023: Remove query from drRemakeNC, Remove kRush condition from getShopDates;
	01/09/2023: Check Hanger/TOP before querying for CustGrp, do not empty RevisionNum;
	01/11/2023: Always set BasePart to "webSMO", replace prepDocRules w/ drSetFinalData;
	01/19/2023: Fix issue with no-addition braces getting default CustGroup level discount;

============================================================================*/