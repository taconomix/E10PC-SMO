/*============================================================================
	Method: SMO-UDM_C-calcDiscount-v1.0.0.cs

	Created: 01/18/2022
	Author:  Kevin Veldman
	Purpose: Set dDiscount & dDiscRate to correct values - CLIENT ONLY
	WARNING: ## MUST MANUALLY KEEP IN SYNC WITH UDMethods.getDiscount ##
============================================================================*/

// Set local functions, arrays, and variables 
	// DO NOT CHANGE 
	
	// Create list from specifiedcolumn in uniqueDiscounts 
		Func<string,string> sDCL = C => PCLookUp.DataColumnList("uniqueDiscounts",C);
		
		Func<string,decimal> sDec = sVal => Convert.ToDecimal(sVal);

	// Arrays for columns in uniqueDiscount table.
		string[]   dscID = sDCL("discountID").Split('~');
		string[] dscShip = sDCL("shipToID"  ).Split('~');
		string[] dscName = sDCL("custID"    ).Split('~');
		string[]  dscGrp = sDCL("custGrp"   ).Split('~');
		string[]  dscVal = sDCL("discAmnt"  ).Split('~');
		string[] dscRate = sDCL("discRate"  ).Split('~');

	// Customer attributes.
		string  myCustID = Context.CustomerID;
		string myCustGrp = getCustGrp(Context.CustomerNumber);
		string  myShipTo = getShipTo(Context.CustomerNumber);

	// Input variables.	
		string bStyle = Inputs.bStyle.Value;
		bool kBoot = Inputs.kInnerBoot.Value;
		bool kChips = Inputs.kDorsalChip.Value;
		bool kRush = Inputs.kRush.Value;
		bool k0day = Inputs.k0day.Value;
		decimal dQty = Inputs.rQty.Value=="B"? 2: 1;


	// Final discounts
		decimal dscValCust=0,  dscValShip=0,  dscValGroup=0;
		decimal dscRateCust=0, dscRateShip=0, dscRateGroup=0;


// Set condition/Multiplier for each discountID & connect them
	// MUST BE UPDATED WHEN ROWS ARE ADDED TO uniqueDiscount TABLE

	// Set multiplier for each discountID (if condition not met, multiplier=0).
		decimal coupaSAI2  = bStyle=="120"?1:0;
		decimal coupaHAI2  = bStyle=="215"?1:0;
		decimal coupaNSkd  = Inputs.kNonSkid.Value?1:0;
		decimal coupaCFFP  = Inputs.kCarbonPlate.Value?1:0;
		decimal coupaLiner = Inputs.kLiner.Value?1:0;
		decimal coupaBoot  = kBoot && lkpSpecs("SPEC")!="BIGSHOT"? 1: 0;
		decimal coupaDAJ   = 0;  // No Hinges in SMO
		decimal coupaTAM   = 0;  // No Hinges in SMO
		decimal coupaChip  = kChips && lkpSpecs("ChipsIncluded")!="X"? 1: 0;
		decimal top22      = 1;  // All TOP orders 22% discount
		decimal ottoWRI    = 1;  // All Ottobock orders 5% discount
		decimal ottoSAR    = 1;  // All Ottobock orders 5% discount
		decimal ottoABIL   = 1;  // All Ottobock orders 5% discount 
		decimal ottoACT    = 1;  // All Ottobock orders 5% discount
		decimal ottoJPO    = 1;  // All Ottobock orders 5% discount
		decimal nuaeBoot   = kBoot && lkpSpecs("SPEC")!="BIGSHOT"? 1: 0;
		decimal spineGOSS  = 0;  // TLSO Only
		decimal coupaPLI2  = bStyle=="123"? 1: 0;
		decimal coupaART   = bStyle=="210"? 1: 0;
		decimal coupaSSA   = bStyle=="130"? 1: 0;
		decimal eurSTAB    = 0; //Stabilizer only
		decimal eurLiner   = Inputs.kLiner.Value?1:0;
		decimal eurNSkd    = Inputs.kNonSkid.Value?1:0;
		decimal eurBoot    = kBoot || lkpSpecs("SPEC")=="BIGSHOT"? 1: 0;
		decimal eurWBase   = 0;
		decimal coupaTWMod = Inputs.kToeWalk.Value? 1: 0;
		decimal coupaRushS = kRush && !k0day && !Inputs.kNCRush.Value? 1/dQty: 0;
		decimal coupa0DayS = k0day && !Inputs.kNCRush.Value? 1/dQty: 0;
		decimal coupaRushA = 0;
		decimal coupa0DayA = 0;
		decimal coupaSOL   = 0;
		decimal coupaBSA   = 0;
		decimal coupaSMO   = bStyle=="SMO"? 1: 0;
		decimal coupaBIG   = lkpSpecs("SPEC")=="BIGSHOT"? 1: 0;
		decimal coupaTLS   = 0;
		decimal coupaPUL   = bStyle=="225"? 1: 0;
		decimal coupaUCB   = bStyle=="UCB"? 1: 0;



	// Pair each discountID with appropriate multiplier.
		Dictionary<string,decimal> kvp = new Dictionary<string,decimal>() {
			{dscID[0],  coupaSAI2 },
			{dscID[1],  coupaHAI2 },
			{dscID[2],  coupaNSkd },
			{dscID[3],  coupaCFFP },
			{dscID[4],  coupaLiner},
			{dscID[5],  coupaBoot },
			{dscID[6],  coupaDAJ  },
			{dscID[7],  coupaTAM  },
			{dscID[8],  coupaChip },
			{dscID[9],  top22     },
			{dscID[10], ottoWRI   },
			{dscID[11], ottoSAR   },
			{dscID[12], ottoABIL  },
			{dscID[13], ottoACT   },
			{dscID[14], ottoJPO   },
			{dscID[15], nuaeBoot  },
			{dscID[16], spineGOSS },
			{dscID[17], coupaPLI2 },
			{dscID[18], coupaART  },
			{dscID[19], coupaSSA  },
			{dscID[20], eurSTAB   },
			{dscID[21], eurLiner  },
			{dscID[22], eurNSkd   },
			{dscID[23], eurBoot   },
			{dscID[24], eurWBase  },
			{dscID[25], coupaTWMod},
			{dscID[26], coupaRushS},
			{dscID[27], coupa0DayS},
			{dscID[28], coupaRushA},
			{dscID[29], coupa0DayA},
			{dscID[30], coupaSOL  },
			{dscID[31], coupaBSA  },
			{dscID[32], coupaSMO  },
			{dscID[33], coupaBIG  },
			{dscID[34], coupaTLS  },
			{dscID[35], coupaPUL  },
			{dscID[36], coupaUCB  }
		}; // ENSURE THAT MULTIPLIER MATCHES DISCOUNT ID ROW WHERE 1ST ROW = 0.


// Loop through arrays and add discounts when conditions are met
	// DO NOT CHANGE
	
	if (Array.IndexOf(dscName,myCustID) != -1) {
		for (int i = 0; i < kvp.Count; i++){
			if (myCustID == dscName[i]){
				dscValCust  += kvp[dscID[i]] * sDec(dscVal[i]);
				dscRateCust += kvp[dscID[i]] * sDec(dscRate[i]);
			}
		}
	} // Customer Level Discounts

	if (Array.IndexOf(dscShip,myShipTo) != -1) {
		for (int i = 0; i < kvp.Count; i++){
			if (myShipTo == dscShip[i]){
				dscValShip  += kvp[dscID[i]] * sDec(dscVal[i]);
				dscRateShip += kvp[dscID[i]] * sDec(dscRate[i]);
			}
		}
	} // ShipTo Level Discounts

	if (Array.IndexOf(dscGrp,myCustGrp) != -1) {
		for (int i = 0; i < kvp.Count; i++){
			if (myCustGrp == dscGrp[i]){
				dscValGroup  += kvp[dscID[i]] * sDec(dscVal[i]);
				dscRateGroup += kvp[dscID[i]] * sDec(dscRate[i]);
			}
		}
	} // CustGroup Level Discounts

	Inputs.dDiscount.Value = dscValCust  + dscValShip  + dscValGroup;
	Inputs.dDiscRate.Value = dscRateCust + dscRateShip + dscRateGroup;

if (dscID.Length > kvp.Count) 
	MessageBox.Show("New discounts not applied. Please alert an Epicor admin.");








/*============================================================================

	Change Log:

		User:  Date:       Changes:
		KV     08/02/2022  Added to documentation
		KV     10/05/2022  Added to documentation

============================================================================*/