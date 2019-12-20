using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.Data;
using back_end.Models;

namespace back_end.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public PersonsController(ApplicationDbContext context) {
            _context = context;
            _context.Database.Migrate();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersonsList () {
            return Ok(await _context.Persons.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(long id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound();
            else return Ok(person);
        }

        bool PersonExists(long id) {
            var person = _context.Persons.Find(id);
            if (person == null) return false;
            return true;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Person>> UpdatePerson(long id, Person person)
        {
            Console.WriteLine("Edited "+id);
            _context.Entry(person).State = EntityState.Modified;
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!PersonExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }

        [HttpPost]
        public async Task<ActionResult<Person>> AddPerson(Person person)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _context.Persons.Add(person);

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!PersonExists(person.Id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Person>> RemovePerson(long id) {
            var person = await _context.Persons.FindAsync(id);
            if (person == null) {
                return NotFound();
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}