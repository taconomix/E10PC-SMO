/*== ImportFromLineDtl v2.0.0 ================================================
	
	Created: 08/01/2022 -Kevin Veldman
	Changed: 01/09/2023 -KV

	File: SMO-UDM_s-ImportFromLineDtl-v2.0.0.cs
	Info: Set Config Input Values based on QuoteDtl|OrderDtl UD Tables.
============================================================================*/

dynamic ld = 0; //Dynamic variable allows PcStatus functionality

//Linq statement to set ld to QuoteDtl|OrderDtl based on Context
	if (Context.Entity == "OrderDtl") {

		ld = Db.OrderDtl.Where(od => od.Company == Context.CompanyID
				&& od.OrderNum  == Context.OrderNumber
				&& od.OrderLine == Context.OrderLineNumber).FirstOrDefault();

	} else if (Context.Entity == "QuoteDtl") {

		ld = Db.QuoteDtl.Where(qd => qd.Company == Context.CompanyID
				&& qd.QuoteNum  == Context.QuoteNumber 
				&& qd.QuoteLine == Context.QuoteLineNumber).FirstOrDefault();
	}



if (Context.Entity == "OrderDtl" || Context.Entity == "QuoteDtl") {

	// Order Info Fields
		Inputs.cPID.Value        = ld.LastNamePID_c;
		Inputs.cFirstName.Value  = ld.FirstName_c;
		Inputs.dPatientHt.Value  = Convert.ToDecimal(ld.dPatientHt_c);
		Inputs.dPatientWt.Value  = ld.dPatientWt_c;
		Inputs.cDx.Value         = ld.Dx_c;
		Inputs.eOrderNotes.Value = ld.OrderNotes_c;
		Inputs.eCustNotes.Value  = ld.cInternalNotes_c;

	//Required to Process any Config Order
		Inputs.bStyle.Value        = ld.cDeviceCode_c;
		Inputs.rQty.Value          = ld.cSBLR_c;
		Inputs.rFinish.Value       = ld.kPrepped_c? "P": "F";
		Inputs.kPrepped.Value      = ld.kPrepped_c;
		Inputs.rCastMeas.Value     = ld.ModType_c;
		Inputs.bPlasticType.Value  = ld.cPlasticType_c;
		Inputs.bPlasticThick.Value = ld.cPlasticDuro_c;
		Inputs.bPattern.Value      = ld.cPattern_c;
		Inputs.bChafeType.Value    = ld.cChafeType_c;
		Inputs.bStrapColor.Value   = ld.cStrapColor_c;

	// Mod Fields
		Inputs.rAnkleMods.Value = ld.cAnkleMods_c;
		Inputs.dAdjHeel.Value   = Convert.ToDecimal(ld.dHeelMod_c);
		Inputs.dAdjAnkle.Value  = Convert.ToDecimal(ld.dAnkleMod_c);
		Inputs.dAdjForeFt.Value = Convert.ToDecimal(ld.dForeMod_c);
		Inputs.cModNotes.Value  = ld.cModNotes_c;

	//Rush Fields
		Inputs.kNCRush.Value = ld.kRushNC_c;
		Inputs.k0day.Value   = ld.kRush0D_c;
		Inputs.kRush.Value   = ld.kRush0D_c || ld.kRush1D_c;

	//Other Device Add-ons
		Inputs.kLiner.Value       = ld.kLiner_c;
		Inputs.kInnerBoot.Value   = ld.kBoot_c;
		Inputs.bInnerBootPN.Value = ld.cBoot_c;
		Inputs.kWedge.Value       = ld.kWedge_c;
		Inputs.dWedgeQty.Value    = ld.dWedgeQty_c;
		Inputs.kToeWalk.Value     = ld.kToeWalk_c;
		Inputs.kTallEars.Value    = ld.kTallEars_c;
		Inputs.kSTPad.Value       = ld.kSTPads_c;
		Inputs.kSpringPlate.Value = ld.kSSplate_c;
		Inputs.kCarbonPlate.Value = ld.kCFplate_c;
		Inputs.kNonSkid.Value     = ld.kNonSkid_c;
		Inputs.kReverseTrim.Value = ld.kRevTrim_c;
		Inputs.kPlateau.Value     = ld.kPlateau_c;
		Inputs.kMetPad.Value      = ld.kMetPads_c;
		Inputs.kHeelCut.Value     = ld.kHeelCut_c;
		Inputs.kDorsalChip.Value  = ld.kDorsalChip_c;
		Inputs.kCutout.Value      = ld.kBSHoles_c;
		Inputs.kClubFoot.Value    = ld.kClubfoot_c;
		Inputs.kFullPlate.Value   = ld.kFullFP_c;
		Inputs.kExtMedWall.Value  = ld.kExtMedWall_c;
		Inputs.kAnkleBuildup.Value = ld.kAnkleBuildup_c;
		

	//__ Measurement Fields ____________________________________________________

		// Function to preserve Meas Value and get Input Values
			Func<decimal,decimal[]> msInput = dMeas => {
				
				decimal dInt = Math.Floor(dMeas), dNmr = 0, dDtr = 0;
				decimal dMod = dMeas-dInt;

				if (dMod > 0) {

					dNmr = Math.Round(dMod * 16);
					dDtr = 16;

					while (dNmr % 2 == 0) {
						dNmr = dNmr/2;
						dDtr = dDtr/2;
					}
				}

				decimal dCM = Math.Round(dMeas/.03937m)/10;
				
				decimal[] returnValues = { dMeas, dInt, dNmr, dDtr, dCM };
				return returnValues;

			}; // 0==Value, 1==Int, 2==Nmr, 3==Dnm, 4==CM

		//-- Use Above Function to create array for each Measurement  --------------
			decimal[] dVals_01L = msInput(ld.dMeas1_c);
			decimal[] dVals_02L = msInput(ld.dMeas2_c);
			decimal[] dVals_03L = msInput(ld.dMeas3_c);
			decimal[] dVals_04L = msInput(ld.dMeas4_c);
			decimal[] dVals_05L = msInput(ld.dMeas5_c);
			decimal[] dVals_06L = msInput(ld.dMeas6_c);
			decimal[] dVals_07L = msInput(ld.dMeas7_c);
			decimal[] dVals_08L = msInput(ld.dMeas8_c);
			decimal[] dVals_09L = msInput(ld.dMeas9_c);
			decimal[] dVals_10L = msInput(ld.dMeas10_c);
			decimal[] dVals_11L = msInput(ld.dMeas11_c);
			decimal[] dVals_12L = msInput(ld.dMeas12_c);
			decimal[] dVals_13L = msInput(ld.dMeas13_c);

			decimal[] dVals_01R = msInput(ld.dMeas1r_c);
			decimal[] dVals_02R = msInput(ld.dMeas2r_c);
			decimal[] dVals_03R = msInput(ld.dMeas3r_c);
			decimal[] dVals_04R = msInput(ld.dMeas4r_c);
			decimal[] dVals_05R = msInput(ld.dMeas5r_c);
			decimal[] dVals_06R = msInput(ld.dMeas6r_c);
			decimal[] dVals_07R = msInput(ld.dMeas7r_c);
			decimal[] dVals_08R = msInput(ld.dMeas8r_c);
			decimal[] dVals_09R = msInput(ld.dMeas9r_c);
			decimal[] dVals_10R = msInput(ld.dMeas10r_c);
			decimal[] dVals_11R = msInput(ld.dMeas11r_c);
			decimal[] dVals_12R = msInput(ld.dMeas12r_c);
			decimal[] dVals_13R = msInput(ld.dMeas13r_c);
		//--------------------------------------------------------------------------

		//-- Set Left Measurement Inputs -------------------------------------------
			Inputs.d1L_Int.Value  = dVals_01L[1];	
			Inputs.d1L_Nmr.Value  = dVals_01L[2];
			Inputs.d1L_Dnm.Value  = dVals_01L[3];
			Inputs.d1L_CM.Value   = dVals_01L[4];

			Inputs.d2L_Int.Value  = dVals_02L[1];
			Inputs.d2L_Nmr.Value  = dVals_02L[2];
			Inputs.d2L_Dnm.Value  = dVals_02L[3];
			Inputs.d2L_CM.Value   = dVals_02L[4];

			Inputs.d3L_Int.Value  = dVals_03L[1];
			Inputs.d3L_Nmr.Value  = dVals_03L[2];
			Inputs.d3L_Dnm.Value  = dVals_03L[3];
			Inputs.d3L_CM.Value   = dVals_03L[4];

			Inputs.d4L_Int.Value  = dVals_04L[1];
			Inputs.d4L_Nmr.Value  = dVals_04L[2];
			Inputs.d4L_Dnm.Value  = dVals_04L[3];
			Inputs.d4L_CM.Value   = dVals_04L[4];

			Inputs.d5L_Int.Value  = dVals_05L[1];
			Inputs.d5L_Nmr.Value  = dVals_05L[2];
			Inputs.d5L_Dnm.Value  = dVals_05L[3];
			Inputs.d5L_CM.Value   = dVals_05L[4];

			Inputs.d6L_Int.Value  = dVals_06L[1];
			Inputs.d6L_Nmr.Value  = dVals_06L[2];
			Inputs.d6L_Dnm.Value  = dVals_06L[3];
			Inputs.d6L_CM.Value   = dVals_06L[4];

			Inputs.d7L_Int.Value  = dVals_07L[1];
			Inputs.d7L_Nmr.Value  = dVals_07L[2];
			Inputs.d7L_Dnm.Value  = dVals_07L[3];
			Inputs.d7L_CM.Value   = dVals_07L[4];

			Inputs.d8L_Int.Value  = dVals_08L[1];
			Inputs.d8L_Nmr.Value  = dVals_08L[2];
			Inputs.d8L_Dnm.Value  = dVals_08L[3];
			Inputs.d8L_CM.Value   = dVals_08L[4];

			Inputs.d9L_Int.Value  = dVals_09L[1];
			Inputs.d9L_Nmr.Value  = dVals_09L[2];
			Inputs.d9L_Dnm.Value  = dVals_09L[3];
			Inputs.d9L_CM.Value   = dVals_09L[4];

			Inputs.d10L_Int.Value  = dVals_10L[1];
			Inputs.d10L_Nmr.Value  = dVals_10L[2];
			Inputs.d10L_Dnm.Value  = dVals_10L[3];
			Inputs.d10L_CM.Value   = dVals_10L[4];

			Inputs.d11L_Int.Value  = dVals_11L[1];
			Inputs.d11L_Nmr.Value  = dVals_11L[2];
			Inputs.d11L_Dnm.Value  = dVals_11L[3];
			Inputs.d11L_CM.Value   = dVals_11L[4];

			Inputs.d12L_Int.Value  = dVals_12L[1];
			Inputs.d12L_Nmr.Value  = dVals_12L[2];
			Inputs.d12L_Dnm.Value  = dVals_12L[3];
			Inputs.d12L_CM.Value   = dVals_12L[4];

			Inputs.d13L_Int.Value  = dVals_13L[1];
			Inputs.d13L_Nmr.Value  = dVals_13L[2];
			Inputs.d13L_Dnm.Value  = dVals_13L[3];
			Inputs.d13L_CM.Value   = dVals_13L[4];
		//--------------------------------------------------------------------------

		//-- Set Right Inputs ------------------------------------------------------
			Inputs.d1R_Int.Value  = dVals_01R[1];
			Inputs.d1R_Nmr.Value  = dVals_01R[2];
			Inputs.d1R_Dnm.Value  = dVals_01R[3];
			Inputs.d1R_CM.Value   = dVals_01R[4];

			Inputs.d2R_Int.Value  = dVals_02R[1];
			Inputs.d2R_Nmr.Value  = dVals_02R[2];
			Inputs.d2R_Dnm.Value  = dVals_02R[3];
			Inputs.d2R_CM.Value   = dVals_02R[4];

			Inputs.d3R_Int.Value  = dVals_03R[1];
			Inputs.d3R_Nmr.Value  = dVals_03R[2];
			Inputs.d3R_Dnm.Value  = dVals_03R[3];
			Inputs.d3R_CM.Value   = dVals_03R[4];

			Inputs.d4R_Int.Value  = dVals_04R[1];
			Inputs.d4R_Nmr.Value  = dVals_04R[2];
			Inputs.d4R_Dnm.Value  = dVals_04R[3];
			Inputs.d4R_CM.Value   = dVals_04R[4];

			Inputs.d5R_Int.Value  = dVals_05R[1];
			Inputs.d5R_Nmr.Value  = dVals_05R[2];
			Inputs.d5R_Dnm.Value  = dVals_05R[3];
			Inputs.d5R_CM.Value   = dVals_05R[4];

			Inputs.d6R_Int.Value  = dVals_06R[1];
			Inputs.d6R_Nmr.Value  = dVals_06R[2];
			Inputs.d6R_Dnm.Value  = dVals_06R[3];
			Inputs.d6R_CM.Value   = dVals_06R[4];

			Inputs.d7R_Int.Value  = dVals_07R[1];
			Inputs.d7R_Nmr.Value  = dVals_07R[2];
			Inputs.d7R_Dnm.Value  = dVals_07R[3];
			Inputs.d7R_CM.Value   = dVals_07R[4];

			Inputs.d8R_Int.Value  = dVals_08R[1];
			Inputs.d8R_Nmr.Value  = dVals_08R[2];
			Inputs.d8R_Dnm.Value  = dVals_08R[3];
			Inputs.d8R_CM.Value   = dVals_08R[4];

			Inputs.d9R_Int.Value  = dVals_09R[1];
			Inputs.d9R_Nmr.Value  = dVals_09R[2];
			Inputs.d9R_Dnm.Value  = dVals_09R[3];
			Inputs.d9R_CM.Value   = dVals_09R[4];

			Inputs.d10R_Int.Value  = dVals_10R[1];
			Inputs.d10R_Nmr.Value  = dVals_10R[2];
			Inputs.d10R_Dnm.Value  = dVals_10R[3];
			Inputs.d10R_CM.Value   = dVals_10R[4];

			Inputs.d11R_Int.Value  = dVals_11R[1];
			Inputs.d11R_Nmr.Value  = dVals_11R[2];
			Inputs.d11R_Dnm.Value  = dVals_11R[3];
			Inputs.d11R_CM.Value   = dVals_11R[4];

			Inputs.d12R_Int.Value  = dVals_12R[1];
			Inputs.d12R_Nmr.Value  = dVals_12R[2];
			Inputs.d12R_Dnm.Value  = dVals_12R[3];
			Inputs.d12R_CM.Value   = dVals_12R[4];

			Inputs.d13R_Int.Value  = dVals_13R[1];
			Inputs.d13R_Nmr.Value  = dVals_13R[2];
			Inputs.d13R_Dnm.Value  = dVals_13R[3];
			Inputs.d13R_CM.Value   = dVals_13R[4];
		//--------------------------------------------------------------------------


		//-- Set Value fields after input to prevent overwrite ---------------------
			Inputs.d1Value.Value = dVals_01L[0];
			Inputs.d2Value.Value = dVals_02L[0];
			Inputs.d3Value.Value = dVals_03L[0];
			Inputs.d4Value.Value = dVals_04L[0];
			Inputs.d5Value.Value = dVals_05L[0];
			Inputs.d6Value.Value = dVals_06L[0];
			Inputs.d7Value.Value = dVals_07L[0];
			Inputs.d8Value.Value = dVals_08L[0];
			Inputs.d9Value.Value = dVals_09L[0];
			Inputs.d10Value.Value = dVals_10L[0];
			Inputs.d11Value.Value = dVals_11L[0];
			Inputs.d12Value.Value = dVals_12L[0];
			Inputs.d13Value.Value = dVals_13L[0];


			Inputs.d1rValue.Value = dVals_01R[0];
			Inputs.d2rValue.Value = dVals_02R[0];
			Inputs.d3rValue.Value = dVals_03R[0];
			Inputs.d4rValue.Value = dVals_04R[0];
			Inputs.d5rValue.Value = dVals_05R[0];
			Inputs.d6rValue.Value = dVals_06R[0];
			Inputs.d7rValue.Value = dVals_07R[0];
			Inputs.d8rValue.Value = dVals_08R[0];
			Inputs.d9rValue.Value = dVals_09R[0];
			Inputs.d10rValue.Value = dVals_10R[0];
			Inputs.d11rValue.Value = dVals_11R[0];
			Inputs.d12rValue.Value = dVals_12R[0];
			Inputs.d13rValue.Value = dVals_13R[0];
		//----------------------------------------------------------------------
	//_____________________________________________ End Measurement Fields _____



	//Shoe Size
		decimal d6 = ld.dMeas6_c; 
		string szShoe = (Math.Floor((d6-2.625m)/0.25m) + 3).ToString();
		Inputs.cShoeSize.Value = d6<2.75m || d6>5.25m? "CUST": szShoe;

	//Set Mold IDs
		Inputs.cMold1.Value = ld.cMold1_c;
		Inputs.cMold2.Value = ld.cMold2_c;
		Inputs.cMold3.Value = ld.cMold3_c;
	
	//Set Remake Fields
		Inputs.kRemake.Value          = ld.kRemake_c;
		Inputs.dRmkSO.Value           = ld.OriginalSO_c;
		Inputs.bRmkReasonActual.Value = ld.cRmkRsnAct_c;
		Inputs.bRemakeReason.Value    = ld.cRmkRsnReq_c;
		Inputs.bRemakePrac2.Value     = ld.cRmkCO_c;
		Inputs.bRemakePrac1.Value     = ld.cEvalCO_c;

	/* //Future/Corrected Remake Fields
		Inputs.kRemake.Value    = ld.kRemake_c;
		Inputs.dRmkSO.Value     = ld.OriginalSO_c;
		Inputs.bRmkResAct.Value = ld.cRmkRsnAct_c;
		Inputs.bRmkResReq.Value = ld.cRmkRsnReq_c;
		Inputs.bRmkPrac2.Value  = ld.cRmkCO_c;
		Inputs.bRmkPrac1.Value  = ld.cEvalCO_c;	*/
}




/*== CHANGE LOG ==============================================================
	
	10/04/2022: Added meas 7/8/9, notes, other new SMO8 Fields.
	10/05/2022: Fix Integer-Decimal conversion issue.
	10/27/2022: Add meas 10-13, add Right-side split measurements
	11/08/2022: Now set Meas Inputs with new client-side method 'setMeasInputs'
	11/16/2022: Add new fields kExtMedWall, kAnkleBuildup
	01/09/2023: Added back Setting Measurement Inputs here;

============================================================================*/