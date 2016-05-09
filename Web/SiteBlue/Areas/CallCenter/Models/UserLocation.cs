using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace yMapWeather.Models
{
    public class UserLocation
    {
        [Required]
        [RegularExpression(@"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$",
            ErrorMessage = "Invalid Phone Format")]
        public string PhoneNumber { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        public string GetLocation()
        {
            return string.Join(",", new[] {this.City, this.State, this.StateCode, this.Country});
        }

        public string Empty = ",,,";

        public string GetWOEIDForLocation(ref Int16 err)
        {
            string location = GetLocation();
            string WOEID = string.Empty;
            try
            {
                string URL =
                    string.Format(
                        "http://query.yahooapis.com/v1/public/yql?q=select%20woeid%20from%20geo.places%20where%20text%3D%22{0}%22&diagnostics=false",
                        location);

                // Create a new XmlDocument  
                XmlDocument doc = new XmlDocument();
                // Load data  

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

                var str = request.GetResponse().GetResponseStream();
                // Load weather rss feed  
                doc.Load(str);

                //doc.Load(URL);
                // Set up namespace manager for XPath  
                XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);

                // Get forecast with XPath  
                WOEID = doc.GetElementsByTagName("woeid")[0].InnerText;
                err = 0;
            }
            catch (Exception)
            {
                err = 1;
            }
            return WOEID;
        }

        public void GetUserLocation(ref Int16 err)
        {
            try
            {
                string phone = GetUnFormatedPhoneNumber();
                string URL = string.Format("http://www.tp2location.com/{0}", phone);
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(URL);
                request.Method = "GET";
                // Set some reasonable limits on resources used by this request
                request.MaximumAutomaticRedirections = 4;
                request.MaximumResponseHeadersLength = 4;
                // Set credentials to use for this request.
                //request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                long l = response.ContentLength;
                string s = response.ContentType;

                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                string HtmlResp = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                Country = UserCountry(HtmlResp);
                string region = UserRegion(HtmlResp);
                string[] RegionStr = region.Split(',');
                if (RegionStr.Length > 2)
                {
                    City = RegionStr[0].Trim();
                    State = RegionStr[1].Trim();
                    StateCode = RegionStr[2].Trim();
                }
                else if (RegionStr.Length == 2)
                {
                    City = RegionStr[0].Trim();
                    State = RegionStr[1].Trim();
                }
                err = 0;
            }
            catch (WebException)
            {
                err = 1;
            }

        }

        private string GetUnFormatedPhoneNumber()
        {
            if (PhoneNumber.StartsWith("1-"))
            {
                return PhoneNumber.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
            }
            return "1" + PhoneNumber.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
        }

        private string UserRegion(string htmlString)
        {
            string pattern =
                @"<table.*?tbl_gray_border[^>]*>[\w|\t|\r|\W]*<td colspan.*?>[\n]*<table><tr><td>[\n]<script";

            MatchCollection m = Regex.Matches(htmlString, pattern, RegexOptions.Multiline);
            if (m.Count > 0)
            {
                pattern = @"<td[^>]*>Region[\w|\t|\r|\W]*</td>[\n]  </tr>[\n]  <tr>[\n]    <td";
                MatchCollection mc = Regex.Matches(m[0].Groups[0].Value, pattern, RegexOptions.Multiline);
                if (mc.Count > 0)
                {
                    pattern = @"<td[^>]*>(.*?)</td>";
                    string trString = mc[0].Groups[0].Value;
                    trString = trString.Substring(0, trString.IndexOf(@"</tr>"));
                    m = Regex.Matches(trString, pattern, RegexOptions.Singleline);
                    if (m.Count > 1)
                    {
                        pattern = @"<(.|\n)*?>";
                        return
                            HttpUtility.HtmlDecode(Regex.Replace(m[1].Groups[0].Value, pattern, string.Empty)).Replace("-",
                                                                                                                  ",").
                                Trim();
                    }
                }
            }
            return "";
        }

        private string UserCountry(string htmlString)
        {
            string pattern =
                @"<table.*?tbl_gray_border[^>]*>[\w|\t|\r|\W]*<td colspan.*?>[\n]*<table><tr><td>[\n]<script";

            MatchCollection m = Regex.Matches(htmlString, pattern, RegexOptions.Multiline);
            if (m.Count > 0)
            {
                pattern = @"<td[^>]*>Country[\w|\t|\r|\W]*BODY_FONT_12_BLACK.*?<img";
                MatchCollection mc = Regex.Matches(m[0].Groups[0].Value, pattern, RegexOptions.Multiline);
                if (mc.Count > 0)
                {
                    pattern = @"<td[^>]*>(.*?)</td>";
                    string trString = mc[0].Groups[0].Value;
                    m = Regex.Matches(trString, pattern, RegexOptions.Multiline);
                    if (m.Count > 1)
                    {
                        pattern = @"<(.|\n)*?>";
                        return HttpUtility.HtmlDecode(Regex.Replace(m[1].Groups[0].Value, pattern, string.Empty));
                    }
                }
            }
            return "";
        }
    }
}