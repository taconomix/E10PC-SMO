/*== MethodRules-Operations-v6.0 =============================================

	Created: 08/18/2022 -Kevin Veldman
	Changed: 12/07/2022 -KV

	Info: Configurator Method Rule script for ALL operations
				Rule conditions should be "Always Execute"
	File: SMO-MethodRules-Operations-v6.0.cs
============================================================================*/

//__ FirstOp Action __________________________________________________________

	mvValStr = mrSetMethodVar(Context.JobNumber);

//__ Keep Rules v5 ___________________________________________________________
	
	int seqID = ( Context.Entity=="JobOper" )? JobOper.OprSeq: QuoteOpr.OprSeq;

	return UDMethods.mrKeepVals( seqID, mvValStr, Context.Entity );


//__ Rule Actions v5 _________________________________________________________

	Func<string,decimal,bool> kStrDec = (s,d) => decimal.TryParse(s, out d); 
	Func<string,decimal> dStr = s => kStrDec(s,0)? Convert.ToDecimal(s): -1;

	if ( Context.Entity == "JobOper" ) {

		string[] OpVals = UDMethods.mrOpVals(JobOper.OprSeq, mvValStr);
			// OpVals[]: { 0=OpCode, 1=OpDesc, 2=ProdStandard, 3=OpDate }

		if ( OpVals[0] != "" && OpVals[0] != JobOper.OpCode ) {
			JobOper.OpCode = OpVals[0];
			SetOperationMasterDefaults();
		}

		JobOper.OpDesc = OpVals[1];

		if ( dStr(OpVals[2]) >= 0 )	JobOper.ProdStandard = dStr(OpVals[2]);

		string[] OpsDue = PCLookUp.DataColumnList("opsDue","opsDue").Split('~');
		if ( Array.IndexOf(OpsDue, JobOper.OprSeq.ToString()) >= 0 ) 
			JobOper.DueDate = DateTime.Parse(OpVals[3]);
	}



/*== CHANGE LOG ==============================================================

	12/05/2022: Add method var mvValStr for relevant values to reduce queries;
	12/07/2022: Add method var mvBlockMR to handle early execution error;
	01/11/2023: Remove Method Variable, set Input from DocRules;
	01/12/2023: New OpVals method to replace 4 separate Methods;

============================================================================*/