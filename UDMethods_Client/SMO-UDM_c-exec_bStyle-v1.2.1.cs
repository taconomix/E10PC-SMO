/*== exec_bStyle-v1.2.1 ======================================================

	Created: 08/03/2022 -Kevin Veldman
	Changed: 12/07/2022 -Kevin Veldman

	Info: Set default values for change on bStyle
	File: SMO-UDM_c-exec_bStyle-v1.2.1.cs
============================================================================*/

// Defaults Bigshot-Style SMOs
	bool kBig = lkpSpecs("SPEC")=="BIGSHOT";

	Inputs.rCastMeas.Value    = kBig? "C": "M";     
	Inputs.kInnerBoot.Value   = kBig;               
	Inputs.bInnerBootPN.Value = kBig? lkpSpecs("INNERBOOT"): "None";

// Determine default plastic type/thickness and get the Plastic PN 
	Inputs.bPlasticType.Value  = lkpSpecs("PLASTICTYPE");
	Inputs.bPlasticThick.Value = lkpSpecs("PLASTICTHICK");

// Default Dorsal Chips
	Inputs.kDorsalChip.Value = lkpSpecs("ChipsIncluded")=="X";

// Removes strap/pad options when appropriate.
	bool kStrap = lkpSpecs("SPEC")!="UCBL";

	Inputs.bStrapColor.Invisible = !kStrap;
	Inputs.bPattern.Invisible    = !kStrap;
	Inputs.bChafeType.Invisible  = !kStrap;
	
	if (!kStrap) Inputs.bStrapColor.Value = "";
	if (!kStrap) Inputs.bPattern.Value = "";
	if (!kStrap) Inputs.bChafeType.Value = "";
	
// Set Brand
	Inputs.rCFabSS.Value = lkpSpecs("BRAND");

// Set new Inputs needed to handle Method Rules Early execution errors
	setPrepLock();
	setMRInputs();

// Set PartNum
	setFinalPN();

// Set boolean to show the Configuration Client was used.
	Inputs.kClientRules.Value = true;




/*== CHANGE LOG ==============================================================
	09/15/2022: Set Inputs.kClientRules to true for OrderHeld Status;
	12/07/2022: Set kPrepped bit field;
============================================================================*/






// Set visibility on CAD options
	/* One of these values set above. Never returns true. Removed.
	bool kCast = Inputs.rCastMeas.Value=="C";
	bool kMeas = Inputs.rCastMeas.Value=="M";
	Inputs.kNoWeirdMods.Invisible = !kCast && !kMeas;
	Inputs.kForceCad.Invisible    = !kCast && !kMeas;	*/