using System;
using System.IO;
using System.Threading.Tasks;
using TriLibCore;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DroCo.Editor {
    internal class ModelListingComponent : EditorContainerComponent<ModelListingViewModel> {

        private int progressPercent = 0;

        public ModelListingComponent(EditorContainer container, ModelListingViewModel viewModel) : base(container, viewModel) {

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

            string text = "Load to scene";
            if (!container.GUIEnabled() && progressPercent != 0 && progressPercent != 100) {
                text = $"{progressPercent}%";
            }
            if (GUILayout.Button(text)) {
                _ = LoadToScene();
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

        private async Task LoadToScene() {
            container.IsProcessing = true;

            try {

                // load detail
                if (ViewModel.Detail is null) {
                    ModelDto dto = await ModelsClient.Get(ViewModel.Id);
                    ViewModel.Detail = new ModelDetailViewModel(dto);
                }

            } catch (Exception ex) {
                Debug.LogException(ex);
                container.IsProcessing = false;
            }

            try {

                // load model
                AssetLoaderOptions assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);

                byte[] data = ViewModel.Detail.Data;

                // using memory stream to ensure same behaviour as loading from server
                MemoryStream stream = new MemoryStream(data);
                AssetLoaderZip.LoadModelFromZipStream(stream, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions/*, fileExtension: "obj"*/);


            } catch (Exception ex) {
                Debug.LogException(ex);
            }
        }

        private void OnProgress(AssetLoaderContext assetLoaderContext, float progress) {
            progressPercent = (int) (progress * 100);
            container.Repaint();
        }

        private void OnError(IContextualizedError contextualizedError) {
            Debug.LogError($"Exception: {contextualizedError.GetInnerException()}, Context: {contextualizedError.GetContext()}");
            container.IsProcessing = false;
        }

        private void OnLoad(AssetLoaderContext assetLoaderContext) {
            Debug.Log("Model loaded.");
        }

        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext) {
            Debug.Log("Model materials loaded.");
            try {
                GameObject loadedGameObject = assetLoaderContext.RootGameObject;
                loadedGameObject.SetActive(true);

                Scene scene = SceneManager.GetActiveScene();

                // add to scene
                SceneManager.MoveGameObjectToScene(loadedGameObject, scene);

                // select
                Selection.activeGameObject = loadedGameObject;

                // focus
                SceneView.lastActiveSceneView.Frame(loadedGameObject.GetComponentInChildren<Renderer>().bounds, true);
            } catch (Exception ex) {
                Debug.LogException(ex);
            } finally {
                container.IsProcessing = false;
            }
        }
    }
}
