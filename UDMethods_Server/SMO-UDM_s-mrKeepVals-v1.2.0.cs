/*== mrKeepVals v1.2.0 ==========================================================

	Created: 01/12/2023 -Kevin Veldman
	Changed: 01/19/2023

	Info: Pass in seqID, mrValStr, Context.Entity, return keep rule value;
	File: SMO-UDM_s-mrKeepVals-v1.2.0.cs
============================================================================*/

//__ Global Functions/Variables __________________________________________
	bool isOper = !thisContext.ToUpper().Contains("MTL");
	string[] od = TblValStr.Split('~');

	Func<string,decimal,bool> kStrDec = (s,d) => decimal.TryParse(s, out d); 
	Func<string,int,    bool> kStrInt = (s,i) => int.TryParse(s, out i); 

	Func<string,decimal> dStr = s => kStrDec(s,0)? Convert.ToDecimal(s): 0;
	Func<string,int    > iStr = s => kStrInt(s,0)? Convert.ToInt32(s)  : 0;
	Func<string,bool   > kStr = s => ( s == "1" || s.ToLower() == "true");
	
	Func<string,string,string> lsRow = (t,r) => PCLookUp.DataRowList(t,r);
	Func<string,string,string> lsCol = (t,c) => PCLookUp.DataColumnList(t,c);

	Func<int,bool   > kOD = i => kStr( od[i] );
	Func<int,decimal> dOD = i => dStr( od[i] );
	Func<int,int    > iOD = i => iStr( od[i] );

	Func<string,string> lkDS = s => sLkp("DefaultSpecs",s,od[0]);


//__ Operation Keep Rules ________________________________________________
	if ( isOper ) {

		int[] alwaysTrue = 
			{ 1,195,200,210,249,250,300,310,320,325,349,350,600,610,950,998,999 };

		if ( Array.IndexOf(alwaysTrue, seqID) >= 0 ) return true;

		switch ( seqID ) { //if no case then false

			case 400: //Inner Boot Pull
				return ( (lkDS("SPEC")=="BIGSHOT") || (kOD(2) || kOD(3)) );
				break; 

			case 699: //Post-Prep QC
				return !kOD(4);
				break; 

			case 750: //Finish Nonskid/FootPlate
				return kOD(5) || kOD(6) || kOD(7);
				break; 

			default:
				return false;
				break;
		}
	} 


//__ Material Keep Rules _________________________________________________
	else {

		int[] noRule = { 10, 110, 200, 210 };
		if ( Array.IndexOf( noRule, seqID ) >= 0 ) return true;
		
		int[] kpStrp = { 50, 60, 70, 90, 100 };
		if ( Array.IndexOf( kpStrp, seqID ) >= 0 ) 
			return !kOD(4) && (lkDS("SPEC")!="UCBL");

		switch ( seqID ) {
			case 20:
				return !kOD(3);
				break;
			
			case 30:
				return lkDS("SPEC")=="BIGSHOT" || kOD(2);
				break;
			
			case 40:
				return od[13].Length > 0; //Pattern
				break;
			
			case 80:
				return kOD(14); //Tall Ears
				break;
			
			case 120:
				return kOD(5); //Nonskid
				break;
			
			case 130:
				return kOD(15); //Dorsal Chips
				break;
			
			case 140:
				return kOD(6) || kOD(7); //Carbon Fiber or Spring Steel footplate
				break;
			
			case 170:
				return !kOD(4) && !kOD(9); //No prep no Liner
				break;			
			
			case 180:
				return kOD(16); //MetPads
				break;
			
			case 190:
				return kOD(17); //STPads
				break;
			
			case 220:
				return od[18]!="L" && kOD(19); //Right Plateau
				break;
			
			case 230:
				return od[18]!="R" && kOD(19); //Left Plateau
				break;
			
			default:
				return false;
				break;
		}
	}



/*== CHANGE LOG ==============================================================
	
	01/12/2023: Combined mrKpMtls and mrKeepOpr, add Context.Entity as Param;
	01/19/2023: Minor change to Global Functions;

============================================================================*/



	/*__ mrFieldsSMO Table Reference _______________________________

		0   str  cDeviceCode_c
		1   str  ModType_c
		2   bit  kBoot_c
		3   bit  kHeelCut_c
		4   bit  kPrepped_c
		5   bit  kNonSkid_c
		6   bit  kCFplate_c
		7   bit  kSSplate_c
		8   dec  dMeas5_c
		9   bit  kLiner_c
		10  int  OrderNum
		11  bit  kRush0D_c
		12  bit  kRush1D_c
		13  str  cPattern_c
		14  bit  kTallEars_c
		15  bit  kDorsalChip_c
		16  bit  kMetPads_c
		17  bit  kSTPads_c
		18  str  cSBLR_c
		19  bit  kPlateau_c
		20  dec  dMeas6_c
		21  str  cPlasticDuro_c
		22  str  cPlasticType_c
		23  str  cStrapColor_c
		24  str  cBoot_c
		25  str  cChafeType_c
		26  str  cShoeSize_c
		27  bit  kToeWalk_c
	______________________________________________________________*/