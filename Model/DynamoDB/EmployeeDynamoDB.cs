using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model.DynamoDB
{
    public class EmployeeDynamoDB : IEmployee
    {
        public EmployeeId Id => throw new NotImplementedException();

        public IdCard ActiveIdCard => throw new NotImplementedException();
    }
}
