/*== drSetFinalData v3.2.0 =====================================================
	
	Created: 12/01/2022 -Kevin Veldman
	Changed: 01/23/2023 -KV
	
	File: SMO-UDM_s-drSetFinalData-v3.2.0.cs
	Info: Clean up Order Data + Get calculated values for Document Rules;
============================================================================*/

/*== Global Functions & Variables ==========================================*/

	Func<string,decimal,bool> kStrDec = (s,d) => decimal.TryParse(s, out d); 
	Func<string,int,    bool> kStrInt = (s,i) => int.TryParse(s, out i); 

	Func<decimal,string> sDec = d => d.ToString(); // works with Int32
	Func<string,decimal> dStr = s => kStrDec(s,0)? Convert.ToDecimal(s): 0;
	Func<string,int    > iStr = s => kStrInt(s,0)? Convert.ToInt32(s)  : 0;
	Func<string,bool   > kStr = s => ( s == "1" || s.ToLower() == "true");
	
	Func<string,string,string> lsRow = (t,r) => PCLookUp.DataRowList(t,r);
	Func<string,string,string> lsCol = (t,c) => PCLookUp.DataColumnList(t,c);
	
	Func<string,bool> kpVal      = s => ( s != "" && s.ToUpper() != "NONE" );
	Func<string,string> getSpec  = s => sLkp("DefaultSpecs", s, sKey);
	Func<string,int,string> left = (s,i) => s.Substring(0,Math.Min(i,s.Length));

	//__ LINQ Query: Get DB Fields from Order Line _______________________
	dynamic ld = 0;

	if (Context.Entity == "OrderDtl") {

		ld = Db.OrderDtl.Where(od => od.Company == Context.CompanyID
				&& od.OrderNum  == Context.OrderNumber
				&& od.OrderLine == Context.OrderLineNumber)
			.FirstOrDefault();

	} else if (Context.Entity == "QuoteDtl") {

		ld = Db.QuoteDtl.Where(qd => qd.Company == Context.CompanyID
				&& qd.QuoteNum  == Context.QuoteNumber 
				&& qd.QuoteLine == Context.QuoteLineNumber)
			.FirstOrDefault();
	}
	//____________________________________________________________________
/*==========================================================================*/


/*== Data Clean-up get/Set Functions =======================================*/

	Action<TimeSpan,TimeSpan> setLateRush = (nts,ots) => { 
		// Change to NextDay Rush if Later than 12PM 
		if ( ld.kRush0D_c && (TimeSpan.Compare(nts,ots) < 0) ) {
			ld.kRush0D_c = false;
			ld.kRush1D_c = true;
		}
	}; 

	Action setLeftMeasFields = () => {
		// Set Left-side meas if only Right has value
		for ( int i = 1; i <= 13; i++ ) {

			var sL = String.Format("dMeas{0}_c" , i);
			var sR = String.Format("dMeas{0}r_c", i);
			if ( ld[sL] == 0 && ld[sR] > 0 ) ld[sL] = ld[sR];
		}
	};

	Action setRequiredFields = () => {
		// Set Defaults if blank + non-Web bools

		if ( ld.cPlasticType_c == "" ) ld.cPlasticType_c = getSpec("PLASTICTYPE");
		if ( ld.cPlasticDuro_c == "" ) ld.cPlasticDuro_c = getSpec("PLASTICTHICK");
		if ( ld.cChafeType_c   == "" ) ld.cChafeType_c   = "DA";
		if ( ld.cStrapColor_c  == "" ) ld.cStrapColor_c  = "White";
		if ( ld.kHeelCut_c && !kpVal(ld.cBoot_c) ) ld.cBoot_c = "OPTEK116";

		ld.kBoot_c  = kpVal(ld.cBoot_c);
		ld.kLiner_c = kpVal(ld.cLinerMtl_c);
		ld.kWedge_c = ld.dWedgeQty_c > 0;
	};

	Func<string,string> getSSRSNotes = s => { 
		// Alter to preserve \r\n on SSRS forms
		return s.ToString().Replace((char)13, (char)10);
	};
/*==========================================================================*/


/*== Calculated Fields get/Set Functions ===================================*/

	Func<string,string,string> getPOLine = (sL,sF) => {
		// Get PO Line to Last, First
		return left(sL + (sF==""? "": ", "+sF), 20);
	};

	Func<string,string> getOformEmail = s => {
		// Get Email for O-Forms
		return sLkp("custEmail", "OForm", s);
	};

	Func<decimal,string> getShoeSize = d => {
		// Get Shoe Size from d6 Measurement

		decimal calcSize = Math.Floor((d-2.625m) * 4) + 3;
		return (d<2.75m || d>5.25m)? "CUST": calcSize.ToString();
	};
	
	//__ Build O-Form Notes ______________________________________________
	Func<string,string> getOformNotes = tbl => {

		StringBuilder ofNotes = new StringBuilder();
		string[]  ofnCol = lsCol(tbl,"Field"   ).Split('~');
		string[] ofnType = lsCol(tbl,"Type"    ).Split('~');
		string[] ofnBit0 = lsCol(tbl,"bitFalse").Split('~');
		string[] ofnBit1 = lsCol(tbl,"bitTrue" ).Split('~');
		string[] ofnTxt0 = lsCol(tbl,"Text0"   ).Split('~');
		string[] ofnTxt1 = lsCol(tbl,"Text1"   ).Split('~');
		string[] ofnTxt2 = lsCol(tbl,"Text2"   ).Split('~');
		string[] ofnTxt3 = lsCol(tbl,"Text3"   ).Split('~');
		string[]  ofnCfg = lsCol(tbl,"Config"  ).Split('~');
		string[]  okCfgs = { "ALL", "LE", "SMO" };

		Func<int,string> getNoteLine = i => {
			
			string thisType = ofnType[i].ToUpper();
			StringBuilder sTxt = new StringBuilder();
			Func<string,bool> kpStr = s => (s!="-" && s!="");

			//-- Handle nulls, Fields specific to other Configs
			if ( Array.IndexOf(okCfgs,ofnCfg[i])<0 || ld[ofnCol[i]]==null ) return "";

			if ( thisType == "BIT"    ) return ld[ofnCol[i]]? ofnBit1[i]: ofnBit0[i];
			if ( thisType == "LOOKUP" ) return sLkp("OFNLookup", ofnCol[i], ld[ofnCol[i]]);

			if ( thisType == "DECIMAL" && ld[ofnCol[i]] > 0 ) {
				if (ofnTxt0[i] != "-") sTxt.Append(ofnTxt0[i] == "(val)"? ld[ofnCol[i]]: ofnTxt0[i]);
				if (ofnTxt1[i] != "-") sTxt.Append(ofnTxt1[i] == "(val)"? ld[ofnCol[i]]: ofnTxt1[i]);
				if (ofnTxt2[i] != "-") sTxt.Append(ofnTxt2[i] == "(val)"? ld[ofnCol[i]]: ofnTxt2[i]);
				if (ofnTxt3[i] != "-") sTxt.Append(ofnTxt3[i] == "(val)"? ld[ofnCol[i]]: ofnTxt3[i]);
			
			} else if ( thisType == "VALUE" && kpStr(ld[ofnCol[i]]) ) {
				if (kpStr(ofnTxt0[i])) sTxt.Append(ofnTxt0[i] == "(val)"? ld[ofnCol[i]]: ofnTxt0[i]);
				if (kpStr(ofnTxt1[i])) sTxt.Append(ofnTxt1[i] == "(val)"? ld[ofnCol[i]]: ofnTxt1[i]);
				if (kpStr(ofnTxt2[i])) sTxt.Append(ofnTxt2[i] == "(val)"? ld[ofnCol[i]]: ofnTxt2[i]);
				if (kpStr(ofnTxt3[i])) sTxt.Append(ofnTxt3[i] == "(val)"? ld[ofnCol[i]]: ofnTxt3[i]);
			}
			return sTxt.ToString();
		};

		for ( int i = 0; i < ofnCol.Length; i++ ) {
			if ( getNoteLine(i).Length > 1 ) 
				ofNotes.AppendLine().Append("> " + getNoteLine(i)).AppendLine();
		}

		return ofNotes.ToString();
	};
	//____________________________________________________________________


	//__ Lookup Mold IDs from SSML Table _________________________________
	Func<decimal,string[]> getMoldIDs = dTest => {

		//-- Return array of empty strings if out of range ---------
		if ( dTest<1.5m && dTest>3 ) return ( new string[3] {"", "", ""} );
		
		//-- Set arrays for Measurements, Ranges, return -----------
		string id0="", id1="", id2="";

		// Set variables for Measurements & Match ranges. 
		decimal d5 = ld.dMeas5_c, dMin5=(d5 - 0.0625m), dMax5=(d5 + 0.1250m);
		decimal d6 = ld.dMeas6_c, dMin6=(d6 - 0.1250m), dMax6=(d6 + 0.2500m);
		decimal d1 = ld.dMeas1_c, dMin1=(d1 - 0.1250m), dMax1=(d1 + 0.2500m);
		decimal d2 = ld.dMeas2_c, dMin2=(d2 - 0.1250m), dMax2=(d2 + 0.2500m);
		decimal d3 = ld.dMeas3_c, dMin3=(d3 - 0.1250m), dMax3=(d3 + 0.2500m);
		decimal d4 = ld.dMeas4_c, dMin4=(d4 - 0.2500m), dMax4=(d4 + 0.2500m);


		//-- Query Config Lookup Table with LINQ -------------------
		var SSML = (
			from ID  in Db.PcLookupTblValues
			join ms5 in Db.PcLookupTblValues on 
				new { ID.LookupTblID,  ID.RowNum,  ID.Company} equals 
				new {ms5.LookupTblID, ms5.RowNum, ms5.Company}
			join ms6 in Db.PcLookupTblValues on 
				new {ms5.LookupTblID, ms5.RowNum, ms5.Company} equals 
				new {ms6.LookupTblID, ms6.RowNum, ms6.Company}
			join ms1 in Db.PcLookupTblValues on 
				new {ms6.LookupTblID, ms6.RowNum, ms6.Company} equals
				new {ms1.LookupTblID, ms1.RowNum, ms1.Company}
			join ms2 in Db.PcLookupTblValues on 
				new {ms1.LookupTblID, ms1.RowNum, ms1.Company} equals
				new {ms2.LookupTblID, ms2.RowNum, ms2.Company}
			join ms3 in Db.PcLookupTblValues on 
				new {ms2.LookupTblID, ms2.RowNum, ms2.Company} equals
				new {ms3.LookupTblID, ms3.RowNum, ms3.Company}
			join ms4 in Db.PcLookupTblValues on 
				new {ms3.LookupTblID, ms3.RowNum, ms3.Company} equals
				new {ms4.LookupTblID, ms4.RowNum, ms4.Company}
			where 
				   ID.ColName  == "MoldID" 
				&& ms4.ColName == "M4" 
				&& ID.LookupTblID == "SSML"
				&& ms5.DataValueDecimal >= dMin5
				&& ms5.DataValueDecimal <= dMax5
				&& ms5.ColName == "M5" 
				&& ms6.DataValueDecimal >= dMin6
				&& ms6.DataValueDecimal <= dMax6
				&& ms6.ColName == "M6" 
				&& ms1.DataValueDecimal >= dMin1
				&& ms1.DataValueDecimal <= dMax1
				&& ms1.ColName == "M1" 
				&& ms2.DataValueDecimal >= dMin2
				&& ms2.DataValueDecimal <= dMax2
				&& ms2.ColName == "M2" 
				&& ms3.DataValueDecimal >= dMin3
				&& ms3.DataValueDecimal <= dMax3
				&& ms3.ColName == "M3" 
				&& ms4.DataValueDecimal >= dMin4
				&& ms4.ColName == "M4"
			select new 
				{ValidID = ID.DataValueString 
				,Score = ((ms5.DataValueDecimal == d5? 65: 40)
						+ (ms6.DataValueDecimal == d6? 25: 15)
						+ (ms1.DataValueDecimal == d1? 25: 15)
						+ (ms2.DataValueDecimal == d2? 25: 15)
						+ (ms3.DataValueDecimal == d3? 25: 15)
						+ (ms4.DataValueDecimal == d4? 06: 0))
				,Row = ID.RowNum } );  

		//-- Create sorted list, set return elements ---------------
		if (SSML.Count() > 0) {
			
			var x = SSML.OrderByDescending(ml => ml.Score).Take(3).ToList();

			if (x.Count() > 0) id0 = x[0].ValidID;
			if (x.Count() > 1) id1 = x[1].ValidID;
			if (x.Count() > 2) id2 = x[2].ValidID;
		}

		//-- Return Mold IDs as String Array -----------------------
		string[] returnVals = { id0, id1, id2 };
		return returnVals;
	};
	//____________________________________________________________________
/*==========================================================================*/


/*== Execute all Functions, set all Fields =================================*/

	//__ Clean-up Data ___________________________________________________
		setLateRush(new TimeSpan(12,0,0), DateTime.Now.TimeOfDay);

		setLeftMeasFields();
		setRequiredFields();

		ld.cInternalNotes_c = getSSRSNotes(ld.cInternalNotes_c);
		ld.OrderNotes_c     = getSSRSNotes(ld.OrderNotes_c);
	//____________________________________________________________________

	//__ Set Calculated Fields ___________________________________________
		ld.POLine       = getPOLine(ld.LastNamePID_c, ld.FirstName_c);
		ld.EmailOForm_c = getOformEmail(getShipTo(ld.OrderNum));
		ld.cShoeSize_c  = getShoeSize(ld.dMeas6_c);
		ld.cFormNotes_c = getOformNotes("OFormText");

		string[] moldIDs = getMoldIDs(ld.dMeas5_c);
		ld.cMold1_c = moldIDs[0];
		ld.cMold2_c = moldIDs[1];
		ld.cMold3_c = moldIDs[2];
	//____________________________________________________________________

/*==========================================================================*/




/*== CHANGE LOG ==============================================================

	11/30/2022: Combined UDMethods drFixData, drSetOFormNotes, drSSML;
	01/10/2023: Add new functions for setting Method Variable string input;
	01/11/2023: Move all calculations to local functions;
	01/11/2023: Renamed from UDMethods.prepDocRules;
	01/13/2023: Replace Decimal[] Arrays in getMoldIDs Function; 
											LINQ can't use array indexes;
	01/13/2023: Move SSML variables to single line for clarity;
	01/23/2023: Remove getMethodVar Function, Now in Method Variables;

============================================================================*/






	/*__ Get Value String for Method Rules _______________________________
	Func<string,string> getMethodVar = tbl => {

		string[]  udCol = lsCol(tbl,"FIELD").Split('~');
		string[] udType = lsCol(tbl,"TYPE" ).Split('~');
		StringBuilder ldVals = new StringBuilder();

		for ( int i = 0; i < udCol.Length; i++ ) {
			
			string tmpVal = string.Empty;

			if ( udType[i]=="str" ) tmpVal = ( ld[udCol[i]] ).ToString();
			if ( udType[i]=="bit" ) tmpVal = ( ld[udCol[i]] ).ToString();
			if ( udType[i]=="int" ) tmpVal = ( ld[udCol[i]] ).ToString();
			if ( udType[i]=="dec" ) tmpVal = ( ld[udCol[i]] ).ToString();

			ldVals.Append(tmpVal.Length > 0? tmpVal : "0").Append("~");
		}
		return ldVals.ToString();
	};
		/*-- mrFieldsSMO Table Reference -------------------------- 

			0   str  cDeviceCode_c
			1   str  ModType_c
			2   bit  kBoot_c
			3   bit  kHeelCut_c
			4   bit  kPrepped_c
			5   bit  kNonSkid_c
			6   bit  kCFplate_c
			7   bit  kSSplate_c
			8   dec  dMeas5_c
			9   bit  kLiner_c
			10  int  OrderNum
			11  bit  kRush0D_c
			12  bit  kRush1D_c
			13  str  cPattern_c
			14  bit  kTallEars_c
			15  bit  kDorsalChip_c
			16  bit  kMetPads_c
			17  bit  kSTPads_c
			18  str  cSBLR_c
			19  bit  kPlateau_c
			20  dec  dMeas6_c
			21  str  cPlasticDuro_c
			22  str  cPlasticType_c
			23  str  cStrapColor_c
			24  str  cBoot_c
			25  str  cChafeType_c
			26  str  cShoeSize_c
			27  bit  kToeWalk_c
			
			Sample:
			"SMO~M~0~0~0~0~0~0~2.5~0~1491481~0~0~PAT-AFO-BASEBALL~
				0~1~0~0~B~0~0.00~116~SSW~White~OPTEK332~DA~CUST~0~"
		----------------------------------------------------------*/

	//	Inputs.mrValStr.Value = getMethodVar("mrFieldsSMO");
	//	Inputs.kBlockMethodRules.Value = false;

	//____________________________________________________________________







	/*__ Original SSML Declared Variables ___________________________
		// Set variables for Measurements & Match ranges. 
		decimal d5 = ld.dMeas5_c;
		decimal d6 = ld.dMeas6_c;
		decimal d1 = ld.dMeas1_c;
		decimal d2 = ld.dMeas2_c;
		decimal d3 = ld.dMeas3_c;
		decimal d4 = ld.dMeas4_c;

		decimal dMin5 = d5 - 0.0625m, dMax5 = d5 + 0.1250m;
		decimal dMin6 = d6 - 0.1250m, dMax6 = d6 + 0.2500m;
		decimal dMin1 = d1 - 0.1250m, dMax1 = d1 + 0.2500m;
		decimal dMin2 = d2 - 0.1250m, dMax2 = d2 + 0.2500m;
		decimal dMin3 = d3 - 0.1250m, dMax3 = d3 + 0.2500m;
		decimal dMin4 = d4 - 0.2500m, dMax4 = d4 + 0.2500m;
	_______________________________________________________________*/