namespace catere_be.Dto
{
    public class SupplierDto
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string? Level { get; set; }
        public int? CityId { get; set; }
        public string? ImageUrl { get; set; }
        public string LoginName { get; set; }
        public bool IsActive { get; set; }
        public List<ServiceDto> Services { get; set; }
        public double MinRoomPrice { get; set; }
    }
}
