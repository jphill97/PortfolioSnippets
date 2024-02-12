using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTile : MonoBehaviour
{
    [SerializeField]
    float rayHeight = 2;

    [SerializeField]
    MeshRenderer mesh;
    public MeshRenderer Mesh => mesh;

    bool valid = true;
    public bool Valid => valid;

    [SerializeField]
    Material validMat;

    [SerializeField]
    Material invalidMat;

    public void ChangeMaterial(Material mat)
    {
        mesh.material = mat;
    }

    public bool CheckForObstacles()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position + Vector3.up * rayHeight,Vector3.down, out hit))
        {
            if (hit.collider.tag == "Building" || hit.collider.tag == "Unit" || hit.collider.tag == "Unbuildable")
            {
                if (valid)
                    ChangeMaterial(invalidMat);

                valid = false;
                return valid;
            }
        }

        if (!valid)
            ChangeMaterial(validMat);

        valid = true;
        return valid;
    }
}
