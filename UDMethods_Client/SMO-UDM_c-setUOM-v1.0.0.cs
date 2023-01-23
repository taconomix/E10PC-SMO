/*============================================================================
	Method: SMO-UDM_c-setUOM-v1.0.0.cs

	Created: 10/27/2022
	Author:  Kevin Veldman
	Purpose: Loop thru config inputs, set visibility for L/R + in/cm
============================================================================*/

bool kCMs = Inputs.rUM.Value == "M";
string sLR = Inputs.rMeasLR.Value;


// Loop through Config inputs. Add all Decimal inputs to List

	var decList = new List<Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiNumericEditor, System.Decimal>> ();

	foreach (Control ctrl in Inputs.d5Value.Control.Parent.Controls) {

		if (ctrl is Ice.Lib.Framework.EpiNumericEditor) {

			decList.Add ((InputControlValueBound<Ice.Lib.Framework.EpiNumericEditor, System.Decimal>) Inputs[ctrl.Name.ToString ()].Value);
		}
	}

// Loop through Decimal list. Set visibility (and reset values) as specified. 

	foreach (Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiNumericEditor, System.Decimal> ctrl in decList) {

		if (sLR == "R") {

			if (ctrl.Name.ToString().Contains("L_")   ) ctrl.Invisible = true;
			if (ctrl.Name.ToString().Contains("R_Int")) ctrl.Invisible = kCMs;
			if (ctrl.Name.ToString().Contains("R_Dnm")) ctrl.Invisible = kCMs;
			if (ctrl.Name.ToString().Contains("R_Nmr")) ctrl.Invisible = kCMs;
			if (ctrl.Name.ToString().Contains("R_CM" )) ctrl.Invisible = !kCMs;

		} else {

			if (ctrl.Name.ToString().Contains("R_")   ) ctrl.Invisible = true;
			if (ctrl.Name.ToString().Contains("L_Int")) ctrl.Invisible = kCMs;
			if (ctrl.Name.ToString().Contains("L_Dnm")) ctrl.Invisible = kCMs;
			if (ctrl.Name.ToString().Contains("L_Nmr")) ctrl.Invisible = kCMs;
			if (ctrl.Name.ToString().Contains("L_CM" )) ctrl.Invisible = !kCMs;
		}

		if (kCMs) {

			if (ctrl.Name.ToString().Contains("_Int")) ctrl.Value = 0;
			if (ctrl.Name.ToString().Contains("_Nmr")) ctrl.Value = 0;
			if (ctrl.Name.ToString().Contains("_Dnm")) ctrl.Value = 0;

		} else {

			if (ctrl.Name.ToString().Contains("_CM")) ctrl.Value = 0;	
		}
	}


// Manually setting Label visibility. Loop throws error. See below.

	Inputs.lbl_d01.Invisible = kCMs;
	Inputs.lbl_d02.Invisible = kCMs;
	Inputs.lbl_d03.Invisible = kCMs;
	Inputs.lbl_d04.Invisible = kCMs;
	Inputs.lbl_d05.Invisible = kCMs;
	Inputs.lbl_d06.Invisible = kCMs;
	Inputs.lbl_d07.Invisible = kCMs;
	Inputs.lbl_d08.Invisible = kCMs;
	Inputs.lbl_d09.Invisible = kCMs;
	Inputs.lbl_d10.Invisible = kCMs;
	Inputs.lbl_d11.Invisible = kCMs;
	Inputs.lbl_d12.Invisible = kCMs;
	Inputs.lbl_d13.Invisible = kCMs;

	










/*============================================================================

	Change Log:

		User:  Date: Changes:

============================================================================*/

	


	/*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

	// Loop through Config inputs; Add EpiLabels to List
			var lblList = new List<Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiLabel, System.String>> ();

			foreach (Control ctrl in Inputs.lblSlash.Control.Parent.Controls) {

				if (ctrl is Ice.Lib.Framework.EpiLabel) {

					lblList.Add ((InputControlValueBound<Ice.Lib.Framework.EpiLabel, System.String>) Inputs[ctrl.Name.ToString ()].Value);
				}
			}

	// Loop through EpiLabel List; Set visibility of specified labels

		foreach (Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiLabel, System.String> ctrl in lblList) {

			if (ctrl.Name.ToString().Contains("lblSlash")) ctrl.Invisible = kCM;
		}

	@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*/

/*
## Error Detail ##
============
##!Message:##! Unable to cast object of type 'Erp.Shared.Lib.Configurator.InputControlValueBound`2[Ice.Lib.Framework.EpiLabel,System.Boolean]' to type 'Erp.Shared.Lib.Configurator.InputControlValueBound`2[Ice.Lib.Framework.EpiLabel,System.String]'.
##!Program:##! Erp.Lib.Configurator.dll
##!Method:##! TriggerInputEvent
*/