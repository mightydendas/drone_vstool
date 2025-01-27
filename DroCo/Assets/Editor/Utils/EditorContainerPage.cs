
namespace DroCo.Editor {
    internal abstract class EditorContainerPage<TViewModel> : IEditorContainerPage {

        protected readonly EditorContainer container;

        public TViewModel ViewModel {
            get;
        }

        protected EditorContainerPage(EditorContainer container, TViewModel viewModel) {
            this.container = container;
            ViewModel = viewModel;
        }

        public abstract void OnEnable();
        public abstract void OnGUI();
    }
}
