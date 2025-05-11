using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitList : MonoBehaviour {

    public Transform List;

    public GameObject ListItemDronePrefab;
    public GameObject ListItemPersonPrefab;
    public GameObject ListItemGroundUnitPrefab;

    public ListItemButton SpawnListItemDrone(DroneStaticData data, InteractiveObject interactiveObject) {
        GameObject listItemGO = Instantiate(ListItemDronePrefab, List);
        ListItemButton listItem = listItemGO.GetComponent<ListItemButton>();
        listItem.InitUnitData(data.ClientId, data.DroneName, interactiveObject);

        return listItem;
    }
}
