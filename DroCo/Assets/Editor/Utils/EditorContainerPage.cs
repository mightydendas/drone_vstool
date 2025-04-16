
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

        public virtual void OnEnable() {

        }

        public virtual void OnGUI() {

        }

        public virtual void OnDisable() {

        }
    }
}
