
namespace DroCo.Editor {
    internal abstract class EditorContainerComponent<TViewModel> {

        protected readonly EditorContainer container;

        public TViewModel ViewModel {
            get;
        }

        protected EditorContainerComponent(EditorContainer container, TViewModel viewModel) {
            this.container = container;
            ViewModel = viewModel;
        }

        public abstract void OnEnable();
        public abstract void OnGUI();
    }
}
