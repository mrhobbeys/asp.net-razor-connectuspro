using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace SiteBlue.Areas.HVAC_App.Models
{
    public class PriceListModel<T>
    {
        public SystemInfoModelWithParts MainSystem { get; set; }
        public T[] Jobs { get; set; }
        public string TotalAmount { get; set; }
        public string Tax { get; set; }
        public string GrandTotal { get; set; }
        public string TaxRate { get; set; }
    }  

    public class EmailClass
    {
        public string email { get; set; }
    }


    public class JSONHelper
    {
        public static T Deserialise<T>(string json)
        {
            var obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                return obj;
            }
        } 
    }
}