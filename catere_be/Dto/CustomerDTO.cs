using catere_be.Models;

namespace catere_be.Dto
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public string CustomerType { get; set; }
        public string LoginName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<CustomerOrder> CustomerOrders { get; set; }
        public virtual ICollection<CustomerInvoice> CustomerInvoices { get; set; }
        public virtual ICollection<CustomerFeedback> CustomerFeedbacks { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
    }
}
