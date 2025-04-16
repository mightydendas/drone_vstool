using UnityEngine;

public class InstanceCreator : Singleton<InstanceCreator> {

    private void OnEnable() {
        tag = "EditorOnly";
    }
}
