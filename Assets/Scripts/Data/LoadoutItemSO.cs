using UnityEngine;

public enum ItemCategory
{
    Pohon,
    Air,
    Pupuk,
    Item
}

[CreateAssetMenu(fileName = "NewLoadoutItem", menuName = "Game/Loadout Item")]
public class LoadoutItemSO : ScriptableObject
{
    [Header("Info Dasar")]
    public string itemName;
    public ItemCategory category;
    public Sprite icon;

    [Header("Stats (Isi 0 semua untuk Pohon)")]
    public int merah;
    public int kuning;
    public int hijau;
}