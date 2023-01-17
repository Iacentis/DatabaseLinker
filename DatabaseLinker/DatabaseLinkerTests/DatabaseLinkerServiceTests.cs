using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLinker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.IO.Ports;
using System.Net.Http.Headers;

namespace DatabaseLinker.Tests
{
    [TestClass()]
    public class DatabaseLinkerServiceTests
    {
        [TestMethod()]
        public async Task DatabaseLinkerServiceTestAsync()
        {

            DatabaseLinkerService.GetterClient.DefaultRequestHeaders.Accept.Clear();
            DatabaseLinkerService.GetterClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            DatabaseLinkerService.SetterClient.DefaultRequestHeaders.Accept.Clear();
            DatabaseLinkerService.SetterClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            DatabaseLinkerService service = new DatabaseLinkerService();
            service.POST_URL = "https://0a112ada-ed43-4c3f-99c6-7ad1c29a4eea.mock.pstmn.io/";
            service.GET_URL = "https://0a112ada-ed43-4c3f-99c6-7ad1c29a4eea.mock.pstmn.io/";
            service.POST_RESOURCE = "/spacecrafts/1";
            service.GET_RESOURCE = "/spacecrafts/1";
            await service.GET();
            await service.GET();
            await service.GET();
        }
    }
}