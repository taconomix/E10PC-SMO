/*============================================================================
	Method: SMO-UDM_C-Combo-v1.0.0.cs

	Created: 05/30/2016
	Author:  Mike Mullaney
	Purpose: Generic Dynamic List: Table, Code, Display, Test, TestValue
              Pass in Lookup Table + Code, Display, Test Columns + TestValue
              Return a tilde delimited string for a combo box. 
              If no test required, enter "None"/"NONE" in Test or TestValue
============================================================================*/

string returnValue = "N`None - Check Table and Column Names";
int i = 0;

// Pull back data columns
string[]    CodeList = PCLookUp.DataColumnList(Table, Code).Split('~');
string[] DisplayList = PCLookUp.DataColumnList(Table, Display).Split('~');
string[]    TestList = new string[CodeList.Length];

// Check for testing required
if (Test.ToUpper() == "NONE" || TestValue.ToUpper() == "NONE") {
	
	TestValue = "NONE";
	TestList[0] = "No Test";

} else {

	TestList = PCLookUp.DataColumnList(Table, Test).Split('~');
}

// Create String
if (CodeList[0].Length >0 && TestList[0].Length >0 && DisplayList[0].Length >0) {  // Validate columns

	StringBuilder MyList = new StringBuilder();
	
	foreach (string item in CodeList) {

	  if (TestValue == "NONE" || TestList[i] == TestValue) {

	    if (DisplayList[i].ToUpper() != "ZZZ") {

	      MyList.Append(item).Append("`").Append(DisplayList[i]).Append("~");
	    } 
	  }
	  i++;
	}
	returnValue = ((MyList.ToString()).Length >0)?(MyList.ToString()).Substring(0,(MyList.ToString()).Length -1):("N`None");
}

return returnValue;









/*============================================================================

	Change Log:

		User:  Date:       Changes:
		KV     10/05/2022  Added to documentation

============================================================================*/
