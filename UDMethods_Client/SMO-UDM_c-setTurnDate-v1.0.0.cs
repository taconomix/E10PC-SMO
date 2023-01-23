/*== setTurnDate v1.1.0 ======================================================

	Created: 08/03/2022 -Kevin Veldman
	Changed: 

	File: SMO-UDM_c-setTurnDate-v1.1.0.cs
	Info: Set dtPromDate and dtTurnDate based on Input Values
============================================================================*/

//__ Global Functions, variables _________________________________________
	Func<string,decimal,bool> kStrDec = (s,d) => decimal.TryParse(s, out d); 
	Func<string,decimal> dStr = s => kStrDec(s,0)? Convert.ToDecimal(s): 0;

	Func<string,string,string,string> sLkp = (t,c,r) => PCLookUp.DataLookup(t,c,r);
	Func<string,string,string,decimal> dLkp = (t,c,r) => dStr(sLkp(t,c,r));
	Func<string,string,string> lsCol = (t,c) => PCLookUp.DataColumnList(t,c);

	DateTime dtTmp = Convert.ToDateTime(Inputs.dtProdStart.Value);
	string sKey = Inputs.bStyle.Value;
//________________________________________________________________________


//__ Set Input fields for extra time added _______________________________
	bool kModTime = Inputs.rCastMeas.Value!="M";
	bool bootTime = Inputs.kInnerBoot.Value && sKey!="BIG" && sKey!="BSL";
//________________________________________________________________________


//__ Function to increment dtTmp _________________________________________
	Action<int> plusDays = (iDays) => {

		Action<int> DateCheck = (checkDays) => {

			string[] Holiday = lsCol("holidays","2023").Split('~');
			dtTmp = dtTmp.AddDays(checkDays);

			for (int i = 0; i < Holiday.Length; i++) {
				if (Holiday[i] == dtTmp.ToString("yyyyMMdd")) dtTmp = dtTmp.AddDays(1);
			}

			if ( dtTmp.DayOfWeek == DayOfWeek.Friday  ) dtTmp = dtTmp.AddDays(3);
			if ( dtTmp.DayOfWeek == DayOfWeek.Saturday) dtTmp = dtTmp.AddDays(2);
			if ( dtTmp.DayOfWeek == DayOfWeek.Sunday  ) dtTmp = dtTmp.AddDays(1);

		};


		for (int i = 0; i < iDays; i++){
			
			DateCheck(1);
			DateCheck(0); // Run function twice for Monday Holidays.
		}
	};
//________________________________________________________________________


//__ Find date based on inputs and set client Date fields ________________
	if (!Inputs.kRush.Value) {

		int iTurns = Convert.ToInt32(lkpSpecs("TurnDays"));

		if (Inputs.rCastMeas.Value != "M") iTurns += 2;
		iTurns += Inputs.rCastMeas.Value != "M"? 2: 0;
		iTurns += Inputs.kBootTime.Value?  1: 0;

		plusDays(iTurns);
		Inputs.dtTurnDate.Value = dtTmp;


		int iProm = Convert.ToInt32(dLkp("Standards","Value","PromiseDays"));

		iProm += (sKey == "SMO" && !Inputs.kModTime.Value && iProm > 2)? 2-iProm: 0;
		iProm += Inputs.kModTime.Value? 1: 0;

		plusDays(iProm);
		Inputs.dtPromised.Value = dtTmp;

	} else {

		plusDays(Inputs.k0day.Value? 0: 1);
		Inputs.dtTurnDate.Value = dtTmp;
		Inputs.dtPromised.Value = dtTmp;	
	}
//________________________________________________________________________




/*== Change Log ==============================================================

	01/19/2023: Change to match Server-Side calculations;

============================================================================*/