using System;
using System.IO;
using System.Threading.Tasks;
using TriLibCore.General;
using UnityEditor;
using UnityEngine;

namespace DroCo.Editor {
    internal class ModelCreatePage : EditorContainerPage<ModelCreateViewModel> {

        private readonly ModelCreatePreviewComponent modelPreviewComponent;

        private bool autoReload = false;
        public ModelCreatePage(EditorContainer container) : base(container, new ModelCreateViewModel()) {
            modelPreviewComponent = new ModelCreatePreviewComponent(container, ViewModel);
        }

        public override void OnEnable() {
            modelPreviewComponent.OnEnable();
        }

        public override void OnGUI() {

            GUILayout.BeginHorizontal();
            GUILayout.Label("Create new model", EditorStyles.boldLabel);
            autoReload = GUILayout.Toggle(autoReload, "Auto-reload");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ViewModel.FilePath = EditorGUILayout.TextField("File path", ViewModel.FilePath);
            if (GUILayout.Button("Browse")) {
                ViewModel.FilePath = EditorUtility.OpenFilePanel("File path", "", "zip");
            }
            GUILayout.EndHorizontal();

            ViewModel.Name = EditorGUILayout.TextField("Model name", ViewModel.Name);
            ViewModel.FileType = (SupportedFileType) EditorGUILayout.EnumPopup("File type", ViewModel.FileType);

            GUILayout.Label("Asset loader options", EditorStyles.boldLabel);

            modelPreviewComponent.OnGUI();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Cancel")) {
                container.Back(false);
            }

            if (GUILayout.Button("Create")) {
                _ = CreateModel();
            }

            GUILayout.EndHorizontal();
        }

        public override void OnDisable() {
            modelPreviewComponent.OnDisable();
        }

        private async Task CreateModel() {
            container.IsProcessing = true;
            try {

                if (File.Exists(ViewModel.FilePath) == false) {
                    Debug.LogError("File doesn't exist.");
                    return;
                }

                if (string.IsNullOrEmpty(ViewModel.Name)) {
                    Debug.LogError("Missing model name.");
                    return;
                }

                byte[] data = File.ReadAllBytes(ViewModel.FilePath);

                ModelCreateDto dto = new ModelCreateDto() {
                    Name = ViewModel.Name,
                    Data = Convert.ToBase64String(data),
                    FileType = ViewModel.FileType,
                };

                int result = await ModelsClient.ModelCreate(dto);

                if (result > 0) {
                    Debug.Log($"Model successfully created id={result}.");
                    container.Back(true);
                } else {
                    Debug.LogError("Failed to create model.");
                }

            } catch (Exception ex) {
                Debug.LogException(ex);
            } finally {
                container.IsProcessing = false;
            }
        }
    }
}
