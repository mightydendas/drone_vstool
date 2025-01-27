
namespace DroCo.Editor {
    internal class ModelListingViewModel {

        public int Id {
            get; set;
        }

        public string Name {
            get; set;
        }

        public ModelDetailViewModel Detail {
            get; set;
        } = null;
    }
}
