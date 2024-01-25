using TicTacToe.Players.Avatars;

namespace TicTacToe.Players
{
    public interface IPlayerModel
    {
        string Uuid { get; }
        string Name { get; }
        AvatarType Avatar { get; }
    }
}