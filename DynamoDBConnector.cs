
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Options;
using PineapplePizza.Config;
using Amazon.DynamoDBv2.Model;
using PineapplePizza.Model;
using PineapplePizza.Model.DynamoDB;

namespace PineapplePizza
{
    public class DynamoDBConnector
    {
        IAmazonDynamoDB _dynamoDBClient { get; set; }
        DynamoDBOperationConfig _opConfig { get; set; }

        public DynamoDBConnector(IOptions<AppConfig> appConfigOptions, IAmazonDynamoDB client) :
            this(appConfigOptions.Value.DynamoDBTableName, client)
        {
        }

        public DynamoDBConnector(string dynamoDBTableName, IAmazonDynamoDB dynamoDBClient)
        {
            _dynamoDBClient = dynamoDBClient;
            _opConfig = new DynamoDBOperationConfig
            {
                OverrideTableName = dynamoDBTableName
            };
        }

        public async Task<Employee> GetEmployeeAsync(EmployeeId id)
        {
            var context = new DynamoDBContext(_dynamoDBClient);
            var idDynamo = new EmployeeIdDynamo();
            idDynamo.FromPoco(id);
            var employee = await context.LoadAsync<EmployeeDynamo>(idDynamo.Value, _opConfig);
            return employee.ToPoco();
        }
    }
}
