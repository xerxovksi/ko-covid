namespace KO.Covid.Domain.Entities
{
    using System.Collections.Generic;

    public class Subscriber : Entity
    {
        public string Name { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public int? Age { get; set; }

        public List<Geo> Districts { get; set; }

        public List<string> Pincodes { get; set; }

        public HashSet<int> LastNotifiedCenters { get; set; }

        public bool? IsActive { get; set; }

        public Subscriber()
        {
            this.Districts = new List<Geo>();
            this.Pincodes = new List<string>();
            this.LastNotifiedCenters = new HashSet<int>();
        }

        public Subscriber AddId()
        {
            this.Id = this.Mobile;
            return this;
        }
    }
}
