using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DroCo.Editor {
    internal class ModelListPage : EditorContainerPage<List<ModelListingViewModel>> {

        private readonly List<ModelListingComponent> components = new List<ModelListingComponent>();

        public ModelListPage(EditorContainer container) : base(container, new List<ModelListingViewModel>()) {

        }

        public override void OnEnable() {
            _ = ModelsGetAll();
        }

        public override void OnGUI() {

            GUILayout.BeginHorizontal();
            GUILayout.Label("Model browser", EditorStyles.boldLabel);
            if (GUILayout.Button("Reload")) {
                _ = ModelsGetAll();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Create new")) {
                container.Navigate(new ModelCreatePage(container));
            }

            if (components.Count == 0) {
                GUILayout.Label("No models created.");
            }

            foreach (ModelListingComponent component in components) {
                component.OnGUI();
            }
        }

        private async Task ModelsGetAll() {
            container.IsProcessing = true;
            try {
                components.Clear();
                ViewModel.Clear();
                List<ModelListDto> modelDtos = await ModelsClient.GetAll();
                if (modelDtos != null) {
                    ViewModel.AddRange(modelDtos.Select(m => new ModelListingViewModel { Id = m.Id, Name = m.Name }));
                    components.AddRange(ViewModel.Select(m => new ModelListingComponent(container, m)));
                }
            } catch (Exception ex) {
                Debug.LogException(ex);
            } finally {
                container.IsProcessing = false;
            }
        }
    }
}
