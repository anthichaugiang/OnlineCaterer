namespace catere_be.Dto
{
    public class CustomerOrderDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int? RoomId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public double VAT { get; set; }
        public string Status { get; set; }
        public int PeopleCount { get; set; }
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public double TotalPrice { get; set; }
        public bool IsActive { get; set; }
        public List<CustomerOrderMenuDto> CustomerOrderMenus { get; set; }
    }
}
