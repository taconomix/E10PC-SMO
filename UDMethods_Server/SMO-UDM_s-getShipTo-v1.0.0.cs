/*== getShipTo-v1.0.0 ========================================================

	Created: 08/11/2022 -KV
	Changed: 

	Info: Pass in OrderNum and Return ShipToNum
	File: SMO-UDM_s-getShipTo-v1.0.0.cs
============================================================================*/

string returnValue = "Not Found";

Erp.Tables.OrderHed OrderHed = (from Row in Db.OrderHed 
	where Row.Company == "SS" && Row.OrderNum == OrderNum 
	select Row).FirstOrDefault();

if (OrderHed != null) returnValue = OrderHed.ShipToNum;

return returnValue;



/*== CHANGE LOG ==============================================================
	10/05/2022: Added to documentation;
============================================================================*/