/*== calcPriceDetail v1.0.1 ==================================================

	Created: 01/13/2023 -Kevin Veldman
	Changed: 01/19/2023 -KV
	
	File: SMO-UDM_c-calcPriceDtl-v1.0.1.cs
	Info: Set UnitPrice, SO Comment, Discount Values - CLIENT INPUTS ONLY
		!! MUST MANUALLY KEEP IN SYNC WITH UDMethods.getPriceDtl !!
============================================================================*/

/*== Global Functions & Variables ==========================================*/
	Func<string,bool> kpVal = s => ( s!="" && s.ToUpper()!="NONE" );
	Func<string,decimal,bool> kStrDec = (s,d) => decimal.TryParse(s, out d); 
	Func<string,decimal> dStr  = s => kStrDec(s,0)? Convert.ToDecimal(s): 0;
	Func<decimal,string> sDec = d => d.ToString();

	Func<string,string,string,string> sLkp = (t,c,r) => PCLookUp.DataLookup(t,c,r);
	Func<string,string> lkSpc = s => sLkp("DefaultSpecs", s, sKey);	
	Func<string,string,string,decimal> dLkp = (t,c,r) => dStr(sLkp(t,c,r));

	Func<string[],string,bool> sAIO = (a,s) => ( Array.IndexOf(a,s) >= 0 );
	Func<string,string,string> lsRow = (t,r) => PCLookUp.DataRowList(t,r);
	Func<string,string,string> lsCol = (t,c) => PCLookUp.DataColumnList(t,c);


	int custNum = Context.CustomerNumber;
	var custID  = Context.CustomerID;
	var custGrp = getCustGrp(custNum);
	var shipTo  = getShipTo(Convert.ToInt32(Inputs.dOrderNum.Value));

	if ( Context.Entity == "PcStatus" ) {
		if ( Inputs.cCustID.Value == "HANGER" ) {
			custNum = 1007;
			custID = "HANGER";
			custGrp = "INC";
			shipTo = "TOP SB";
		} else if ( Inputs.cCustID.Value == "EUR" ) {
			custGrp = "EUR";
		}
	}

	decimal dQty = Inputs.rQty.Value=="B"? 2: 1;
	bool kRush = Inputs.kRush.Value;
	bool kBoot = Inputs.kInnerBoot.Value && lkSpc("SPEC") != "BIGSHOT";
/*==========================================================================*/

/*== UnitPrice & Itemized OrderComment =====================================*/

	//-- Set Price & Desc Variables ------------------------------------
		StringBuilder smo = new StringBuilder();

		string[] Price = lsRow("PriceMaster", sKey  ).Split('~');
		string[] Desc  = lsRow("PriceMaster", "Desc").Split('~');
		string[] Dmstc = lsCol("dmstcGrps", "Groups").Split('~');

		Func<int,decimal> drPrc = i => dStr(Price[i]);

		decimal prcAlt  = dLkp("AltBasePrices", custGrp, sKey);
		decimal prcBase = prcAlt > 0? prcAlt: drPrc(3) + drPrc(4);
		decimal prcUnit = 0;

		decimal prcPrep = dLkp("Standards", "Value", "Prep") * -1;
		string  dscPrep = sLkp("Standards", "Desc" , "Prep");

		string  rushRow = (Inputs.k0day.Value? "0": "1") + "RushSMO";
		decimal prcRush = Inputs.kNCRush.Value?0:dLkp("Standards","Value",rushRow);
		string  dscRush = sLkp("Standards","Desc",rushRow);

		decimal dWdg = Inputs.dWedgeQty.Value;
		bool   kPrep = Inputs.rFinish.Value=="P";
	//-------------------------------------------------------------------


	//-- Functions to add lines & increment price -----------------------
		Action<int,bool> addLn0 = (i,k) => {
			if ( k ) {
				smo.Append( Desc[i].PadRight(28) );
				smo.Append( dQty.ToString() );
				smo.Append( Price[i].PadLeft(10) );
				smo.Append( (dQty*drPrc(i)).ToString().PadLeft(11) );
				smo.AppendLine();

				prcUnit += drPrc(i);
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


		addLn1( Price[0], true, 1, prcBase, 0 );        //Base Device
		addLn0( 11, Inputs.kToeWalk.Value     );        //Toe Walker Mods
		addLn0( 18, Inputs.kSpringPlate.Value );        //Spring Steel Soles
		addLn0( 17, Inputs.kCarbonPlate.Value );        //Carbon Fiber Soles
		addLn0( 21, Inputs.kNonSkid.Value     );        //Non-Skid Soles
		addLn0( 19, Inputs.kDorsalChip.Value  );        //Dorsal Chips
		addLn0( 10, kBoot );                            //Inner Boot
		addLn0( 12, Inputs.kLiner.Value   );            //Integrated Liner
		addLn0( 5,  Inputs.kSTPad.Value   );            //ST Pads
		addLn0( 6,  Inputs.kMetPad.Value  );            //Metatarsal Pads
		addLn0( 31, Inputs.kPlateau.Value );            //Toe Plateaus
		addLn0( 32, Inputs.kCutout.Value  );            //Extra Cutouts
		addLn1( Desc[33], dWdg>0, dWdg, drPrc(33), 1 ); //Heel Wedge
		addLn0( 35, Inputs.kAppScanFee.Value  );        //Scan Submission Fee
		addLn1( dscPrep, kPrep, 1, prcPrep, 0 );        //Prep Discount
		addLn1( dscRush, kRush, 1, prcRush, 1 );        //Rush Charge


		smo.AppendLine();
		smo.Append("Total Price".PadRight(28));
		smo.Append(sDec(dQty));
		smo.Append(sDec(prcUnit).PadLeft(10));
		smo.Append(sDec(dQty * prcUnit).PadLeft(11));
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
		string[]   dscID = lsCol("uniqueDiscounts","discountID").Split('~');
		string[] dscShip = lsCol("uniqueDiscounts","shipToID"  ).Split('~');
		string[] dscName = lsCol("uniqueDiscounts","custID"    ).Split('~');
		string[]  dscGrp = lsCol("uniqueDiscounts","custGrp"   ).Split('~');
		string[]  dscVal = lsCol("uniqueDiscounts","discAmnt"  ).Split('~');
		string[] dscRate = lsCol("uniqueDiscounts","discRate"  ).Split('~');

		decimal dscValCust=0,  dscValShip=0,  dscValGroup=0;
		decimal dscRateCust=0, dscRateShip=0, dscRateGroup=0;
	//-------------------------------------------!! DO NOT CHANGE !!---------


	//-- Set Condition/Multiplier for each discountID -----------------------
		decimal coupaSAI2  = 0;
		decimal coupaHAI2  = 0;
		decimal coupaNSkd  = 0; //Inputs.kNonSkid.Value? 1: 0;
		decimal coupaCFFP  = 0; //Inputs.kCarbonPlate.Value? 1: 0;
		decimal coupaLiner = 0; //Inputs.kLiner.Value? 1: 0;
		decimal coupaBoot  = 0; //kBoot? 1: 0;
		decimal coupaDAJ   = 0;
		decimal coupaTAM   = 0;
		decimal coupaChip  = 0; //Inputs.kDorsalChip.Value && lkpSpecs("ChipsIncluded")!="X"? 1: 0;
		decimal top22      = 1;
		decimal ottoWRI    = 1;
		decimal ottoSAR    = 1;
		decimal ottoABIL   = 1;
		decimal ottoACT    = 1;
		decimal ottoJPO    = 1;
		decimal nuaeBoot   = kBoot? 1: 0;
		decimal spineGOSS  = 0;  // TLSO Only
		decimal coupaPLI2  = 0;
		decimal coupaART   = 0;
		decimal coupaSSA   = 0;
		decimal eurSTAB    = 0; //Stabilizer only
		decimal eurLiner   = Inputs.kLiner.Value?1:0;
		decimal eurNSkd    = Inputs.kNonSkid.Value?1:0;
		decimal eurBoot    = kBoot? 1: 0;
		decimal eurWBase   = 0;
		decimal coupaTWMod = 0; //Inputs.kToeWalk.Value? 1: 0;
		decimal coupaRushS = 0; //prcRush > 0 && !Inputs.k0day.Value? 1/dQty: 0;
		decimal coupa0DayS = 0; //prcRush > 0 &&  Inputs.k0day.Value? 1/dQty: 0;
		decimal coupaRushA = 0;
		decimal coupa0DayA = 0;
		decimal coupaSOL   = 0;
		decimal coupaBSA   = 0;
		decimal coupaSMO   = 0; //sKey=="SMO"? 1: 0;
		decimal coupaBIG   = 0; //lkpSpecs("SPEC")=="BIGSHOT"? 1: 0;
		decimal coupaTLS   = 0;
		decimal coupaPUL   = 0;
		decimal coupaUCB   = 0; //sKey=="UCB"? 1: 0;
	//-------------------------!! Update when Table is changed !!------------


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
	
		if (Array.IndexOf(dscName,custID) != -1) {
			for (int i = 0; i < kvp.Count; i++){
				if (custID == dscName[i]){
					dscValCust  += kvp[dscID[i]] * dStr(dscVal[i]);
					dscRateCust += kvp[dscID[i]] * dStr(dscRate[i]);
				}
			}
		} // Customer Level Discounts

		if (Array.IndexOf(dscShip,shipTo) != -1) {
			for (int i = 0; i < kvp.Count; i++){
				if (shipTo == dscShip[i]){
					dscValShip  += kvp[dscID[i]] * dStr(dscVal[i]);
					dscRateShip += kvp[dscID[i]] * dStr(dscRate[i]);
				}
			}
		} // ShipTo Level Discounts

		if (Array.IndexOf(dscGrp,custGrp) != -1) {
			for (int i = 0; i < kvp.Count; i++){
				if (custGrp == dscGrp[i]){
					dscValGroup  += kvp[dscID[i]] * dStr(dscVal[i]);
					dscRateGroup += kvp[dscID[i]] * dStr(dscRate[i]);
				}
			}
		} // CustGroup Level Discounts

		decimal discount = dscValCust  + dscValShip  + dscValGroup;
		decimal discRate = dscRateCust + dscRateShip + dscRateGroup;
	//-------------------------------------------!! DO NOT CHANGE !!---------

	
	//-- Overwrite Discounts on Special Intra-Company Prices ----------------
		if ( custGrp=="INC" && sKey!="UCB" && sKey!="TRA" ) {

			discount = ( prcUnit - (prcBase + prcPrep) ) * 0.22m;
			discRate = 0.00m;
		}
	//--------------------------------------?? Remove Coupa Discounts ??-----


	//-- Send Pop-Up Message if Table Rows > Declared Discount Vars ---------
		if (dscID.Length > kvp.Count) 
			MessageBox.Show("New discounts not applied. Please alert an Epicor admin.");
	//-----------------------------------------------------------------------

/*==========================================================================*/



/*== SET CLIENT INPUT VALUES ===============================================*/
	// Unit Price, Price Detail, Discount ($), Discount (%);

	Inputs.dUnitPrice.Value = prcUnit;
	Inputs.eShowInfo.Value  = smo.ToString();
	Inputs.dDiscount.Value  = discount;
	Inputs.dDiscRate.Value  = discRate;
/*==========================================================================*/




/*== CHANGE LOG ==============================================================

	01/13/2023: Combine UDMethods calcComment, calcDiscount;
	01/19/2023: PcStatus ability to test INC/EUR customer pricing;

============================================================================*/