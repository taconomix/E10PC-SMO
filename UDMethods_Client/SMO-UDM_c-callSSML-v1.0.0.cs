/*============================================================================
	Method: SMO-UDM_C-callSSML-v1.0.0.cs

	Created: 08/26/2021
	Author:  Dylan Anderson (CodaBears)
	Purpose: Call External BAQ w/ Measurement parameters, return Mold IDs
============================================================================*/

//Name of desired BAQ
	object[] @BAQID = new object[] {"Select-Mold-SMO"};

//Define Transaction Scope. Any text box input should work.
	var OTrans = ((InputControlValueBound<EpiTextBox, string>)Inputs["eShowInfo"].Value).Control.EpiTransaction;

//Adapt BAQ to Dynamic Query (c#), get it's parameters and types.
	string dqa = "DynamicQueryAdapter";
	string getQEPs = "GetQueryExecutionParametersbyID";
	var BAQ = Ice.Lib.Framework.AdapterHelper.GetAdapterInstance(OTrans as ILaunch, dqa);
	var ExeParm = ProcessCaller.InvokeAdapterMethod(OTrans, dqa, getQEPs, @BAQID);
	var ep = ExeParm.GetType().GetProperty("ExecutionParameter").GetValue(ExeParm, null);
	var ClearMethod = ep.GetType().GetMethod("Clear");
		ClearMethod.Invoke(ep, null);

//Method to add parameters based on our ep method
	string addEPRow = "AddExecutionParameterRow";
	var AddParamMethod = ep.GetType().GetMethod(addEPRow, new Type[] 
		{typeof(string),typeof(string),typeof(string),typeof(bool),typeof(Guid),typeof(string)});

//Define objects and send to BAQ

	Action<Object[],string,decimal> sendDecParam = (myObj,sParam,dInput) => {
		myObj = new object[] 
			{sParam, dInput.ToString(), "decimal", false, Guid.NewGuid(), "A"}; 
		AddParamMethod.Invoke(ep, myObj);
	};

	Action<Object[],string,string> sendStrParam = (myObj,sParam,sInput) => {
		myObj = new object[] 
			{sParam, sInput, "nvarchar", false, Guid.NewGuid(), "A"}; 
		AddParamMethod.Invoke(ep, myObj);
	};

	object[] @meas5 = new object[6];
	object[] @meas6 = new object[6];
	object[] @circ1 = new object[6];
	object[] @circ2 = new object[6];
	object[] @circ3 = new object[6];
	object[] @circ4 = new object[6];

	sendDecParam(@meas5, "d5Val", Inputs.d5Value.Value);
	sendDecParam(@meas6, "d6Val", Inputs.d6Value.Value);
	sendDecParam(@circ1, "d1Val", Inputs.d1Value.Value);
	sendDecParam(@circ2, "d2Val", Inputs.d2Value.Value);
	sendDecParam(@circ3, "d3Val", Inputs.d3Value.Value);
	sendDecParam(@circ4, "d4Val", Inputs.d4Value.Value);

	//Object Array to hold all Parameters
		object[] @paramsBAQ = new object[] {"Select-Mold-SMO", ExeParm};


//Call the BAQ with the defined Parameters
	var ExecuteMethod = BAQ.GetType().GetMethod("ExecuteByID", new Type[] {
		typeof(string), ExeParm.GetType()});
		ExecuteMethod.Invoke(BAQ, @paramsBAQ);
	var results = BAQ.GetType().GetProperty("QueryResults").GetValue(BAQ, null) as System.Data.DataSet;
	var ResultTable = results.Tables["Results"];


//Return the results of the BAQ for CSR and Reporting
	string MoldIDs = "AdjustedSSML_MoldID";
	StringBuilder molds = new StringBuilder();

	for (int i = 0; i < ResultTable.Rows.Count; i++) {
		string moldID = ResultTable.Rows[i][MoldIDs].ToString();
		Inputs.cMold1.Value = i==0? moldID: Inputs.cMold1.Value;
		Inputs.cMold2.Value = i==1? moldID: Inputs.cMold2.Value;
		Inputs.cMold3.Value = i==2? moldID: Inputs.cMold3.Value;
		molds.Append(moldID).AppendLine();
	}

if (ResultTable.Rows.Count == 0)
	MessageBox.Show("No matching SMO molds found. Please send to the lab for Selection.");




	/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		Syntax for adding param objects

			object[] @anyName = new object[] {

				"BAQparam",     // Must match the param name in the BAQ
				InputVal,       // The value to set that param. Must be string
				"decimal",      // Param type from BAQ
				false,          // Leave as false
				Guid.NewGuid(), // Leave as Guid.NewGuid()
				"A"             // Leave as "A"
			}
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/






/*============================================================================

	Change Log:

		User:  Date:       Changes:
		KV     10/05/2022  Added to documentation

============================================================================*/

