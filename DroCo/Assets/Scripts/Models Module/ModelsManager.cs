using Esri.ArcGISMapsSDK.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TriLibCore;
using UnityEngine;

public class ModelsManager : Singleton<ModelsManager>
{
    [SerializeField]
    private int prefabDisableDelaySeconds = 1;

    [SerializeField]
    private GameObject scene3d;

    private readonly Dictionary<int, Model> models = new Dictionary<int, Model>();
    private readonly Dictionary<int, Instance> instances = new Dictionary<int, Instance>();

    private AssetLoaderOptions options;

    private void Start()
    {
        ModelsModule.Instance.Activated += OnModelsModuleActivated;
        ModelsModule.Instance.Deactivated += OnModelsModuleDeactivated;

        options = AssetLoader.CreateDefaultLoaderOptions(false, true);
        options.UseUnityNativeNormalCalculator = true;
        options.UseUnityNativeTextureLoader = true;
    }

    private void OnModelsModuleActivated()
    {
        ModelsModule.Instance.ModelCreated += OnModelsModuleModelCreated;
        ModelsModule.Instance.ModelUpdated += OnModelsModuleModelUpdated;
        ModelsModule.Instance.ModelDeleted += OnModelsModuleModelDeleted;

        ModelsModule.Instance.InstanceCreated += OnModelsModuleInstanceCreated;
        ModelsModule.Instance.InstanceUpdated += OnModelsModuleInstanceUpdated;
        ModelsModule.Instance.InstanceDeleted += OnModelsModuleInstanceDeleted;

        StartCoroutine(DelayLoad());
    }

    private void OnModelsModuleInstanceDeleted(int instanceId)
    {
        if (instances.TryGetValue(instanceId, out Instance instance))
        {
            if (instance.Model.Instances.Contains(instance))
                instance.Model.Instances.Remove(instance);
            Destroy(instance.gameObject);
        }
    }

    private void OnModelsModuleInstanceUpdated(InstanceJson instanceJson)
    {
        if (instances.TryGetValue(instanceJson.Id, out Instance instance))
        {
            instance.UpdateInstanceData(instanceJson);
        }
        else
        {
            if (models.TryGetValue(instanceJson.ModelId, out Model model))
            {
                CreateInstance(model, instanceJson);
            }
        }
    }

    private void OnModelsModuleInstanceCreated(InstanceJson json)
    {
        if (models.TryGetValue(json.ModelId, out Model model))
        {
            CreateInstance(model, json);
        }
    }

    private void OnModelsModuleModelDeleted(int id)
    {
        if (models.TryGetValue(id, out Model model))
        {
            foreach (var instance in model.Instances)
            {
                Destroy(instance.gameObject);
                instances.Remove(instance.Id);
            }
            Destroy(model.gameObject);
            models.Remove(id);
        }
    }

    private void OnModelsModuleModelUpdated(ModelSimpleJson modelSimple)
    {
        LoadModel(modelSimple.Id, onModelLoadProgress, onModelLoadSuccess, onModelLoadError);

        void onModelLoadProgress(string progress)
        {
            Debug.Log($"Loading updated model {modelSimple.Id}: {progress}");
        }

        void onModelLoadSuccess(Model newModel)
        {
            Debug.Log($"Updated model {newModel.Id} loaded");

            if (models.TryGetValue(newModel.Id, out Model oldModel))
            {
                foreach (var instance in oldModel.Instances)
                    instance.UpdateInstanceObject(newModel.Prefab);
                Destroy(oldModel.gameObject);
                models[newModel.Id] = newModel;
            }
            else
            {
                ModelsModule.Instance.GetAllInstances(newModel.Id, onInstancesGetSuccess, onInstancesGetError);

                void onInstancesGetSuccess(InstanceGetAllResponseJson response)
                {
                    foreach (var instanceJson in response.Instances)
                    {
                        try
                        {
                            CreateInstance(newModel, instanceJson);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                            Debug.LogError($"Faield to create instance of model {newModel.Id}: {ex.Message}.");
                        }
                    }
                }

                void onInstancesGetError(string error)
                {
                    Debug.LogError($"Failed to get instances of model {newModel.Id}: {error}");
                }
            }
        }

        void onModelLoadError(string error)
        {
            Debug.LogError($"Failed to load updated model {modelSimple.Id}: {error}");
        }
    }

    private void OnModelsModuleModelCreated(ModelSimpleJson modelSimple)
    {
        LoadModel(modelSimple.Id, onModelLoadProgress, onModelLoadSuccess, onModelLoadError);

        void onModelLoadProgress(string progress)
        {
            Debug.Log($"Loading created model {modelSimple.Id}: {progress}");
        }

        void onModelLoadSuccess(Model newModel)
        {
            Debug.Log($"Created model {newModel.Id} loaded");

            if (models.TryGetValue(newModel.Id, out Model oldModel))
            {
                foreach (var instance in oldModel.Instances)
                {
                    Destroy(instance.gameObject);
                    instances.Remove(newModel.Id);
                }
                Destroy(oldModel.gameObject);
                models.Remove(oldModel.Id);
            }

            ModelsModule.Instance.GetAllInstances(newModel.Id, onInstancesGetSuccess, onInstancesGetError);

            void onInstancesGetSuccess(InstanceGetAllResponseJson response)
            {
                foreach (var instanceJson in response.Instances)
                {
                    try
                    {
                        CreateInstance(newModel, instanceJson);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                        Debug.LogError($"Faield to create instance of model {newModel.Id}: {ex.Message}.");
                    }
                }
            }

            void onInstancesGetError(string error)
            {
                Debug.LogError($"Failed to get instances of model {newModel.Id}: {error}");
            }
        }

        void onModelLoadError(string error)
        {
            Debug.LogError($"Failed to load updated model {modelSimple.Id}: {error}");
        }
    }

    private void OnModelsModuleDeactivated()
    {
        foreach (var model in models.Values)
            Destroy(model.gameObject);
        models.Clear();

        foreach (var instance in instances.Values)
            Destroy(instance.gameObject);
        instances.Clear();
    }

    private IEnumerator DelayLoad()
    {
        yield return new WaitForSeconds(5);
        LoadInstances(s => Debug.Log(s), s => Debug.LogError(s));
    }

    private void LoadInstances(Action<string> onProgress, Action<string> onError)
    {
        string status = "Getting models...";
        Debug.Log(status);
        onProgress.Invoke(status);

        ModelsModule.Instance.GetAll((r) => StartCoroutine(onModelsLoadSuccess(r)), onError);

        IEnumerator onModelsLoadSuccess(ModelGetAllResponseJson response)
        {
            List<string> errors = new List<string>();

            var models = response.Models;

            for (int i = 0; i < models.Length; i++)
            {
                bool canContinue = false;

                int index = i;
                LoadModel(models[i].Id, p => onModelLoadProgress(index, p), m => onModelLoadSuccess(index, m), e => onModelLoadError(index, e));

                void onModelLoadProgress(int index, string progress)
                {
                    string status = $"Model {index + 1}/{models.Length}: {progress}";
                    onProgress.Invoke(status);
                }

                void onModelLoadSuccess(int index, Model model)
                {
                    string status = $"Model {index + 1}/{models.Length}: Getting instaces...";
                    onProgress.Invoke(status);

                    ModelsModule.Instance.GetAllInstances(model.Id, onInstancesGetSuccess, onInstancesGetError);

                    void onInstancesGetSuccess(InstanceGetAllResponseJson response)
                    {
                        foreach (var instanceJson in response.Instances)
                        {
                            try
                            {
                                CreateInstance(model, instanceJson);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogException(ex);
                                errors.Add($"Faield to create instance of model {model.Id}: {ex.Message}.");
                            }
                        }

                        canContinue = true;
                    }

                    void onInstancesGetError(string error)
                    {
                        canContinue = true;
                        Debug.LogError($"Failed to get instances of model {model.Id}: {error}");
                        errors.Add(error);
                    }
                }

                void onModelLoadError(int index, string error)
                {
                    canContinue = true;
                    Debug.LogError($"Failed to load model {models[index].Id}: {error}");
                    errors.Add(error);
                }

                yield return new WaitUntil(() => canContinue);
            }

            if (errors.Count > 0)
            {
                string error = string.Join(Environment.NewLine, errors);
                onError.Invoke(error);
            }
        }
    }

    public void LoadInstanceTemp(int modelId, Action<string> onProgress, Action<Instance> onSuccess, Action<string> onError)
    {
        LoadModel(modelId, onProgress, onLoadSuccess, onError);

        void onLoadSuccess(Model model)
        {
            Instance instance = CreateInstanceTemp(model);
            onSuccess.Invoke(instance);
        }
    }

    private void LoadModel(int modelId, Action<string> onProgress, Action<Model> onSuccess, Action<string> onError)
    {
        if (models.TryGetValue(modelId, out Model prefab))
        {
            onSuccess.Invoke(prefab);
            return;
        }

        ModelsModule.Instance.Get(modelId, onGetSuccess, onError);

        void onGetSuccess(ModelGetResponseJson response)
        {
            try
            {
                int id = response.Model.Id;
                byte[] data = Convert.FromBase64String(response.Model.Data);
                string fileExtension = response.Model.FileType.ToString().ToLower();

                MemoryStream stream = new MemoryStream(data);
                AssetLoaderZip.LoadModelFromZipStream(stream, c => OnLoad(onProgress, c), c => OnMaterialsLoad(id, onProgress, onSuccess, c), (c, p) => OnProgress(onProgress, c, p), e => OnError(onError, e), null, options, fileExtension: fileExtension);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                onError.Invoke(ex.Message);
            }
        }
    }

    private void OnProgress(Action<string> onProgress, AssetLoaderContext assetLoaderContext, float progress)
    {
        onProgress.Invoke($"Loading {(int)(progress * 100)}%");
    }

    private void OnError(Action<string> onError, IContextualizedError contextualizedError)
    {
        Debug.LogException(contextualizedError.GetInnerException());

        string error = $"Exception: {contextualizedError.GetInnerException().Message}, Context: {contextualizedError.GetContext()}";
        onError.Invoke(error);
    }

    private void OnLoad(Action<string> onProgress, AssetLoaderContext context)
    {
        Debug.Log("Model loaded.");
        onProgress.Invoke("Model loaded");
    }



    private void OnMaterialsLoad(int id, Action<string> onProgress, Action<Model> onSuccess, AssetLoaderContext context)
    {
        Debug.Log("Model materials loaded.");
        onProgress.Invoke("Materials loaded");

        Model model = CreateModel(id, context.RootGameObject);

        // 'prefab' se musi nacist aby se zavolalo AssetUnloader.Start()
        // takze ho nebudeme hned zneviditelnovat
        // zneviditelime ho az po urcite dobe
        //prefab.SetActive(false);
        StartCoroutine(DisableModelPrefabDelayed(model.Prefab));

        onSuccess.Invoke(model);
    }

    private IEnumerator DisableModelPrefabDelayed(GameObject prefab)
    {
        yield return new WaitForSeconds(prefabDisableDelaySeconds);
        prefab.SetActive(false);
    }

    private Model CreateModel(int id, GameObject prefab)
    {
        GameObject gameObject = new GameObject($"Model - {id}");
        gameObject.transform.parent = transform;
        Model model = gameObject.AddComponent<Model>();
        model.Id = id;
        model.Prefab = prefab;
        model.Prefab.transform.parent = gameObject.transform;

        // cache
        models.Add(id, model);

        return model;
    }

    private Instance CreateInstanceTemp(Model model)
    {
        GameObject gameObject = new GameObject("Instance - Temp");
        gameObject.transform.parent = scene3d.transform;
        gameObject.AddComponent<ArcGISLocationComponent>();
        Instance instance = gameObject.AddComponent<Instance>();
        instance.Id = -1;
        instance.Model = model;
        instance.UpdateInstanceObject(model.Prefab);
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Buildings"));
        return instance;
    }

    private Instance CreateInstance(Model model, InstanceJson instanceJson)
    {
        GameObject gameObject = new GameObject($"Instance - {model.Id} - {instanceJson.Id}");
        gameObject.transform.parent = scene3d.transform;
        gameObject.AddComponent<ArcGISLocationComponent>();
        Instance instance = gameObject.AddComponent<Instance>();
        instance.Id = instanceJson.Id;
        instance.Model = model;
        instance.UpdateInstanceData(instanceJson);
        instance.UpdateInstanceObject(model.Prefab);
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Buildings"));

        // link
        model.Instances.Add(instance);

        // cache
        instances.Add(instanceJson.Id, instance);

        return instance;
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null)
            return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }
}
