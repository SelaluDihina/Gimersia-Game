using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutManager : MonoBehaviour
{
    [Header("Target Misi Saat Ini")]
    [SerializeField] private int targetRed = 4;
    [SerializeField] private int targetYellow = 3;
    [SerializeField] private int targetGreen = 5;

    [Header("Sistem Tab Kategori (Urutan: 0=Pohon, 1=Air, 2=Pupuk, 3=Item)")]
    [SerializeField] private GameObject[] categoryPanels;
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Color activeTabColor = Color.white;
    [SerializeField] private Color inactiveTabColor = new Color(0.75f, 0.75f, 0.75f, 1f);

    [Header("Tombol Submission")]
    [SerializeField] private Button submitButton;

    [Header("Daftar Item Tersedia (Masukkan ScriptableObject)")]
    [SerializeField] private List<LoadoutItemSO> treeItems;
    [SerializeField] private List<LoadoutItemSO> waterItems;
    [SerializeField] private List<LoadoutItemSO> fertilizerItems;
    [SerializeField] private List<LoadoutItemSO> generalItems;

    [Header("Prefab UI")]
    [SerializeField] private ItemSlotUI itemSlotPrefab;
    [SerializeField] private Image targetSlotPrefab;

    [Header("Container List Item & Teks Kapasitas")]
    [SerializeField] private Transform treeListContainer;
    [SerializeField] private TextMeshProUGUI treeCapacityText;

    [SerializeField] private Transform waterListContainer;
    [SerializeField] private TextMeshProUGUI waterCapacityText;

    [SerializeField] private Transform fertilizerListContainer;
    [SerializeField] private TextMeshProUGUI fertilizerCapacityText;

    [SerializeField] private Transform itemListContainer;
    [SerializeField] private TextMeshProUGUI itemCapacityText;

    [Header("Bar Target Kiri Bawah")]
    [SerializeField] private Transform redBarContainer;
    [SerializeField] private Transform yellowBarContainer;
    [SerializeField] private Transform greenBarContainer;

    [Header("Warna Pengisian Bar Target")]
    [SerializeField] private Color emptySlotColor = Color.white;
    [SerializeField] private Color filledRedColor = new Color(0.9f, 0.25f, 0.25f);
    [SerializeField] private Color filledYellowColor = new Color(0.95f, 0.8f, 0.2f);
    [SerializeField] private Color filledGreenColor = new Color(0.25f, 0.8f, 0.35f);

    private ItemSlotUI selectedTree;
    private ItemSlotUI selectedWater;
    private ItemSlotUI selectedFertilizer;
    private readonly List<ItemSlotUI> selectedItems = new List<ItemSlotUI>();
    private const int MAX_GENERAL_ITEMS = 2;

    private readonly List<Image> redTargetSlots = new List<Image>();
    private readonly List<Image> yellowTargetSlots = new List<Image>();
    private readonly List<Image> greenTargetSlots = new List<Image>();

    private int finalRed;
    private int finalYellow;
    private int finalGreen;

    private void Start()
    {
        SetupTabsAndSubmit();
        SetupTargetBars();
        PopulateAllCategories();
        RecalculateAndRefreshUI();

        // Buka tab pertama (Pohon) saat mulai
        SwitchTab(0);
    }

    private void SetupTabsAndSubmit()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.RemoveAllListeners();
            tabButtons[i].onClick.AddListener(() => SwitchTab(index));
        }

        if (submitButton != null)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(OnSubmitClicked);
        }
    }

    public void SwitchTab(int activeIndex)
    {
        for (int i = 0; i < categoryPanels.Length; i++)
        {
            bool isActive = (i == activeIndex);
            if (categoryPanels[i] != null)
                categoryPanels[i].SetActive(isActive);

            if (i < tabButtons.Length && tabButtons[i] != null)
            {
                Image tabImg = tabButtons[i].GetComponent<Image>();
                if (tabImg != null)
                    tabImg.color = isActive ? activeTabColor : inactiveTabColor;
            }
        }
    }

    private void SetupTargetBars()
    {
        BuildBarSlots(redBarContainer, targetRed, redTargetSlots);
        BuildBarSlots(yellowBarContainer, targetYellow, yellowTargetSlots);
        BuildBarSlots(greenBarContainer, targetGreen, greenTargetSlots);
    }

    private void BuildBarSlots(Transform barContainer, int targetCount, List<Image> slotList)
    {
        slotList.Clear();

        for (int i = barContainer.childCount - 1; i >= 1; i--)
        {
            Destroy(barContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < targetCount; i++)
        {
            Image slot = Instantiate(targetSlotPrefab, barContainer);
            slot.color = emptySlotColor;
            slotList.Add(slot);
        }
    }

    private void PopulateAllCategories()
    {
        SpawnCategoryItems(treeItems, treeListContainer);
        SpawnCategoryItems(waterItems, waterListContainer);
        SpawnCategoryItems(fertilizerItems, fertilizerListContainer);
        SpawnCategoryItems(generalItems, itemListContainer);
    }

    private void SpawnCategoryItems(List<LoadoutItemSO> items, Transform container)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach (LoadoutItemSO data in items)
        {
            ItemSlotUI slot = Instantiate(itemSlotPrefab, container);
            slot.Setup(data, this);
        }
    }

    public void OnItemClicked(ItemSlotUI clickedSlot)
    {
        switch (clickedSlot.ItemData.category)
        {
            case ItemCategory.Pohon:
                HandleSingleSelection(ref selectedTree, clickedSlot);
                break;

            case ItemCategory.Air:
                HandleSingleSelection(ref selectedWater, clickedSlot);
                break;

            case ItemCategory.Pupuk:
                HandleSingleSelection(ref selectedFertilizer, clickedSlot);
                break;

            case ItemCategory.Item:
                HandleMultiSelection(clickedSlot);
                break;
        }

        RecalculateAndRefreshUI();
    }

    private void HandleSingleSelection(ref ItemSlotUI currentSelected, ItemSlotUI clickedSlot)
    {
        if (currentSelected == clickedSlot)
        {
            currentSelected.SetSelectedVisual(false);
            currentSelected = null;
        }
        else
        {
            if (currentSelected != null)
                currentSelected.SetSelectedVisual(false);

            currentSelected = clickedSlot;
            currentSelected.SetSelectedVisual(true);
        }
    }

    private void HandleMultiSelection(ItemSlotUI clickedSlot)
    {
        if (selectedItems.Contains(clickedSlot))
        {
            selectedItems.Remove(clickedSlot);
            clickedSlot.SetSelectedVisual(false);
        }
        else if (selectedItems.Count < MAX_GENERAL_ITEMS)
        {
            selectedItems.Add(clickedSlot);
            clickedSlot.SetSelectedVisual(true);
        }
    }

    private void RecalculateAndRefreshUI()
    {
        treeCapacityText.text = $"{(selectedTree != null ? 1 : 0)}/1";
        waterCapacityText.text = $"{(selectedWater != null ? 1 : 0)}/1";
        fertilizerCapacityText.text = $"{(selectedFertilizer != null ? 1 : 0)}/1";
        itemCapacityText.text = $"{selectedItems.Count}/{MAX_GENERAL_ITEMS}";

        int rawRed = 0;
        int rawYellow = 0;
        int rawGreen = 0;

        AddStats(selectedWater, ref rawRed, ref rawYellow, ref rawGreen);
        AddStats(selectedFertilizer, ref rawRed, ref rawYellow, ref rawGreen);

        foreach (ItemSlotUI item in selectedItems)
        {
            AddStats(item, ref rawRed, ref rawYellow, ref rawGreen);
        }

        // Double Clamp: mentok di 0 jika minus, dan mentok di target maksimal jika berlebih
        finalRed = Mathf.Clamp(rawRed, 0, targetRed);
        finalYellow = Mathf.Clamp(rawYellow, 0, targetYellow);
        finalGreen = Mathf.Clamp(rawGreen, 0, targetGreen);

        UpdateBarVisual(redTargetSlots, finalRed, filledRedColor);
        UpdateBarVisual(yellowTargetSlots, finalYellow, filledYellowColor);
        UpdateBarVisual(greenTargetSlots, finalGreen, filledGreenColor);
    }

    private void AddStats(ItemSlotUI slot, ref int red, ref int yellow, ref int green)
    {
        if (slot == null) return;
        red += slot.ItemData.merah;
        yellow += slot.ItemData.kuning;
        green += slot.ItemData.hijau;
    }

    private void UpdateBarVisual(List<Image> slots, int filledCount, Color activeColor)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].color = (i < filledCount) ? activeColor : emptySlotColor;
        }
    }

    private void OnSubmitClicked()
    {
        if (selectedTree == null)
        {
            Debug.LogWarning("Submission Gagal: Kamu belum memilih Pohon!");
            return;
        }

        bool isStatsComplete = (finalRed == targetRed) &&
                               (finalYellow == targetYellow) &&
                               (finalGreen == targetGreen);

        if (isStatsComplete)
        {
            Debug.Log($"Submission BERHASIL! Pohon: {selectedTree.ItemData.itemName} | Stats: ({finalRed}, {finalYellow}, {finalGreen})");
        }
        else
        {
            Debug.LogWarning($"Submission Gagal: Stats belum memenuhi target! Saat ini ({finalRed}/{targetRed}, {finalYellow}/{targetYellow}, {finalGreen}/{targetGreen})");
        }
    }
}