using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestaurantBLL;
using RestaurantDAL.Interface;
using RestaurantDTO.Response;

namespace RestaurantBLLTest
{
    [TestClass]
    public class ManageMenuItemBLLTest
    {
        private readonly Mock<IManageMenuItemDAL> _mockManagemenuItemDAL;

        public ManageMenuItemBLLTest()
        {
            _mockManagemenuItemDAL = new Mock<IManageMenuItemDAL>();
        }

        [TestMethod]
        public void ManageMenuItemBLLTest_GetMenuItems_SuccessResponse()
        {
            var response = new MenuItemResponse();
            response.IsSuccessFull = true;

            _mockManagemenuItemDAL.Setup(x => x.GetMenuItemFromRestaurantId(It.IsAny<int>())).Returns(response);

            ManageMenuItemBLL ManageMenuItemBLL = new ManageMenuItemBLL(_mockManagemenuItemDAL.Object);
            var MenuItemsResponse = ManageMenuItemBLL.GetMenuItems(0);
            Assert.AreEqual(response.MenuItems.Count, MenuItemsResponse?.MenuItems.Count);
        }

        [TestMethod]
        public void ManageMenuItemBLLTest_GetMenuItems_FailResponse()
        {
            var response = new MenuItemResponse();
            response.IsSuccessFull = false;

            _mockManagemenuItemDAL.Setup(x => x.GetMenuItemFromRestaurantId(It.IsAny<int>())).Returns(response);

            ManageMenuItemBLL ManageMenuItemBLL = new ManageMenuItemBLL(_mockManagemenuItemDAL.Object);
            var MenuItemsResponse = ManageMenuItemBLL.GetMenuItems(0);
            Assert.AreEqual(response.IsSuccessFull, MenuItemsResponse?.IsSuccessFull);
        }
       
    }
}