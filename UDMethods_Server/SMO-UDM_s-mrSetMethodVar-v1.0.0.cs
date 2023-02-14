/*== mrSetMethodVar v1.0.0 ===================================================

	Created: 01/12/2023 -Kevin Veldman
	Changed: 01/30/2023

	Info: Build string for Method Variable from fields in mrFieldsSMO Table.
	File: SMO-UDM_s-mrSetMethodVar-v1.0.0.cs
============================================================================*/

	var ld = (from x in Db.OrderDtl
				join y in Db.JobProd on 
					new {x.Company, x.OrderNum, x.OrderLine} equals 
					new {y.Company, y.OrderNum, y.OrderLine}
				join z in Db.JobHead on
					new {y.Company, y.JobNum} equals
					new {z.Company, z.JobNum}	
				where 
					z.JobNum == JobNum
				select x).FirstOrDefault();


	Func<string,string,string> lsCol = (t,c) => PCLookUp.DataColumnList(t,c);

	string Table = "mrFieldsSMO";

	string[]  udCol = lsCol( Table, "FIELD" ).Split('~');
	string[] udType = lsCol( Table, "TYPE"  ).Split('~');
	StringBuilder ldVals = new StringBuilder();

	for ( int i = 0; i < udCol.Length; i++ ) {
			
		string tmpVal = string.Empty;

		if ( udType[i]=="str" ) tmpVal = ( ld[udCol[i]] ).ToString();
		if ( udType[i]=="bit" ) tmpVal = ( ld[udCol[i]] ).ToString();
		if ( udType[i]=="int" ) tmpVal = ( ld[udCol[i]] ).ToString();
		if ( udType[i]=="dec" ) tmpVal = ( ld[udCol[i]] ).ToString();

		ldVals.Append(( tmpVal.Length > 0 ) ? tmpVal : "0");
		ldVals.Append(( i + 1 < udCol.Length ) ? "~": "");
	}

	return ldVals.ToString();


/*== CHANGE LOG ==============================================================
	
	01/30/2023: Change for use in Operation Rule Action

============================================================================*/



	/*__ mrFieldsSMO Table Reference _______________________________

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
	______________________________________________________________*/