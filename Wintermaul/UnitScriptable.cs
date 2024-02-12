using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScriptable : ScriptableObject
{
    [SerializeField]
    string unitName;
    public string UnitName => unitName;

    [SerializeField]
    Sprite icon;
    public Sprite Icon => icon;

    [SerializeField]
    GameObject meshPrefab;
    public GameObject MeshPrefab => meshPrefab;

    [SerializeField]
    UNIT_TYPE unitType;
    public UNIT_TYPE UnitType => unitType;

    [SerializeField]
    float health;
    public float Health => health;

    [SerializeField]
    float healthRegen;
    public float HealthRegen => healthRegen;

    [SerializeField]
    ARMOR_TYPE armorType;
    public ARMOR_TYPE ArmorType => armorType;

    [SerializeField]
    int armor;
    public int Armor => armor;

    [SerializeField]
    int movementSpeed;
    public int MovementSpeed => movementSpeed;

    [SerializeField]
    ATTACK_TYPE attackType;
    public ATTACK_TYPE AttackType => attackType;

    [SerializeField]
    [Tooltip("Attacks per minute")]
    int attackSpeed;
    public int AttackSpeed => attackSpeed;

    [SerializeField]
    int baseDamage;
    public int BaseDamage => baseDamage;

    [SerializeField]
    float attackRange;
    public float AttackRange => attackRange;

    [SerializeField]
    DAMAGE_TYPE damageType;
    public DAMAGE_TYPE DamageType => damageType;
}

public enum  ARMOR_TYPE
{
    STANDARD,
    FORTIFIED
}

public enum UNIT_TYPE
{
    CREEP,
    TOWER,
    HERO
}

public enum ATTACK_TYPE
{
    MELEE,
    RANGED
}

public enum DAMAGE_TYPE
{
    NORMAL,
    PIERCING,
    SIEGE
}
