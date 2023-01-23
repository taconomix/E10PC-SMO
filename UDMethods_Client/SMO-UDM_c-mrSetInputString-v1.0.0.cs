/*== mrSetInputString-v1.0.0 =================================================
	
	Created: 01/11/2023 -Kevin Veldman
	Changed:
	
	File: SMO-UDM_c-mrSetInputString-v1.0.0.cs
	Info: Calculate, Set Method Variable Input as "~"-separated string
	
	!!! WARNING DO NOT REORDER ROWS IN mrFieldsSMO LOOKUP TABLE !!! 
			-rows may be *added* if necessary
============================================================================*/

//-- Set values for unselected Combo Boxes ---------------------------------
	Func<string,string> handleEmptyCombo = s => (s.Length > 0)? s: "NONE";

	Inputs.cDeviceCode.Value  = handleEmptyCombo(Inputs.bStyle.Value);
	Inputs.cModType.Value     = handleEmptyCombo(Inputs.rCastMeas.Value);
	Inputs.cPattern.Value     = handleEmptyCombo(Inputs.bPattern.Value);
	Inputs.cSBLR.Value        = handleEmptyCombo(Inputs.rQty.Value);
	Inputs.cPlasticDuro.Value = handleEmptyCombo(Inputs.bPlasticThick.Value);
	Inputs.cPlasticType.Value = handleEmptyCombo(Inputs.bPlasticType.Value);
	Inputs.cStrapColor.Value  = handleEmptyCombo(Inputs.bStrapColor.Value);
	Inputs.cBoot.Value        = handleEmptyCombo(Inputs.bInnerBootPN.Value);
	Inputs.cChafeType.Value   = handleEmptyCombo(Inputs.bChafeType.Value);

	Inputs.kPrepped.Value     = Inputs.rFinish.Value == "P";
//--------------------------------------------------------------------------



//-- Create Input Control Lists --------------------------------------------
	var decList = new List<Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiNumericEditor, System.Decimal>> ();
	var chkList = new List<Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiCheckBox,      System.Boolean>> ();
	var chrList = new List<Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiTextBox,       System.String>>  ();

	foreach (Control ctrl in Inputs.d5mr.Control.Parent.Controls) {

		if (ctrl is Ice.Lib.Framework.EpiNumericEditor) decList.Add ((InputControlValueBound<Ice.Lib.Framework.EpiNumericEditor, System.Decimal>) Inputs[ctrl.Name.ToString ()].Value);
		if (ctrl is Ice.Lib.Framework.EpiCheckBox     ) chkList.Add ((InputControlValueBound<Ice.Lib.Framework.EpiCheckBox,      System.Boolean>) Inputs[ctrl.Name.ToString ()].Value);
		if (ctrl is Ice.Lib.Framework.EpiTextBox      ) chrList.Add ((InputControlValueBound<Ice.Lib.Framework.EpiTextBox,       System.String> ) Inputs[ctrl.Name.ToString ()].Value);
	}
//--------------------------------------------------------------------------



//-- Build Input Array as '~' Separated String -----------------------------
	StringBuilder ldVals = new StringBuilder();

	string[]  pciCol = PCLookUp.DataColumnList("mrFieldsSMO","INPUT").Split('~');
	string[] pciType = PCLookUp.DataColumnList("mrFieldsSMO","TYPE" ).Split('~');

	for ( int i = 0; i < pciCol.Length; i++ ) {

		string tmpLine = string.Empty;

		if ( pciType[i] == "str" ) {

			foreach ( Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiTextBox, System.String> ctrl in chrList ) {
				if (ctrl.Name.ToString() == pciCol[i]) tmpLine = ( ctrl.Value != "" ) ? ctrl.Value : "NONE";
			}
		} else if ( pciType[i] == "bit" ) {
			
			foreach ( Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiCheckBox, System.Boolean> ctrl in chkList ) {
				if (ctrl.Name.ToString() == pciCol[i]) tmpLine = ( ctrl.Value ) ? "1" : "0";
			}
		} else if ( pciType[i] == "dec" || pciType[i] == "int" ) {

			foreach ( Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiNumericEditor, System.Decimal> ctrl in decList ) {
			    if (ctrl.Name.ToString() == pciCol[i]) tmpLine = ctrl.Value.ToString();
			}
		}

		ldVals.Append(tmpLine.Length > 0? tmpLine : "0").Append("~");
	}
//--------------------------------------------------------------------------



//-- Set Method Variable input to Constructed String------------------------
	Inputs.mrValStr.Value = ldVals.ToString();
//--------------------------------------------------------------------------



/*== CHANGE LOG ==============================================================
	12/07/2022: Client-side version of mrGetValueString;
	01/10/2023: Handle Combo Boxes here rather than separate Method;
============================================================================*/


	/*>> mrFieldsSMO Table Reference >>>>>>>>>>>>>>>>>>>>>>>>>>

		0   str  Inputs.cDeviceCode
		1   str  Inputs.cModType
		2   bit  Inputs.kInnerBoot
		3   bit  Inputs.kHeelCut
		4   bit  Inputs.kPrepped
		5   bit  Inputs.kNonSkid
		6   bit  Inputs.kCarbonPlate
		7   bit  Inputs.kSpringPlate
		8   dec  Inputs.d5mr
		9   bit  Inputs.kLiner
		10  int  Inputs.dOrderNum
		11  bit  Inputs.k0day
		12  bit  Inputs.kRush
		13  str  Inputs.cPattern
		14  bit  Inputs.kTallEars
		15  bit  Inputs.kDorsalChip
		16  bit  Inputs.kMetPad
		17  bit  Inputs.kSTPad
		18  str  Inputs.cSBLR
		19  bit  Inputs.kPlateau
		20  dec  Inputs.d6mr
		21  str  Inputs.cPlasticDuro
		22  str  Inputs.cPlasticType
		23  str  Inputs.cStrapColor
		24  str  Inputs.cBoot
		25  str  Inputs.cChafeType
		26  str  Inputs.cShoeSize
		27  bit  Inputs.kToeWalk

		
		Sample:
			"SMO~M~0~0~0~0~0~0~2.5~0~1491481~0~0~PAT-AFO-BASEBALL~0~1~0~0~B~0~0.00~116~SSW~White~OPTEK332~DA~CUST~0~"
	<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<*/

