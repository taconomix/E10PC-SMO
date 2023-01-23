/*== OnPageLoaded 2023 01 23 =================================================

	Created: 10/13/2016 -Kevin Veldman
	Changed: 01/23/2023 -KV
	
	File: SMO-OnPageLoaded-Exp.cs
	Info: Various actions and variables set when Config client is loaded.
============================================================================*/

// Inputs.kBlockMethodRules.Value = true;
Inputs.blockMV.Value = true;
string sType = Context.Entity;
decimal oNum = sType == "QuoteDtl"? Context.QuoteNumber: Context.OrderNumber;


if (sType != "PcStatus") {

	UDMethods.ImportFromLineDtl();
	//UDMethods.mrSetInputString();
	Pages.Page1.Size = new Size(1045,725);

} else {
	// Set visibility for testing. 
		Inputs.d1rValue.Invisible  = false;
		Inputs.d2rValue.Invisible  = false;
		Inputs.d3rValue.Invisible  = false;
		Inputs.d4rValue.Invisible  = false;
		Inputs.d5rValue.Invisible  = false;
		Inputs.d6rValue.Invisible  = false;
		Inputs.d7rValue.Invisible  = false;
		Inputs.d8rValue.Invisible  = false;
		Inputs.d9rValue.Invisible  = false;
		Inputs.d10rValue.Invisible = false;
		Inputs.d11rValue.Invisible = false;
		Inputs.d12rValue.Invisible = false;
		Inputs.d13rValue.Invisible = false;
		Inputs.d1Value.Invisible   = false;
		Inputs.d2Value.Invisible   = false;
		Inputs.d3Value.Invisible   = false;
		Inputs.d4Value.Invisible   = false;
		Inputs.d5Value.Invisible   = false;
		Inputs.d6Value.Invisible   = false;
		Inputs.d7Value.Invisible   = false;
		Inputs.d8Value.Invisible   = false;
		Inputs.d9Value.Invisible   = false;
		Inputs.d10Value.Invisible  = false;
		Inputs.d11Value.Invisible  = false;
		Inputs.d12Value.Invisible  = false;
		Inputs.d13Value.Invisible  = false;
		Inputs.dOrderNum.Invisible = false;
		Inputs.dStrapQty.Invisible = false;
		Inputs.eShowInfo.Invisible = false;
		Inputs.k0day.Invisible     = false;
		Inputs.kModTime.Invisible  = false;
		Inputs.kKafoTime.Invisible = false;
		Inputs.kShellTime.Invisible   = false;
		Inputs.kBootTime.Invisible    = false;
		Inputs.dtPromised.Invisible   = false;
		Inputs.kClientRules.Invisible = false;
		Inputs.cCustID.Invisible      = false;
		Inputs.rCFabSS.Invisible      = false;
		Inputs.dtOrderDate.Invisible  = false;
		Inputs.dDiscount.Invisible    = false;
		Inputs.dDiscRate.Invisible    = false;
		Inputs.dtTurnDate.Invisible   = false;
		Inputs.cMold3.Invisible       = false;
		Inputs.dtProdStart.Invisible  = false;
		Inputs.btnTester.Invisible    = false;
		Inputs.epiPcGroupBox5.Invisible = false;
		Inputs.kMoldRW.Invisible      = false;
}

Inputs.dOrderNum.Value = Convert.ToDecimal(oNum);
Inputs.cCustID.Value = Context.CustomerID;
Inputs.dtOrderDate.Value = UDMethods.getOrderDate(Convert.ToInt32(oNum));
	
if (Inputs.dtOrderDate.Value == null) Inputs.dtOrderDate.Value = DateTime.Today;
if (Inputs.dtProdStart.Value == null) Inputs.dtProdStart.Value = Inputs.dtOrderDate.Value;

// Set font for eShowInfo
Infragistics.Win.Appearance TextCustom = new Infragistics.Win.Appearance();
TextCustom.FontData.SizeInPoints = 9;
TextCustom.FontData.Name = "Courier New";

foreach (Control ctrl in Inputs.eShowInfo.Control.Parent.Controls) {

	if (ctrl.Name.ToString().Contains("eShowInfo")== true) {

		((Ice.Lib.Framework.EpiTextBox)ctrl).UseAppStyling = false;
		((Ice.Lib.Framework.EpiTextBox)ctrl).Appearance = TextCustom;
    }
}

// Set background for Converter mm Label
Infragistics.Win.Appearance lblColor = new Infragistics.Win.Appearance();
lblColor.BackColor = System.Drawing.ColorTranslator.FromHtml("#A7C6CB");
lblColor.ForeColor = System.Drawing.Color.White;

foreach (Control ctl in Inputs.lblConv.Control.Parent.Controls) {

	if (ctl.Name.ToString().Contains("lblConv")) {

		((Ice.Lib.Framework.EpiLabel)ctl).UseAppStyling = false;
		((Ice.Lib.Framework.EpiLabel)ctl).Appearance = lblColor;
	}
} 

// Set background for Converter
Infragistics.Win.Appearance boxColor = new Infragistics.Win.Appearance();
boxColor.BackColor = System.Drawing.ColorTranslator.FromHtml("#A7C6CB");
boxColor.ForeColor = System.Drawing.ColorTranslator.FromHtml("#A7C6CB");

foreach (Control ctl in Inputs.boxConv.Control.Parent.Controls) {

	if (ctl.Name.ToString().Contains("boxConv")) {

		((Ice.Lib.Framework.EpiGroupBox)ctl).UseAppStyling = false;
		((Ice.Lib.Framework.EpiGroupBox)ctl).Appearance = boxColor;
	}
}

Inputs.epiPcGroupBox2.Control.SendToBack();









/*== CHANGE LOG ==============================================================

	08/30/2022: Update OrderDate setting and other date variables.
	10/04/2022: Set color for new conversion label. 
	10/27/2022: Testing visibility of background inputs. 
	12/06/2022: Add kBlockMethodRules to prevent early execution error;
	01/09/2023: Remove setMeasInputs, move BlockMR Input below Import
	01/11/2023: Move blockMR to top, add mrSetInputString;
	01/23/2023: New blockMR;

============================================================================*/