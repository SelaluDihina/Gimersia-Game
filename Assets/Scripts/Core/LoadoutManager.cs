using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutManager : MonoBehaviour
{
    [Header("Target Misi Saat Ini")]
    [Range(1, 5)][SerializeField] private int targetRed = 4;
    [Range(1, 5)][SerializeField] private int targetYellow = 3;
    [Range(1, 5)][SerializeField] private int targetGreen = 2;

    [Header("Sistem Tab Kategori (Urutan: 0=Pohon, 1=Air, 2=Pupuk, 3=Item)")]
    [SerializeField] private GameObject[] categoryPanels;
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Image[] tabIcons;
    [SerializeField] private RectTransform activeCircle;
    [SerializeField] private float tabSlideSpeed = 15f;
    [Range(0f, 1f)][SerializeField] private float activeIconAlpha = 1f;
    [Range(0f, 1f)][SerializeField] private float inactiveIconAlpha = 0.35f;

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

    [Header("Warna Pengisian Bar Target & Hover Preview")]
    [SerializeField] private Color emptySlotColor = new Color(0.87f, 0.84f, 0.80f);
    [SerializeField] private Color filledRedColor = new Color(0.9f, 0.25f, 0.25f);
    [SerializeField] private Color filledYellowColor = new Color(0.95f, 0.8f, 0.2f);
    [SerializeField] private Color filledGreenColor = new Color(0.25f, 0.8f, 0.35f);
    [Range(0f, 1f)][SerializeField] private float hoverPreviewAlpha = 0.7f;

    // Status item yang sedang dipilih pemain
    private ItemSlotUI selectedTree;
    private ItemSlotUI selectedWater;
    private ItemSlotUI selectedFertilizer;
    private readonly List<ItemSlotUI> selectedItems = new List<ItemSlotUI>();
    private const int MAX_GENERAL_ITEMS = 2;

    // Daftar slot bar target di pojok kiri bawah
    private readonly List<Image> redTargetSlots = new List<Image>();
    private readonly List<Image> yellowTargetSlots = new List<Image>();
    private readonly List<Image> greenTargetSlots = new List<Image>();

    // Total poin stat saat ini
    private int finalRed;
    private int finalYellow;
    private int finalGreen;

    // Variabel hover & posisi geser tab circle
    private ItemSlotUI hoveredSlot;
    private float targetCircleX;
    private bool hasInitializedCircle;

    private void Start()
    {
        SetupTabsAndSubmit();
        SetupTargetBars();
        PopulateAllCategories();
        RecalculateAndRefreshUI();

        Canvas.ForceUpdateCanvases();
        SwitchTab(0);
    }

    private void Update()
    {
        if (activeCircle == null || !hasInitializedCircle) return;

        // Geser lingkaran secara mulus lurus ke target tanpa membal atau jiggle
        Vector2 pos = activeCircle.anchoredPosition;
        pos.x = Mathf.Lerp(pos.x, targetCircleX, Time.deltaTime * tabSlideSpeed);
        activeCircle.anchoredPosition = pos;
    }

    private void SetupTabsAndSubmit()
    {
        if (tabIcons == null || tabIcons.Length != tabButtons.Length)
        {
            tabIcons = new Image[tabButtons.Length];
            for (int i = 0; i < tabButtons.Length; i++)
            {
                if (tabButtons[i] != null && tabButtons[i].transform.childCount > 0)
                {
                    tabIcons[i] = tabButtons[i].transform.GetChild(0).GetComponent<Image>();
                }
            }
        }

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
        // Tampilkan panel kategori yang dipilih
        for (int i = 0; i < categoryPanels.Length; i++)
        {
            if (categoryPanels[i] != null)
                categoryPanels[i].SetActive(i == activeIndex);
        }

        // Atur transparansi ikon tab aktif vs tidak aktif
        for (int i = 0; i < tabIcons.Length; i++)
        {
            if (tabIcons[i] != null)
            {
                Color c = tabIcons[i].color;
                c.a = (i == activeIndex) ? activeIconAlpha : inactiveIconAlpha;
                tabIcons[i].color = c;
            }
        }

        // Atur target posisi X untuk activeCircle
        if (activeCircle != null && activeIndex >= 0 && activeIndex < tabButtons.Length && tabButtons[activeIndex] != null)
        {
            RectTransform targetBtnRect = tabButtons[activeIndex].GetComponent<RectTransform>();
            targetCircleX = targetBtnRect.anchoredPosition.x;

            if (!hasInitializedCircle)
            {
                Vector2 pos = activeCircle.anchoredPosition;
                pos.x = targetCircleX;
                activeCircle.anchoredPosition = pos;
                hasInitializedCircle = true;
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

        // Sisakan child(0) sebagai pil indikator warna di paling kiri
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

    public void OnItemHoverEnter(ItemSlotUI slot)
    {
        hoveredSlot = slot;
        RefreshBarDisplay();
    }

    public void OnItemHoverExit(ItemSlotUI slot)
    {
        if (hoveredSlot == slot)
        {
            hoveredSlot = null;
            RefreshBarDisplay();
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

        finalRed = Mathf.Clamp(rawRed, 0, targetRed);
        finalYellow = Mathf.Clamp(rawYellow, 0, targetYellow);
        finalGreen = Mathf.Clamp(rawGreen, 0, targetGreen);

        RefreshBarDisplay();
    }

    private void RefreshBarDisplay()
    {
        if (hoveredSlot == null)
        {
            UpdateBarWithPreview(redTargetSlots, finalRed, finalRed, filledRedColor);
            UpdateBarWithPreview(yellowTargetSlots, finalYellow, finalYellow, filledYellowColor);
            UpdateBarWithPreview(greenTargetSlots, finalGreen, finalGreen, filledGreenColor);
            return;
        }

        CalculateSimulatedStats(hoveredSlot, out int simRed, out int simYellow, out int simGreen);

        UpdateBarWithPreview(redTargetSlots, finalRed, simRed, filledRedColor);
        UpdateBarWithPreview(yellowTargetSlots, finalYellow, simYellow, filledYellowColor);
        UpdateBarWithPreview(greenTargetSlots, finalGreen, simGreen, filledGreenColor);
    }

    private void CalculateSimulatedStats(ItemSlotUI hoverSlot, out int simRed, out int simYellow, out int simGreen)
    {
        ItemSlotUI simWater = selectedWater;
        ItemSlotUI simFert = selectedFertilizer;
        List<ItemSlotUI> simItems = new List<ItemSlotUI>(selectedItems);

        switch (hoverSlot.ItemData.category)
        {
            case ItemCategory.Air:
                simWater = (simWater == hoverSlot) ? null : hoverSlot;
                break;

            case ItemCategory.Pupuk:
                simFert = (simFert == hoverSlot) ? null : hoverSlot;
                break;

            case ItemCategory.Item:
                if (simItems.Contains(hoverSlot))
                    simItems.Remove(hoverSlot);
                else if (simItems.Count < MAX_GENERAL_ITEMS)
                    simItems.Add(hoverSlot);
                break;
        }

        int rawRed = 0;
        int rawYellow = 0;
        int rawGreen = 0;

        AddStats(simWater, ref rawRed, ref rawYellow, ref rawGreen);
        AddStats(simFert, ref rawRed, ref rawYellow, ref rawGreen);

        foreach (ItemSlotUI item in simItems)
        {
            AddStats(item, ref rawRed, ref rawYellow, ref rawGreen);
        }

        simRed = Mathf.Clamp(rawRed, 0, targetRed);
        simYellow = Mathf.Clamp(rawYellow, 0, targetYellow);
        simGreen = Mathf.Clamp(rawGreen, 0, targetGreen);
    }

    private void AddStats(ItemSlotUI slot, ref int red, ref int yellow, ref int green)
    {
        if (slot == null) return;
        red += slot.ItemData.merah;
        yellow += slot.ItemData.kuning;
        green += slot.ItemData.hijau;
    }

    private void UpdateBarWithPreview(List<Image> slots, int currentCount, int previewCount, Color activeColor)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (previewCount >= currentCount)
            {
                if (i < currentCount)
                    slots[i].color = new Color(activeColor.r, activeColor.g, activeColor.b, 1f);
                else if (i < previewCount)
                    slots[i].color = new Color(activeColor.r, activeColor.g, activeColor.b, hoverPreviewAlpha);
                else
                    slots[i].color = emptySlotColor;
            }
            else
            {
                if (i < previewCount)
                    slots[i].color = new Color(activeColor.r, activeColor.g, activeColor.b, 1f);
                else if (i < currentCount)
                    slots[i].color = new Color(activeColor.r, activeColor.g, activeColor.b, 0.3f);
                else
                    slots[i].color = emptySlotColor;
            }
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