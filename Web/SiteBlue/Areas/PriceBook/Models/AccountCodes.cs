using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteBlue.Areas.PriceBook.Models
{
    [Table("tbl_Account_Codes")]
    public class AccountCodes
    {
        [Key]
        [Display(Name="AccountCode")]
        [MaxLength(5)]
        public string AccountCode { get; set; }

        [Display(Name = "AccountName")]
        [MaxLength(50)]
        public string AccountName { get; set; }

        [Display(Name = "AccountType")]
        public int AccountType { get; set; }
    }
}