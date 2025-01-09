using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace catere_be.Models
{
    public class CustomerOrderMenu
    {
        [Key]
        public int OrderMenuId { get; set; }
        public int MenuItemId { get; set; }
        public int? RoomId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("OrderId")]
        public virtual CustomerOrder CustomerOrder { get; set; }
    }

}
