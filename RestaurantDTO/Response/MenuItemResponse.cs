namespace RestaurantDTO.Response
{
    public class MenuItemResponse : BaseResponse
    {
        public List<MenuItem> MenuItems { get; set; } = new();
    }

    public class MenuItem
    {
        public int MenuItemID { get; set; }

        public int CuisineID { get; set; }

        public string ItemName { get; set; }

        public double ItemPrice { get; set; }
    }
}
