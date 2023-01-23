/*== drRemakeNC-v2.2.0 =======================================================
	
	Created: 08/02/2022 -Kevin Veldman
	Changed: 

	Info: Return true when no-charge remakes rules are met.
	File: SMO-UDM_s-drRemakeNC-v2.2.0.cs
============================================================================*/

	// Param 0 (bool) = kRemake    = OrderDtl.kRemake_c;
	// Param 1 (bool) = kInternal  = Context.CustomerNumber == 1110
		// Include Hanger (1007) later?

bool returnVal = false;

if (kRemake) {
		
	string Col = kInternal ? "TOPCharge?" : "Charge?";
	string Row = Inputs.bRmkReasonActual.Value;
		
	if (sLkp("RemakeReasons", Col, Row) == "N") returnVal = true;
}

return returnVal;



/*== CHANGE LOG ==============================================================
	01/09/2023: Updated to add parameters and remove query;
============================================================================*/