using System;
using System.Threading.Tasks;
using IMS.Service.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest2
    {
        private OrderService orderService = new OrderService();
        [TestMethod]
        public async Task TestMethod2()
        {
            await orderService.ValidOrder(1);
        }
    }
}
