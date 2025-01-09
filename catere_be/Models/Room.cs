using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace catere_be.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public double Price { get; set; }
        public int ServiceId { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        [ForeignKey("ServiceId")]
        public Service ? Service { get; set; }
        //public virtual Service Service { get; set; }
        //public virtual ICollection<CustomerOrder> CustomerOrders { get; set; }
        //public virtual ICollection<CustomerInvoice> CustomerInvoices { get; set; }
        //public virtual ICollection<CustomerOrderMenu> CustomerOrderMenus { get; set; }
    }

}
