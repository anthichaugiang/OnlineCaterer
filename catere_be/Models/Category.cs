using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        //public virtual ICollection<Menu> Menus { get; set; }
    }
   
}
