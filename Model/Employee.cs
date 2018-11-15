using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model
{
    public interface IEmployee
    {
        EmployeeId Id { get; }
        IdCard ActiveIdCard { get; }
    }

    public class Employee: IEmployee
    {
        public EmployeeId Id { get; set; }
        public IdCard ActiveIdCard { get; set; }
    }
}
