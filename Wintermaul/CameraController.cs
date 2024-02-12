using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5f;

    [SerializeField]
    float scrollSpeed = 5f;

    [SerializeField]
    float maxHeight = 36f;

    [SerializeField]
    float minHeight = 10f;

    MouseManager mouseManager;
    // Start is called before the first frame update
    void Start()
    {
        mouseManager = GetComponent<MouseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMousePosition(mouseManager.mousePosition);

        Vector2 scroll = Input.mouseScrollDelta;
        if (scroll != Vector2.zero)
            ZoomCamera(scroll.y);
    }

    void ZoomCamera(float scrollValue)
    {
        Vector3 newPos = transform.position + transform.forward * Mathf.Sign(scrollValue) * scrollSpeed;
        if(newPos.y >= minHeight &&  newPos.y <= maxHeight)
        {
            transform.position = newPos;
        }
    }

    void CheckMousePosition(Vector3 mousePosition)
    {
        Vector3 moveDir = Vector3.zero;

        if(mousePosition.x > Screen.width - 10)
        {
            moveDir.z = 1;
        }
        else if(mousePosition.x < 10f)
        {
            moveDir.z = -1;
        }

        if (mousePosition.y > Screen.height - 10)
        {
            moveDir.x = -1;
        }
        else if (mousePosition.y < 10)
        {
            moveDir.x = 1;
        }

        Vector3 newPos = transform.position + moveDir * moveSpeed * Time.deltaTime;
        newPos.z = Mathf.Clamp(newPos.z,15,130);
        newPos.x = Mathf.Clamp(newPos.x, 15, 170);
        transform.position = newPos;
    }
}
