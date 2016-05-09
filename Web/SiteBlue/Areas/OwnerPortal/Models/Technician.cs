using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class Technician 
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public decimal SalesActual { get; set; }
        public decimal SalesEstimates { get; set; }
        public decimal OutstandingEstimateSales { get; set; }
        public int JobsComplete { get; set; }
        public int JobsEstimate { get; set; }
        public int JobsOutstandingEstimate { get; set; }
        public decimal JobsAVG { get; set; }
        public int JobsRecall { get; set; }
        public int JobsDispatched { get; set; }
        public int UpSalesNumber { get; set; }
        public decimal UpSalesCost { get; set; }
        public int UpSalesHG { get; set; }
        public int UpSalesBio { get; set; }
        public int DiscountsNumber { get; set; }
        public int ClosingRate { get; set; }
        public int RecoverRate { get; set; }
        public decimal DiscountsCost { get; set; }
    }
    public class Details
    {
        public int JobID { get; set; }
        public int? EmployeeId { get; set; }
        public int? Invoicenumber { get; set; }
        public string BillTo { get; set; }
        public string JobLocation { get; set; }
        public string Status { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? CompletedDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ClosedDate { get; set; }
        public string Tech { get; set; }
        public double JobAmt { get; set; }
        public decimal Balance { get; set; }
        public string strJobAmt { get; set; }
        public string strBalance { get; set; }
        public string JobType { get; set; }
        public string Comments { get; set; }
        public string Phone { get; set; }
        public string age { get; set; }
        public string totalbalance { get; set; }
        public string shortdatestring { get; set; }
        public string completedshortdatestring { get; set; }
        public int CustomerID { get; set; } 
        
    }
    public class JobTaskDetails
    {
        public bool addonyn;
       public int JobTaskPartsID;
       public int jobtaskid;
       public decimal taskQty;
       public string Code;
       public string TaskDescription;
       public decimal Unit;
       public decimal Line;
       public string Part;
       public string PartDesc;
       public decimal PartQty;
       public decimal Price;
       public bool memberyn;
       public string strUnit;
       public string strLine;
       public string strPrice;
       public int statusid;
    }
    public class History
    {
        public string jobsid;
       public string JobType;
       public string Status;
       public DateTime? Date1;
       public string Gross;
       public string Balance;
       public string ServicedBy;
       
    }
    public class Emplist
    {
        public int EmpId;
       public string EmpName;
       public string EmpPrimaryPhone;
       public string EmpAddress;
       public string EmpCity;
       public string EmpState;
       public string EmpPostal;
    }
    static class Ext
    {
        public static IQueryable<TSource> Between<TSource, TKey>
             (this IQueryable<TSource> source,
              Expression<Func<TSource, TKey>> keySelector,
              TKey low, TKey high) where TKey : IComparable<TKey>
        {
            Expression key = Expression.Invoke(keySelector,
                 keySelector.Parameters.ToArray());
            Expression lowerBound = Expression.LessThanOrEqual
                (Expression.Constant(low), key);
            Expression upperBound = Expression.LessThanOrEqual
                (key, Expression.Constant(high));
            Expression and = Expression.AndAlso(lowerBound, upperBound);
            Expression<Func<TSource, bool>> lambda =
                Expression.Lambda<Func<TSource, bool>>(and, keySelector.Parameters);
            return source.Where(lambda);
        }
    }
}