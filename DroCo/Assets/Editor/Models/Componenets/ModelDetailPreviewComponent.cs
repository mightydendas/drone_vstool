using System;
using TriLibCore;

namespace DroCo.Editor {
    internal class ModelDetailPreviewComponent : ModelPreviewComponent<ModelDetailViewModel> {

        public ModelDetailPreviewComponent(EditorContainer container, ModelDetailViewModel viewModel) : base(container, viewModel) {
        }

        protected override byte[] LoadModel(AssetLoaderOptions assetLoaderOptions) {
            assetLoaderOptions.AlphaMaterialMode = ViewModel.AlphaMaterialMode;
            assetLoaderOptions.UseUnityNativeNormalCalculator = ViewModel.UseUnityNativeNormalCalculator;
            assetLoaderOptions.UseUnityNativeTextureLoader = ViewModel.UseUnityNativeTextureLoader;
            assetLoaderOptions.GetCompatibleTextureFormat = ViewModel.GetCompatibleTextureFormat;
            assetLoaderOptions.EnforceAlphaChannelTextures = ViewModel.EnforceAlphaChannelTextures;

            return Convert.FromBase64String(ViewModel.Data);
        }
    }
}
