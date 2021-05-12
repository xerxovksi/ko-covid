namespace KO.Covid.Domain.Test.Models
{
    public class Person
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int? Age { get; set; }

        public string[] Comorbidities { get; set; }
    }
}
