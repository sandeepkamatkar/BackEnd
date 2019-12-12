using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using back_end.Models;
using Newtonsoft.Json;

namespace back_end.IntegrationTests {
    public class PersonsRouteShould : IClassFixture<TestFixture> {
        private readonly HttpClient _client;
        public static bool isInitialised = false;

        public PersonsRouteShould(TestFixture fixture) {
            _client = fixture.Client;
            if (!PersonsRouteShould.isInitialised)
                this.initForGet();
        }

        private void initForGet() {
            PersonsRouteShould.isInitialised = true;

            Person[] persons = new Person[] {
                new Person() { name = "Motish Mehta", email = "motish.mehta@varian.com", dob = new DateTimeOffset(new DateTime(1997, 11, 11)), country = "India" },
                new Person() { name = "Atul Gunjal", email = "Atul.Gunjal@varian.com", dob = new DateTimeOffset(new DateTime(1997, 07, 01)), country = "India" },
                new Person() { name = "Shreyas Bhaskarwar", email = "Shreyas.Bhaskarwar@varian.com", dob = new DateTimeOffset(new DateTime(1997, 01, 24)), country = "India" },
                new Person() { name = "Tejas Chinchole", email = "Tejas.Chinchole@varian.com", dob = new DateTimeOffset(new DateTime(1997, 09, 05)), country = "India" },
                new Person() { name = "Vaibhav Kumbhar", email = "Vaibhav.Kumbhar@varian.com", dob = new DateTimeOffset(new DateTime(1997, 11, 14)) }
            };

            foreach (var person in persons) {
                var request = new HttpRequestMessage(HttpMethod.Post,"/api/persons/"){Content = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json")};
                var response = _client.SendAsync(request).Result;
            }
        }

        [Fact]
        public async Task Get_ReturnsAllItems () {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/persons/");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string jsonFromGetResponse = await response.Content.ReadAsStringAsync();

            Person[] singleResponse = JsonConvert.DeserializeObject<Person[]>(jsonFromGetResponse);

            Assert.Equal(singleResponse[0].name, "Motish Mehta");
        }

        [Fact]
        public async Task GetById_ReturnsExpectedItem()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/persons/1");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string jsonFromGetResponse = await response.Content.ReadAsStringAsync();

            Person singleResponse = JsonConvert.DeserializeObject<Person>(jsonFromGetResponse);

            Assert.Equal(singleResponse.name, "Motish Mehta");
        }

        [Fact]
        public async Task Post_SendsCreatedResopnse()
        {
            var person = new Person() { name = "Test12", email = "test.12@varian.com", dob = new DateTimeOffset(new DateTime(2001, 01, 21)), country = "India" };
                
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/persons/"){ Content = new StringContent(JsonConvert.SerializeObject(person), Encoding.UTF8, "application/json") };

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            string jsonFromGetResponse = await response.Content.ReadAsStringAsync();

            Person singleResponse = JsonConvert.DeserializeObject<Person>(jsonFromGetResponse);

            Assert.Equal(singleResponse.name, "Test12");
        }
    }
}
