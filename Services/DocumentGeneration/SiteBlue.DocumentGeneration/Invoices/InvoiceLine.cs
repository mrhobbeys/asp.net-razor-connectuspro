
namespace SiteBlue.DocumentGeneration.Invoices
{
    public class InvoiceLine
    {
        public int Sequence { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ExtendedPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
    }
}