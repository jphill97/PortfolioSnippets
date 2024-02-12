using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [SerializeField]
    RaceScriptable race;
    public RaceScriptable Race => race;

    [SerializeField]
    MouseManager mouseManager;

    Vector3 mousePosition = Vector3.zero;

    [SerializeField]
    PreviewBlock previewBlock;

    GameObject previewBuilding;

    bool validBuildingSite = false;
    bool buildingActive = false;
    int selectedID;
    ClickableIcon selectedIcon;
    // Start is called before the first frame update
    void Start()
    {
        mouseManager = GetComponent<MouseManager>();
        mouseManager.onMouseClicked += MouseClicked;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (buildingActive)
        {
            ProcessMousePosition(mouseManager.mouseWorldPosition);
        }
    }

    public void ActivateBuilding(int buildingID, ClickableIcon newIcon) 
    { 
        if (buildingActive)
            return;

        buildingActive = true;
        selectedIcon = newIcon;
        selectedID = buildingID;
        previewBuilding = Instantiate<GameObject>(race.Towers[selectedID].MeshPrefab);
    }

    void MouseClicked()
    {
        if (!buildingActive)
            return;

        if (validBuildingSite)
        {
            if (selectedIcon != null)
            {
                selectedIcon.ResetButton();
                selectedIcon = null;
            }

            ConstructBuilding();
            buildingActive = false;
        }
        else
        {
            Debug.Log("Invalid Build Location");
        }
    }

    void ConstructBuilding()
    {
        previewBuilding.GetComponent<Tower>().StartConstruction(race.Towers[selectedID]);
        previewBuilding = null;
    }


    void ProcessMousePosition(Vector3 mouseWorldPos)
    {
        Vector3 floorPos = new Vector3((int)mouseWorldPos.x, (int)mouseWorldPos.y, (int)mouseWorldPos.z);

        if(mousePosition != floorPos)
        {
            mousePosition = floorPos;
            previewBlock.transform.position = mousePosition;
            previewBuilding.transform.position = mousePosition;
            validBuildingSite = previewBlock.CheckTiles();
        }
    }
}
