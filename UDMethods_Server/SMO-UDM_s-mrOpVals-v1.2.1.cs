/*== mrOpVals-v1.2.1 =========================================================
	
	Created: 01/09/2023 -Kevin Veldman
	Changed: 02/08/2023

	Info: Calculate and return OpCode + ProdStandard + OpDesc + OpDate
	File: SMO-UDM_s-mrOpVals-v1.2.1.cs
============================================================================*/

//-- Field Values Array Variable + Call Functions ---------------------------
	string[] od = TblValStr.Split('~');

	Func<string,decimal,bool> kStrDec = (s,d) => decimal.TryParse(s, out d); 
	Func<string,int,    bool> kStrInt = (s,i) => int.TryParse(s, out i); 

	Func<string,decimal> dStr = s => kStrDec(s,0)? Convert.ToDecimal(s): 0;
	Func<string,int    > iStr = s => kStrInt(s,0)? Convert.ToInt32(s)  : 0;
	Func<string,bool   > kStr = s => ( s == "1" || s.ToLower() == "true");
	
	Func<string,string,string> lsRow = (t,r) => PCLookUp.DataRowList(t,r);
	Func<string,string,string> lsCol = (t,c) => PCLookUp.DataColumnList(t,c);

	Func<int,bool   > kOD = i => kStr( od[i] );
	Func<int,decimal> dOD = i => dStr( od[i] );
	Func<int,int    > iOD = i => iStr( od[i] );
//---------------------------------------------------------------------------


//-- Lookup Values opCode, opDesc, prodStandard -----------------------------
	string strID = iOperSeq.ToString();

	string  returnCode = sLkp("OpsAFO"    , strID, od[0]);
	string  lookupDesc = sLkp("OpDescAFO" , strID, od[0]);
	decimal lookupTime = dLkp("OpTimesAFO", strID, od[0]);
//---------------------------------------------------------------------------


//-- OpDesc: Substring Replace ----------------------------------------------
	string str0 = "", str1 = "", returnDesc = lookupDesc;

	switch (iOperSeq) {
		case 750:
			str0 = "NONSKID SOLE";
			if ( kStr(od[6]) ) str1 = "CARBON FIBER FOOTPLATE";
			if ( kStr(od[7]) ) str1 = "SPRING STEEL FOOTPLATE";
			break;
		case 950:
			str0 = "FINISH";
			if ( kStr(od[4]) ) str1 = "REMOTE FINISH";
			break;
	}

	if ( str0!="" && str1!="" ) returnDesc = lookupDesc.Replace(str0,str1);
//---------------------------------------------------------------------------


//-- OpTime: Additional time by selection -----------------------------------
	string specTime, myShipTo, spcOpKey, spcOpVal;
	bool kBigShot, kHeelCut;
	decimal addTime = 0;

	kBigShot = sLkp("DefaultSpecs", "SPEC", od[0]) == "BIGSHOT";
	kHeelCut = kStr(od[3]);
	myShipTo = getShipTo(iStr(od[10]));
	spcOpKey = myShipTo.Length > 0? Context.CustomerID + "-" + myShipTo: "";
	spcOpVal = spcOpKey!=""? sLkp("ShipToSpecs","SMO",spcOpKey): "NONE";

	Func<int,string,string> SpecOps = (i,s) => 
		sLkp("SpecialOps",i.ToString(), spcOpKey + "-" + s);
	
	specTime = ( spcOpVal == "ALL"   )? SpecOps(iOperSeq,"SMOS"): "";
	specTime = ( spcOpVal == "STYLE" )? SpecOps(iOperSeq,od[0] ): specTime;
	addTime += ( specTime.Length > 0 )? dStr(specTime          ): 0;

	Func<string,int,decimal> aoTime = (s,i) => 
		dLkp("AddOnTimes", "Time", ( (i==0? "smo-"+strID+"-": "")+s ));

	switch (iOperSeq) {
		case 325:
			if ( dStr(od[8]) >= 3.25m ) addTime += aoTime("d5",0);
			break;
		case 600:
			if ( !kHeelCut && !kBigShot ) addTime +=aoTime("heelCut", 0);
			if ( kStr(od[9])            ) addTime +=aoTime("liner"  , 0);
			break;
		case 610:
			if ( kHeelCut ) addTime += aoTime("heelCut", 0);
			break;
		case 750:
			if ( kStr(od[7]) ) addTime += aoTime("ssPlate", 0);
			break;
		case 950:
			if ( kHeelCut && !kBigShot    ) addTime += aoTime("heelCut", 0);
			if ( kStr(od[2]) && !kBigShot ) addTime += aoTime("boot"   , 0);
			break;
	} 

	string returnTime = (lookupTime + addTime).ToString();
//---------------------------------------------------------------------------


//-- OpDate for Operations with calculated Due Date -------------------------
	string[] opsDue = lsCol("opsDue","opsDue").Split('~');
	string returnDate = string.Empty;

	if ( Array.IndexOf(opsDue, strID) >= 0 ) {

		var jh = Db.JobHead.Where(j => j.Company == "SS" && j.JobNum == Context.JobNumber).FirstOrDefault();

		DateTime opDate = ( jh == null )? DateTime.Today: Convert.ToDateTime(jh.CreateDate);
		if (Inputs.dtProdStart.Value != null) {
			if (Convert.ToDateTime(Inputs.dtProdStart.Value) > opDate) 
				opDate = Convert.ToDateTime(Inputs.dtProdStart.Value);
		}

		// Functions to Increment opDate
		Action<int> DateCheck = (iDays) => {

			string[] Holiday = lsCol("holidays","2023").Split('~');
			opDate = opDate.AddDays(iDays);

			for (int i = 0; i < Holiday.Length; i++) {
				if (Holiday[i] == opDate.ToString("yyyyMMdd")) opDate = opDate.AddDays(1);
			}

			if (opDate.DayOfWeek == DayOfWeek.Friday  ) opDate = opDate.AddDays(3);
			if (opDate.DayOfWeek == DayOfWeek.Saturday) opDate = opDate.AddDays(2);
			if (opDate.DayOfWeek == DayOfWeek.Sunday  ) opDate = opDate.AddDays(1);
		};

		Action<int> plusDays = (iDays) => {

			for (int i = 0; i < iDays; i++){
				DateCheck(1);
				DateCheck(0);
			}
		};


		// Increment opDate and set variables for each level
		DateTime dtPour, dtMod, dtBoot, dtPull, dtFin, dtShell;

		if ( !kStr(od[11]) && !kStr(od[12]) ) { //!kRush0D_c && !kRush1D_c

			if ( od[1]!="M" || dStr(od[8])>3.125m ) plusDays(1);
			dtPour = opDate;

			if ( od[1]!="M" || dStr(od[8])>3.125m ) plusDays(1);
			dtMod = opDate;

			if ( kStr(od[2]) ) plusDays(1);
			dtBoot = opDate;
			dtShell = opDate;
			
			plusDays(1);
			dtPull = opDate;

			plusDays(1);
			dtFin = opDate;

		} else {
			
			if ( !kStr(od[11]) ) plusDays(1); // !kRush0D_c
			dtPour  = opDate;
			dtMod   = opDate;
			dtBoot  = opDate;
			dtPull  = opDate;
			dtShell = opDate;
			dtFin   = opDate;
		}


		// Return date if iSeq is found in Array
		Func<int[],bool> setDays = a => Array.IndexOf(a, iOperSeq) != -1;
		DateTime dtReturn = opDate;

		int[] pour = { 200,300,310 };
		int[] mods = { 210,320,325 };
		int[] boot = { 400,405,450 };
		int[] bend = { 500 };
		int[] pull = { 600 };
		int[] shel = { 700,710,750 };
		int[] fin  = { 950 };

		if ( setDays(pour) ) { dtReturn = dtPour; }
		if ( setDays(mods) ) { dtReturn = dtMod;  }
		if ( setDays(boot) ) { dtReturn = dtBoot; }
		if ( setDays(bend) ) { dtReturn = opDate; }
		if ( setDays(pull) ) { dtReturn = dtPull; }
		if ( setDays(shel) ) { dtReturn = dtShell;}
		if ( setDays(fin)  ) { dtReturn = dtFin;  }

		returnDate = dtReturn.ToString("MM/dd/yyyy");
	}
//---------------------------------------------------------------------------


//-- Return Values ----------------------------------------------------------
	string[] returnVals = { returnCode, returnDesc, returnTime, returnDate };
	return returnVals;
//---------------------------------------------------------------------------




/*== CHANGE LOG =============================================================
	01/12/2023: Fix mixed variable names;
	01/19/2023: Minor change to Global Functions;
	02/08/2023: Fix row ID on some Add-On Times;
============================================================================*/



	/*== mr_UD_Fields Table Reference ============================

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

	============================================================*/