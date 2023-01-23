# E10PC-SMO
Epicor 10.2.700 Product Configurator - Surestep SMO version 11.0

This repository includes a full copy of the SMO Configurator (SMO11.xml) exported from Epicor. 

Also included: *.cs files for all Document and Method Rules, as well as User Defined Methods and various Input and Page expressions for the Client UI.


Configurator Overview:

  Client User Interface (Configurator Design)
  
      >Inputs for all info related Order Line Pricing
    
      >Inputs for all info to determine Method of Manufacturing, both Operations and Materials)
    
      >Inptus for any additional information for production in Surestep Central Fab
    
      >All Measurements, with additional fields for Left / Right.
        -Includes 3 fields for Standard Measurements (Int, Fraction Divisor & Divident), as well as a field to enter Metric measurements as CM.
        -Also includes single field for Measurement value.
    
      >For PartTrap orders, all data either exists or can be calculated from OrderDtl and OrderDtl_UD Fields. 
        -The UD Method "ImportFromLineDtl" pulls (or calculates) all this data into configurator Inputs onPageLoaded. 
        -The UD Method "ExportToLineDtl" sends / translates all Input data back to OrderDtl only on successful OnPageLeave (save).
     
  
  Document Rules (Configuration Entry)
      
      >Runs on submission from PartTrap and after Configurator is saved from Client UI.
      
      >First method runs "drSetFinalData" to clean-up any data formatting issues, and populate calculated UD Fields. Includes:
        -Surestep Mold Library selection,
        -Full "O-Form" notes for Epicor-generated Orthotmetry Forms,
        -Changing same-day to next-day rush if late in the day
        -Get O-Form email from custom Lookup Table
        -calculate shoe size from measurements.
        -Copy measurements from other foot if 0,
        -If any required fields are skipped somehow, populate with default values
        -Set legacy Bool fields from strings so Method Rules didn't have to be completely rewritten,
        -Fix Order Notes formatting issue (SSRS not recognizing \r)
     
      >Calculate and set Pricing and Discounts for Order Line
      
      >Calculate and set Turnaround and Promised Dates 
      
      >Build and Set new Part Number and Description
      
 <<More to be added later>>
