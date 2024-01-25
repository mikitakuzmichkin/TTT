using TicTacToe.Players;

namespace TicTacToe
{
    public class GameplayData
    {
        private EGameMode _mode;
        private EWebType _webType;
        private EPersonType _personType;
        private bool _continue;

        private IPlayerModel _playerYour;
        private IPlayerModel _playerEnemy;

        public EGameMode Mode { get => _mode; set => _mode = value; }
        public EWebType WebType { get => _webType; set => _webType = value; }
        public EPersonType PersonType { get => _personType; set => _personType = value; }
        public bool Continue { get => _continue; set => _continue = value; }
        public IPlayerModel PlayerYour { get => _playerYour; set => _playerYour = value; }
        public IPlayerModel PlayerEnemy { get => _playerEnemy; set => _playerEnemy = value; }
    }
}