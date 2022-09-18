namespace RestaurantDTO.Response
{
    public class DiningTableResponse : BaseResponse
    {
        public List<DiningTable> DiningTables { get; set; } = new();
    }

    public class DiningTable
    {
        public int DiningTableID { get; set; }

        public int RestaurantID { get; set; }

        public string Location { get; set; }
    }
}
