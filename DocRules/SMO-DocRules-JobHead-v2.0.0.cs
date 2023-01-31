/*== JobHead-v2.0.0 ==========================================================

	Created: 08/09/2022 -Kevin Veldman
	Changed: 01/25/2023 -KV

	Info: Set Modification Deadline date ( JobHead.UserDate4 )
	File: SMO-DocRules-JobHead-v2.0.0.cs
============================================================================*/

DateTime ModDate = Convert.ToDateTime(JobHead.CreateDate ?? DateTime.Today);

Action<int> DateCheck = (iDays) => {

	string[] Holiday = PCLookUp.DataColumnList("holidays","dates").Split('~');
	ModDate = ModDate.AddDays(iDays);

	for (int i = 0; i < Holiday.Length; i++) {
		if (Holiday[i] == ModDate.ToString("yyyyMMdd")) ModDate = ModDate.AddDays(1);
	}

	if (ModDate.DayOfWeek == DayOfWeek.Friday  ) ModDate = ModDate.AddDays(3);
	if (ModDate.DayOfWeek == DayOfWeek.Saturday) ModDate = ModDate.AddDays(2);
	if (ModDate.DayOfWeek == DayOfWeek.Sunday  ) ModDate = ModDate.AddDays(1);
};

Action<int> plusDays = (iDays) => {
	for (int i = 0; i < iDays; i++){
		DateCheck(1);
		DateCheck(0);
	}
};

	bool doMods = OrderDtl.ModType_c != "M" || OrderDtl.dMeas5_c > 3.125m;

	int daysAdded = ( !doMods || OrderDtl.kRush0D_c ) ? 0: 
	                ( OrderDtl.kRush1D_c            ) ? 1: 2 ;

	plusDays( daysAdded );


JobHead.UserDate4 = ModDate;


string MethodValTbl = "mrFieldsSMO";
string myJobNum = JobHead.JobNum;

JobHead.UserMapData = UDMethods.drGetMethodArray(myJobNum, MethodValTbl);

/*== CHANGE LOG ==============================================================
	09/14/2022: Add ProdQty, CreateDate, JobEngineered, JobReleased;
	09/15/2022: Revert to v1, extra JobHead fields unnecessary;
	01/02/2023: Minor refactoring; 
	01/25/2023: Fix Method Rules;
============================================================================*/