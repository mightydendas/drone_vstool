using UnityEditor;
using UnityEngine;

namespace DroCo.Editor {
    internal class ModelDetailPage : EditorContainerPage<ModelDetailViewModel> {

        private readonly ModelDetailPreviewComponent modelPreviewComponent;

        public ModelDetailPage(EditorContainer container, ModelDetailViewModel viewModel) : base(container, viewModel) {
            modelPreviewComponent = new ModelDetailPreviewComponent(container, ViewModel);
        }

        public override void OnEnable() {
            modelPreviewComponent.OnEnable();
        }

        public override void OnGUI() {

            GUILayout.BeginHorizontal();
            GUILayout.Label("Model detail", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
            GUI.enabled = false;

            EditorGUILayout.IntField("Id", ViewModel.Id);
            EditorGUILayout.TextField("Model name", ViewModel.ModelName);
            EditorGUILayout.TextField("File type", ViewModel.FileType.ToString());

            GUI.enabled = container.GUIEnabled();

            modelPreviewComponent.OnGUI();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Back")) {
                container.Back(false);
            }

            GUILayout.EndHorizontal();
        }

        public override void OnDisable() {
            modelPreviewComponent.OnDisable();
        }
    }
}
