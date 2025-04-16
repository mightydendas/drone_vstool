
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

        public virtual void OnEnable() {

        }

        public virtual void OnGUI() {

        }

        public virtual void OnDisable() {

        }
    }
}
