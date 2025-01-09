using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class SupplierDetail
    {
        [Key]
        public int DetailId { get; set; }
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public int PeopleCount { get; set; }
        public float CustomerCost { get; set; }
        public float SupplierCost { get; set; }
        public bool IsActive { get; set; }

        public virtual Supplier Supplier { get; set; }
    }

}
