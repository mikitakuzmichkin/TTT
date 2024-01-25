using TicTacToe.UI.SceneAnimation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TicTacToe
{
    public class SceneManagerWrapper : MonoBehaviour
    {
        [SerializeField] private AChangeSceneAnimation _defaultAnimation;

        public void LoadScene(string sceneName)
        {
            _defaultAnimation.PlayBeforeSceneChanged(() =>
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(sceneName);
            });
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _defaultAnimation.PlayAfterSceneChanged();
        }
    }
}