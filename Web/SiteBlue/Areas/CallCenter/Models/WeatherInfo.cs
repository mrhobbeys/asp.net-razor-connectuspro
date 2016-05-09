using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace yMapWeather.Models
{
    public class WeatherInfo
    {
        public WeatherInfo()
        {
            DaysForcast = new List<WeatherDayInfo>(); 
        }

        public string ForcastImg { get; set; }
        public string Text { get; set; }
        public string Tempr { get; set; }
        public string Code { get; set; }
        public string DatePub { get; set; }

        public string Location { get; set; }

        public string Berometer { get; set; }
        public string Humidity { get; set; }
        public string Visibility { get; set; }
        
        public string SunRise { get; set; }
        public string SunSet { get; set; }

        public List<WeatherDayInfo> DaysForcast { get; set; }

        public bool IsObjectFilled() 
        {
            return (ForcastImg != null) && (Tempr != null) && (DatePub != null);
        }

        public void WeatherForcast(string WOEID)
        {
            // Create a new XmlDocument  
            XmlDocument doc = new XmlDocument();

            ////-----
            var url = string.Format("http://weather.yahooapis.com/forecastrss?w={0}", WOEID);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            var str = request.GetResponse().GetResponseStream();
            // Load weather rss feed  
            doc.Load(str);
            ////-----

           // doc.Load(string.Format("http://weather.yahooapis.com/forecastrss?w={0}", WOEID));

            // Set up namespace manager for XPath  
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode weatherNode = doc.SelectSingleNode("/rss/channel/item/yweather:condition", ns);

            if (weatherNode != null)
            {
                Text = weatherNode.Attributes["text"].InnerText;
                Code = weatherNode.Attributes["code"].InnerText;
                Tempr = string.Format("{0} F", weatherNode.Attributes["temp"].InnerText);
                DatePub = weatherNode.Attributes["date"].InnerText;
            }

            XmlNode atmosNode = doc.SelectSingleNode("/rss/channel/yweather:atmosphere", ns);
            if (atmosNode != null)
            {
                Humidity = string.Format("{0} %", atmosNode.Attributes["humidity"].InnerText);
                Visibility = string.Format("{0} mi", atmosNode.Attributes["visibility"].InnerText);
                Berometer = string.Format("{0} in {1}", atmosNode.Attributes["pressure"].InnerText, (atmosNode.Attributes["rising"].InnerText == "1") ? "rising" : "setting");
            }

            XmlNode locNode = doc.SelectSingleNode("/rss/channel/yweather:location", ns);
            if (locNode != null)
            {
                Location = string.Format("{0}, {1}, {2}", locNode.Attributes["city"].InnerText, locNode.Attributes["region"].InnerText, locNode.Attributes["country"].InnerText);
            }

            XmlNode astroNode = doc.SelectSingleNode("/rss/channel/yweather:astronomy", ns);
            if (astroNode != null)
            {
                SunRise = astroNode.Attributes["sunrise"].InnerText;
                SunSet = astroNode.Attributes["sunset"].InnerText;
            }

            XmlNode descNode = doc.SelectSingleNode("/rss/channel/description", ns);
            string descString = doc.SelectSingleNode("/rss/channel/item/description", ns).InnerText;

            Match m = Regex.Match(descString, "<img src=(.*?)/>", RegexOptions.Multiline);
            if (m.Success)
            {
                ForcastImg = m.Value.Replace("<img src=", "").Replace("/>", "").Replace("\"", "").Trim();
            }

            // Get forecast with XPath  
            XmlNodeList nodes = doc.SelectNodes("/rss/channel/item/yweather:forecast", ns);

            DaysForcast.Clear();

            foreach (XmlNode node in nodes)
            {
                WeatherDayInfo wdi = new WeatherDayInfo();
                wdi.Day = node.Attributes["day"].InnerText;
                wdi.Text = node.Attributes["text"].InnerText;
                wdi.Low = string.Format("{0}F", node.Attributes["low"].InnerText);
                wdi.Heigh = string.Format("{0}F", node.Attributes["high"].InnerText);
                DaysForcast.Add(wdi);
            }
        }
    }

    public class WeatherDayInfo
    {
        public string Day { get; set; }
        public string Text { get; set; }
        public string Low { get; set; }
        public string Heigh { get; set; }
    }
}