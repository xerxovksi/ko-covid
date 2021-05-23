namespace KO.Covid.Application.Contracts
{
    using KO.Covid.Application.Models;

    public interface ITokenLoadBalancer
    {
        TokenType TokenType { get; }

        int Threshold { get; }

        int GetIndex(int modulus);
    }
}
