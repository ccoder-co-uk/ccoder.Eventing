namespace EventLibrary.Tests
{
    public partial class ConfigurationTests
    {
        public class TestObject
        {
            public int Id { get; set; } 
            public string Name { get; set; }
        }


        ValueTask HandleTestEvent(IServiceProvider serviceProvider, object data) =>
            ValueTask.CompletedTask;
    }
}