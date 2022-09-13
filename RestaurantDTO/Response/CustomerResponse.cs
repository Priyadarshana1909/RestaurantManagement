using System.ComponentModel.DataAnnotations;

namespace RestaurantDTO.Response
{
    public class CustomerResponse : BaseResponse
    {
        public List<Customer> Customers { get; set; } = new();
    }

    public class Customer
    {
        public int CustomerID { get; set; }

        public int RestaurantID { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }

        [Display(Name = "Restaurant Name")]
        public string RestaurantName { get; set; }

    }
}
