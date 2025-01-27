using UnityEditor;

namespace DroCo.Editor {
    internal class ModelBrowserContainer : EditorContainer {

        [MenuItem("Tools/DroCo/Model Browser")]
        public static void ShowWindow() {
            GetWindow<ModelBrowserContainer>("Model Browser");
        }

        protected override void LoadDefaultPage() {
            defaultPage = new ModelListPage(this);
        }
    }
}
