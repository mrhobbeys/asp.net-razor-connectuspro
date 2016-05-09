using System;
using System.Collections.Generic;
using System.Linq;
using SiteBlue.Data.EightHundred;
using System.Globalization;
using System.Threading;
using SiteBlue.Data;

namespace SiteBlue.Business
{
    public class mod_common
    {
        private readonly EightHundredEntities DB;

        public mod_common(Guid userKey)
        {
            DB = new EightHundredEntities(userKey);
        }

        public  tbl_Customer GetCustomers(int tmpCustomersID)
        {
            try
            {
                var Customers = (from c in DB.tbl_Customer where c.CustomerID == tmpCustomersID select c).Single();
                return Customers;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetCustomerName(tbl_Customer Customers)
        {
            return GetCustomerName(Customers.CustomerName, Customers.CompanyName);
        }
       
        public string GetCustomerName(string Customer, string Company)
        {
            if (Customer == null)
                return "N/A";

            try
            {
                var name = string.Format("{0} - {1}", Customer, Company);

                if (name.StartsWith(" - "))
                    name = name.Substring(2);

                if (name.EndsWith(" - "))
                    name = name.Substring(0, name.Length - 2);

                var cultureInfo = Thread.CurrentThread.CurrentCulture;
                var textInfo = cultureInfo.TextInfo;

                return textInfo.ToTitleCase(name);
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

        public string getStatus(int tmpStatusID)
        {
            try
            {
                var status = (from s in DB.tbl_Job_Status where s.StatusID == tmpStatusID select s).Single();
                return status.Status;
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

        public tbl_Locations getBillTo(int tmpCustomersID)
        {
            try
            {
                var location = (from l in DB.tbl_Locations where l.BilltoCustomerID == tmpCustomersID select l).Single();
                return location;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Format_PhoneNumber(string Str)
        {
            if (Str == null) return Str;

            if ((Str.Length == 7))
            {
                // handle no area code
                return (Str.Substring(0, 3) + ("-" + Str.Substring(3, 4)));
            }

            if ((Str.Length == 10))
            {
                // handle with an area code
                return ("("
                        + (Str.Substring(0, 3) + (") "
                                                  + (Str.Substring(3, 3) + ("-" + Str.Substring(6, 4))))));
            }

            if ((Str.Length == 11))
            {
                return String.Format("{0} ({1}) {2}-{3}", Str.Substring(0, 1), Str.Substring(1, 3), Str.Substring(4, 3),
                                     Str.Substring(7, 4));
            }

            if ((Str.Length == 12))
            {
                // handle with an area code
                return ("("
                        + (Str.Substring(0, 3) + (") "
                                                  + (Str.Substring(4, 3) + ("-" + Str.Substring(8, 4))))));
            }

            // do nothing because we don't don't how to format it
            return Str;
        }

        public string GetCallTalkerID(string phone)
        {
            try
            {
                var context = new IncomingCallsQAEntities();

                return context.StatisticTracks.First(q => q.CalledNumber == phone).UserId.ToString();
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

        public string GetCallTalkerName(string phone)
        {
            try
            {
                var context = new IncomingCallsQAEntities();

                var uid = context.StatisticTracks.First(q => q.CalledNumber == phone).UserId;

                try
                {
                    var context1 = new MembershipEntities();

                    return context1.aspnet_Users.First(q => q.UserId == uid).UserName;
                }
                catch(Exception)
                {
                    return "N/A";
                }
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

        public Guid GetCallTalkerUserID(string phone)
        {
            try
            {
                var context = new IncomingCallsQAEntities();

                return context.StatisticTracks.First(q => q.CalledNumber == phone).UserId;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        public string GetCallTalkerNameByUserId(Guid UserId)
        {
            try
            {
                try
                {
                    var context1 = new MembershipEntities();

                    return context1.aspnet_Users.First(q => q.UserId == UserId).UserName;
                }
                catch (Exception)
                {
                    return "N/A";
                }
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

        public string GetDisplayName(int userid)
        {
            try
            {
                var user = (from u in DB.tbl_User where u.UserID == userid select u).Single();
                return user.DisplayName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public tbl_Locations getLocation(int tmpLocationID)
        {
            try
            {
                var location = (from l in DB.tbl_Locations where l.LocationID == tmpLocationID select l).Single();
                return location;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public tbl_Job_Type getJobType(int tmpJobTypeID)
        {
            try
            {
                var jobtype = (from j in DB.tbl_Job_Type where j.JobTypeID == tmpJobTypeID select j).Single();
                return jobtype;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetWarrantyType(int WarrantyTypeID)
        {
            try
            {
                var WarrantyType = (from m in DB.tbl_Job_WarrantyType where m.WarrantyTypeID == WarrantyTypeID select m).Single();
                return WarrantyType.WarrantyType;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public int GetWarrantyLengthID(string WarrantyLengthstr)
        {
            try
            {
                var WarrantyLength = (from m in DB.tbl_Job_WarrantyLength where m.WarrantyLength == WarrantyLengthstr select m).Single();
                return WarrantyLength.WarrantyLengthID;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string GetWarrantyLength(int WarrantyLengthID)
        {
            try
            {
                var WarrantyLength = (from m in DB.tbl_Job_WarrantyLength where m.WarrantyLengthID == WarrantyLengthID select m).Single();
                return WarrantyLength.WarrantyLength;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string Get_Job_DBAName(int tmpDBAID)
        {
            try
            {
                var tmpDBA = (from t in DB.tbl_Dispatch_DBA where t.DBAID == tmpDBAID select t.DBAName).Single();
                return tmpDBA;
                //Get_Job_DBAName = tmpDBA;
            }
            catch (Exception)
            {
                return null;
                //Get_Job_DBAName = "";
            }
        }

        public string getPaymentType_Actual(int tmpJobID)
        {
            string tmpStr = "";
            int tmppid;

            try
            {
                var Paymentlist = (from p in DB.tbl_Payments where p.JobID == tmpJobID select p);
                foreach (var Payrec in Paymentlist)
                {
                    if ((Payrec.PaymentAmount != 0))
                    {
                        tmppid = Payrec.PaymentTypeID;
                        tmpStr += getPaymentType(tmppid) + "  " + Convert.ToDateTime(Payrec.PaymentDate).ToString("MM/dd/yyyy") + "\r\n";
                    }
                }
                
                return tmpStr;
            }
            catch (Exception)
            {
                return "N/A";
            }
        }
         
        public string getPaymentType(int tmpPaymentTypeID)
        {
            try
            {
                var PaymentType = (from p in DB.tbl_Payment_Types where p.PaymentTypeId == tmpPaymentTypeID select p).Single();
                return PaymentType.PaymentType;
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

        public tbl_Referral getConnectusReferral(int tmpReferralID)
        {
            try
            {
                var referral = (from d in DB.tbl_Referral where d.referralID == tmpReferralID select d).Single();
                return referral;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetJobTypeName(int JobTypeID)
        {
            try
            {
                var JobType = (from a in DB.tbl_Job_Type where a.JobTypeID == JobTypeID where a.ActiveYN == true select a).Single();
                return JobType.JobType;
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

        public string Get_Employee_Name(int tmpEmployeeID)
        {
            try
            {
                var tmpEmployee = (from t in DB.tbl_Employee where t.EmployeeID == tmpEmployeeID select t.Employee).Single();
                return tmpEmployee;
                //Get_Employee_Name = tmpEmployee;

            }
            catch (Exception)
            {
                return null;
                //Get_Employee_Name = "N/A";
            }
        }

        public Int32? Get_Employee_ID(int tmpEmployeeID)
        {
            try
            {
                var tmpEmployee = (from t in DB.tbl_Employee where t.EmployeeID == tmpEmployeeID select t.EmployeeID).Single();
                return tmpEmployee;
                //Get_Employee_Name = tmpEmployee;

            }
            catch (Exception)
            {
                return null;
                //Get_Employee_Name = "N/A";
            }
        }

        public DateTime? ParseNullableDateTime(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            else
            {
                return DateTime.Parse(s);
            }
        }

        public int? ToNullableInt32(string s)
        {
            int i;
            if (Int32.TryParse(s, out i)) return i;
            return null;
        }

        public tbl_Referral getReferral(int tmpReferralID)
        {
            try
            {
                var referral = (from d in DB.tbl_Referral where d.referralID == tmpReferralID select d).Single();
                return referral;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public enum DateInterval
        {
            Year,
            Month,
            Weekday,
            Day,
            Hour,
            Minute,
            Second
        }

        public long DateDiff(DateInterval interval, DateTime? date1, DateTime? date2)
        {
            TimeSpan? ts = ts = date2 - date1;

            switch (interval)
            {
                case DateInterval.Year:
                    return date2.Value.Year - date1.Value.Year;
                case DateInterval.Month:
                    return (date2.Value.Month - date1.Value.Month) + (12 * (date2.Value.Year - date1.Value.Year));
                case DateInterval.Weekday:
                    return Fix(ts.Value.TotalDays) / 7;
                case DateInterval.Day:
                    return Fix(ts.Value.TotalDays);
                case DateInterval.Hour:
                    return Fix(ts.Value.TotalHours);
                case DateInterval.Minute:
                    return Fix(ts.Value.TotalMinutes);
                default:
                    return Fix(ts.Value.TotalSeconds);
            }
        }

        public long DateDiffArList(DateInterval interval, DateTime? date1, DateTime? date2)
        {
            TimeSpan? ts = ts = date2 - date1;

            switch (interval)
            {
                case DateInterval.Year:
                    return date2.Value.Year - date1.Value.Year;
                case DateInterval.Month:
                    return (date2.Value.Month - date1.Value.Month) + (12 * (date2.Value.Year - date1.Value.Year));
                case DateInterval.Weekday:
                    return FixArlist(ts.Value.TotalDays) / 7;
                case DateInterval.Day:
                    return FixArlist(ts.Value.TotalDays);
                case DateInterval.Hour:
                    return FixArlist(ts.Value.TotalHours);
                case DateInterval.Minute:
                    return FixArlist(ts.Value.TotalMinutes);
                default:
                    return FixArlist(ts.Value.TotalSeconds);
            }
        }

        private long Fix(double Number)
        {
            if (Number >= 0)
                return (long)Math.Floor(Number);

            return (long)Math.Ceiling(Number);
        }

        private long FixArlist(double Number)
        {
            if (Number > 0 && Number < 1)
                Number = 1;

            if (Number >= 0)
                return (long)Math.Floor(Number);

            return (long)Math.Ceiling(Number);
        }

        public List<tbl_TaxRates> GetTaxRate(int TaxRateID)
        {
	        try 
            {
                var TaxRate = (from t in DB.tbl_TaxRates where t.TaxRateID == TaxRateID select t).ToList();

		        return TaxRate;
	        } 
            catch (Exception ex) {
                throw ex;
	        }
        }

        public string GetPartCode(int PartID)
        {
            try
            {
                var MasterPart = (from m in DB.tbl_PB_Parts
                                  join p in DB.tbl_PB_MasterParts on m.MasterPartID equals p.MasterPartID
                                  where m.PartID == PartID
                                  select p).Single();

                return MasterPart.PartCode;
            }
            catch (Exception)
            {
                return "Unassigned";
            }
        }

        public string GetPartName(int PartID)
        {
            try
            {
                var MasterPart = (from m in DB.tbl_PB_Parts
                                  join p in DB.tbl_PB_MasterParts on m.MasterPartID equals p.MasterPartID
                                  where m.PartID == PartID
                                  select p).Single();

                return MasterPart.PartName;
            }
            catch (Exception)
            {
                return "Unassigned";
            }
        }

        public string Get_Account_Code(int tmpJobCodeID, int BusTypeId)
        {
            string functionReturnValue = null;
            string accountCode = "";

            try
            {
                var jobcodelist = (from j in DB.tbl_PB_JobCodes where j.JobCodeID == tmpJobCodeID select j);
                foreach (var jobcode_loopVariable in jobcodelist)
                {

                    if (BusTypeId == 3)
                    {
                        accountCode = jobcode_loopVariable.ComAccountCode;
                    }
                    else
                    {
                        accountCode = jobcode_loopVariable.ResAccountCode;
                    }
                    functionReturnValue = accountCode;
                }

            }
            catch (Exception)
            {
                functionReturnValue = "00000";
            }

            return functionReturnValue;
        }
        
        public DateTime getCurrentDate()
        {
            try
            {
                //return DateTime.DateAdd(DateInterval.Day, -30, DateTime.Today);
                TimeSpan objtimespan = new TimeSpan(-30, 0, 0, 0);
                return DateTime.Now.Add(objtimespan);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime get60dayDate()
        {
            try
            {
                //return DateTime.DateAdd(DateInterval.Day, -60, DateTime.Today);
                TimeSpan objtimespan = new TimeSpan(-60, 0, 0, 0);
                return DateTime.Now.Add(objtimespan);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //get60dayDate
        public DateTime get90dayDate()
        {
            try
            {
                //return DateTime.DateAdd(DateInterval.Day, -90, DateTime.Today);
                TimeSpan objtimespan = new TimeSpan(-90, 0, 0, 0);
                return DateTime.Now.Add(objtimespan);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //get90dayDate
        public string getAge(DateTime? closedDate)
        {
            try
            {
                if (DateDiff(DateInterval.Day, getCurrentDate(), closedDate) > 0)
                {
                    return "Current";
                }
                else if (DateDiff(DateInterval.Day, get60dayDate(), closedDate) > 0)
                {
                    return "30-60";
                }
                else if (DateDiff(DateInterval.Day, get90dayDate(), closedDate) > 0)
                {
                    return "60-90";
                }
                else
                {
                    return "Over 90";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getAgeArList(DateTime? closedDate)
        {
            try
            {
                if (DateDiffArList(DateInterval.Day, getCurrentDate(), closedDate) > 0)
                {
                    return "1-30";
                }
                else if (DateDiffArList(DateInterval.Day, get60dayDate(), closedDate) > 0)
                {
                    return "31-60";
                }
                else if (DateDiffArList(DateInterval.Day, get90dayDate(), closedDate) > 0)
                {
                    return "61-90";
                }
                else
                {
                    return "Over 90";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public tbl_Job getJob(int tmpjobID)
        {
            try
            {
                var job = (from j in DB.tbl_Job where j.JobID == tmpjobID select j).Single();

                return job;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}