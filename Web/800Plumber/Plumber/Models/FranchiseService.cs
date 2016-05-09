using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("view_Franchise_Services")]
    public class FranchiseService
    {
        [Key]
        [ScaffoldColumn(false)]
        public int FranchiseeServiceID { get; set; }

        [DisplayName("Franchise")]
        [Required(ErrorMessage = "Required")]
        public int FranchiseID { get; set; }

        [DisplayName("Service")]
        [Required(ErrorMessage = "Required")]
        public int ServiceID { get; set; }

        public virtual Franchise Franchise { get; set; }

        public virtual Services ServiceCategory { get; set; }
    }
}