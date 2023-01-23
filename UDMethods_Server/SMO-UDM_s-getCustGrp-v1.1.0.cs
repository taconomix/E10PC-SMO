/*== getCustGrp v1.1.0 =======================================================
	
	Created: 07/15/2022 -Kevin Veldman
	Changed: 01/12/2023 -KV

	File: SMO-UDM_s-getCustGrp-v1.1.0.cs
	Info: Pass in CustNum return GroupCode
============================================================================*/

var cust = Db.Customer.Where(x => 
	x.Company == Context.CompanyID && x.CustNum == CustNum).FirstOrDefault();

return ( cust == null ) ? "Not Found": cust.GroupCode;


/*== CHANGE LOG ==============================================================
	01/12/2023: Add lambda expression, shorten return syntax;
============================================================================*/