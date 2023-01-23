/*== ExportToLineDtl v1.3.0 ==================================================
	
	Created: 08/01/2022 -Kevin Veldman
	Changed: 01/23/2023 -KV

	File: SMO-UDM_s-ExportToLineDtl-v1.3.0.cs
	Purpose: Send values from Inputs to Order/Quote UD Tables.
		## Overrides PartTrap data. Be careful ##
============================================================================*/

//__ LINQ Query: Get DB Fields from Order Line _______________________
	dynamic ld = 0;

	if (Context.Entity == "OrderDtl") {

		ld = Db.OrderDtl.Where(od => od.Company == Context.CompanyID
				&& od.OrderNum  == Context.OrderNumber
				&& od.OrderLine == Context.OrderLineNumber).FirstOrDefault();

	} else if (Context.Entity == "QuoteDtl") {

		ld = Db.QuoteDtl.Where(qd => qd.Company == Context.CompanyID
				&& qd.QuoteNum  == Context.QuoteNumber 
				&& qd.QuoteLine == Context.QuoteLineNumber).FirstOrDefault();
	}
//____________________________________________________________________


if (Context.Entity == "OrderDtl" || Context.Entity == "QuoteDtl") {

	// Order Info Fields
		ld.LastNamePID_c    = Inputs.cPID.Value;
		ld.FirstName_c      = Inputs.cFirstName.Value;
		ld.dPatientHt_c     = Convert.ToInt32(Inputs.dPatientHt.Value);
		ld.dPatientWt_c     = Inputs.dPatientWt.Value;
		ld.Dx_c             = Inputs.cDx.Value;
		ld.OrderNotes_c     = Inputs.eOrderNotes.Value;
		ld.cInternalNotes_c = Inputs.eCustNotes.Value;

	//Required Fields
		ld.cDeviceCode_c  = Inputs.bStyle.Value;
		ld.cSBLR_c        = Inputs.rQty.Value;
		ld.kPrepped_c     = Inputs.rFinish.Value == "P";
		ld.ModType_c      = Inputs.rCastMeas.Value;
		ld.cPlasticType_c = Inputs.bPlasticType.Value;
		ld.cPlasticDuro_c = Inputs.bPlasticThick.Value;
		ld.cPattern_c     = Inputs.bPattern.Value;
		ld.cChafeType_c   = Inputs.bChafeType.Value;
		ld.cStrapColor_c  = Inputs.bStrapColor.Value;

	// Mod Fields
		ld.cAnkleMods_c = Inputs.rAnkleMods.Value;
		ld.dHeelMod_c   = Convert.ToInt32(Inputs.dAdjHeel.Value);
		ld.dAnkleMod_c  = Convert.ToInt32(Inputs.dAdjAnkle.Value);
		ld.dForeMod_c   = Convert.ToInt32(Inputs.dAdjForeFt.Value);
		ld.cModNotes_c  = Inputs.cModNotes.Value;

	//Rush Fields
		ld.kRushNC_c = Inputs.kNCRush.Value;
		ld.kRush1D_c = Inputs.kRush.Value && !Inputs.k0day.Value;
		ld.kRush0D_c = Inputs.k0day.Value;

	//Other Device Add-ons
		ld.kLiner_c      = Inputs.kLiner.Value;
		ld.cLinerMtl_c   = Inputs.kLiner.Value? "VOL18": "";
		ld.kBoot_c       = Inputs.kInnerBoot.Value;
		ld.cBoot_c       = Inputs.bInnerBootPN.Value;
		ld.kWedge_c      = Inputs.kWedge.Value;
		ld.dWedgeQty_c   = Inputs.dWedgeQty.Value;
		ld.kToeWalk_c    = Inputs.kToeWalk.Value;
		ld.kTallEars_c   = Inputs.kTallEars.Value;
		ld.kSTPads_c     = Inputs.kSTPad.Value;
		ld.kSSplate_c    = Inputs.kSpringPlate.Value;
		ld.kCFplate_c    = Inputs.kCarbonPlate.Value;
		ld.kRevTrim_c    = Inputs.kReverseTrim.Value;
		ld.kNonSkid_c    = Inputs.kNonSkid.Value;
		ld.kPlateau_c    = Inputs.kPlateau.Value;
		ld.kMetPads_c    = Inputs.kMetPad.Value;
		ld.kHeelCut_c    = Inputs.kHeelCut.Value;
		ld.kDorsalChip_c = Inputs.kDorsalChip.Value;
		ld.kBSHoles_c    = Inputs.kCutout.Value;
		ld.kClubfoot_c   = Inputs.kClubFoot.Value;
		ld.cShoeSize_c   = Inputs.cShoeSize.Value;
		ld.kFullFP_c     = Inputs.kFullPlate.Value;
		ld.kExtMedWall_c = Inputs.kExtMedWall.Value;
		ld.kAnkleBuildup_c = Inputs.kAnkleBuildup.Value;

	//Set Measurement Fields
		ld.cSsmlUnit_c = "Imperial";

		ld.dMeas1_c  = Inputs.d1Value.Value;
		ld.dMeas2_c  = Inputs.d2Value.Value;
		ld.dMeas3_c  = Inputs.d3Value.Value;
		ld.dMeas4_c  = Inputs.d4Value.Value;
		ld.dMeas5_c  = Inputs.d5Value.Value;
		ld.dMeas6_c  = Inputs.d6Value.Value;
		ld.dMeas7_c  = Inputs.d7Value.Value;
		ld.dMeas8_c  = Inputs.d8Value.Value;
		ld.dMeas9_c  = Inputs.d9Value.Value;
		ld.dMeas10_c = Inputs.d10Value.Value;
		ld.dMeas11_c = Inputs.d11Value.Value;
		ld.dMeas12_c = Inputs.d12Value.Value;
		ld.dMeas13_c = Inputs.d13Value.Value;

		ld.dMeas1r_c  = Inputs.d1rValue.Value;
		ld.dMeas2r_c  = Inputs.d2rValue.Value;
		ld.dMeas3r_c  = Inputs.d3rValue.Value;
		ld.dMeas4r_c  = Inputs.d4rValue.Value;
		ld.dMeas5r_c  = Inputs.d5rValue.Value;
		ld.dMeas6r_c  = Inputs.d6rValue.Value;
		ld.dMeas7r_c  = Inputs.d7rValue.Value;
		ld.dMeas8r_c  = Inputs.d8rValue.Value;
		ld.dMeas9r_c  = Inputs.d9rValue.Value;
		ld.dMeas10r_c = Inputs.d10rValue.Value;
		ld.dMeas11r_c = Inputs.d11rValue.Value;
		ld.dMeas12r_c = Inputs.d12rValue.Value;
		ld.dMeas13r_c = Inputs.d13rValue.Value;
		

	//Set Mold IDs (selected from SSML)
		ld.cMold1_c = Inputs.cMold1.Value;
		ld.cMold2_c = Inputs.cMold2.Value;
		ld.cMold3_c = Inputs.cMold3.Value;
	
	//Set Remake Fields
		ld.kRemake_c    = Inputs.kRemake.Value;	
		ld.OriginalSO_c = Convert.ToInt32(Inputs.dRmkSO.Value);
		ld.cRmkRsnReq_c = Inputs.bRmkReasonActual.Value;
		ld.cRmkRsnAct_c = Inputs.bRemakeReason.Value;
		ld.cRmkCO_c     = Inputs.bRemakePrac2.Value;
		ld.cEvalCO_c    = Inputs.bRemakePrac1.Value;


	/* //Future/Corrected Remake Fields
		ld.kRemake_c    = Inputs.kRemake.Value;	
		ld.OriginalSO_c = Convert.ToInt32(Inputs.dRmkSO.Value);
		ld.cRmkRsnReq_c = Inputs.bRmkResAct.Value;
		ld.cRmkRsnAct_c = Inputs.bRmkResReq.Value;
		ld.cRmkCO_c     = Inputs.bRmkPrac2.Value;
		ld.cEvalCO_c    = Inputs.bRmkPrac1.Value;	*/
}

Inputs.blockMV.Value = false;




/*== CHANGE LOG ==============================================================

	10/04/2022: Added meas 7/8/9, notes, other new SMO8 Fields;
	10/05/2022: Fix Integer-Decimal conversion issue;
	10/27/2022: Add all Right-split measurements & Left #10-#13;
	10/27/2022: UM from Client never Metric;
	11/16/2022: Add new fields kExtMedWall, kAnkleBuildup;
	11/17/2022: Always set Right-side measurements;
	01/23/2023: Set Inputs.blockMV to false (unblock Method Variable);

============================================================================*/