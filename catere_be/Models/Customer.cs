using System.ComponentModel.DataAnnotations;

namespace catere_be.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string ? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string  ?Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ?PhoneNumber { get; set; }
        public string ?Email { get; set; }
        public string? Address { get; set; }
        public string ?ImageUrl { get; set; }
        public string ?CustomerType { get; set; }
        public string ?LoginName { get; set; }
        public string ?PasswordHash { get; set; }
        public bool IsActive { get; set; }

     
    }
}
