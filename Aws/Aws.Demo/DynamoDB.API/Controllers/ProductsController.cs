using DynamoDB.API.Models.DynamoDBModel;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;
using Amazon;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using System.Runtime.Intrinsics.X86;


namespace DynamoDB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly AmazonDynamoDBClient _client;
        private const string tblName = "products";
        public ProductsController() {

              _client = new AmazonDynamoDBClient("AWS Access key",
                            "AWS Secret access key", 
                            RegionEndpoint.GetBySystemName("ap-south-1")
                                                );
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tableName = tblName;
            // Create a ScanRequest to retrieve all items in the table
            var scanRequest = new ScanRequest
            {
                TableName = tableName,
            };

            var scanResponse = await _client.ScanAsync(scanRequest);
            // Process the scan results

            var products = new List<Product>();

            foreach (var item in scanResponse.Items)
            {
                var document = Document.FromAttributeMap(item);
                var json = document.ToJson();
                var product = JsonConvert.DeserializeObject<Product>(json);
                if (product != null)
                {
                    products.Add(product);
                }
            }
            return Ok(products);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
          
            string tableName = tblName;
            string partitionKey = "id";
            string partitionKeyValue = id.ToString();

            var queryRequest = new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = $"{partitionKey} = :value",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":value", new AttributeValue { S = partitionKeyValue } }
                }
            };

            var product = new  Product();
            var response = await _client.QueryAsync(queryRequest);

            foreach (var item in response.Items)
            {
                var document = Document.FromAttributeMap(item);
                var json = document.ToJson();
                var dbProduct = JsonConvert.DeserializeObject<Product>(json);
                if (dbProduct != null)
                {
                    product.Id = dbProduct.Id;
                    product.Name = dbProduct.Name;
                    product.Barcode = dbProduct.Barcode;
                    product.Description = dbProduct.Description;
                    product.Price = dbProduct.Price;
                }
            }
            return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Product product)
        {
            string tableName = tblName;

            var table = Table.LoadTable(_client, tableName);

            // Create an item to be inserted
            var newItem = new Document
            {
                ["id"] = product.Id,
                ["barcode"] = product.Barcode,
                ["description"] = product.Description,
                ["name"] = product.Name,
                ["price"] = product.Price
                // Add more attributes as needed
            };

            // Insert the item into DynamoDB
            await table.PutItemAsync(newItem);

            return Ok("Record inserted");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Product product)
        {
            string tableName = tblName;

            var key = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { S = product.Id } }, // Replace with your actual key values
                { "barcode", new AttributeValue  { S = product.Barcode } } 
            };

            var updates = new Dictionary<string, AttributeValueUpdate>
            {
               
                { "description", new AttributeValueUpdate { Action = AttributeAction.PUT, Value = new AttributeValue { S = product.Description } } },
                { "name", new AttributeValueUpdate { Action = AttributeAction.PUT, Value = new AttributeValue { S = product.Name } } },
                { "price", new AttributeValueUpdate { Action = AttributeAction.PUT, Value = new AttributeValue { N = product.Price.ToString() } } }
            };

            var request = new UpdateItemRequest
            {
                Key = key,
                AttributeUpdates = updates,
                TableName = tableName
            };
             
            var response = await _client.UpdateItemAsync(request);
        
            return Ok($"Updated sucessfully  { response.HttpStatusCode } ");
        } 


        [HttpDelete("{id},{barcode}")]
        public async Task<IActionResult> DeleteEmployee(string id,string barcode)
        {
            var key = new Dictionary<string, AttributeValue>
            {
                { "id", new AttributeValue { S = id } },  // Replace with your actual key values
                { "barcode", new AttributeValue { S = barcode } } // Include sort key if applicable
            };

            var request = new DeleteItemRequest
            {
                Key = key,
                TableName = tblName
            };

            var response = await _client.DeleteItemAsync(request);
            return Ok();
        }
    }
}
