using System;
using System.IO;
using System.Threading.Tasks;
using TriLibCore;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace DroCo.Editor {
    internal abstract class ModelPreviewComponent<TViewModel> : EditorContainerComponent<TViewModel> {

        private int progressPercent = 0;

        protected ModelPreviewComponent(EditorContainer container, TViewModel viewModel) : base(container, viewModel) {

        }

        public override void OnEnable() {

        }

        public override void OnGUI() {

            GUI.enabled = container.GUIEnabled(Application.isPlaying);

            GUILayout.BeginHorizontal();
            string text = "Preview";
            if (!container.GUIEnabled() && progressPercent != 0 && progressPercent != 100) {
                text = $"{progressPercent}%";
            }
            if (GUILayout.Button(text)) {
                _ = PreviewModel();
            }
            GUILayout.EndHorizontal();

            GUI.enabled = container.GUIEnabled();
        }

        protected abstract byte[] LoadModel(AssetLoaderOptions assetLoaderOptions);

        private Task PreviewModel() {
            container.IsProcessing = true;
            try {

                // load scene
                Scene scene = EditorSceneManager.LoadSceneInPlayMode("Assets/Scenes/AssetLoaderScene.unity", new LoadSceneParameters(LoadSceneMode.Single));

                AssetLoaderOptions assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();

                byte[] data = LoadModel(assetLoaderOptions);

                if (data == null) {
                    container.IsProcessing = false;
                    return Task.CompletedTask;
                }

                // using memory stream to ensure same behaviour as loading from server
                MemoryStream stream = new MemoryStream(data);
                AssetLoaderZip.LoadModelFromZipStream(stream, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions/*, fileExtension: "obj"*/, modelFilename: "Loaded Object");
                return Task.CompletedTask;
            } catch (Exception ex) {
                Debug.LogException(ex);
                container.IsProcessing = false;
                return Task.CompletedTask;
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
