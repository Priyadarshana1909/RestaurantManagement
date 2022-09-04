namespace RestaurantDTO.Response
{
    public class RestaurantResponse : BaseResponse
    {
        public List<Restaurant> Restaurants { get; set; } = new();
    }

    public class Restaurant
    {
        public int RestaurantID { get; set; }

        public string RestaurantName { get; set; }

        public string Address { get; set; }

        public string MobileNo { get; set; }
    }
}
