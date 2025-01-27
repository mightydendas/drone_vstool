using TriLibCore.General;

namespace DroCo.Editor {
    internal class ModelDetailViewModel {

        public int Id {
            get; set;
        }

        public string ModelName {
            get; set;
        }

        public string Data {
            get; set;
        }

        public AlphaMaterialMode AlphaMaterialMode {
            get; set;
        }

        public bool UseUnityNativeNormalCalculator {
            get; set;
        }

        public bool UseUnityNativeTextureLoader {
            get; set;
        }

        public bool GetCompatibleTextureFormat {
            get; set;
        }

        public bool EnforceAlphaChannelTextures {
            get; set;
        }

        public ModelDetailViewModel(ModelDto modelDto) {
            Id = modelDto.Id;
            ModelName = modelDto.Name;
            Data = modelDto.Data;
            AlphaMaterialMode = modelDto.AlphaMaterialMode;
            UseUnityNativeNormalCalculator = modelDto.UseUnityNativeNormalCalculator;
            UseUnityNativeTextureLoader = modelDto.UseUnityNativeTextureLoader;
            GetCompatibleTextureFormat = modelDto.GetCompatibleTextureFormat;
            EnforceAlphaChannelTextures = modelDto.EnforceAlphaChannelTextures;
        }
    }
}
