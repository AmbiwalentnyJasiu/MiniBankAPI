using Microsoft.AspNetCore.Mvc;
using MiniBankAPI.Controllers;
using MiniBankAPI.Interfaces;
using MiniBankAPI.Models;
using MiniBankAPI.Services;
using System.Collections.Generic;
using Xunit;

namespace MiniBankTest {

    [TestCaseOrderer("MiniBankTest.PriorityOrderer", "MiniBankTest")]
    public class ContactTest {
        private IDbService dbService;
        private ContactInfoController controller;

        public ContactTest() {
            dbService = new DbService();
            controller = new ContactInfoController(dbService);
        }

        [Fact, TestPriority(-5)]
        public void GetReturnsAllItems() {
            var okResult = controller.Get() as OkObjectResult;

            var items = Assert.IsType<List<ContactInfoModel>>(okResult.Value);
            Assert.Equal(5, items.Count);
        }

        [Fact, TestPriority(-5)]
        public void GetFromServerReturnsAllItems() {
            var okResult = controller.GetFromServer("WINSERV019") as OkObjectResult;

            var items = Assert.IsType<List<ContactInfoModel>>(okResult.Value);
            Assert.Equal(2, items.Count);
        }

        [Fact, TestPriority(-5)]
        public void GetByNumberReturnsCorrectItem() {
            var okResult = controller.Get(1) as OkObjectResult;

            var item = Assert.IsType<ContactInfoModel>(okResult.Value);
            Assert.Equal("Maciej", item.FirstName);
        }

        [Fact, TestPriority(0)]
        public void PostReturnsCreatedItem() {
            var insert = new ContactInfoModel() { FirstName = "Imie", MiddleName = "", LastName = "Nazwisko", PhoneNumber = "123409875", PNDirCode = "+32", AddressInfo = 6 };
            var okResult = controller.Post(insert, "WINSERV022") as OkObjectResult;

            var item = Assert.IsType<ContactInfoModel>(okResult.Value);
            Assert.Equal("123409875", item.PhoneNumber);
        }

        [Fact, TestPriority(5)]
        public void RemoveWorks() {
            var okResult = controller.Remove(6, "Winserv022");

            Assert.IsType<OkObjectResult>(okResult);
        }
    }
}
