using Dainty.UI;
using TicTacToe.DI;
using TicTacToe.SceneControllers.Params;
using UnityEngine;

namespace TicTacToe.SceneControllers
{
    [DefaultExecutionOrder(-30)]
    public abstract class ASceneController : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] protected UiRoot uiRoot;
        [SerializeField] protected UiManagerSettings uiManagerSettings;
#pragma warning restore 649

        protected UiManager uiManager;

        protected virtual void Awake()
        {
            if (uiRoot != null && uiManagerSettings != null)
            {
                uiManager = new UiManager(uiRoot, uiManagerSettings);
            }
        }

        protected virtual void Start()
        {
            var @params = ProjectContext.GetInstance<SceneStartupParams>();
            Initialize(@params);
        }

        protected abstract void Initialize(SceneStartupParams @params);
    }
}