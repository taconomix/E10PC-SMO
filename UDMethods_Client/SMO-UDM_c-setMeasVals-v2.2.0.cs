/*== setMeasVals-v2.2.0 ======================================================

	Created: 08/18/2022 -Kevin Veldman
	Changed: 01/13/2023 -Kevin Veldman

	Info: Set measurement values & shoe size for export.
	File: SMO-UDM_c-setMeasVals-v2.2.0.cs
============================================================================*/

// Function taking Measurement inputs as parameters, set related Value input
	Func<decimal,decimal,decimal,decimal,decimal> measVal = (dI,dN,dD,dC) => {
		
		bool kIN = Inputs.rUM.Value != "M";
		decimal dMod = dD==0? 0: dN/dD;

		return kIN? dI + dMod: Math.Round(dC * 0.3937m * 16) / 16;
	};


Inputs.d1Value.Value  = measVal(Inputs.d1L_Int.Value,  Inputs.d1L_Nmr.Value,  Inputs.d1L_Dnm.Value,  Inputs.d1L_CM.Value );
Inputs.d2Value.Value  = measVal(Inputs.d2L_Int.Value,  Inputs.d2L_Nmr.Value,  Inputs.d2L_Dnm.Value,  Inputs.d2L_CM.Value );
Inputs.d3Value.Value  = measVal(Inputs.d3L_Int.Value,  Inputs.d3L_Nmr.Value,  Inputs.d3L_Dnm.Value,  Inputs.d3L_CM.Value );
Inputs.d4Value.Value  = measVal(Inputs.d4L_Int.Value,  Inputs.d4L_Nmr.Value,  Inputs.d4L_Dnm.Value,  Inputs.d4L_CM.Value );
Inputs.d5Value.Value  = measVal(Inputs.d5L_Int.Value,  Inputs.d5L_Nmr.Value,  Inputs.d5L_Dnm.Value,  Inputs.d5L_CM.Value );
Inputs.d6Value.Value  = measVal(Inputs.d6L_Int.Value,  Inputs.d6L_Nmr.Value,  Inputs.d6L_Dnm.Value,  Inputs.d6L_CM.Value );
Inputs.d7Value.Value  = measVal(Inputs.d7L_Int.Value,  Inputs.d7L_Nmr.Value,  Inputs.d7L_Dnm.Value,  Inputs.d7L_CM.Value );
Inputs.d8Value.Value  = measVal(Inputs.d8L_Int.Value,  Inputs.d8L_Nmr.Value,  Inputs.d8L_Dnm.Value,  Inputs.d8L_CM.Value );
Inputs.d9Value.Value  = measVal(Inputs.d9L_Int.Value,  Inputs.d9L_Nmr.Value,  Inputs.d9L_Dnm.Value,  Inputs.d9L_CM.Value );
Inputs.d10Value.Value = measVal(Inputs.d10L_Int.Value, Inputs.d10L_Nmr.Value, Inputs.d10L_Dnm.Value, Inputs.d10L_CM.Value);
Inputs.d11Value.Value = measVal(Inputs.d11L_Int.Value, Inputs.d11L_Nmr.Value, Inputs.d11L_Dnm.Value, Inputs.d11L_CM.Value);
Inputs.d12Value.Value = measVal(Inputs.d12L_Int.Value, Inputs.d12L_Nmr.Value, Inputs.d12L_Dnm.Value, Inputs.d12L_CM.Value);
Inputs.d13Value.Value = measVal(Inputs.d13L_Int.Value, Inputs.d13L_Nmr.Value, Inputs.d13L_Dnm.Value, Inputs.d13L_CM.Value);

Inputs.d1rValue.Value  = measVal(Inputs.d1R_Int.Value,  Inputs.d1R_Nmr.Value,  Inputs.d1R_Dnm.Value,  Inputs.d1R_CM.Value);
Inputs.d2rValue.Value  = measVal(Inputs.d2R_Int.Value,  Inputs.d2R_Nmr.Value,  Inputs.d2R_Dnm.Value,  Inputs.d2R_CM.Value);
Inputs.d3rValue.Value  = measVal(Inputs.d3R_Int.Value,  Inputs.d3R_Nmr.Value,  Inputs.d3R_Dnm.Value,  Inputs.d3R_CM.Value);
Inputs.d4rValue.Value  = measVal(Inputs.d4R_Int.Value,  Inputs.d4R_Nmr.Value,  Inputs.d4R_Dnm.Value,  Inputs.d4R_CM.Value);
Inputs.d5rValue.Value  = measVal(Inputs.d5R_Int.Value,  Inputs.d5R_Nmr.Value,  Inputs.d5R_Dnm.Value,  Inputs.d5R_CM.Value);
Inputs.d6rValue.Value  = measVal(Inputs.d6R_Int.Value,  Inputs.d6R_Nmr.Value,  Inputs.d6R_Dnm.Value,  Inputs.d6R_CM.Value);
Inputs.d7rValue.Value  = measVal(Inputs.d7R_Int.Value,  Inputs.d7R_Nmr.Value,  Inputs.d7R_Dnm.Value,  Inputs.d7R_CM.Value);
Inputs.d8rValue.Value  = measVal(Inputs.d8R_Int.Value,  Inputs.d8R_Nmr.Value,  Inputs.d8R_Dnm.Value,  Inputs.d8R_CM.Value);
Inputs.d9rValue.Value  = measVal(Inputs.d9R_Int.Value,  Inputs.d9R_Nmr.Value,  Inputs.d9R_Dnm.Value,  Inputs.d9R_CM.Value);
Inputs.d10rValue.Value = measVal(Inputs.d10R_Int.Value, Inputs.d10R_Nmr.Value, Inputs.d10R_Dnm.Value, Inputs.d10R_CM.Value);
Inputs.d11rValue.Value = measVal(Inputs.d11R_Int.Value, Inputs.d11R_Nmr.Value, Inputs.d11R_Dnm.Value, Inputs.d11R_CM.Value);
Inputs.d12rValue.Value = measVal(Inputs.d12R_Int.Value, Inputs.d12R_Nmr.Value, Inputs.d12R_Dnm.Value, Inputs.d12R_CM.Value);
Inputs.d13rValue.Value = measVal(Inputs.d13R_Int.Value, Inputs.d13R_Nmr.Value, Inputs.d13R_Dnm.Value, Inputs.d13R_CM.Value);

Inputs.d5mr.Value = Inputs.d5Value.Value > 0? Inputs.d5Value.Value: Inputs.d5rValue.Value;
Inputs.d6mr.Value = Inputs.d6Value.Value > 0? Inputs.d6Value.Value: Inputs.d6rValue.Value;

mrSetInputString();

//Shoe Size
	decimal d6 = Inputs.d6mr.Value;
	string szShoe = (Math.Floor((d6-2.625m)/0.25m) + 3).ToString();
	Inputs.cShoeSize.Value = d6<2.75m || d6>5.25m? "CUST": szShoe;

if (Context.Entity == "PcStatus") {
	var decList = new List<Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiNumericEditor, System.Decimal>> ();
	foreach (Control ctrl in Inputs.d5Value.Control.Parent.Controls) {
		if (ctrl is Ice.Lib.Framework.EpiNumericEditor) {
			decList.Add ((InputControlValueBound<Ice.Lib.Framework.EpiNumericEditor, System.Decimal>) Inputs[ctrl.Name.ToString ()].Value);
		}
	}
	foreach (Erp.Shared.Lib.Configurator.InputControlValueBound<Ice.Lib.Framework.EpiNumericEditor, System.Decimal> ctrl in decList) {
		if (ctrl.Name.ToString().Contains("Value")) ctrl.Invisible = false;
	}
}


/*== CHANGE LOG ==============================================================
	10/16/2022: Added meas 4,7,8,9,10,11;
	10/19/2022: Added split left/right; Show Values fields in PcStatus;
	12/07/2022: Set d5mr and d6mr to handle early exectuion of MethodRules;
	01/10/2023: Add mrSetInputString() Method;
============================================================================*/