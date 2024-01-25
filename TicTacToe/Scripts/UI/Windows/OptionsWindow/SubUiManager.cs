using System;
using Dainty.UI;
using Dainty.UI.WindowBase;
using TicTacToe.UI.Windows.OptionsWindow.SubWindows;

namespace TicTacToe.UI.Windows.OptionsWindow
{
    public class SubUiManager : UiManager
    {
        private readonly Action<string, bool> _titleSetter;

        public SubUiManager(UiRoot root, UiManagerSettings settings, Action<string, bool> titleSetter) : base(root, settings)
        {
            _titleSetter = titleSetter;
        }

    #region Open

        public override T Open<T>(bool isPopup = false, WindowTransition transition = WindowTransition.AnimateOpening)
        {
            var controller = base.Open<T>(isPopup, transition);

            if (controller is ISubWindowController subWindow)
            {
                _titleSetter?.Invoke(subWindow.Title, transition != WindowTransition.None);
            }

            return controller;
        }

        public override T Open<T, TS>(TS data, bool isPopup,
            WindowTransition transition = WindowTransition.AnimateOpening)
        {
            var controller = base.Open<T, TS>(data, isPopup, transition);

            if (controller is ISubWindowController subWindow)
            {
                _titleSetter?.Invoke(subWindow.Title, transition != WindowTransition.None);
            }

            return controller;
        }

    #endregion

    #region Close

        public override bool Close<T>(WindowTransition transition = WindowTransition.AnimateClosing,
            Action onClosed = null)
        {
            var result = base.Close<T>(transition, onClosed);

            if (CurrentWindow is ISubWindowController subWindow)
            {
                _titleSetter?.Invoke(subWindow.Title, transition != WindowTransition.None);
            }

            return result;
        }

    #endregion

    #region Back

        public override bool Back(WindowTransition transition = WindowTransition.AnimateClosing, Action onClosed = null)
        {
            var result = base.Back(transition, onClosed);

            if (CurrentWindow is ISubWindowController subWindow)
            {
                _titleSetter?.Invoke(subWindow.Title, transition != WindowTransition.None);
            }

            return result;
        }

        public override bool Back(out IWindowController window,
            WindowTransition transition = WindowTransition.AnimateClosing,
            Action onClosed = null)
        {
            var result = base.Back(out window, transition, onClosed);

            if (CurrentWindow is ISubWindowController subWindow)
            {
                _titleSetter?.Invoke(subWindow.Title, transition != WindowTransition.None);
            }

            return result;
        }

    #endregion
    }
}