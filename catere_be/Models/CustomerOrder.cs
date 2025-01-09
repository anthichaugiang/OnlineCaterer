using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class CustomerOrder
    {
        [Key]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int? RoomId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public double VAT { get; set; }
        public string Status { get; set; }
        public int PeopleCount { get; set; }
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public double TotalPrice { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<CustomerOrderMenu> CustomerOrderMenu { get; set; }
    }
}
