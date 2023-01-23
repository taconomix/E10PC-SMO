/*== setMRInputs-v1.0.1 ======================================================
	
	Created: 12/07/2022 -Kevin Veldman
	Changed: 12/08/2022 -KV
	
	File: SMO-UDM_c-setMRInputs-v1.0.1.cs
	Info: Set Radio/Combo input => TextBox values for Method Variable string
============================================================================*/

	Inputs.mrValStr.Value = mrGetInputString();


/*== CHANGE LOG =============================================================
	12/08/2022: Fix issue with combo box values set to null;
============================================================================*/


/*

Input Expression 
Input Name: bChafeType 
Function: Changed

Input Expression 
Input Name: bInnerBootPN 
Function: Changed

Input Expression 
Input Name: bPattern 
Function: Changed

Input Expression 
Input Name: bPlasticThick 
Function: Changed

Input Expression 
Input Name: bPlasticType 
Function: Changed

Input Expression 
Input Name: bStrapColor 
Function: Changed

Input Expression 
Input Name: kHeelCut 
Function: Changed

Input Expression 
Input Name: kInnerBoot 
Function: Changed

Input Expression 
Input Name: rCastMeas 
Function: Changed

Input Expression 
Input Name: rQty 
Function: Changed

User Defined Function 
Name: exec_bStyle 
Type: client

*/






Func<bool,int,decimal,decimal> toDec = (k,i,d) => k? i: d;




