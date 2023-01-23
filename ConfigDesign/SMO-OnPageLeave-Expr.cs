/*============================================================================
	Method: SMO-Expr-OnPageLeave.cs

	Created: 08/03/2022
	Author:  Kevin Veldman
	Purpose: Check that all required fields are populated & valid, run 
	          calculations & send data to OrderDtl_UD fields.
============================================================================*/

if (UDMethods.exec_OnLeaveTest() == "") {

	UDMethods.exec_OnLeaveSuccess();
	UDMethods.ExportToLineDtl();

} else {

	MessageBox.Show(UDMethods.exec_OnLeaveTest());
	Args.Cancel = true;
}








/*============================================================================

	Change Log:

		User:  Date:     Changes:

============================================================================*/