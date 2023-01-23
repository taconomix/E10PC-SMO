/*== Methods Variable mvValStr ===============================================
	
	Changed: 01/23/2023 -Kevin Veldman
	
	Info: Return values for Method Rules as '~'-separated String;
			- Allows single LINQ query for all Method Rules;
============================================================================*/

if ( Inputs.blockMV.Value || Context.Entity == "PcStatus" ) {

	/*__ Get values for Inputs below ____________________________________*/

	StringBuilder rtrn = new StringBuilder();

	Action<string> plus = s => { 
		rtrn.Append(s==""? "0": s).Append("~"); 
	};

	plus(Inputs.bStyle.Value);
	plus(Inputs.rCastMeas.Value);
	plus(Inputs.kInnerBoot.Value.ToString());
	plus(Inputs.kHeelCut.Value.ToString());
	plus((Inputs.rFinish.Value=="F").ToString());
	plus(Inputs.kNonSkid.Value.ToString());
	plus(Inputs.kCarbonPlate.Value.ToString());
	plus(Inputs.kSpringPlate.Value.ToString());
	plus(Inputs.d5Value.Value.ToString());
	plus(Inputs.kLiner.Value.ToString());
	plus(Inputs.dOrderNum.Value.ToString());
	plus(Inputs.k0day.Value.ToString());
	plus(Inputs.kRush.Value.ToString());
	plus(Inputs.bPattern.Value);
	plus(Inputs.kTallEars.Value.ToString());
	plus(Inputs.kDorsalChip.Value.ToString());
	plus(Inputs.kMetPad.Value.ToString());
	plus(Inputs.kSTPad.Value.ToString());
	plus(Inputs.rQty.Value);
	plus(Inputs.kPlateau.Value.ToString());
	plus(Inputs.d6Value.Value.ToString());
	plus(Inputs.bPlasticThick.Value);
	plus(Inputs.bPlasticType.Value);
	plus(Inputs.bStrapColor.Value);
	plus(Inputs.bInnerBootPN.Value);
	plus(Inputs.bChafeType.Value);
	plus(Inputs.cShoeSize.Value);
	plus(Inputs.kToeWalk.Value.ToString());

	return rtrn.ToString();


} else {

	/*__ Get values for fields listed in mrFieldsSMO table _______________*/

	string Table = "mrFieldsSMO";

	var ld = Db.OrderDtl.Where(od => od.Company == "SS"
			&& od.OrderNum  == Context.OrderNumber
			&& od.OrderLine == Context.OrderLineNumber)
		.FirstOrDefault();

	Func<string,string,string> lsCol = (t,c) => PCLookUp.DataColumnList(t,c);

	string[]  udCol = lsCol( Table, "FIELD" ).Split('~');
	string[] udType = lsCol( Table, "TYPE"  ).Split('~');
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
}



/*== CHANGE LOG ==============================================================

	01/23/2023: Changed Input loop to StringBuilder to reduce complexity;

============================================================================*/