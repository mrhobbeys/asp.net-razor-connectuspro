using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("Hvac")]
    public class Hvac
    {
        [Key]
        [ScaffoldColumn(false)]
        public long HvacId { get; set; }

        [DisplayName("Technician information")]

        public long TechnicianInformationId { get; set; }

        [DisplayName("AC Unit - Diagnosis and Repair")]
        public string ACU_DiagnosisRepair { get; set; }

        [DisplayName("AC System Annual Maintenance")]        
        public string ACS_AnnualMaintenance { get; set; }

        [DisplayName("AC Unit - Clean Coil")]        
        public string ACU_CleanCoil { get; set; }

        [DisplayName("Indoor AC Unit Installation")]        
        public string IACU_Installation { get; set; }

        [DisplayName("Outdoor AC Unit Installation")]        
        public string OACU_Installation { get; set; }

        [DisplayName("Dehumidifier -Diagnosis and Repair")]        
        public string DH_DiagnosisRepair { get; set; }

        [DisplayName("Dehumidifier Installation")]        
        public string DH_Installation { get; set; }

        [DisplayName("HVAC System - Replace Filters")]        
        public string HS_ReplaceFilters { get; set; }

        [DisplayName("Heat Pump - Diagnosis and Repair")]        
        public string HP_DiagnosisRepair { get; set; }

        [DisplayName("Heat Pump Installation")]        
        public string HP_Installation { get; set; }

        [DisplayName("Boiler - Annual Maintenance")]        
        public string BO_AnnualMaintenance { get; set; }

        [DisplayName("Boiler Installation")]        
        public string BO_Installation { get; set; }

        [DisplayName("Boiler - Repair")]        
        public string BO_Repair { get; set; }

        [DisplayName("Boiler - Pump Repair")]        
        public string BO_PumpRepair { get; set; }

        [DisplayName("Boiler - Low Water Cut Off")]        
        public string BO_LowWaterCutOff { get; set; }

        [DisplayName("Furnace - Annual Maintenance")]       
        public string FR_AnnualMaintenance { get; set; }

        [DisplayName("Furnace Installation")]        
        public string FR_Installation { get; set; }

        [DisplayName("Furnace - Repair")]        
        public string FR_Repair { get; set; }

        [DisplayName("Electronic Air Cleaner - Diagnosis and Repair")]        
        public string EAC_DiagnosisRepair { get; set; }

        [DisplayName("Electronic Air Cleaner Installation")]        
        public string EAC_Installation { get; set; }

        [DisplayName("UV Lights - Replace")]        
        public string UVL_Replace { get; set; }

        public virtual TechnicianInformation Technician { get; set; }

    }
}
