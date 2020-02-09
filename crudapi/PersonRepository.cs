
using crudapi;
using eShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crudapi
{
    public interface IPersonRepository
    {
        IEnumerable<Person> GetPersons();
        Person GetPerson(int personId);
        void AddPerson(Person aPerson);
        bool Save();
    }


    public class PersonRepository : IPersonRepository
    {
        private readonly PersonContext _context;

        public PersonRepository(PersonContext context)
        {
            _context = context;
        }

        //add person
        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void AddPerson(Person aPerson)
        {
            _context.Persons.Add(aPerson);
        }

        public Person GetPerson(int personId)
        {
            return _context.Persons.Where(c => c.Id == personId).FirstOrDefault();
        }

        public IEnumerable<Person> GetPersons()
        {
            return _context.Persons.OrderBy(c => c.Id).ToList();
        }
    }
}
