using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public int CustomerId { get;set; }
        public int SupplierId { get; set; }
        public bool whosend { get; set; }
        public string Content { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsActive { get; set; }

        //public virtual Customer Customer { get; set; }
        //public virtual Supplier Supplier { get; set; }
    }

}
