namespace catere_be.Dto
{

    public class ServiceDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string? Description { get; set; }
        public int SupplierId { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public List<RoomDto>? Rooms { get; set; }
    }
}
