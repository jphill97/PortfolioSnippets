using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;

public class InputHud : MonoBehaviour
{
    public PlayerCharacterController player;

    public UnityEngine.UI.Image[] images;

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
    }

    void ProcessInputs()
    {
        Color col;
        int[] ints = new int[] { (int)player.inputVec.x, (int)player.inputVec.y, (int)player.inputVec.z, (int)player.inputVec.w };
        for (int i = 0; i < images.Length; i++)
        {
            col = images[i].color;

            if (ints[i] == 0)
                col.a = .25f;
            else
                col.a = 1f;

            images[i].color = col;
        }
    }
}
