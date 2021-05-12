namespace KO.Covid.Application
{
    using System;

    public class Correlator
    {
        private int id = -1;
        public int Id
        {
            get => this.id;
            set => this.id = value;
        }

        public Correlator() => this.id = Math.Abs(Guid.NewGuid().GetHashCode());
    }
}
