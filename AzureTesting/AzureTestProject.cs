using crudWithAzure.Data;
using crudWithAzure.models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;

namespace AzureTesting
{
    public class AzureTestProject
    {
        [Fact]
        public async Task getAllDatas()
        {
            var id = 1;
            await using var application = new
            WebApplicationFactory<Program>();
            using var client = application.CreateClient();
            var response = await client.GetAsync($"/getAllData/{id}");
            var data = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task getData()
        {
            await using var application = new
            WebApplicationFactory<Program>();
            using var client = application.CreateClient();
            string fileName = "cat";
            var Id = "6a76562e-7895-4fe7-a1ee-4beb483e9b1c";
            var response = await client.GetAsync($"/getentityasync/{fileName}/{Id}");
            //var data = await response.Content.ReadAsStringAsync();
            Assert.NotNull(response);
        }
        [Fact]
        public async Task postData()
        {
            await using var application = new
            WebApplicationFactory<Program>();
            var client = application.CreateClient();            
            var response = await client.PostAsJsonAsync("/upsertentityasync", new FileData()
            {
                // Id = Guid.NewGuid().ToString(),
                FileExtension = "png",
                FileName = "ak",
                FileCreated = DateTime.Today.ToUniversalTime(),
                UserId = 2
            });
          
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
        }
        [Fact]
        public async Task updateData()
        {
            await using var application = new
            WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.PutAsJsonAsync("/updateentityasync", new FileData()
           {
  Id= "d39c5a82-72e8-425c-8928-5d1e0f6da1b5",
  FileExtension= "rahul",
  FileName="beautyplus",
 FileCreated= DateTime.Today.ToUniversalTime(),
  UserId= 2,
  PartitionKey= "jddj",
 RowKey= "d39c5a82-72e8-425c-8928-5d1e0f6da1b5",
 
});

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
        [Fact]
        public async Task deleteData()
        {
            await using var application = new
            WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var name = "gg";
            var id = "acefa2b0-84b8-43c3-b846-1ed9469c0f8f";
            var extension = "jpg";
            var response = await client.DeleteAsync($"/Delete/{name}/{id}/{extension}");
           

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

    }
}