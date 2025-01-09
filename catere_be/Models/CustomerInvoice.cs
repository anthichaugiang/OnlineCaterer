using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class CustomerInvoice
    {
        [Key]
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public float GrandTotal { get; set; }
        public float VAT { get; set; }
        public bool IsActive { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Room Room { get; set; }
        public virtual CustomerOrder Order { get; set; }
        public virtual ICollection<SupplierInvoice> SupplierInvoices { get; set; }
    }

}
