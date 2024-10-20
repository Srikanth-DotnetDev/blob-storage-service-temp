using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public record StudentRecordLocator(string StudentId, string LocalId)
    {
        public override string? ToString()
        {
            var stu = StudentId switch
            {
                _ when !string.IsNullOrEmpty(StudentId.Split("@").First()) => StudentId,
                _ when !string.IsNullOrEmpty(LocalId.Split("@").First()) => LocalId,
                _ => null
            };
            return stu;
        }
    }
    //public sealed record LegacyStatewideStudentIdentifier(string Value) 
    //{
    //    public override string ToString()
    //    {
    //        return string.IsNullOrWhiteSpace(Value) || Value.EndsWith("@ssid", true, null) ? Value : $"{Value}@ssid";
    //    }
    //}
    //public sealed record LeaStudentIdentifier(LeaNumber LeaNumber, string Value)
    //{ 
    //    public override string ToString()
    //        => $"{Value}@lea{LeaNumber.Number}";
    //}

    //public sealed record LeaNumber(string? Number);
}
