using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xunit;

using back_end.Data;
using back_end.Models;
using back_end.Controllers;

namespace back_end.UnitTests {
    public class PersonsControllerTest {
        private PersonsController _controllerForPost;
        private PersonsController _controllerForGet;

        public static bool isInitialised = false;

        public PersonsControllerTest() {
            var optionsforPost = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("forPostDB").Options;
            var optionsforGet = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("forGetDB").Options;
            ApplicationDbContext contextforPost = new ApplicationDbContext(optionsforPost);
            ApplicationDbContext contextforGet = new ApplicationDbContext(optionsforGet);


            _controllerForPost = new PersonsController(contextforPost);
            _controllerForGet = new PersonsController(contextforGet);

            if ( !PersonsControllerTest.isInitialised )
                this.initForGet();
        }

        private async void initForGet() {
            PersonsControllerTest.isInitialised = true;
            Person[] persons = new Person[] {
                new Person() { name = "Motish Mehta", email = "motish.mehta@varian.com", dob = new DateTimeOffset(new DateTime(1997, 11, 11)), country = "India" },
                new Person() { name = "Atul Gunjal", email = "Atul.Gunjal@varian.com", dob = new DateTimeOffset(new DateTime(1997, 01, 07)), country = "India" },
                new Person() { name = "Shreyas Bhaskarwar", email = "Shreyas.Bhaskarwar@varian.com", dob = new DateTimeOffset(new DateTime(1997, 10, 08)), country = "India" },
                new Person() { name = "Tejas Chinchole", email = "Tejas.Chinchole@varian.com", dob = new DateTimeOffset(new DateTime(1997, 01, 05)), country = "India" },
                new Person() { name = "Vaibhav Kumbhar", email = "Vaibhav.Kumbhar@varian.com", dob = new DateTimeOffset(new DateTime(1997, 10, 08)) }
            };

            foreach (var person in persons) {
                await _controllerForGet.AddPerson(person);
            }
        }

        [Fact]
        public async Task Add_InvalidObjectPassed_ReturnsBadRequest() {

            Person person = new Person();

            // person.name = "Motish M";
            person.email = "motish.mehta11@varian.com";
            person.dob = new DateTimeOffset(new DateTime(1997,11,11));
            person.country = "India";
            _controllerForPost.ModelState.AddModelError("Name", "Required");

            var postPersonResult = (await _controllerForPost.AddPerson(person)).Result;

            Assert.IsType<BadRequestObjectResult>(postPersonResult);
        }


        [Fact]
        public async Task Add_ValidObjectPassed_ReturnsCreatedResponse_ResponseHasItem()
        {
            Person person = new Person() {
                name = "Motish Mehta",
                email = "motish.mehta@varian.com",
                dob = new DateTimeOffset(new DateTime(1997, 11, 11)),
                country = "India"
            };

            var postPersonResult = (await _controllerForPost.AddPerson(person)).Result; // "motish", "motish.mehta@varian.com", new DateTimeOffset(new DateTime(1997,11,11)), "India"

            Assert.IsType<CreatedAtActionResult>(postPersonResult);

            var item = (postPersonResult as CreatedAtActionResult).Value as Person; 

            Assert.IsType<Person>(item);

            Assert.Equal("Motish Mehta", item.name);
        }

        [Fact]
        public async Task Get_WhenCalled_ReturnsOkResult_WithAllItems()
        {
            var getPersonResult = (await _controllerForGet.GetPersonsList()).Result; // "motish", "motish.mehta@varian.com", new DateTimeOffset(new DateTime(1997,11,11)), "India"

            Assert.IsType<OkObjectResult>(getPersonResult);

            var items = Assert.IsType<List<Person>>((getPersonResult as OkObjectResult).Value);

            Assert.Equal(5, items.Count);
        }
        
        [Fact]
        public async Task GetById_UnknownIdPassed_ReturnsNotFoundResult()
        {
            var getPersonResult = (await _controllerForGet.GetPerson(1001)).Result; // "motish", "motish.mehta@varian.com", new DateTimeOffset(new DateTime(1997,11,11)), "India"

            Assert.IsType<NotFoundResult>(getPersonResult);
        }

        [Fact]
        public async Task GetById_ExistingIdPassed_ReturnsOkResult_WithRightItem()
        {
            var getPersonResult = (await _controllerForGet.GetPerson(1)).Result; // "motish", "motish.mehta@varian.com", new DateTimeOffset(new DateTime(1997,11,11)), "India"

            Assert.IsType<OkObjectResult>(getPersonResult);

            var item = Assert.IsType<Person>((getPersonResult as OkObjectResult).Value);

            Assert.Equal("Motish Mehta", item.name);
        }
    }
}