using System;
using TriLibCore;

namespace DroCo.Editor {
    internal class ModelDetailPreviewComponent : ModelPreviewComponent<ModelDetailViewModel> {

        public ModelDetailPreviewComponent(EditorContainer container, ModelDetailViewModel viewModel) : base(container, viewModel) {
        }

        protected override byte[] LoadModel(AssetLoaderOptions assetLoaderOptions) {
            return ViewModel.Data;
        }
    }
}
