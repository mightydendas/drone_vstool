using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DroCo.Editor {
    internal class ModelListingComponent : EditorContainerComponent<ModelListingViewModel> {

        public ModelListingComponent(EditorContainer container, ModelListingViewModel viewModel) : base(container, viewModel) {

        }

        public override void OnEnable() {

        }

        public override void OnGUI() {

            GUILayout.BeginVertical("box");

            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Id", ViewModel.Id.ToString());
            EditorGUILayout.LabelField("Name", ViewModel.Name);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Detail")) {
                if (ViewModel.Detail != null) {
                    container.Navigate(new ModelDetailPage(container, ViewModel.Detail));
                } else {
                    _ = LoadDetail();
                }
            }
            if (GUILayout.Button("Edit")) {
            }
            if (GUILayout.Button("Delete")) {
                container.Navigate(new ModelDeletePage(container, new ModelDeleteViewModel() { Id = ViewModel.Id, Name = ViewModel.Name }));
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private async Task LoadDetail() {
            container.IsProcessing = true;
            try {
                ModelDto modelDto = await ModelsClient.Get(ViewModel.Id);

                if (modelDto == null) {
                    Debug.LogError("Failed to fetch model detail.");
                    return;
                }

                ViewModel.Detail = new ModelDetailViewModel(modelDto);

                container.Navigate(new ModelDetailPage(container, ViewModel.Detail));

            } catch (Exception ex) {
                Debug.LogException(ex);
            } finally {
                container.IsProcessing = false;
            }
        }
    }
}
