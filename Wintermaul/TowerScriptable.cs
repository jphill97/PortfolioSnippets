using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Units/New Tower")]
public class TowerScriptable : UnitScriptable
{
    [SerializeField]
    int cost = 0;
    public int Cost => cost;

    [SerializeField]
    float constructionTime = 3.0f;
    public float ConstructionTime => constructionTime;

    [SerializeField]
    ATTACK_ELEMENT attackElement;
    public ATTACK_ELEMENT AttackElement => attackElement;

    [SerializeField]
    float projectileSpeed;
    public float ProjectileSpeed => projectileSpeed;

    [SerializeField]
    float splashRadius;
    public float SplashRadius => splashRadius;

    [SerializeField]
    float splashPercentage;
    public float SplashPercentage => splashPercentage;

    [SerializeField]
    float dotDPS;
    public float DotDPS => dotDPS;

    [SerializeField]
    float dotSlowPercentage;
    public float DotSlowPercentage => dotSlowPercentage;

    [SerializeField]
    float dotDuration;
    public float DotDuration => dotDuration;
}

public enum ATTACK_ELEMENT
{
    NEUTRAL,
    POISON,
    FIRE,
    ICE
}

