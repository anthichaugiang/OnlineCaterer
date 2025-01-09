using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public bool IsActive { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }

}
