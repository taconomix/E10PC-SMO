/*== exec_kRush-v1.1.0 =======================================================

    Created: 08/03/2022 -Kevin Veldman
    Changed: 12/07/2022 -Kevin Veldman

    Info: If Rush is selected, show dialog box to toggle Same/Next-Day
    File: SMO-UDM_c-exec_kRush-v1.1.0.cs
============================================================================*/

if (Inputs.kRush.Value) {

    string updateTtl = "Rush Selected";
    string updateMsg = "Does this order need to go out today?";

    Func<string,string,DialogResult> mbYN = (sM,sT) => {
        var tmp = MessageBoxIcon.Question;
        return MessageBox.Show(sM,sT,MessageBoxButtons.YesNo,tmp);
    };

    DialogResult diResult0 = mbYN(updateMsg,updateTtl);

    Inputs.k0day.Value = diResult0 == DialogResult.Yes? true: false;

}

Inputs.mrValStr.Value = mrGetInputString();




/*== CHANGE LOG ==============================================================
    12/07/2022: Set mrValStr for Method Variable
============================================================================*/