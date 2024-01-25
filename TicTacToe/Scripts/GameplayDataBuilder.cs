using TicTacToe.DI;
using TicTacToe.Players;
using TicTacToe.Players.Avatars;
using TicTacToe.Services.Saving;

namespace TicTacToe
{
    public class GameplayDataBuilder
    {
        private GameplayData _gameplayData;

        public static GameplayDataBuilder NewOffline(EGameMode mode, EPersonType personType)
        {
            var gameplayData = new GameplayData
            {
                Mode = mode,
                WebType = EWebType.Offline,
                PersonType = personType,
                PlayerYour = GetMainPlayerModel(),
                PlayerEnemy = personType == EPersonType.Human ? GetOfflinePlayerModel() : GetBotPlayerModel(),
                Continue = false
            };

            return new GameplayDataBuilder {_gameplayData = gameplayData};
        }

        public static GameplayDataBuilder Continue(EGameMode gameMode)
        {
            var saveManager = ProjectContext.GetInstance<ISaveManager>();
            var save = saveManager.Load(gameMode);
            var personType = save.PersonType;

            return new GameplayDataBuilder
            {
                _gameplayData = new GameplayData
                {
                    Mode = gameMode,
                    WebType = EWebType.Offline,
                    PersonType = personType,
                    PlayerYour = GetMainPlayerModel(),
                    PlayerEnemy = personType == EPersonType.Human ? GetOfflinePlayerModel() : GetBotPlayerModel(),
                    Continue = true,
                }
            };
        }

        public GameplayData GetResult()
        {
            return _gameplayData;
        }

        public static IPlayerModel GetMainPlayerModel()
        {
            return new PlayerModel
            {
                Name = "You",
                Avatar = AvatarType.Incognito
            };
        }

        public static IPlayerModel GetOfflinePlayerModel()
        {
            return new PlayerModel
            {
                Name = "Another Player",
                Avatar = AvatarType.Incognito
            };
        }

        public static IPlayerModel GetBotPlayerModel()
        {
            return new PlayerModel
            {
                Name = "Bot",
                Avatar = AvatarType.Bot
            };
        }
    }
}