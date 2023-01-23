/*== MethodRules-Materials-v5.0 ==============================================

	Created: 08/18/2022 -Kevin Veldman
	Changed: 01/23/2023 -KV

	Info:  Method Rule script for ALL materials;
				Rule conditions should be "Always Execute";
	File: SMO-MethodRules-Materials-v5.0.cs
============================================================================*/

//__ Keep Rules v5 ___________________________________________________________

	int seqID = ( Context.Entity=="JobMtl" )? JobMtl.MtlSeq: QuoteMtl.MtlSeq;

	return UDMethods.mrKeepVals( seqID, mvValStr, Context.Entity );


//__ Rule Actions v5 _________________________________________________________

	Func<string,decimal,bool> kStrDec = (s,d) => decimal.TryParse(s, out d); 
	Func<string,decimal> dStr = s => kStrDec(s,0)? Convert.ToDecimal(s): -1;

	if ( Context.Entity == "JobMtl" ) {

		string[] MtlVals = UDMethods.mrMtlVals(JobMtl.MtlSeq, mvValStr);
			// MtlVals[]: { 0=PartNum, 1=Quantity, 2=UoM }

		if ( MtlVals[0]!="" && MtlVals[0]!=JobMtl.PartNum ) {
			JobMtl.PartNum = MtlVals[0];
			SetPartDefaults();
		}

		if ( dStr(MtlVals[1]) >= 0 ) JobMtl.QtyPer = dStr(MtlVals[1]);

		if ( MtlVals[2] != "" ) JobMtl.IUM = MtlVals[2];
	}



/*== CHANGE LOG =============================================================
	12/06/2022: Update for new methods, reduce LINQ queries;
	12/07/2022: Add method var mvBlockMR to handle early execution error;
	01/12/2023: Use new mrMtlVals instead of old Part/Qty/UOM methods;
	01/23/2023: Method Variable instead of Input Value;
	7519471
============================================================================*/