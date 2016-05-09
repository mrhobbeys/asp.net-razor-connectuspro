using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace SiteBlue.Areas.Dispatch.Models
{
    public class Technician 
    {
        public int EmployeeId { get; set; }
        //public int FrenchiseID { get; set; }
        public string Name { get; set; }
        public int SalesActual { get; set; }
        public decimal SalesEstimates { get; set; }
        public int JobsComplete { get; set; }
        public int JobsEstimate { get; set; }
        public int JobsAVG { get; set; }
        public double JobsRecall { get; set; }
        public int JobsDispatched { get; set; }
        public int UpSalesNumber { get; set; }
        public int UpSalesCost { get; set; }
        public int UpSalesHG { get; set; }
        public int UpSalesBio { get; set; }
        public int DiscountsNumber { get; set; }
        public int ClosingRate { get; set; }
        public decimal DiscountsCost { get; set; }
        public bool IsSummary { get; set; }

        public string maindiv { get; set; }
        public string secondarydiv { get; set; }
        public string secondarydivP { get; set; }
        public string griddiv { get; set; }
        public string trdetails { get; set; }
        public string Actualdiv { get; set; }
        public string Upsalesdiv { get; set; }
        public string EstimateSalesdiv { get; set; }
        public string Discountdiv { get; set; }
        public string Arrowdiv { get; set; }
        
    }
    public class Details
    {
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
        public double Balance { get; set; }
        public string JobType { get; set; }
        public string Comments { get; set; }
        
    }
    public class JobTaskDetails
    {
       public decimal taskQty;
       public string Code;
       public string TaskDescription;
       public decimal Unit;
       public decimal Line;
       public string Part;
       public string PartDesc;
       public decimal PartQty;
       public decimal Price;
    }
    public class History
    {
       public string JobType;
       public string Status;
       public DateTime? Date1;
       public string Gross;
       public string Balance;
       public string ServicedBy;
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