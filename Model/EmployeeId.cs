using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model
{
    public interface IEmployeeId
    {
        string Name { get; }
        int Number { get;  }
    }

    public class EmployeeId : IEmployeeId
    {
        public string Name { get; set; }
        public int Number { get; set; }

        public EmployeeId(string name, int number)
        {
            Name = name;
            Number = number;
        }

        public override string ToString()
        {
            return Number + "-" + Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Number.GetHashCode();
        }
    }
}
