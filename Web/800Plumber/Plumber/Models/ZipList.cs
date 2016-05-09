using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("view_Franchise_ZipList")]
    public class ZipList
    {
        [Key]
        [ScaffoldColumn(false)]
        public int ZipID { get; set; }

        [Required(ErrorMessage = "Required")]
        public string FranchiseZipID { get; set; }

        [Required(ErrorMessage = "Required")]
        [ForeignKey("Franchise")]
        public int FranchiseID { get; set; }

        [Required(ErrorMessage = "Required")]
        [DefaultValue(true)]
        public bool ActiveYN { get; set; }

        public DateTime? DateAdded { get; set; }

        public DateTime? DateRemoved { get; set; }

        [Required(ErrorMessage = "Required")]
        public bool OwnedYN { get; set; }

        [Required(ErrorMessage = "Required")]
        public bool ServicesYN { get; set; }

        [StringLength(50, ErrorMessage = "Validation error - Length: 50")]
        public string City { get; set; }

        [StringLength(2, ErrorMessage = "Validation error - Length: 2")]
        public string State { get; set; }

        public string CallTakerMessage { get; set; }

        public byte[] timestamp { get; set; }

        public virtual Franchise Franchise { get; set; }
    }
}