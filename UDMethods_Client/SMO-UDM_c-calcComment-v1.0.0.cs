/*============================================================================
	Method: SMO-UDM_C-calcComment-v1.0.0.cs

	Created: 04/18/2018
	Author:  Kevin Veldman
	Purpose: Set dUnitPrice & eShowInfo to correct values - CLIENT ONLY
	WARNING: ## MUST MANUALLY KEEP IN SYNC WITH UDMethods.getPriceCom ##
============================================================================*/

// Arrays for Pricing and Descriptions
	string[] Price, Desc, Dmstc;
	Price = PCLookUp.DataRowList("PriceMaster",Inputs.bStyle.Value).Split('~');
	Desc  = PCLookUp.DataRowList("PriceMaster","Desc").Split('~');
	Dmstc = PCLookUp.DataColumnList("dmstcGrps","Groups").Split('~');

// Set Local Functions
	Func<decimal,string> toStr = (dInput) => {return Convert.ToString(dInput);};

	Func<string,string,string,string> strLkp = (sTbl,sCol,sRow) => {
		return PCLookUp.DataLookup(sTbl, sCol, sRow);
	};

	Func<string,string,string,decimal> decLkp = (sTbl,sCol,sRow) => {
		return Convert.ToDecimal(strLkp(sTbl, sCol, sRow));
	};

	Func<int,decimal> drPrc = (iColIndex) => {
		return Convert.ToDecimal(Price[iColIndex]);
	}; //Return price value from PricingAddOns based on Column References

// Set Local Variables
	string bStyle, sQty, custGrp, dscPrep, dscRush, rushRow, tTbl;
	bool kPc, kIntl, kBig, kWedge, kPrep, kRush, kNCRush, k0day;
	decimal dQty, prcRush, prcIntl, prcBase, prcUnit, prcPrep, qtyWdg;
	int custNum = Context.CustomerNumber;
	StringBuilder smo = new StringBuilder();

	bStyle  = Inputs.bStyle.Value;     //Device Type
	sQty    = Inputs.rQty.Value;       //Order Qty (string)
	dQty    = sQty=="B"? 2: 1; //Order Qty (decimal)

	kPc     = Context.Entity=="PcStatus";        //For Config Tests
	custGrp = kPc? "SSC": getCustGrp(custNum);   //Customer Group
	kIntl   = Array.IndexOf(Dmstc,custGrp) < 0;  //International order
	kBig    = lkpSpecs("SPEC") == "BIGSHOT";     //Bigshot style brace
	tTbl    = "IntlPricing";                     //Temp Table for Intl
	prcIntl = kIntl?decLkp(tTbl,custGrp,bStyle):0; //Int'l Price
	prcBase = drPrc(3) + drPrc(4);               //Device Price
	prcBase = prcIntl>0? prcIntl: prcBase;       //Base Price
	prcUnit = 0;                                 //Unit Price

	kWedge  = Inputs.kWedge.Value;                   // Heel Wedges
	qtyWdg  = Inputs.dWedgeQty.Value;                // Wedge Quantity
	kPrep   = Inputs.rFinish.Value=="P";             // Device Finish/Prep
	tTbl    = "Standards";                           // Tmp Table
	prcPrep = -1*decLkp(tTbl,"Value","Prep");          // Prep Discount
	dscPrep = strLkp(tTbl,"Desc","Prep");              // Prep Description
	kRush   = Inputs.kRush.Value;                    // Rush Order
	kNCRush = Inputs.kNCRush.Value;                  // Waive Rush Fee
	k0day   = Inputs.k0day.Value;                    // Same Day rush
	rushRow = k0day? "0RushSMO": "1RushSMO";         // Lookup Row for Rush
	prcRush = kNCRush? 0: decLkp(tTbl,"Value",rushRow);// Rush Fee
	dscRush = strLkp(tTbl,"Desc",rushRow);             // Rush Description
	
// Actions (functions) for redundant code snippets
	Action<int,bool> addLn0 = (iColumn,kInput) => {
		if (kInput)	{
			smo.Append(Desc[iColumn].PadRight(28)).Append(sQty=="B"?"2":"1");
			smo.Append(Price[iColumn].PadLeft(10));
			smo.Append(toStr(dQty*drPrc(iColumn)).PadLeft(11)).AppendLine();
			prcUnit += drPrc(iColumn);
		}}; // Use if only inclusion rule is Input checkbox, PartQty==OrderQty

	Action<string,bool,decimal,decimal,bool> addLn1 = 
	(sDesc,kInput,iQty,dPrice,kFixed) => {
		if (kInput)	{
			decimal trueQty = kFixed? iQty:(iQty*dQty);
			smo.Append(sDesc.PadRight(28)).Append(toStr(trueQty));
			smo.Append(toStr(dPrice).PadLeft(10));	
			smo.Append(toStr(trueQty*dPrice).PadLeft(11)).AppendLine();
			prcUnit += ((dPrice*iQty)/(kFixed? dQty: 1));
		}}; // Use if custom keep rules/quantities/prices are required.

// All appended descriptions, quanitites, and charges
	//Set Titles
		smo.Append("Description".PadRight(27)).Append("Qty");
		smo.Append("UnitPrice".PadLeft(10));
		smo.Append("ExtPrice".PadLeft(11)).AppendLine();

	// Function Calls for all Charges  
		addLn1(Price[0],true,1,prcBase,false);         //Base Device
		addLn0(11,Inputs.kToeWalk.Value);              //Toe Walker Mods
		addLn0(18,Inputs.kSpringPlate.Value);          //Spring Steel Soles
		addLn0(17,Inputs.kCarbonPlate.Value);          //Carbon Fiber Soles
		addLn0(21,Inputs.kNonSkid.Value);              //Non-Skid Soles
		addLn0(19,Inputs.kDorsalChip.Value);           //Dorsal Chips
		addLn0(10,Inputs.kInnerBoot.Value && !kBig);   //Inner Boot
		addLn0(12,Inputs.kLiner.Value);                //Integrated Liner
		addLn0(5,Inputs.kSTPad.Value);                 //ST Pads
		addLn0(6,Inputs.kMetPad.Value);                //Metatarsal Pads
		addLn0(31,Inputs.kPlateau.Value);              //Toe Plateaus
		addLn0(32,Inputs.kCutout.Value);               //Extra Cutouts
		addLn1(Desc[33],kWedge,qtyWdg,drPrc(33),true); //Heel Wedge
		addLn0(35,Inputs.kAppScanFee.Value);           //Scan Submission Fee
		addLn1(dscPrep,kPrep,1,prcPrep,false);         //Prep Discount
		addLn1(dscRush,kRush,1,prcRush,true);          //Rush Charge

	//Final Unit Price. 
		smo.AppendLine().Append("Total Price".PadRight(28)).Append(sQty=="B"?"2":"1");
		smo.Append(toStr(prcUnit).PadLeft(10));
		smo.Append(toStr(dQty * prcUnit).PadLeft(11));

	// Set Configurator inputs for Unit Price and SO Comment Lines
		Inputs.dUnitPrice.Value = prcUnit;
		Inputs.eShowInfo.Value = smo.ToString();




	/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ 
		PriceMaster Column reference
			
			0	Desc1
			1	HCPSC
			2	Code
			3	BASE
			4	STD ADD
			5	STPad
			6	MetPad
			7	tlsoGills
			8	tlsoGusset
			9	tlsoKIEU
			10	kInnerBootOpt
			11	kToeWalk
			12	kLinerOpt
			13	kTStrap
			14	kAntShellOpt
			15	kAntShellOpt2
			16	kDiabeticInsert
			17	kCarbonPlate
			18	kSpringPlate
			19	kDorsalChips
			20	kCarbonSupport
			21	kNonSkid
			22	kWalkingBase
			23	kNightStretch
			24	kQuickRelease
			25	kPowderCoat
			26	kUpgrade
			27	CarbonFiberShank
			28	RGOBucket
			29	HKAFOPelvicBand
			30	AFOFCLift
			31	kPlateau
			32	kBigShotHoles
			33	kWedge
			34	kExtraMods
			35	kAppScanFee 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/









/*============================================================================

	Change Log:

		User:  Date:       Changes:
		KV     08/02/2022  Update dQty for Bilateral/Left/Right

		KV     10/05/2022  Added to documentation

============================================================================*/

