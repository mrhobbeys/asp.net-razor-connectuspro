using System;

namespace SiteBlue.Areas.OwnerPortal.Models
{
    public class DailyBudgetInfo
    {
        public DailyBudgetInfo(DateTime asOfDate)
        {
            AsOfDate = asOfDate;
            var monthRatioFactor = (decimal)asOfDate.Day / DateTime.DaysInMonth(AsOfDate.Year, AsOfDate.Month);
            DailySales = new BudgetDetails();
            DailyJobs = new BudgetDetails();
            DailyCloseRate = new BudgetDetails();
            DailyAverageTicket = new BudgetDetails();
            DailyHomeGuard = new BudgetDetails();
            DailyBio = new BudgetDetails();
            DailyRecalls = new BudgetDetails { UnderBudgetIsGood = true };
            DailyPayroll = new BudgetDetails { UnderBudgetIsGood = true };

            MonthlyCloseRate = new BudgetDetails();
            MonthlyAverageTicket = new BudgetDetails();
            MonthlySales = new MTDBudgetDetails(monthRatioFactor);
            MonthlyJobs = new MTDBudgetDetails(monthRatioFactor);
            MonthlyHomeGuard = new MTDBudgetDetails(monthRatioFactor);
            MonthlyBio = new MTDBudgetDetails(monthRatioFactor);
            MonthlyRecalls = new MTDBudgetDetails(monthRatioFactor) { UnderBudgetIsGood = true };
            MonthlyPayroll = new BudgetDetails { UnderBudgetIsGood = true };
        }

        public BudgetDetails DailySales { get; private set; }
        public BudgetDetails DailyJobs { get; private set; }
        public BudgetDetails DailyCloseRate { get; private set; }
        public BudgetDetails DailyAverageTicket { get; private set; }
        public BudgetDetails DailyHomeGuard { get; private set; }
        public BudgetDetails DailyBio { get; private set; }
        public BudgetDetails DailyRecalls { get; private set; }
        public BudgetDetails DailyPayroll { get; private set; }

        public MTDBudgetDetails MonthlySales { get; private set; }
        public MTDBudgetDetails MonthlyJobs { get; private set; }
        public BudgetDetails MonthlyCloseRate { get; private set; }
        public BudgetDetails MonthlyAverageTicket { get; private set; }
        public MTDBudgetDetails MonthlyHomeGuard { get; private set; }
        public MTDBudgetDetails MonthlyBio { get; private set; }
        public MTDBudgetDetails MonthlyRecalls { get; private set; }
        public BudgetDetails MonthlyPayroll { get; private set; }

        public DateTime AsOfDate { get; private set; }
    }

    public class MTDBudgetDetails : AbstractBudgetDetails
    {
        private readonly decimal _ratio;

        public MTDBudgetDetails(decimal ratio)
        {
            _ratio = ratio;
        }

        public decimal CurrentBudget
        {
            get { return Math.Round(Budget * _ratio); }
        }

        public override decimal Diff { get { return Actual - CurrentBudget; } }
        public override decimal DiffPercent
        {
            get
            {
                if (CurrentBudget == 0) return Actual > CurrentBudget ? 100 : 0;
                return Math.Round(Diff / CurrentBudget * 100);
            }
        }

    }
    public class BudgetDetails : AbstractBudgetDetails
    {
    }

    public abstract class AbstractBudgetDetails
    {
        public bool UnderBudgetIsGood { get; set; }

        private decimal _actual;
        public decimal Actual
        {
            get { return _actual; }
            set { _actual = Math.Round(value); }
        }

        private decimal _budget;
        public decimal Budget
        {
            get { return _budget; }
            set { _budget = Math.Round(value); }
        }

        public virtual decimal Diff { get { return Actual - Budget; } }

        public virtual decimal DiffPercent
        {
            get
            {
                if (Budget == 0) return Actual > Budget ? 100 : 0;
                return Math.Round(Diff / Budget * 100);
            }
        }

        public bool OnBudget
        {
            get
            {
                return (UnderBudgetIsGood && DiffPercent <= 0) ||
                       (!UnderBudgetIsGood && DiffPercent >= 0);
            }
        }
    }
};