using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;
using System;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using challenge.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private HttpClient _httpClient;
        private TestServer _testServer;

        [TestInitialize]
        public void InitializeTest()
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [TestCleanup]
        public void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Employee = "10dd63a3-0627-43fd-947e-bef59b3fa6c1",
                EffectiveDate = new DateTimeOffset(DateTime.UtcNow.Date),
                Salary = 200000
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            //Assert.IsNotNull(newCompensation.Employee.Employee);
            Assert.IsNotNull(newCompensation.Employee);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
        }

        [TestMethod]
        public void CreateCompensation_Returns_NotFound()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Employee = Guid.NewGuid().ToString(),
                EffectiveDate = new DateTimeOffset(DateTime.UtcNow.Date),
                Salary = 1
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;


            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }


        [TestMethod]
        public async Task CreateCompensation_RequiresSalary()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Employee = Guid.NewGuid().ToString(),
                EffectiveDate = new DateTimeOffset(DateTime.UtcNow.Date),
            };

            var jsonSerialization = new JsonSerialization();
            var requestContent = jsonSerialization.ToJson(compensation);

            // Execute
            var postRequestTask = await _httpClient.PostAsync("api/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask;
            

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var jsonContent = response.DeserializeContent<JObject>();
            var salaryErrors = jsonContent.Property(nameof(Compensation.Salary));
            Assert.IsNotNull(salaryErrors);
            Assert.IsTrue(salaryErrors.HasValues);
        }

        [TestMethod]
        public async Task CreateCompensation_RequiresNonNegativeSalary()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Employee = Guid.NewGuid().ToString(),
                EffectiveDate = new DateTimeOffset(DateTime.UtcNow.Date),
                Salary = -1
            };

            var jsonSerialization = new JsonSerialization();
            var requestContent = jsonSerialization.ToJson(compensation);

            // Execute
            var postRequestTask = await _httpClient.PostAsync("api/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask;


            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var jsonContent = response.DeserializeContent<JObject>();
            var salaryErrors = jsonContent.Property(nameof(Compensation.Salary));
            Assert.IsNotNull(salaryErrors);
            Assert.IsTrue(salaryErrors.HasValues);
        }

        [TestMethod]
        public async Task CreateCompensation_RequiresEffectiveDate()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Employee = Guid.NewGuid().ToString(),
                Salary = 4
            };

            var jsonSerialization = new JsonSerialization();
            var requestContent = jsonSerialization.ToJson(compensation);

            // Execute
            var postRequestTask = await _httpClient.PostAsync("api/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask;


            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            var jsonContent = response.DeserializeContent<JObject>();
            var effectiveDateErrors = jsonContent.Property(nameof(Compensation.EffectiveDate));
            Assert.IsNotNull(effectiveDateErrors);
            Assert.IsTrue(effectiveDateErrors.HasValues);
        }


        [TestMethod]
        public async Task CreateCompensation_RequiresUniqueDatePerEmployee()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Employee = "10dd63a3-0627-43fd-947e-bef59b3fa6c1",
                EffectiveDate = new DateTimeOffset(DateTime.UtcNow.Date),
                Salary = 200000
            };


            var jsonSerialization = new JsonSerialization();
            var requestContent = jsonSerialization.ToJson(compensation);

            var postRequestTask = await _httpClient.PostAsync("api/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask;
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            compensation = new Compensation()
            {
                Employee = "10dd63a3-0627-43fd-947e-bef59b3fa6c1",
                EffectiveDate = new DateTimeOffset(DateTime.UtcNow.Date),
                Salary = 100000
            };

            requestContent = jsonSerialization.ToJson(compensation);

            // Execute
            postRequestTask = await _httpClient.PostAsync("api/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            response = postRequestTask;

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [TestMethod]
        public void GetCompensationsById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensations = response.DeserializeContent<List<Compensation>>();
            Assert.AreEqual(0, compensations.Count);
        }

        [TestMethod]
        public void GetCompensationsById_Returns_SingleResultAsArray()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            using (var scope = _testServer.Host.Services.CreateScope())
            {
                var compensationService = scope.ServiceProvider.GetRequiredService<ICompensationService>();

                compensationService.Create(
                    new Compensation()
                    {
                        Employee = employeeId,
                        Salary = 200000,
                        EffectiveDate = DateTimeOffset.UtcNow
                    });

            }

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensations = response.DeserializeContent<List<Compensation>>();
            Assert.AreEqual(1, compensations.Count);
        }

        [TestMethod]
        public void GetCompensationsById_Returns_MultipleResults()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            using (var scope = _testServer.Host.Services.CreateScope())
            {
                var compensationService = scope.ServiceProvider.GetRequiredService<ICompensationService>();

                compensationService.Create(
                    new Compensation()
                    {
                        Employee = employeeId,
                        Salary = 100000,
                        EffectiveDate = DateTimeOffset.UtcNow.AddDays(-365)
                    });

                compensationService.Create(
                    new Compensation()
                    {
                        Employee = employeeId,
                        Salary = 200000,
                        EffectiveDate = DateTimeOffset.UtcNow
                    });

            }

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensations = response.DeserializeContent<List<Compensation>>();
            Assert.AreEqual(2, compensations.Count);
        }
    }
}
