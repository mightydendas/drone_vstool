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
        private AssetLoaderOptions optionsInstance;
        private UnityEditor.Editor optionsEditor;

        private Vector2 scrollPosition;

        protected ModelPreviewComponent(EditorContainer container, TViewModel viewModel) : base(container, viewModel) {

        }

        public override void OnEnable() {
            optionsInstance = AssetLoader.CreateDefaultLoaderOptions(false, true);
        }

        public override void OnGUI() {

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Select or assign the ScriptableObject
            optionsInstance = (AssetLoaderOptions) EditorGUILayout.ObjectField(
                "ScriptableObject",
                optionsInstance,
                typeof(ScriptableObject),
                false
            );

            if (optionsInstance != null) {
                // Create the editor for the ScriptableObject if it doesn't exist or has changed
                if (optionsEditor == null || optionsEditor.target != optionsInstance) {
                    UnityEngine.Object.DestroyImmediate(optionsEditor);
                    optionsEditor = UnityEditor.Editor.CreateEditor(optionsInstance);
                }

                // Draw the ScriptableObject editor
                optionsEditor.OnInspectorGUI();
            } else {
                // Clear the editor if no ScriptableObject is assigned
                if (optionsEditor != null) {
                    UnityEngine.Object.DestroyImmediate(optionsEditor);
                    optionsEditor = null;
                }
            }

            EditorGUILayout.EndScrollView();

            GUI.enabled = container.GUIEnabled(Application.isPlaying);

            string text = "Preview";
            if (!container.GUIEnabled() && progressPercent != 0 && progressPercent != 100) {
                text = $"{progressPercent}%";
            }
            if (GUILayout.Button(text)) {
                _ = PreviewModel();
            }

            GUI.enabled = container.GUIEnabled();
        }

        public override void OnDisable() {
            // Cleanup the editor when the window is closed
            if (optionsEditor != null) {
                UnityEngine.Object.DestroyImmediate(optionsEditor);
            }
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
