using System;
using System.ComponentModel.DataAnnotations;

namespace back_end.Models {
    public class Person {
        public long Id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string email { get; set; } 

        [Required]
        public DateTimeOffset dob { get; set; }

        public string country { get; set; }
    } 
}