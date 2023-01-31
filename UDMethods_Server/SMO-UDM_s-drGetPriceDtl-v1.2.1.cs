/*== drGetPriceDtl-v1.2.1 ======================================================

	Created: 12/01/2022 -Kevin Veldman
	Changed: 01/24/2023 -KV

	Info: Return all pricing/discount details as string array.
	File: SMO-UDM_s-drGetPriceDtl-v1.2.1.cs
============================================================================*/

/*== Global Functions & Variables ==========================================*/

	Func<string,decimal,bool> kStrDec = (s,d) => decimal.TryParse(s, out d); 
	Func<string,decimal> sDec    = s => kStrDec(s,0)? Convert.ToDecimal(s): 0;
	Func<string,bool   > kpVal   = s => ( s!="" && s.ToUpper()!="NONE" );
	Func<string,string > getSpec = s => sLkp("DefaultSpecs", s, sDevKey);
	Func<string[],string,bool> sAIO  = (a,s) => ( Array.IndexOf(a,s) >= 0 );
	Func<string,string,string> lsRow = (t,r) => PCLookUp.DataRowList(t,r);
	Func<string,string,string> lsCol = (t,c) => PCLookUp.DataColumnList(t,c);

	//-- LINQ Query to get DB Fields from Order Line ---------------------
		dynamic ld = 0;

		if (Context.Entity == "OrderDtl") {

			ld = Db.OrderDtl.Where(od => od.Company == Context.CompanyID
					&& od.OrderNum  == Context.OrderNumber
					&& od.OrderLine == Context.OrderLineNumber)
				.FirstOrDefault();

		} else if (Context.Entity == "QuoteDtl") {

			ld = Db.QuoteDtl.Where(qd => qd.Company == Context.CompanyID
					&& qd.QuoteNum  == Context.QuoteNumber 
					&& qd.QuoteLine == Context.QuoteLineNumber)
				.FirstOrDefault();
		}
	//--------------------------------------------------------------------

	int custNum = ld.CustNum;
	var custID  = Context.CustomerID;
	var custGrp = getCustGrp(ld.CustNum);
	var shipTo  = getShipTo(ld.OrderNum);
	
	decimal dQty = ld.cSBLR_c=="B"? 2: 1;
	bool kRush = ld.kRush0D_c || ld.kRush1D_c;
	bool kBoot = ld.kBoot_c && getSpec("SPEC")!="BIGSHOT";

//===========================================================================




/*== UnitPrice & Itemized OrderComment =====================================*/

	//-- Set Price & Desc Variables ------------------------------------
		StringBuilder smo = new StringBuilder();
		
		string[] Price = lsRow("PriceMaster", sDevKey).Split('~');
		string[] Desc  = lsRow("PriceMaster", "Desc" ).Split('~');

		Func<int,decimal> drPrc = i => sDec(Price[i]);
	
		decimal prcAlt  = dLkp("AltBasePrices", custGrp, sDevKey);
		decimal prcBase = prcAlt > 0? prcAlt: drPrc(3) + drPrc(4);
		decimal prcUnit = 0;

		decimal prcPrep = dLkp("Standards", "Value", "Prep") * -1;
		string  dscPrep = sLkp("Standards", "Desc" , "Prep");

		string  rushRow = (ld.kRush0D_c? "0": "1") + "RushSMO";
		decimal prcRush = ld.kRushNC_c? 0: dLkp("Standards", "Value", rushRow);
		string  dscRush = sLkp("Standards","Desc",rushRow);

		decimal dWdg = ld.dWedgeQty_c;
	//-------------------------------------------------------------------


	//-- Functions to add lines & increment price -----------------------
		Action<int,bool> addLn0 = // Single input, PartQty==OrderQty
			(iCol,Keep) => {

			if (Keep) {

				smo.Append(Desc[iCol].PadRight(28));
				smo.Append(dQty.ToString());
				smo.Append(Price[iCol].PadLeft(10));
				smo.Append((dQty*drPrc(iCol)).ToString().PadLeft(11));
				smo.AppendLine();

				prcUnit += drPrc(iCol);
			}
		};

		Action<string,bool,decimal,decimal,int> addLn1 = 
			(sDesc,Keep,Qty,dPrc,iFixed) => { //Custom Price|Qty|Desc|Fixed Params
			
			if ( Keep ) {
				
				decimal myQty = iFixed==1? Qty:(Qty*dQty);

				smo.Append( sDesc.PadRight(28) );
				smo.Append( myQty.ToString() );
				smo.Append( dPrc.ToString().PadLeft(10) );
				smo.Append( (myQty*dPrc).ToString().PadLeft(11) );
				smo.AppendLine();
				
				prcUnit += ((dPrc*Qty)/(iFixed==1? dQty: 1));
			}
		}; 
	//-------------------------------------------------------------------


	//-- Set Title + Charge + Subtotal Lines ----------------------------
		smo.Append("Description".PadRight(27));
		smo.Append("Qty");
		smo.Append("UnitPrice".PadLeft(10));
		smo.Append("ExtPrice".PadLeft(11));
		smo.AppendLine();

		addLn1(Price[0], true, 1, prcBase, 0);        // Base Device
		addLn0(11, ld.kToeWalk_c);                    // Toe-Walker Mod
		addLn0(18, ld.kSSplate_c);                    // SpringSteel Footplate
		addLn0(17, ld.kCFplate_c);                    // CarbonFiber Footplate
		addLn0(21, ld.kNonSkid_c);                    // Non-Skid Soles
		addLn0(19, ld.kDorsalChip_c);                 // Dorsal Chips
		addLn0(10, kBoot);                            // Inner Boot
		addLn0(12, ld.kLiner_c);                      // Integrated Liner
		addLn0(5,  ld.kSTPads_c);                     // ST Pads
		addLn0(6,  ld.kMetPads_c);                    // Metatarsal Pads
		addLn0(31, ld.kPlateau_c);                    // Toe Plateaus
		addLn0(32, ld.kBSHoles_c);                    // Extra Cutouts
		addLn1(Desc[33], dWdg>0, dWdg, drPrc(33), 1); // Heel Wedge
		addLn1(dscPrep, ld.kPrepped_c, 1, prcPrep, 0);// Prep Discount
		addLn1(dscRush, kRush, 1, prcRush, 1);        // Rush Charge

		smo.AppendLine();
		smo.Append("Total Price".PadRight(28));
		smo.Append(dQty.ToString());
		smo.Append(prcUnit.ToString().PadLeft(10));
		smo.Append((dQty * prcUnit).ToString().PadLeft(11));
	//-------------------------------------------------------------------


	/*-- PriceMaster Table Column-Index Reference -----------------------
		0	Desc1
		1	HCPSC
		2	Code
		3	BASE
		4	STD ADD
		5	STPad
		6	MetPad
		7	tlsoGills
		8	tlsoGusset
		9	tlsoKIEU
		10	kInnerBootOpt
		11	kToeWalk
		12	kLinerOpt
		13	kTStrap
		14	kAntShellOpt
		15	kAntShellOpt2
		16	kDiabeticInsert
		17	kCarbonPlate
		18	kSpringPlate
		19	kDorsalChips
		20	kCarbonSupport
		21	kNonSkid
		22	kWalkingBase
		23	kNightStretch
		24	kQuickRelease
		25	kPowderCoat
		26	kUpgrade
		27	CarbonFiberShank
		28	RGOBucket
		29	HKAFOPelvicBand
		30	AFOFCLift
		31	kPlateau
		32	kBigShotHoles
		33	kWedge
		34	kExtraMods
		35	kAppScanFee 
	-------------------------------------------------------------------*/

/*==========================================================================*/





/*== Discounts on Configured Pricing =======================================*/

	//-- Local Functions & Variables ----------------------------------------
		string[]   dscID = lsCol("uniqueDiscounts", "discountID").Split('~');
		string[] dscShip = lsCol("uniqueDiscounts", "shipToID"  ).Split('~');
		string[] dscName = lsCol("uniqueDiscounts", "custID"    ).Split('~');
		string[]  dscGrp = lsCol("uniqueDiscounts", "custGrp"   ).Split('~');
		string[]  dscVal = lsCol("uniqueDiscounts", "discAmnt"  ).Split('~');
		string[] dscRate = lsCol("uniqueDiscounts", "discRate"  ).Split('~');

		string  myCustID = Context.CustomerID;
		string myCustGrp = custGrp;
		string  myShipTo = getShipTo(ld.CustNum);

		decimal dscValCust=0,  dscValShip=0,  dscValGroup=0;
		decimal dscRateCust=0, dscRateShip=0, dscRateGroup=0;
	//-------------------------------------------!! DO NOT CHANGE !!---------


	//-- Set Condition/Multiplier for each discountID -----------------------
		decimal coupaSAI2  = 0;
		decimal coupaHAI2  = 0;
		decimal coupaNSkd  = 0; // ld.kNonSkid_c? 1: 0;
		decimal coupaCFFP  = 0; // ld.kCFplate_c? 1: 0;
		decimal coupaLiner = 0; // ld.kLiner_c? 1: 0;
		decimal coupaBoot  = 0; // kBoot? 1: 0;
		decimal coupaDAJ   = 0;
		decimal coupaTAM   = 0;
		decimal coupaChip  = 0; // ld.kDorsalChip_c && getSpec("ChipsIncluded")!="X"? 1: 0;
		decimal top22      = 1;
		decimal ottoWRI    = 1;  // All Ottobock orders 5% discount
		decimal ottoSAR    = 1;  // All Ottobock orders 5% discount
		decimal ottoABIL   = 1;  // All Ottobock orders 5% discount 
		decimal ottoACT    = 1;  // All Ottobock orders 5% discount
		decimal ottoJPO    = 1;  // All Ottobock orders 5% discount
		decimal nuaeBoot   = kBoot? 1: 0;
		decimal spineGOSS  = 0; 
		decimal coupaPLI2  = 0;
		decimal coupaART   = 0;
		decimal coupaSSA   = 0;
		decimal eurSTAB    = 0; 
		decimal eurLiner   = ld.kLiner_c? 1: 0;
		decimal eurNSkd    = ld.kNonSkid_c? 1: 0;
		decimal eurBoot    = kBoot? 1: 0;
		decimal eurWBase   = 0;
		decimal coupaTWMod = 0; // ld.kToeWalk_c? 1: 0;
		decimal coupaRushS = 0; // kRush && !ld.kRush0D_c && !ld.kRushNC_c? 1/dQty: 0;
		decimal coupa0DayS = 0; // ld.kRush0D_c && !ld.kRushNC_c? 1/dQty: 0;
		decimal coupaRushA = 0;
		decimal coupa0DayA = 0;
		decimal coupaSOL   = 0;
		decimal coupaBSA   = 0;
		decimal coupaSMO   = 0; // sDevKey=="SMO"? 1: 0;
		decimal coupaBIG   = 0; // getSpec("SPEC")=="BIGSHOT"? 1: 0;
		decimal coupaTLS   = 0;
		decimal coupaPUL   = 0;
		decimal coupaUCB   = 0; // sDevKey=="UCB"? 1: 0;
	//-------------------------!! Coupa Discounts Removed !!-----------------


	//-- Pair discountID w/ multiplier --------------------------------------
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
		}; 
	//---------------!! MAKE SURE MULTIPLIER MATCHES DISCOUNT ID ROW !!------


	//-- Loop thru arrays, add discounts on CustID, Group, ShipTo -----------

		if ( sAIO(dscName,custID) ) {
			for (int i = 0; i < kvp.Count; i++){
				if (custID == dscName[i]){
					dscValCust  += kvp[dscID[i]] * sDec(dscVal[i]);
					dscRateCust += kvp[dscID[i]] * sDec(dscRate[i]);
				}
			}
		} // Customer Level Discounts

		if ( sAIO(dscShip,shipTo) ) {
			for (int i = 0; i < kvp.Count; i++){
				if (shipTo == dscShip[i]){
					dscValShip  += kvp[dscID[i]] * sDec(dscVal[i]);
					dscRateShip += kvp[dscID[i]] * sDec(dscRate[i]);
				}
			}
		} // ShipTo Level Discounts

		if ( sAIO(dscGrp,custGrp) ) {
			for (int i = 0; i < kvp.Count; i++){
				if (custGrp == dscGrp[i]){
					dscValGroup  += kvp[dscID[i]] * sDec(dscVal[i]);
					dscRateGroup += kvp[dscID[i]] * sDec(dscRate[i]);
				}
			}
		} // CustGroup Level Discounts

		decimal discount = dscValCust  + dscValShip  + dscValGroup;
		decimal discRate = dscRateCust + dscRateShip + dscRateGroup;
	//-------------------------------------------!! DO NOT CHANGE !!---------


	//-- Overwrite Discounts on Special Intra-Company Prices ----------------
		if ( custGrp=="INC" && sDevKey!="UCB" && sDevKey!="TRA" ) {

			decimal NoDiscAmt = prcBase + ( ld.kPrepped_c? prcPrep: 0 );
			discount = 0.22m * ( prcUnit - NoDiscAmt );
			discRate = 0.00m;
		}
	//---------------------------------------- Removed Coupa Discounts ------

/*==========================================================================*/




/*== SET RETURN ARRAY ======================================================*/
	// UnitPrice[0], Pricing Detail[1], Discount[2], Discount Rate[3]

	string[] returnVals = {
		prcUnit.ToString(), 
		smo.ToString(), 
		discount.ToString(), 
		discRate.ToString()
	};

	return returnVals;

/*==========================================================================*/




/*== CHANGE LOG ==============================================================
	11/30/2022: Combined UDMethods getPriceCom, getDiscounts;
	01/05/2023: Add overwrite for new Hanger Discounts; Minor refactoring;
	01/19/2023: Old COUPA Discounts Removed;
	01/24/2023: Fix issue with discount on Prepped pricing & INC Discounts;
============================================================================*/