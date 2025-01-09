namespace catere_be.Dto
{
    public class OrderRequestModel
    {
        public int SupplierId { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int NumPeople { get; set; }
        public List<MenuOrder> ListMenu { get; set; }
        public int? RoomId { get; set; }
        public float TotalPrice { get; set; }
        public float VAT { get; set; } // Add VAT field
    }

}
