using System;
using IMS.Service.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private OrderService orderService =new OrderService();
        [TestMethod]
        public void TestMethod1()
        {
            orderService.AutoConfirm();
        }
    }
}
