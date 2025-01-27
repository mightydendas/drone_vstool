using System;
using System.IO;
using System.Threading.Tasks;
using TriLibCore.General;
using UnityEditor;
using UnityEngine;

namespace DroCo.Editor {
    internal class ModelCreatePage : EditorContainerPage<ModelCreateViewModel> {

        private readonly ModelCreatePreviewComponent modelPreview;
        private bool autoReload = false;

        public ModelCreatePage(EditorContainer container) : base(container, new ModelCreateViewModel()) {
            modelPreview = new ModelCreatePreviewComponent(container, ViewModel);
        }

        public override void OnEnable() {

        }

        public override void OnGUI() {

            GUILayout.BeginHorizontal();
            GUILayout.Label("Create new model", EditorStyles.boldLabel);
            autoReload = GUILayout.Toggle(autoReload, "Auto-reload");
            GUILayout.EndHorizontal();

            ViewModel.FilePath = EditorGUILayout.TextField("File path", ViewModel.FilePath);
            ViewModel.Name = EditorGUILayout.TextField("Model name", ViewModel.Name);
            ViewModel.AlphaMaterialMode = (AlphaMaterialMode) EditorGUILayout.EnumPopup("Alpha material mode", ViewModel.AlphaMaterialMode);
            ViewModel.UseUnityNativeNormalCalculator = EditorGUILayout.Toggle("Use Unity native normal calculator", ViewModel.UseUnityNativeNormalCalculator);
            ViewModel.UseUnityNativeTextureLoader = EditorGUILayout.Toggle("Use Unity native texture loader", ViewModel.UseUnityNativeTextureLoader);
            ViewModel.GetCompatibleTextureFormat = EditorGUILayout.Toggle("Get compatible texture format", ViewModel.GetCompatibleTextureFormat);
            ViewModel.EnforceAlphaChannelTextures = EditorGUILayout.Toggle("Enforce alpha channel textures", ViewModel.EnforceAlphaChannelTextures);

            modelPreview.OnGUI();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Cancel")) {
                container.Back(false);
            }

            if (GUILayout.Button("Create")) {
                _ = CreateModel();
            }

            GUILayout.EndHorizontal();
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
                    AlphaMaterialMode = ViewModel.AlphaMaterialMode,
                    EnforceAlphaChannelTextures = ViewModel.EnforceAlphaChannelTextures,
                    GetCompatibleTextureFormat = ViewModel.GetCompatibleTextureFormat,
                    UseUnityNativeNormalCalculator = ViewModel.UseUnityNativeNormalCalculator,
                    UseUnityNativeTextureLoader = ViewModel.UseUnityNativeTextureLoader,
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
