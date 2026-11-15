using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Komponen UI Kartu (Wajib)")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Image cardBackground;
    [SerializeField] private Transform statsContainer;

    [Header("Komponen Opsional (Kosongkan saat Playtest)")]
    [SerializeField] private Image circleIcon;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Prefab Kapsul Stats")]
    [SerializeField] private GameObject statPipPrefab;

    [Header("Warna Status Pilihan")]
    [SerializeField] private Color normalCardColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private Color selectedCardColor = new Color(0.65f, 0.9f, 0.7f, 1f);
    [SerializeField] private Color normalCircleColor = Color.white;
    [SerializeField] private Color selectedCircleColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    [Header("Warna Kapsul Stats")]
    [SerializeField] private Color redColor = new Color(0.9f, 0.25f, 0.25f);
    [SerializeField] private Color yellowColor = new Color(0.95f, 0.8f, 0.2f);
    [SerializeField] private Color greenColor = new Color(0.25f, 0.8f, 0.35f);

    private LoadoutItemSO itemData;
    private LoadoutManager manager;

    public LoadoutItemSO ItemData => itemData;

    public void Setup(LoadoutItemSO data, LoadoutManager loadoutManager)
    {
        itemData = data;
        manager = loadoutManager;

        if (nameText != null)
            nameText.text = data.itemName;

        if (circleIcon != null && data.icon != null)
            circleIcon.sprite = data.icon;

        foreach (Transform child in statsContainer)
            Destroy(child.gameObject);

        SpawnStatPips(data.merah, redColor);
        SpawnStatPips(data.kuning, yellowColor);
        SpawnStatPips(data.hijau, greenColor);

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => manager.OnItemClicked(this));

        SetSelectedVisual(false);
    }

    private void SpawnStatPips(int value, Color baseColor)
    {
        if (value == 0 || statPipPrefab == null) return;

        int count = Mathf.Abs(value);
        bool isNegative = value < 0;

        for (int i = 0; i < count; i++)
        {
            GameObject pip = Instantiate(statPipPrefab, statsContainer);
            Image pipImg = pip.GetComponent<Image>();
            TextMeshProUGUI pipText = pip.GetComponentInChildren<TextMeshProUGUI>();

            if (pipImg != null)
            {
                pipImg.color = isNegative
                    ? new Color(baseColor.r * 0.6f, baseColor.g * 0.6f, baseColor.b * 0.6f, 0.85f)
                    : baseColor;
            }

            if (pipText != null)
            {
                pipText.text = isNegative ? "-" : "";
            }
        }
    }

    public void SetSelectedVisual(bool isSelected)
    {
        if (cardBackground != null)
            cardBackground.color = isSelected ? selectedCardColor : normalCardColor;

        if (circleIcon != null)
            circleIcon.color = isSelected ? selectedCircleColor : normalCircleColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (manager != null)
            manager.OnItemHoverEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (manager != null)
            manager.OnItemHoverExit(this);
    }

    private void OnDisable()
    {
        if (manager != null)
            manager.OnItemHoverExit(this);
    }
}