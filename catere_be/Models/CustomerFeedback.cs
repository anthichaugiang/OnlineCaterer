using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class CustomerFeedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public int SupplierId { get; set; }
        public int CustomerId { get; set; }
        public string Comment { get; set; }
        public DateTime FeedbackDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }

      //  public virtual Supplier Supplier { get; set; }
      //  public virtual Customer Customer { get; set; }
    }

}
