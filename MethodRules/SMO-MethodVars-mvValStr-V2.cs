/*== Methods Variable mvValStr v2 ===============================================
	
	Changed: 01/25/2023 -Kevin Veldman
	
	Info: Return values for Method Rules as '~'-separated String;
			- Allows single LINQ query for all Method Rules;
============================================================================*/

StringBuilder rtrn = new StringBuilder();

Action<string> plus = s => { rtrn.Append(s==""? "0": s).Append("~"); };

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
	plus(Inputs.bPlasticThick.Value);JobHead
	plus(Inputs.bPlasticType.Value);
	plus(Inputs.bStrapColor.Value);
	plus(Inputs.bInnerBootPN.Value);
	plus(Inputs.bChafeType.Value);
	plus(Inputs.cShoeSize.Value);
	plus(Inputs.kToeWalk.Value.ToString());

return rtrn.ToString();	



/*== CHANGE LOG ==============================================================

	01/23/2023: Changed Input loop to StringBuilder to reduce complexity;

============================================================================*/



//OLD WORKING

string clientString = string.Empty;
string serverString = string.Empty;

StringBuilder rtrn = new StringBuilder();

Action<string> plus = s => { rtrn.Append(s==""? "0": s).Append("~"); };

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

clientString = rtrn.ToString();	

if ( Inputs.blockMV.Value || Context.Entity == "PcStatus" ) {
	serverString = JobHead.UserMapData;
}

return ( serverString.Length > 0 ) ? serverString: clientString;


/*== CHANGE LOG ==============================================================

	01/23/2023: Changed Input loop to StringBuilder to reduce complexity;

============================================================================*/

