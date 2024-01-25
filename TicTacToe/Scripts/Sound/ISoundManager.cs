using Dainty.Sound;

namespace TicTacToe.Sound
{
    public interface ISoundManager : IDaintySoundManager
    {
        void MusicStart(GameSound sound);
        void Sound(GameSound sound, bool playCompletely = false);
    }
}