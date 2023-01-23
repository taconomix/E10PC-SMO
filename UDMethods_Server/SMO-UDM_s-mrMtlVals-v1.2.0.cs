/*== mrMtlVals v1.2.0 =====================================================
	
	Created: 01/09/2023 -Kevin Veldman
	Changed: 01/19/2023 -KV

	File: SMO-UDM_s-mrMtlVals-v1.2.0.cs
	Info: Pass in MtlSeq & mvValStr, return PartNum, Qty, UOM;
============================================================================*/

//__ Global Function and Variables _______________________________________
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

	decimal d6 = dOD(20);
	decimal d5 = dOD(8);


//__ Part Number _________________________________________________________
	string returnPart = "";

	int[] noPNRule = { 70,90,110,150,160,200,210 };

	if ( Array.IndexOf(noPNRule,iMtlSeq) < 0 ) {

		string strapSize = dOD(20)>3 && dOD(20)<=4? "75-16": "100-20";
		
		bool kBig = lkDS("SPEC") == "BIGSHOT";
		bool kBlk = ( od[23] == "Black") ? true : od[13].Contains("CAMO");

		switch (iMtlSeq) {
		
			case 10:
				string plDuro = od[21], plType = od[22];
				bool kWide = plDuro=="332" || plDuro=="18";
				string szSheet = plType=="SSW"? (kWide? "16X24": "12X16"): "";
				returnPart = sLkp("PlasticPN", plType, plDuro) + szSheet;
				break;
		
			case 20: 
				int szHeel = ( lkDS("SPEC") == "BIGSHOT" ) ? 4 : 
				             ( d6>4.25m                  ) ? 3 : 
				             ( d6>3.25m || od[0]=="TRA"  ) ? 2 : 1;
				returnPart = sLkp("HeelPosts", (kBig? "BIG": "SMO"), szHeel.ToString());
				break;
	
			case 30: // 24==Boot
				returnPart = ( od[24] != "NONE" ) ? od[24]: ""; 
				break;
		
			case 40: // 13==Pattern
				returnPart = od[13]; 
				break;
		
			case 50: // 23==Strap Color
				returnPart = sLkp("Straps", strapSize, od[23]); 
				break;
		
			case 60: // 25==Chafe Type
				string chafeCol = sLkp("ChafeType", ( kBlk?"Black":"White" ), od[25]); 
				returnPart = sLkp("ChafePN", chafeCol, strapSize);
				break;
		
			case 80:
				returnPart = "STRAP-TOP-ELASTIC";
				break;
		
			case 100:			
				returnPart = "PAD-" + ( (d6>3 && d6<=4)? "15X15x34": "15X175x01" ) + "-NEO";
				break;
		
			case 120: // 26==Shoe Size
				returnPart = sLkp("FootplatePN", ( kBlk? "NSB": "NSC" ), od[26]); 
				break;
		
			case 130:
				returnPart = "SSDORSALCHIP-" + (d5>2.625m? "LG": ( d5>2.25m? "MD": "SM" ));
				break;
		
			case 140:
				returnPart = sLkp("FootplatePN", ( kOD(6)? "CF": "SS" ), od[26] ); 
				break; // 6==CFPlate, 26==Shoe Size
		
			case 170:  // 27==ToeWalk
				int szPad = 1;
				szPad += d6 > 3.0m? 1: 0;
				szPad += d6 > 4.5m? 1: 0;
				szPad += d6 > 7.0m? 1: 0;
				szPad += d6 > 9.0m? 1: 0;
				string padCol = kBig? "BIG":(kOD(27)? "TWMod": (szPad>3? "BIG": "SMO"));
				returnPart = sLkp("AnklePads", padCol, szPad.ToString());
				break; // 27==kToeWalk
		
			case 180:
				string szMets = d6<2.5m && d6>0? "SMALL": (d6>3.0m? "LRG": "MED");
				returnPart = "METPAD-" + szMets;
				break;
		
			case 190:
				returnPart = "STPAD";
				break;

			case 220:
			case 230:
				int szPlat = 1;
				string sRL = iMtlSeq==220? "RT": "LT";
				szPlat += d5 >= 1.750m? 1: 0;
				szPlat += d5 >= 2.125m? 1: 0;
				szPlat += d5 >= 2.500m? 1: 0;
				szPlat += d5 >  3.000m? 1: 0;
				returnPart = sLkp( "Plateaus", sRL, szPlat.ToString() );
				break;
		}
	}




//__ Part Quantity _______________________________________________________
	decimal newPartQty = -1;

	int[] noQtyRule = 
		{ 20,30,40,90,100,110,120,130,140,150,160,180,190,200,210 };
	
	if ( Array.IndexOf(noQtyRule,iMtlSeq) < 0 ) {

		decimal qtyStrap = ( d6>3 )? 2: 1;

		switch (iMtlSeq) {
			case 10:
				string plDuro = od[21], plType = od[22];
				newPartQty = plDuro=="532" && (plType=="CP" || plType=="SSW")? 0.25m: 1;
				break;
			case 50:
			case 60:
				newPartQty = qtyStrap;
				break;
			case 70:
				newPartQty = 2 * qtyStrap;
				break;
			case 80:
				newPartQty = 1;
				break;
			case 170:
				newPartQty = kOD(27)? 2: 1; //ToeWalk
				break;
			case 220:
			case 230:
				newPartQty = od[18] == "B"? 0.5m: 1; //ToePlateaus (divQty/cSBLR_c)
				break;
		}
	}

	string returnQty = newPartQty.ToString();


//__ Part Unit of Measure ________________________________________________
	string returnUM = "EACH";


//__ Return Array ________________________________________________________
	string[] returnVals = { returnPart, returnQty, returnUM };
	return returnVals;




/*== CHANGE LOG =============================================================
	
	01/12/2023: Combine Method Rules UDMethods here;
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