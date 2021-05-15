namespace KO.Covid.Domain.Entities
{
    using System;
    using System.Collections.Generic;

    public class Subscriber : Entity
    {
        public string Mobile { get; set; }

        public string Email { get; set; }

        public List<string> Pincodes { get; set; }

        public List<int> Districts { get; set; }

        public List<int> LastNotifiedCenters { get; set; }

        public DateTime? LastNotifiedOn { get; set; }

        public DateTime? LastAuthorizedOn { get; set; }

        public Subscriber()
        {
            this.Id = this.Mobile;

            this.Pincodes = new List<string>();
            this.Districts = new List<int>();
            this.LastNotifiedCenters = new List<int>();
        }
    }
}
