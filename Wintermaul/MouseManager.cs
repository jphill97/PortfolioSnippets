using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public delegate void OnMouseClicked();
    public OnMouseClicked onMouseClicked;

    public Vector3 mouseWorldPosition {  get; private set; }
    public Vector3 mousePosition {  get; private set; }

    LayerMask envMask;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        envMask = LayerMask.GetMask("Environment");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMousePosition();

        if (Input.GetMouseButtonDown(0))
        {
            onMouseClicked();
        }
    }

    public void UpdateMousePosition()
    {
        mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
   

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit,100f, envMask))
            mouseWorldPosition = hit.point;
        else
            mouseWorldPosition = - Vector3.one;
    }
}
