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

        private SupportedFileType fileType = SupportedFileType.OBJ;
        public SupportedFileType FileType {
            get => fileType;
            set {
                if (value == fileType)
                    return;
                fileType = value;
                IsDirty = true;
            }
        }
    }
}
