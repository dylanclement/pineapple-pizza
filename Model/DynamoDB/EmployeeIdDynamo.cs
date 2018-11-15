namespace PineapplePizza.Model.DynamoDB
{
    internal class EmployeeIdDynamo : IDynamoObject<EmployeeId>
    {
        public string Value { get; set; }

        public void FromPoco(EmployeeId poco)
        {
            Value = poco.ToString();
        }

        public EmployeeId ToPoco()
        {
            var employeeIdSplit = Value.Split('-');
            return new EmployeeId(employeeIdSplit[1], int.Parse(employeeIdSplit[0]));
        }
    }
}