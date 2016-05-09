using System;
using System.Linq;

namespace SiteBlue.DocumentGeneration.Invoices
{
    public class Invoice
    {
        public int JobID { get; set; }
        public int DBAId { get; set; }
        public int FranchiseId { get; set; }
        public int FranchiseTypeID { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string Technician { get; set; }
        public string BusinessType { get; set; }
        public string Warranty1 { get; set; }
        public string Warranty1Length { get; set; }
        public string Warranty2 { get; set; }
        public string Warranty2Length { get; set; }
        public string CallReason { get; set; }
        public string Diagnostics { get; set; }
        public string Recommendations { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string FromCity { get; set; }
        public string FromState { get; set; }
        public string FromZip { get; set; }
        public string FromPhone { get; set; }
        public string ToName { get; set; }
        public string ToAddress { get; set; }
        public string ToCity { get; set; }
        public string ToState { get; set; }
        public string ToZip { get; set; }
        public string ToEmail { get; set; }
        public string ToPhone { get; set; }
        public string LocationName { get; set; }
        public string LocationAddress { get; set; }
        public string LocationCity { get; set; }
        public string LocationState { get; set; }
        public string LocationZip { get; set; }
        public string LocationPhone { get; set; }
        public DateTime Completed { get; set; }
        public bool IsMember { get; set; }
        public string LicenseNumber { get; set; }
        public InvoiceLine[] Lines { get; set; }
        public Payment[] Payments { get; set; }
        public bool IsEstimate { get; set; }
        public bool HasAuthSignature { get; set; }
        public bool HasAcceptSignature { get; set; }

        public decimal Balance
        {
            get { return Math.Round(TotalAmount - (Payments.Length == 0 ? 0 : Payments.Sum(p => p.Amount)), 2); }
            set { } //to support serialization.
        }
    }
}