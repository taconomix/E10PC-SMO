/*== OrderHed-v1.1.0 =========================================================

	Created: 09/20/2022 -Kevin Veldman
	Changed: 09/21/2022 -KV

	Info: Set order to hold for online when criteria are met. 
	File: SMO-DocRules-OrderHed-v1.1.0.cs
============================================================================*/

	if (!Inputs.kClientRules.Value) {

		OrderHed.OrderDate = Convert.ToDateTime(DateTime.Today);

		if (OrderDtl.OrderNotes_c.Length > 0) OrderHed.OrderHeld = true;
		if (OrderDtl.kRemake_c              ) OrderHed.OrderHeld = true;
		if (OrderDtl.ModType_c != "M"       ) OrderHed.OrderHeld = true;
		
		if (OrderHed.OrderHeld) OrderHed.dtOnHold_c = OrderHed.OrderDate;
	}



/*== CHANGE LOG ==============================================================
	09/21/2022: Fixed bug: Hold was being overridden by next line;
============================================================================*/