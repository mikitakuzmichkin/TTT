using TicTacToe.Players.Avatars;

namespace TicTacToe.Players
{
    public class PlayerModel : IPlayerModel
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public AvatarType Avatar { get; set; }
    }
}