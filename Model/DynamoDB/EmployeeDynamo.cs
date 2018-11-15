using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model.DynamoDB
{
    public class EmployeeDynamo : IDynamoObject<Employee>
    {
        [DynamoDBHashKey]
        public string EmployeeId { get; set; }

        [DynamoDBProperty]
        public IdCardDynamo ActiveIdCard { get; set; }

        public void FromPoco(Employee poco)
        {
            var tmp = new EmployeeIdDynamo();
            tmp.FromPoco(poco.Id);
            EmployeeId = tmp.Value;
            ActiveIdCard = new IdCardDynamo();
            ActiveIdCard.FromPoco(poco.ActiveIdCard);
        }

        public Employee ToPoco()
        {
            var tmp = new EmployeeIdDynamo
            {
                Value = EmployeeId
            };
            return new Employee
            {
                ActiveIdCard = ActiveIdCard.ToPoco(),
                Id = tmp.ToPoco()
            };
        }
    }
}
