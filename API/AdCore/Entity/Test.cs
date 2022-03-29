namespace AdCore.Entity
{
    public class Test
    {
        public Test()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string? Name { get; set; }
    }
}
