using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

//namespace yMapWeather.Models
//{
//    public class UserLocation
//    {
//        [Required]
//        [RegularExpression(@"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$",ErrorMessage="Invalid Phone Format")]
//        public string PhoneNumber { get { return "423-555-7778"; } set { PhoneNumber = "423-555-7778"; } }
//        public string City { get; set; }
//        public string State { get; set; }
//        public string StateCode { get; set; }
//        public string ZipCode { get; set; }
//        public string Country { get; set; }

//        public string GetLocation()
//        {
//            return string.Join(",", new[] { this.City, this.State, this.StateCode, this.Country });  
//        }

//        public string Empty = ",,,";
//    }
//}