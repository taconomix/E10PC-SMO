/*== getNewPart v1.1.0 =======================================================

	Created: 09/06/2022 -Kevin Veldman
	Changed: 

	File: SMO-UDM_s-getNewPart-v1.1.0.cs
	Info: Construct custom PartNum from DefaultSpecs table. 
============================================================================*/

Func<string,string> spc = s => 
	PCLookUp.DataLookup("DefaultSpecs", s, devKey);

string[] ca = {
	spc("HCPCSCODE"), spc("PARTCODE"), (spc("BRAND")=="SS"? "SS": "CF")
};

return String.Join("-", ca);