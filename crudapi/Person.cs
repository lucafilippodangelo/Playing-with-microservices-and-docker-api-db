using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eShopWeb.Models
{
    public class Person
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)] //LD identity option because I want that a new "Id" for the "Key" is generated once just when created
        public int Id { get; set; } //LD authomatic recorded as a primary key by convention

        public string name { get; set; }
        public string surname { get; set; }
        public string mail { get; set; }

        public Person() { }

        public Person(String Name, String Surname, String Mail)
        {
            this.name = Name;
            this.surname = Surname;
            this.mail = Mail;
        }
    }
}
