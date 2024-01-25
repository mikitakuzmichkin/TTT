namespace TicTacToe.Services.RateUs
{
    public interface IRateUsResolver
    {
        bool CanBeShown { get; }

        void RegisterResult(RateUsResult result);
    }
}