using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using UnityEngine;

[RequireComponent (typeof(ArcGISLocationComponent))]
public class Instance : MonoBehaviour
{
    public Model Model;
    public int Id;

    public ArcGISLocationComponent Location;

    private GameObject instanceObject;

    private void Start()
    {
        if (Location == null)
        {
            Location = GetComponent<ArcGISLocationComponent>();
            if (Location == null)
                throw new MissingComponentException($"Missing {nameof(ArcGISLocationComponent)} component.");
        }
    }

    public void UpdateInstanceData(InstanceJson instanceJson)
    {
        if (Location == null)
        {
            Location = GetComponent<ArcGISLocationComponent>();
            if (Location == null)
                throw new MissingComponentException($"Missing {nameof(ArcGISLocationComponent)} component.");
        }
        
        Location.Position = new ArcGISPoint(instanceJson.Position.Latitude, instanceJson.Position.Longitude, instanceJson.Position.Altitude, ArcGISSpatialReference.WGS84());
        Location.Rotation = new ArcGISRotation(instanceJson.Rotation.Heading, instanceJson.Rotation.Pitch, instanceJson.Rotation.Roll);
        transform.localScale = new Vector3(instanceJson.Scale, instanceJson.Scale, instanceJson.Scale);
    }

    public void UpdateInstanceObject(GameObject prefab)
    {
        if (instanceObject != null)
        {
            Destroy(instanceObject);
        }

        instanceObject = Instantiate(prefab);
        instanceObject.transform.parent = transform;
        instanceObject.SetActive(true);
    }
}
