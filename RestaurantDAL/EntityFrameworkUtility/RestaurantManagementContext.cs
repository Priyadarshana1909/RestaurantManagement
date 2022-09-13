using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RestaurantDAL.EntityFrameworkUtility
{
    public class RestaurantManagementContext : DbContext
    {
        public RestaurantManagementContext(DbContextOptions<RestaurantManagementContext> options) : base(options)
        {
        }

       
    }
}
