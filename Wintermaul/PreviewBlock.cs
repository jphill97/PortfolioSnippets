using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBlock : MonoBehaviour
{
    PreviewTile[] tiles;

    // Start is called before the first frame update
    void Start()
    {
        GetTiles();
    }

    void GetTiles()
    {
        tiles = GetComponentsInChildren<PreviewTile>();
    }

    public bool CheckTiles()
    {
        bool isValid = true;

        foreach (var tile in tiles) 
        {
            if (!tile.CheckForObstacles())
            {
                isValid = false;
            }
        }

        return isValid;
    }
}
