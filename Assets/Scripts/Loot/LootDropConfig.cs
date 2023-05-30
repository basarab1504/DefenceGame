using System;
using UnityEngine;

[Serializable]
public struct LootDropConfig
{
    public Loot prefab;
    public int count;
    [SerializeField]
    [Range(0, 1)]
    public float dropChance;
}
