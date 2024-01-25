namespace TicTacToe.Players
{
    public class HumanPlayerController : APlayerController
    {
        public HumanPlayerController(IPlayerModel model) : base(model)
        {
        }

        public void SetCell(params int[] indexes)
        {
            //todo controller.SetCell(indexes);
        }
    }
}