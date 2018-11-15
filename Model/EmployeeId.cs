using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model
{
    public struct EmployeeId
    {
        public string Name { get; set; }
        public int Number { get; set; }

        public override string ToString()
        {
            return Name + Number;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Number.GetHashCode();
        }
    }
}
