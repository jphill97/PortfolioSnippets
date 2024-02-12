using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public PlayerCharacterController player;
    public TMPro.TextMeshProUGUI text;
    public bool horizontalOnly = false;
    public bool verticalOnly = false;
    // Update is called once per frame
    void Update()
    {
        if (horizontalOnly)
        {
            Vector3 horVel = player.CharacterVelocity;
            horVel.y = 0;
            text.text = Mathf.FloorToInt(horVel.magnitude * 32f).ToString();
        }
        else if(verticalOnly) 
        {
            Vector3 vertVel = player.CharacterVelocity;
            vertVel.x = 0;
            vertVel.z = 0;
            text.text = Mathf.FloorToInt(vertVel.magnitude * 10f).ToString();
        }
        else
        {
            text.text = Mathf.FloorToInt(player.CharacterVelocity.magnitude * 32f).ToString();
        }
    }
}
