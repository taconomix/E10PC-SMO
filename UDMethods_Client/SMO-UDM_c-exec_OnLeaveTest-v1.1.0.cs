/*============================================================================
	Method: SMO-UDM_c-exec_OnLeaveTest-v1.1.0.cs

	Created: 08/03/2022
	Author:  Kevin Veldman
	Purpose: Validates all required Inputs. Triggers exec_OnLeaveSuccess when 
	           successful (return string.Empty). Also triggers ExportToLineDtl
	           when successful during OnPageLeave. 
============================================================================*/

setMeasVals();

string bStyle  = Inputs.bStyle.Value;
string sPick   = "Please Select ";
string sReturn = "";

if (bStyle == "") {

	sReturn = sPick + "Device Type";

} else if (Inputs.cPID.Value == ""){

	sReturn = sPick + "Patient ID or Last Name";

} else if (Inputs.rQty.Value == ""){

	sReturn = sPick + "Left, Right, or Bilateral";
	
} else if (Inputs.rFinish.Value == ""){

	sReturn = sPick + "Prepped or Finished";
	
} else if (Inputs.rCastMeas.Value == ""){

	sReturn = sPick + "from Cast or Measurements";

} else if (Inputs.bPattern.Value == "" && bStyle != "UCB") {

	sReturn = sPick + "Pattern";
	
} else if (Inputs.kRemake.Value) {

	string rmkSO  = "Original SO Number Invalid. Please Correct.";
	if (Inputs.bRmkReasonActual.Value=="") sReturn = sPick + "Remake Reason";
	if (Inputs.dRmkSO.Value < 200000 ) sReturn = rmkSO;
	if (Inputs.dRmkSO.Value > 9999999) sReturn = rmkSO;

} else if (Inputs.d6Value.Value > 0 && Inputs.d6Value.Value <= Inputs.d7Value.Value) {
	
	sReturn = String.Format(@"#6 Measurement must be longer than #7, please correct.");

} else if (Inputs.d6rValue.Value > 0 && Inputs.d6rValue.Value <= Inputs.d7rValue.Value) {
	
	sReturn = String.Format(@"#6 Measurement must be longer than #7, please correct.");
		
}

return sReturn;










/*============================================================================

	Change Log:

		User:  Date:       Changes:
		KV     10/04/2022  Added PID/Last Name validation.
		KV     11/23/2022  Add d6 > d7 Validation (left and right).
============================================================================*/