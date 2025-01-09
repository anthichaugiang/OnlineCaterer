using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Menu MenuItem { get; set; }
    }

}
