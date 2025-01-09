namespace catere_be.Dto
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public double Price { get; set; }
        public int ServiceId { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
