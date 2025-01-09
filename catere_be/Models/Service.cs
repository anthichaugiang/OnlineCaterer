using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace catere_be.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string? Description { get; set; }
        public int SupplierId { get; set; }
        public string ?ImageUrl { get; set; }
        public bool IsActive { get; set; }


        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; } // Thêm thuộc tính dẫn hướng

        public ICollection<Room>? Room { get; set; }
    }

}
