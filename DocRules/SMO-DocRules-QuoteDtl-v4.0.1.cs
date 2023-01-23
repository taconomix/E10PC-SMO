/*== QuoteDtl-v4.0.1 - DOCRULE ACTION ============================================================

	Created: 01/02/2023 -Kevin Veldman
	Changed: 01/12/2023 -KV

	Info: Clean/Set defaults for missing data; Get calculated values; Set Quote Line fields;
	File: SMO-DocRules-QuoteDtl-v4.0.1.cs
================================================================================================*/

	string sKey = QuoteDtl.cDeviceCode_c;

	UDMethods.drSetFinalData(sKey); // Clean up data and set calculated values

// Variables 
	Func<string,string> mySpec = c => UDMethods.sLkp("DefaultSpecs", c, sKey);
	string mod = QuoteDtl.ModType_c, sBrand = mySpec("BRAND").ToUpper();
	decimal dQty = QuoteDtl.cSBLR_c=="B"? 2: 1;
	var isTop = Context.CustomerNumber == 1110;
	string basePN = ( OrderDtl.BasePartNum!=""   ) ? OrderDtl.BasePartNum :
	                ( OrderDtl.PartNum=="webSMO" ) ? "webSMO" : "cfgSMO";

	bool kRmkNC = UDMethods.drRemakeNC(QuoteDtl.kRemake_c, isTop);
	string[] PriceDtl = UDMethods.drGetPriceDtl(sKey);
	
	var dUnitPrice = kRmkNC? 0.00m: Convert.ToDecimal(PriceDtl[0]);
	var dDiscount  = kRmkNC? 0.00m: Convert.ToDecimal(PriceDtl[2]);
	var dDiscRate  = kRmkNC? 0.00m: Convert.ToDecimal(PriceDtl[3]);

	StringBuilder dsc = new StringBuilder();
	var dscTop = " from " +(mod=="C"?"Cast":(mod=="S"?"Scan":"Measurements"));
	dsc.Append(mySpec("DESC")).Append(isTop? dscTop: ", Custom Molded");
	dsc.Append(QuoteDtl.kPrepped_c? ", Prepped": "");


//Set Product Group, Price, Quantity, Rush Status.
	QuoteDtl.BasePartNum        = basePN;
	QuoteDtl.BaseRevisionNum    = QuoteDtl.RevisionNum;
	QuoteDtl.PartNum            = UDMethods.getNewPart(sKey);
	//QuoteDtl.RevisionNum      = "";
	Pricing.QuotePrice          = dUnitPrice;
	QuoteDtl.DocExpUnitPrice    = dUnitPrice;
	QuoteDtl.SellingExpectedQty = dQty;
	QuoteDtl.OrderQty           = dQty;
	QuoteDtl.LineDesc           = dsc.ToString();
	QuoteDtl.QuoteComment       = dUnitPrice > 0? PriceDtl[1]: ""; 
	QuoteDtl.ProdCode           = UDMethods.sLkp("Standards","Value",sBrand);

	if (dDiscount > 0) QuoteDtl.DocDiscount = dDiscount * dQty;
	if (dDiscRate > 0) QuoteDtl.DiscountPercent = dDiscRate;

// Set Tariff Code for International Quotes only
	string[] nonIntl = PCLookUp.DataColumnList("dmstcGrps","Groups").Split('~');
	string custGroup = isTop || Context.CustomerID=="HANGER"? "INC": UDMethods.getCustGrp(Context.CustomerNumber);

	if (Array.IndexOf(nonIntl, custGroup) < 0)
		QuoteDtl.cIntlCustomsCode_c = "90211090";

// Set OrderDate, review-hold when generated w/out using client configurator
	if (!Inputs.kClientRules.Value) QuoteDtl.kPartTrap_c = true;



/*== CHANGE LOG ==============================================================
	11/15/2022: Additional unit price and quantity fields set;
	01/02/2023: Copy and adapt from OrderDtl 5.2.2;
	01/09/2023: Remove query from drRemakeNC (OrderDtl v5.3.0);
============================================================================*/
