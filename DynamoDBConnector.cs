
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Options;
using PineapplePizza.Config;
using Amazon.DynamoDBv2.Model;
using PineapplePizza.Model;

namespace PineapplePizza
{
    public class DynamoDBConnector
    {
        IAmazonDynamoDB _dynamoDBClient { get; set; }
        string _dynamoDBTableName { get; set; }

        public DynamoDBConnector(IOptions<AppConfig> appConfigOptions, IAmazonDynamoDB client) :
            this(appConfigOptions.Value.DynamoDBTableName, client)
        {
        }

        public DynamoDBConnector(string dynamoDBTableName, IAmazonDynamoDB dynamoDBClient)
        {
            _dynamoDBClient = dynamoDBClient;
            _dynamoDBTableName = dynamoDBTableName;
        }

        public async Task<Employee> GetEmployeeAsync(EmployeeId id)
        {
            var request = new GetItemRequest
            {
                TableName = _dynamoDBTableName,
                Key = id.ToDynamoDB(),
                ConsistentRead = true
            };
            var response = await _dynamoDBClient.GetItemAsync(request);
            return null;
        }
    }
}
