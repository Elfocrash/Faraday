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

            serviceCollection.AddSingleton<IDynamoStore<Book>>(provider => new DynamoStore<Book>(dynamoDbClient));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var dynamoStore = serviceProvider.GetRequiredService<IDynamoStore<Book>>();

            var book = new Book
            {
                Id = Guid.NewGuid().ToString(),
                Author = "J. R. R. Tolkien",
                Title = "The Hobbit",
                YearReleased = 1937
            };

            await dynamoStore.UpsertAsync(book);

            var returned = await dynamoStore.GetAsync(book.Id);

            await dynamoStore.DeleteAsync(returned);

            Console.WriteLine("Hello World!");
        }
    }
}
