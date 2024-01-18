namespace RabbitMQ.Producer
{
    public class Product
    {
        public Int64 Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public Decimal Price { get; set; }
    }
}
