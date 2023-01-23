/*== drGetShopDates-v1.3.0 =====================================================

	Created: 12/22/2022 -KV
	Changed: 01/23/2023 -KV

	Info: Get Promised/Turnaround Dates for job based on Configuration.
	File: SMO-UDM_s-drGetShopDates-v1.3.0.cs
============================================================================*/

//Linq statement to set ld to QuoteDtl|OrderDtl based on Context
	dynamic ld = 0; //Dynamic variable allows PcStatus functionality

	if (Context.Entity == "OrderDtl") {

		ld = Db.OrderDtl.Where(od => od.Company == Context.CompanyID
				&& od.OrderNum  == Context.OrderNumber
				&& od.OrderLine == Context.OrderLineNumber).FirstOrDefault();

	} else if (Context.Entity == "QuoteDtl") {

		ld = Db.QuoteDtl.Where(qd => qd.Company == Context.CompanyID
				&& qd.QuoteNum  == Context.QuoteNumber 
				&& qd.QuoteLine == Context.QuoteLineNumber).FirstOrDefault();
	}


//Variables
	DateTime dtTurn, dtProm, dtTmp;
	
	bool kModTime  = ld.ModType_c != "M";
	bool kBootTime = sKey == "SMO" && ld.kBoot_c;
	bool kRush     = ld.kRush0D_c || ld.kRush1D_c;
	bool k0day     = ld.kRush0D_c;

	dtTmp = getOrderDate(Context.Entity=="QuoteDtl"?ld.QuoteNum:ld.OrderNum);
	dtTmp = Convert.ToDateTime(Inputs.dtProdStart.Value ?? dtTmp);


//Functions to add days to start date, skipping weekends and holidays.
	Action<int> DateCheck = (iDays) => {

		string[] Holiday = PCLookUp.DataColumnList("holidays","2023").Split('~');
		dtTmp = dtTmp.AddDays(iDays);

		for (int i = 0; i < Holiday.Length; i++) {
			if (Holiday[i] == dtTmp.ToString("yyyyMMdd")) dtTmp = dtTmp.AddDays(1);
		}

		if (dtTmp.DayOfWeek == DayOfWeek.Friday  ) dtTmp = dtTmp.AddDays(3);
		if (dtTmp.DayOfWeek == DayOfWeek.Saturday) dtTmp = dtTmp.AddDays(2);
		if (dtTmp.DayOfWeek == DayOfWeek.Sunday  ) dtTmp = dtTmp.AddDays(1);
	};

	Action<int> plusDays = (iDays) => {

		for (int i = 0; i < iDays; i++){
			DateCheck(1);
			DateCheck(0);
		}
	};


// Modify Date variables as needed. 
	if (!kRush) {

		int iTurn = Convert.ToInt32(sLkp("DefaultSpecs", "TurnDays", sKey));
		plusDays(iTurn + (kModTime? 2: 0) + (kBootTime? 1: 0));
		dtTurn = dtTmp;

		int iProm = Convert.ToInt32(sLkp("Standards", "Value", "PromiseDays"));
		plusDays(iProm + (kModTime? 1:(sKey=="SMO" && iProm>2? 2-iProm: 0)));
		dtProm = dtTmp;

	} else {

		plusDays(k0day? 0: 1);
		dtTurn = dtTmp;
		dtProm = dtTmp;	
	}

	string[] returnVals = { 
		dtTurn.ToString("MM/dd/yyyy"), dtProm.ToString("MM/dd/yyyy") 
	};

	return returnVals;



/*== CHANGE LOG ==============================================================
	09/15/2022: Check for null when setting dtTmp to dtProdStart;
	12/22/2022: Rmv choose date w/ parameter -> Return string[] w/ both dates;
	01/02/2023: Flipped the dates, had them in wrong order;
	01/23/2023: sKey Parameter added;
============================================================================*/