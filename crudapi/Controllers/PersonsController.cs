using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace crudapi.Controllers
{
    [Route("api/persons")]
    public class PersonsController : Controller
    {

        private readonly IPersonRepository _personRepository;

        public PersonsController(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        [HttpGet("getPersons")]
        public IActionResult GetPersons()
        {
            var model = _personRepository.GetPersons();

            return Ok(new { Persons = model });
        }

        // GET api/values/5
        [HttpGet("getPerson/{id}")]
        public Person GetPerson(int id)
        {
            return _personRepository.GetPerson(id);
        }

        // POST api/values
        [HttpPost("addPerson")]
        public IActionResult AddPerson([FromBody]Person aPerson)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _personRepository.AddPerson(aPerson);
            _personRepository.Save();

            return Ok(aPerson);
        }

        #region dummy postman calls

        [HttpGet]
        public IEnumerable<string> GetDummy()
        {
            return new string[] { "value1", "value2" };
        }

        #endregion
    }
}