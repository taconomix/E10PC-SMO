/*== getOrderDate v1.2.0 =====================================================
	
	Created: 07/15/2022 -Kevin Veldman
	Changed:

	File: SMO-UDM_s-getOrderDate-v1.2.0.cs
	Info: Pass in OrderNum and get Order/QuoteDate
============================================================================*/

if (Context.Entity == "OrderDtl") {
	return Db.OrderHed.Where(x => x.Company == Context.CompanyID 
		&& x.OrderNum == OrderNum).FirstOrDefault().OrderDate;

} else if (Context.Entity == "QuoteDtl") {
	return Db.QuoteHed.Where(x => x.Company == Context.CompanyID 
		&& x.QuoteNum == OrderNum).FirstOrDefault().EntryDate;

} else {
	return DateTime.Now;
}




/*== CHANGE LOG ==============================================================

	01/12/2023: Add query for QuoteHed.EntryDate;

============================================================================*/