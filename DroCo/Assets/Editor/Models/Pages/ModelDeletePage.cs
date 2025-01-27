using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DroCo.Editor {
    internal class ModelDeletePage : EditorContainerPage<ModelDeleteViewModel> {

        public ModelDeletePage(EditorContainer container, ModelDeleteViewModel viewModel) : base(container, viewModel) {

        }

        public override void OnEnable() {

        }

        public override void OnGUI() {

            GUILayout.Label("Are you sure you want to delete this model?", EditorStyles.boldLabel);

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Id", ViewModel.Id.ToString());
            EditorGUILayout.LabelField("Name", ViewModel.Name);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("No")) {
                container.Back(false);
            }
            if (GUILayout.Button("Yes")) {
                _ = ModelDelete(ViewModel.Id);
            }
            GUILayout.EndHorizontal();

        }

        private async Task ModelDelete(int id) {
            container.IsProcessing = true;
            try {
                bool result = await ModelsClient.ModelDelete(id);

                if (result == false) {
                    Debug.LogError("Failed to remove model.");
                    return;
                }

            } catch (Exception ex) {
                Debug.LogException(ex);
            } finally {
                container.IsProcessing = false;
                container.Back(true);
            }
        }
    }
}
