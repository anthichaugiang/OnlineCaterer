namespace catere_be.Dto
{
    public class CustomerOrderMenuDto
    {
        public int OrderMenuId { get; set; }
        public int MenuItemId { get; set; }
        public int? RoomId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
    }
}
