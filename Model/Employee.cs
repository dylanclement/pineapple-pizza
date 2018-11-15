using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model
{
    public class Employee
    {
        public EmployeeId Id { get; set; }
        public IdCard ActiveIdCard { get; set; }
    }
}
