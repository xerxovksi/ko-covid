namespace KO.Covid.Application.LoadBalancers
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Models;

    public class PublicTokenLoadBalancer : ITokenLoadBalancer
    {
        private const int Default = 1;

        private int incrementer = 0;

        public TokenType TokenType { get; }

        public int Threshold { get; }

        public PublicTokenLoadBalancer(TokenType tokenType, int threshold)
        {
            this.TokenType = tokenType;
            this.Threshold = threshold;

            this.incrementer = Default;
        }

        public int GetIndex(int modulus)
        {
            if (this.incrementer >= this.Threshold)
            {
                this.Reset();
            }

            var index = this.incrementer % modulus;
            incrementer++;

            return index;
        }

        private void Reset() => this.incrementer = Default;
    }
}
