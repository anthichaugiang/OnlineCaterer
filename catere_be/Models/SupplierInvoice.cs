using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class SupplierInvoice
    {
        [Key]
        public int SupplierInvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int SupplierId { get; set; }
        public int CustomerInvoiceId { get; set; }
        public bool IsActive { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual CustomerInvoice CustomerInvoice { get; set; }
    }

}
