using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;

namespace Faraday.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dynamoDbClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest2);

            var serviceCollection = new ServiceCollection();

            //serviceCollection.AddSingleton<IDynamoStore<Car>>(provider => new DynamoStore<Car>(dynamoDbClient,"faraday", it => it.Id));

            serviceCollection.AddSingleton<IDynamoStore<Car>>(provider =>
                new DynamoStore<Car>(dynamoDbClient,"faradaywithsort", it => it.Id, sortKeyDescriptor: it => it.Brand));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var dynamoStore = serviceProvider.GetRequiredService<IDynamoStore<Car>>();

            var car = new Car
            {
                Id = Guid.NewGuid().ToString(),
                Brand = "Tesla",
                ModelName = "Model 3",
                HorsePower = 360
            };

            await dynamoStore.UpsertAsync(car);

            var returned = await dynamoStore.GetAsync(car.Id, car.Brand);

            await dynamoStore.DeleteAsync(returned);

            Console.WriteLine("Hello World!");
        }
    }
}
