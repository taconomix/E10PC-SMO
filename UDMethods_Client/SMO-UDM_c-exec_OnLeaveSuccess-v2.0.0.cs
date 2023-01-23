/*== exec_OnLeaveSuccess v2.0.0 ==============================================

	Created: 08/03/2022 -Kevin Veldman
	Changed: 01/19/2023 -KV

	File: SMO-UDM_c-exec_OnLeaveSuccess-v2.0.0.cs
	Info: Calculate measurements, call SSML, set kClientRules onPageLeave
============================================================================*/

// Set measurement Value fields
	setMeasVals(); 

// Call SSML if there is a value for Met-Head M/L
	if ( Inputs.d5Value.Value == 0 ) Inputs.d5Value.Value = Inputs.d5rValue.Value;
	if ( Inputs.d5Value.Value <= 3 && Inputs.d5Value.Value >= 1 ) callSSML();

// Set client pricing fields in test status
	if ( Context.Entity == "PcStatus" ) calcPriceDtl(Inputs.bStyle.Value);
	if ( Context.Entity == "PcStatus" ) setTurnDate();

// Adjust pricing to 0, show warning for rush on NC remake. 
	if ( Inputs.kRemake.Value ) {
		
		string Col, Row, Msg;
		Col = (Context.CustomerNumber==1110? "TOP": "") + "Charge?";
		Row = Inputs.bRmkReasonActual.Value;
		Msg = "Rush added to No Charge Remake. Please manually add Rush Charge.";
		
		if (sLkp("RemakeReasons", Col, Row) == "N") {

			Inputs.dUnitPrice.Value = 0.00m;
			Inputs.dDiscount.Value  = 0.00m; 
			Inputs.dDiscRate.Value  = 0.00m;

			if (Inputs.kRush.Value) MessageBox.Show(Msg);
		}
	}

// Order was created or updated in Epicor Config client
	Inputs.kClientRules.Value = true;



/*== CHANGE LOG ==============================================================
	09/15/2022: Move kClientRules=true from exec_bStyle;
	12/01/2022: Removed calcComment+calcDiscount. Redundant to Doc Rules;
	01/19/2023: Add back calcPriceDtl for testing purposed;
============================================================================*/