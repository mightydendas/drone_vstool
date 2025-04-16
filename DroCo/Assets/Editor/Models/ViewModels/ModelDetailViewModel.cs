using System;

namespace DroCo.Editor {
    internal class ModelDetailViewModel {

        public int Id {
            get; set;
        }

        public string ModelName {
            get; set;
        }

        public SupportedFileType FileType {
            get; set;
        }

        public byte[] Data {
            get; set;
        }

        public ModelDetailViewModel(ModelDto modelDto) {
            Id = modelDto.Id;
            ModelName = modelDto.Name;
            FileType = modelDto.FileType;
            Data = Convert.FromBase64String(modelDto.Data);
        }
    }
}
