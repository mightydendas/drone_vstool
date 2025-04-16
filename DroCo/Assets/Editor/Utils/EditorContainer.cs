using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DroCo.Editor {
    internal abstract class EditorContainer : EditorWindow {

        protected readonly Stack<IEditorContainerPage> pages = new Stack<IEditorContainerPage>();
        protected IEditorContainerPage defaultPage;

        private bool isProcessing = false;
        public bool IsProcessing {
            set {
                isProcessing = value;
                Repaint();
            }
        }

        public bool GUIEnabled(bool condition = true) => condition && !isProcessing;

        protected abstract void LoadDefaultPage();

        protected virtual void OnEnableContainer() {

        }

        protected virtual void OnDisableContainer() {

        }

        private void OnEnable() {
            LoadDefaultPage();
            Reload();
            OnEnableContainer();
        }

        private void OnGUI() {
            if (pages.Count > 0) {
                GUI.enabled = !isProcessing;
                pages.Peek().OnGUI();
            } else {
                Debug.LogError("No page loaded.");
                Reload();
            }
        }

        private void OnDisable() {
            OnDisableContainer();
        }

        public void Back(bool reloadPage) {
            IEditorContainerPage lastPage = pages.Pop();
            lastPage.OnDisable();
            if (pages.Count > 0) {
                if (reloadPage)
                    pages.Peek().OnEnable();
                Repaint();
            } else {
                Debug.LogError("No page loaded.");
                Reload();
            }

        }

        public void Navigate<TViewModel>(EditorContainerPage<TViewModel> page) {
            pages.Push(page);
            page.OnEnable();
            Repaint();
        }

        protected void Reload() {
            pages.Clear();
            pages.Push(defaultPage);
            defaultPage.OnEnable();
            Repaint();
        }
    }
}
