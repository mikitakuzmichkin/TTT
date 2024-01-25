using System;

namespace TicTacToe.Mechanics.Base
{
    public abstract class ABoardController
    {
        public abstract ABoardModel Model { get; }
        public abstract ABoardView View { get; }
        public abstract Type ViewType { get; }

        public virtual void Initialize(ABoardModel model, ABoardView view)
        {
            Uninitialize();
        }

        public virtual void Uninitialize()
        {
            Model?.Uninitialize();

            if (View != null)
            {
                View.Uninitialize();
            }
        }

        public void Show()
        {
            View.Show();
        }

        public void Close()
        {
            View.Close();
        }

        public void BlockView()
        {
            View.Interactable = false;
        }

        public void UnblockView()
        {
            View.Interactable = true;
        }
    }

    public abstract class ABoardController<TView, TModel> : ABoardController
        where TView : ABoardView
        where TModel : ABoardModel
    {
        protected TView view;
        protected TModel model;

        public override ABoardModel Model => model;
        public override ABoardView View => view;
        public override Type ViewType => typeof(TView);

        public override void Initialize(ABoardModel model, ABoardView view)
        {
            base.Initialize(model, view);

            if (!(model is TModel) || !(view is TView))
            {
                //todo custom exception
                throw new ArgumentException("Components type is invalid!");
            }

            this.model = (TModel) model;

            this.view = (TView) view;
            this.view.Initialize(model);
        }
    }
}