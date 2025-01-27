using UnityEditor;
using UnityEngine;

namespace DroCo.Editor {
    internal class ModelDetailPage : EditorContainerPage<ModelDetailViewModel> {

        private readonly ModelDetailPreviewComponent modelPreviewComponent;

        public ModelDetailPage(EditorContainer container, ModelDetailViewModel viewModel) : base(container, viewModel) {
            modelPreviewComponent = new ModelDetailPreviewComponent(container, ViewModel);
        }

        public override void OnEnable() {
        }

        public override void OnGUI() {

            GUILayout.BeginHorizontal();
            GUILayout.Label("Model detail", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
            GUI.enabled = false;

            EditorGUILayout.IntField("Id", ViewModel.Id);
            EditorGUILayout.TextField("Model name", ViewModel.ModelName);
            EditorGUILayout.EnumPopup("Alpha material mode", ViewModel.AlphaMaterialMode);
            EditorGUILayout.Toggle("Use Unity native normal calculator", ViewModel.UseUnityNativeNormalCalculator);
            EditorGUILayout.Toggle("Use Unity native texture loader", ViewModel.UseUnityNativeTextureLoader);
            EditorGUILayout.Toggle("Get compatible texture format", ViewModel.GetCompatibleTextureFormat);
            EditorGUILayout.Toggle("Enforce alpha channel textures", ViewModel.EnforceAlphaChannelTextures);

            GUI.enabled = container.GUIEnabled();

            modelPreviewComponent.OnGUI();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Back")) {
                container.Back(false);
            }

            GUILayout.EndHorizontal();
        }
    }
}
