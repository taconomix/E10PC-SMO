/*== setPrepLock-v1.1.0 ======================================================

	Created: 07/18/2022 -Kevin Veldman
	Changed: 12/07/2022 -Kevin Veldman

	Info: Lockout Prepped for SS brand for most customers, set kPrepped
	File: SMO-UDM_c-setPrepLock-v1.1.0.cs
============================================================================*/

string[] pIDs = PCLookUp.DataColumnList("PrepAllowCust", "CustID").Split('~');

if ( Array.IndexOf(pIDs, Context.CustomerID) < 0 ) {
	bool kCF = sLkp("DefaultSpecs","BRAND",Inputs.bStyle.Value)=="CFab";
	if (!kCF && Context.Entity!="PcStatus") Inputs.rFinish.Value = "F";
}

Inputs.kPrepped.Value = (Inputs.rFinish.Value == "P");
Inputs.mrValStr.Value = mrGetInputString();



/*== CHANGE LOG ==============================================================
	12/07/2022: Set kPrepped bit field for global Method Variable;
============================================================================*/