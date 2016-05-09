using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SiteBlue.Areas.HVAC_App.Models.Interfaces
{
    public interface IStore
    {
        List<IItem> ListOfItems { get; set; }
        void SetIntoStory();
    }

    public interface IItem
    {
        
    }

    public interface IItemStore<T>
    {
        T StoredItem { get; }
        void SetStoreItem(T item);
        void ClearStoreItem();
    }

    public class ConfigStore: IItemStore<int>
    {
        private const string CookieName = "id_config";
        private readonly int _configId;

        public ConfigStore()
        {
            var httpCookie = HttpContext.Current.Request.Cookies.Get(CookieName);
            if (httpCookie != null)
                _configId = int.Parse(httpCookie.Value.Replace("\"", ""));
            _configId = - 1;
        }

        
        public int StoredItem
        {
            get { return _configId; }
        }

        public void SetStoreItem(int item)
        {
            var httpCookie = HttpContext.Current.Request.Cookies.Get(CookieName);
            if (httpCookie != null)
                httpCookie.Value =  item.ToString() ;
            else
            {
               httpCookie = new HttpCookie(CookieName,item.ToString()) { Expires = DateTime.Now.AddDays(7) };
            }
            HttpContext.Current.Response.Cookies.Add(httpCookie);
        }

        public void ClearStoreItem()
        {
            var httpCookie = HttpContext.Current.Request.Cookies.Get(CookieName);
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now;
                HttpContext.Current.Response.Cookies.Add(httpCookie);
            }
        }
    }

    public class FranchiseStoreInCookies: IItemStore<int>
    {
        private const string CookieName = "franchise_id";
        private readonly int _franchiseId;

        public FranchiseStoreInCookies()
        {
            _franchiseId = 0;
            var httpCookie = HttpContext.Current.Request.Cookies.Get(CookieName);
            if (httpCookie != null)
                _franchiseId = int.Parse(httpCookie.Value.Replace("\"", ""));
           
        }

        public int StoredItem
        {
            get { return _franchiseId; }
        }

        public void SetStoreItem(int item)
        {
            var cookie = new HttpCookie(CookieName, item.ToString(CultureInfo.InvariantCulture)) {Expires = DateTime.Now.AddDays(7)};
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public void ClearStoreItem()
        {
            var httpCookie = HttpContext.Current.Request.Cookies.Get(CookieName);
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now;
                HttpContext.Current.Response.Cookies.Add(httpCookie);
            }

        }
    }

    public class FranchiseStoreInUserInfo: IItemStore<int>
    {
        public FranchiseStoreInUserInfo(SessionContainer userInfo)
        {
            _franchiseId = userInfo.CurrentFranchise.FranchiseID;
        }

        private readonly int _franchiseId;
        public int StoredItem
        {
            get { return _franchiseId; }
        }

        public void SetStoreItem(int item)
        {
            throw new Exception("Can`t set value from this class: " + this);
        }

        public void ClearStoreItem()
        {
            throw new Exception("Can`t delete value from this class: " + this);
        }
    }
}
