using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

public class ModelsModule : Singleton<ModelsModule>
{
    public event Action Activated;
    public event Action Deactivated;

    public event Action<ModelJson> ModelCreated;
    public event Action<ModelJson> ModelUpdated;
    public event Action<int> ModelDeleted;

    public event Action<InstanceJson> InstanceCreated;
    public event Action<InstanceJson> InstanceUpdated;
    public event Action<int> InstanceDeleted;

    private void Start()
    {
        WebSocketClient.Instance.Connected += OnWebSocketConnected;
        WebSocketClient.Instance.Disconnected += OnWebSocketDisconnected;
    }

    private void OnWebSocketConnected(HelloResponse helloResponse)
    {
        if (helloResponse.Modules.Contains(nameof(ModelsModule)))
        {
            WebSocketClient.Instance.RegisterHandler("model_created", OnModelCreated);
            WebSocketClient.Instance.RegisterHandler("model_updated", OnModelUpdated);
            WebSocketClient.Instance.RegisterHandler("model_deleted", OnModelDeleted);
            WebSocketClient.Instance.RegisterHandler("model_instance_created", OnInstanceCreated);
            WebSocketClient.Instance.RegisterHandler("model_instance_updated", OnInstanceUpdated);
            WebSocketClient.Instance.RegisterHandler("model_instance_deleted", OnInstanceDeleted);
            Activated?.Invoke();
        }
    }

    private void OnWebSocketDisconnected()
    {
        if (WebSocketClient.Instance != null)
        {
            WebSocketClient.Instance.RemoveHandler("model_created");
            WebSocketClient.Instance.RemoveHandler("model_updated");
            WebSocketClient.Instance.RemoveHandler("model_deleted");
            WebSocketClient.Instance.RemoveHandler("model_instance_created");
            WebSocketClient.Instance.RemoveHandler("model_instance_updated");
            WebSocketClient.Instance.RemoveHandler("model_instance_deleted");
        }
        Deactivated?.Invoke();
    }

    private void OnModelCreated(JObject data)
    {
        var notification = data.ToObject<ModelCreatedNotificationJson>();

        if (notification == null)
        {
            Debug.LogError($"Failed to deserialize {data}");
            return;
        }

        ModelCreated?.Invoke(notification.Model);
    }

    private void OnModelUpdated(JObject data)
    {
        var notification = data.ToObject<ModelUpdatedNotificationJson>();

        if (notification == null)
        {
            Debug.LogError($"Failed to deserialize {data}");
            return;
        }

        ModelUpdated?.Invoke(notification.Model);
    }

    private void OnModelDeleted(JObject data)
    {
        var notification = data.ToObject<ModelDeletedNotificationJson>();

        if (notification == null)
        {
            Debug.LogError($"Failed to deserialize {data}");
            return;
        }

        ModelDeleted?.Invoke(notification.Id);
    }

    private void OnInstanceCreated(JObject data)
    {
        var notification = data.ToObject<InstanceCreatedNotificationJson>();

        if (notification == null)
        {
            Debug.LogError($"Failed to deserialize {data}");
            return;
        }

        InstanceCreated?.Invoke(notification.Instance);
    }

    private void OnInstanceUpdated(JObject data)
    {
        var notification = data.ToObject<InstanceUpdatedNotificationJson>();

        if (notification == null)
        {
            Debug.LogError($"Failed to deserialize {data}");
            return;
        }

        InstanceUpdated?.Invoke(notification.Instance);
    }

    private void OnInstanceDeleted(JObject data)
    {
        var notification = data.ToObject<InstanceDeletedNotificationJson>();

        if (notification == null)
        {
            Debug.LogError($"Failed to deserialize {data}");
            return;
        }

        InstanceDeleted?.Invoke(notification.Id);
    }

    public void Create(string name, SupportedFileType fileType, string data, Action<ModelCreateResponseJson> onSuccess, Action<string> onError)
    {
        RequestJson<ModelCreateRequestJson> request = new RequestJson<ModelCreateRequestJson>()
        {
            Type = "model_create",
            Data = new ModelCreateRequestJson()
            {
                Name = name,
                FileType = fileType,
                Data = data,
            },
        };

        WebSocketClient.Instance.Send(request, onRecieved, onError, 20);

        void onRecieved(JObject data)
        {
            ModelCreateResponseJson response = data.ToObject<ModelCreateResponseJson>();

            if (response == null)
            {
                onError?.Invoke($"Failed to deserialize {data}");
                return;
            }

            onSuccess?.Invoke(response);
        }
    }

    public void Delete(int id, bool force, Action<ModelDeleteResponseJson> onSuccess, Action<string> onError)
    {
        RequestJson<ModelDeleteRequestJson> request = new RequestJson<ModelDeleteRequestJson>()
        {
            Type = "model_delete",
            Data = new ModelDeleteRequestJson()
            {
                Id = id,
            },
        };

        WebSocketClient.Instance.Send(request, onRecieved, onError);

        void onRecieved(JObject data)
        {
            ModelDeleteResponseJson response = data.ToObject<ModelDeleteResponseJson>();

            if (response == null)
            {
                onError?.Invoke($"Failed to deserialize {data}");
                return;
            }

            onSuccess?.Invoke(response);
        }
    }

    public void GetAll(Action<ModelGetAllResponseJson> onSuccess, Action<string> onError)
    {
        RequestJson request = new RequestJson()
        {
            Type = "model_get_all",
        };

        WebSocketClient.Instance.Send(request, onRecieved, onError);

        void onRecieved(JObject data)
        {
            ModelGetAllResponseJson response = data.ToObject<ModelGetAllResponseJson>();

            if (response == null)
            {
                onError?.Invoke($"Failed to deserialize {data}");
                return;
            }

            onSuccess?.Invoke(response);
        }
    }

    public void Get(int id, Action<ModelGetResponseJson> onSuccess, Action<string> onError)
    {
        RequestJson<ModelGetRequestJson> request = new RequestJson<ModelGetRequestJson>()
        {
            Type = "model_get",
            Data = new ModelGetRequestJson()
            {
                Id = id,
            },
        };

        WebSocketClient.Instance.Send(request, onRecieved, onError, 20);

        void onRecieved(JObject data)
        {
            ModelGetResponseJson response = data.ToObject<ModelGetResponseJson>();

            if (response == null)
            {
                onError?.Invoke($"Failed to deserialize {data}");
                return;
            }

            onSuccess?.Invoke(response);
        }
    }

    public void CreateInstance(InstanceCreateViewModel viewModel, Action<InstanceCreateResponseJson> onSuccess, Action<string> onError)
    {
        RequestJson<InstanceCreateRequestJson> request = new RequestJson<InstanceCreateRequestJson>()
        {
            Type = "model_instance_create",
            Data = new InstanceCreateRequestJson()
            {
                ModelId = viewModel.ModelId,
                Position = new PositionJson()
                {
                    Latitude = viewModel.Latitude,
                    Longitude = viewModel.Longitude,
                    Altitude = viewModel.Altitude,
                },
                Rotation = new RotationJson()
                {
                    Pitch = viewModel.Pitch,
                    Roll = viewModel.Roll,
                    Heading = viewModel.Heading,
                },
                Scale = viewModel.Scale,
            },
        };

        WebSocketClient.Instance.Send(request, onRecieved, onError);

        void onRecieved(JObject data)
        {
            InstanceCreateResponseJson response = data.ToObject<InstanceCreateResponseJson>();

            if (response == null)
            {
                onError?.Invoke($"Failed to deserialize {data}");
                return;
            }

            onSuccess?.Invoke(response);
        }
    }

    public void DeleteInstance(int id, Action<InstanceDeleteResponseJson> onSuccess, Action<string> onError)
    {
        RequestJson<InstanceDeleteRequestJson> request = new RequestJson<InstanceDeleteRequestJson>()
        {
            Type = "model_instance_delete",
            Data = new InstanceDeleteRequestJson()
            {
                Id = id,
            },
        };

        WebSocketClient.Instance.Send(request, onRecieved, onError);

        void onRecieved(JObject data)
        {
            InstanceDeleteResponseJson response = data.ToObject<InstanceDeleteResponseJson>();

            if (response == null)
            {
                onError?.Invoke($"Failed to deserialize {data}");
                return;
            }

            onSuccess?.Invoke(response);
        }
    }

    public void GetAllInstances(int? modelId, Action<InstanceGetAllResponseJson> onSuccess, Action<string> onError)
    {
        RequestJson<InstanceGetAllRequestJson> request = new RequestJson<InstanceGetAllRequestJson>()
        {
            Type = "model_instance_get_all",
            Data = new InstanceGetAllRequestJson()
            {
                ModelId = modelId,
            },
        };

        WebSocketClient.Instance.Send(request, onRecieved, onError);

        void onRecieved(JObject data)
        {
            InstanceGetAllResponseJson response = data.ToObject<InstanceGetAllResponseJson>();

            if (response == null)
            {
                onError?.Invoke($"Failed to deserialize {data}");
                return;
            }

            onSuccess?.Invoke(response);
        }
    }
}

