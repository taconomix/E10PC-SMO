/*============================================================================
	Method: SMO-Expr-dConvMM.cs

	Created: 10/04/2022
	Author:  Kevin Veldman
	Purpose: Client-page MM to Inches converter
============================================================================*/


decimal dNmr, dDtr, dIN = Math.Round(Inputs.dConvMM.Value * 0.03937m *16)/16;

decimal dInt = Math.Floor(dIN);
decimal dMod = dIN - dInt;

string sAmerican = String.Empty;

if (dMod == 0) {

	sAmerican = String.Format("{0}''", dInt);

} else {
	dNmr = Math.Round(dMod * 16);
	dDtr = 16;

	while (dNmr % 2 == 0) {
		dNmr = dNmr/2;
		dDtr = dDtr/2;
	}

	sAmerican = String.Format("{0} {1}/{2}''",dInt,dNmr,dDtr);
}

Inputs.cConvInch.Value = sAmerican;






/*============================================================================

	Change Log:

		User:  Date:     Changes:

============================================================================*/