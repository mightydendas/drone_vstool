using System.IO;
using TriLibCore;
using UnityEngine;

namespace DroCo.Editor {
    internal class ModelCreatePreviewComponent : ModelPreviewComponent<ModelCreateViewModel> {

        public ModelCreatePreviewComponent(EditorContainer container, ModelCreateViewModel viewModel) : base(container, viewModel) {
        }

        protected override byte[] LoadModel(AssetLoaderOptions assetLoaderOptions) {

            assetLoaderOptions.AlphaMaterialMode = ViewModel.AlphaMaterialMode;
            assetLoaderOptions.UseUnityNativeNormalCalculator = ViewModel.UseUnityNativeNormalCalculator;
            assetLoaderOptions.UseUnityNativeTextureLoader = ViewModel.UseUnityNativeTextureLoader;
            assetLoaderOptions.GetCompatibleTextureFormat = ViewModel.GetCompatibleTextureFormat;
            assetLoaderOptions.EnforceAlphaChannelTextures = ViewModel.EnforceAlphaChannelTextures;


            if (File.Exists(ViewModel.FilePath) == false) {
                Debug.LogError("File doesn't exists.");
                container.IsProcessing = false;
                return null;
            }

            // using memory stream to ensure same behaviour as loading from server
            return File.ReadAllBytes(ViewModel.FilePath);
        }
    }
}
