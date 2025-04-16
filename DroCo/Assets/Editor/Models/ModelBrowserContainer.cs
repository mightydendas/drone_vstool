using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DroCo.Editor {
    internal class ModelBrowserContainer : EditorContainer {

        [MenuItem("Tools/DroCo/Model Browser")]
        public static void ShowWindow() {
            GetWindow<ModelBrowserContainer>("Model Browser");
        }

        protected override void LoadDefaultPage() {
            defaultPage = new ModelListPage(this);
        }

        protected override void OnEnableContainer() {
            if (InstanceCreator.Instance.gameObject == null) {

            }
        }

        protected override void OnDisableContainer() {
            DestroyImmediate(InstanceCreator.Instance);
        }
    }
}
