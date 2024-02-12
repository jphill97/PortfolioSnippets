using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Race", menuName = "New Race")]
public class RaceScriptable : ScriptableObject
{
    [SerializeField]
    TowerScriptable[] towers;
    public TowerScriptable[] Towers => towers;
}
