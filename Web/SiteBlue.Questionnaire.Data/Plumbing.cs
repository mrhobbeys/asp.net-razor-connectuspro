using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Questionnaire.Data
{
    [Table("Plumbing")]
    public class Plumbing
    {
        [Key]
        [ScaffoldColumn(false)]
        public long PlumbingId { get; set; }

        [DisplayName("Technician information")]
        
        public long TechnicianInformationId { get; set; }

        [DisplayName("Backflow Device - Diagnosis and Repair")]
        
        
        public string BD_diagnosisRepair { get; set; }

        [DisplayName("Backflow Device - Test & Certify")]
        
        
        public string BD_TestCertify { get; set; }

        [DisplayName("Sink Drain - Diagnosis and Repair")]
        
        
        public string SD_DiagnosisRepair { get; set; }

        [DisplayName("Sink Faucet - Repair/Replace")]
        
        
        public string SF_RepairReplace { get; set; }

        [DisplayName("Outdoor Faucets - Repair/Replace")]
        
        
        public string OF_ReplaceRepair { get; set; }

        [DisplayName("Sink Replacement")]
        
        
        public string SinkReplacement { get; set; }

        [DisplayName("Toilet - Repair/Replace")]
        
        
        public string TO_RepairReplace { get; set; }

        [DisplayName("Urinal - Repair/Replace")]
        
        
        public string UR_RepairReplace { get; set; }

        [DisplayName("Shower Pan Replacement")]
        
        
        public string ShowerPanReplacement { get; set; }

        [DisplayName("Drain Stoppage - Diagnosis and Repair")]
        
        
        public string DS_DiagnosisRepair { get; set; }

        [DisplayName("Tub/Shower Drain - Diagnosis and Repair")]
        
        
        public string TSD_DiagnosisRepair { get; set; }

        [DisplayName("Tub/Shower Faucet - Repair/Replace")]
        
        
        public string TSF_DiagnosisRepair { get; set; }

        [DisplayName("Gas Line - Installation")]
        
        
        public string GL_Installation { get; set; }

        [DisplayName("Gas Line - Diagnosis and Repair")]
        
        
        public string GL_DiagnosisRepair { get; set; }

        [DisplayName("Water Heater - Installation")]
        
        
        public string WH_Installation { get; set; }

        [DisplayName("Water Heater - Diagnosis and Repair")]
        
        
        public string WH_DiagnosisRepair { get; set; }

        [DisplayName("Kitchen Disposer - Repair/Replace")]
        
        
        public string KD_RepairReplace { get; set; }

        [DisplayName("Dishwasher Water Line - Repair/Replace")]
        
        
        public string DWL_RepairReplace { get; set; }

        [DisplayName("Slab Leaks - Diagnosis and Repair")]
        
        
        public string SL_DiagnosisRepair { get; set; }

        [DisplayName("Water Leaks - Diagnosis and Repair")]
        
        
        public string WL_DiagnosisRepair { get; set; }

        [DisplayName("Medical Gas Lines - Installation")]
        
        
        public string MGL_Installation { get; set; }

        [DisplayName("Medical Gas Lines - Diagnosis and Repair")]
        
        
        public string MGL_DiagnosisRepair { get; set; }

        [DisplayName("Well Water Pump - Repair/Replace")]
        
        
        public string WWP_RepairReplace { get; set; }

        [DisplayName("Sewage Pump -Repair/Replace")]
        
        
        public string SWP_RepairReplace { get; set; }

        [DisplayName("Sump Pump - Repair/Replace")]        
        public string SUP_RepairReplace { get; set; }

        [DisplayName("Water Pressure - Diagnosis and Repair")]        
        public string WP_DiagnosisRepair { get; set; }

        [DisplayName("Sewer Line - Repair/Replace")]        
        public string SL_RepairReplace { get; set; }

        [DisplayName("House - Re-Pipe")]
        public string HO_RePipe { get; set; }

        [DisplayName("Water Filteration System - Installation")]        
        public string WFS_Installation { get; set; }

        [DisplayName("Water Filteration System - Diagnosis and Repair")]        
        public string WFS_DiagnosisRepair { get; set; }

        [DisplayName("Drain Camera - Inspect and Diagnosis")]
        public string DC_InspectDiagnosis { get; set; }

        public virtual TechnicianInformation Technician { get; set; }
    }
}
