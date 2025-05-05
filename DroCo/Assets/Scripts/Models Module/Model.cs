using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public int Id;

    private GameObject prefab = null;
    public GameObject Prefab
    {
        get => prefab;
        set
        {
            if (value == prefab)
                return;
            prefab = value;

            foreach (var instance in Instances)
                instance.UpdateInstanceObject(prefab);
        }
    }

    public List<Instance> Instances = new List<Instance>();
}
