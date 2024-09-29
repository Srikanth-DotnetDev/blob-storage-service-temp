using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain
{
    public class Person
    {
        public Person(string? name, DateTime dOB)
        {
            Name = name;
            DOB = dOB;
        }

        public string? Name { get; set; }   
        public DateTime DOB { get; set; }
        
    }
    //public static class Age
    //{
    //    public static int GetAge(this object obj)
    //    {
    //        var person = (Person)obj!;
    //        if(person is null)
    //        {
    //            return 0;
    //        } 
    //        return DateTime.Today.Year - obj.do.Year;
    //    }
    //}
}
