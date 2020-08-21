using Faraday.Attributes;

namespace Faraday.Examples
{
    [FaradayTableName("books")]
    public class Book
    {
        [FaradayPartitionKey()]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int YearReleased { get; set; }
    }
}
