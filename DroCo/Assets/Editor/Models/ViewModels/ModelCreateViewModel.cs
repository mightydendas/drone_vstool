using TriLibCore.General;

namespace DroCo.Editor {
    internal class ModelCreateViewModel {

        public bool IsDirty = false;

        private string filePath = "C:\\Users\\dendas\\Downloads\\whnp3v2jflkw-Tree\\Tree.zip";
        public string FilePath {
            get => filePath;
            set {
                if (value == filePath)
                    return;
                filePath = value;
                IsDirty = true;
            }
        }

        private string modelName = "";
        public string Name {
            get => modelName;
            set {
                if (value == modelName)
                    return;
                modelName = value;
                IsDirty = true;
            }
        }

        private AlphaMaterialMode alphaMaterialMode = AlphaMaterialMode.Transparent;
        public AlphaMaterialMode AlphaMaterialMode {
            get => alphaMaterialMode;
            set {
                if (value == alphaMaterialMode)
                    return;
                alphaMaterialMode = value;
                IsDirty = true;
            }
        }

        private bool useUnityNativeNormalCalculator = true;
        public bool UseUnityNativeNormalCalculator {
            get => useUnityNativeNormalCalculator;
            set {
                if (value == useUnityNativeNormalCalculator)
                    return;
                useUnityNativeNormalCalculator = value;
                IsDirty = true;
            }
        }

        private bool useUnityNativeTextureLoader = true;
        public bool UseUnityNativeTextureLoader {
            get => useUnityNativeTextureLoader;
            set {
                if (value == useUnityNativeTextureLoader)
                    return;
                useUnityNativeTextureLoader = value;
                IsDirty = true;
            }
        }

        private bool getCompatibleTextureFormat = true;
        public bool GetCompatibleTextureFormat {
            get => getCompatibleTextureFormat;
            set {
                if (value == getCompatibleTextureFormat)
                    return;
                getCompatibleTextureFormat = value;
                IsDirty = true;
            }
        }

        private bool enforceAlphaChannelTextures = true;
        public bool EnforceAlphaChannelTextures {
            get => enforceAlphaChannelTextures;

            set {
                if (value == enforceAlphaChannelTextures)
                    return;
                enforceAlphaChannelTextures = value;
                IsDirty = true;
            }
        }
    }
}
