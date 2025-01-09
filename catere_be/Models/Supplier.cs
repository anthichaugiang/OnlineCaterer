using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ?Level { get; set; }
        public int? CityId { get; set; }
        public string? ImageUrl { get; set; }
        public string LoginName { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsActive { get; set; }

        //public virtual City City { get; set; }
        public virtual ICollection<Service> ?Services { get; set; }
        //public virtual ICollection<Menu> Menus { get; set; }
        //public virtual ICollection<SupplierDetail> SupplierDetails { get; set; }
        //public virtual ICollection<CustomerOrder> CustomerOrders { get; set; }
        //public virtual ICollection<SupplierInvoice> SupplierInvoices { get; set; }
        //public virtual ICollection<CustomerFeedback> CustomerFeedbacks { get; set; }
        //public virtual ICollection<Message> Messages { get; set; }
    }

}
