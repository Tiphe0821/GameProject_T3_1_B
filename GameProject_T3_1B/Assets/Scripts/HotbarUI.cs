using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using GameFramework.Gameplay;

public class HotbarUI : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private Hotbar hotbar;
    [SerializeField] private TMP_FontAsset font;              // 한글 아이템 이름을 쓰면 한글을 지원하는 TMP 폰트를 넣을 것

    [Header("모양")]
    [SerializeField] private float slotSize = 80f;
    [SerializeField] private float spacing = 8f;
    [SerializeField] private float bottomMargin = 30f;

    [Header("드래그용 커서 해제")]
    [SerializeField] private bool useCursorUnlockKey = true;  // 커서가 잠겨 있을 때, 이 키를 누르는 동안 커서를 풀어 준다
    [SerializeField] private Key cursorUnlockKey = Key.LeftAlt;

    public static bool IsCursorFree { get; private set; }     // 커서가 풀려 있는 동안 true (카메라가 확인한다)

    private HotbarSlotUI[] slotUIs;

    private bool cursorFree;
    private CursorLockMode previousLockState;
    private bool previousCursorVisible;

    private void Start()
    {
        if (hotbar == null) hotbar = FindAnyObjectByType<Hotbar>();
        if (hotbar == null)
        {
            Debug.LogWarning("[HotbarUI] Hotbar를 찾을 수 없습니다.");
            enabled = false;
            return;
        }

        if (FindAnyObjectByType<EventSystem>() == null)
            Debug.LogWarning("[HotbarUI] 씬에 EventSystem이 없습니다. 드래그와 클릭이 동작하려면 필요합니다.");

        BuildUI();
    }

    private void OnDisable()
    {
        IsCursorFree = false;
    }

    private void Update()
    {
        if (slotUIs == null) return;

        InventoryManager inventory = InventoryManager.Instance;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            string id = hotbar.GetSlotItemId(i);
            int count = (inventory != null && !string.IsNullOrEmpty(id)) ? inventory.GetCount(id) : 0;

            slotUIs[i].Refresh(id, count, i == hotbar.SelectedIndex);
        }
    }

    // 드래그를 하려면 커서가 보여야 하므로, 지정한 키를 누르는 동안 커서를 풀어 준다
    private void LateUpdate()
    {
        if (!useCursorUnlockKey) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        bool wantFree = keyboard[cursorUnlockKey].isPressed || HotbarSlotUI.IsDragging;
        IsCursorFree = wantFree;

        if (wantFree && !cursorFree)
        {
            previousLockState = Cursor.lockState;
            previousCursorVisible = Cursor.visible;
            cursorFree = true;
        }

        if (wantFree)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (cursorFree)
        {
            Cursor.lockState = previousLockState;
            Cursor.visible = previousCursorVisible;
            cursorFree = false;
        }
    }

    // ===================== UI 생성 =====================

    private void BuildUI()
    {
        GameObject canvasGo = new GameObject("HotbarCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGo.transform.SetParent(transform, false);

        Canvas canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        int count = hotbar.SlotCount;
        float totalWidth = count * slotSize + (count - 1) * spacing;

        GameObject container = new GameObject("Slots", typeof(RectTransform));
        container.transform.SetParent(canvasGo.transform, false);

        RectTransform containerRect = container.GetComponent<RectTransform>();
        containerRect.anchorMin = containerRect.anchorMax = containerRect.pivot = new Vector2(0.5f, 0f);
        containerRect.anchoredPosition = new Vector2(0f, bottomMargin);
        containerRect.sizeDelta = new Vector2(totalWidth, slotSize);

        slotUIs = new HotbarSlotUI[count];

        for (int i = 0; i < count; i++)
        {
            GameObject slotGo = new GameObject($"Slot{i + 1}", typeof(RectTransform), typeof(Image), typeof(HotbarSlotUI));
            slotGo.transform.SetParent(container.transform, false);

            RectTransform slotRect = slotGo.GetComponent<RectTransform>();
            slotRect.anchorMin = slotRect.anchorMax = new Vector2(0f, 0.5f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);
            slotRect.sizeDelta = new Vector2(slotSize, slotSize);
            slotRect.anchoredPosition = new Vector2(slotSize * 0.5f + i * (slotSize + spacing), 0f);

            Image background = slotGo.GetComponent<Image>();

            TextMeshProUGUI nameText = CreateText(slotGo.transform, "Name", font, 16f,
                TextAlignmentOptions.Center, new Vector2(0.05f, 0.25f), new Vector2(0.95f, 0.8f));
            TextMeshProUGUI countText = CreateText(slotGo.transform, "Count", font, 18f,
                TextAlignmentOptions.BottomRight, new Vector2(0.4f, 0f), new Vector2(0.97f, 0.3f));
            TextMeshProUGUI numberText = CreateText(slotGo.transform, "Number", font, 12f,
                TextAlignmentOptions.TopLeft, new Vector2(0.05f, 0.75f), new Vector2(0.4f, 0.98f));
            numberText.text = i < 9 ? (i + 1).ToString() : "";

            HotbarSlotUI slot = slotGo.GetComponent<HotbarSlotUI>();
            slot.Init(hotbar, i, canvas, background, nameText, countText);
            slotUIs[i] = slot;
        }
    }

    // ===================== 공용 도우미 =====================

    public static TextMeshProUGUI CreateText(Transform parent, string objectName, TMP_FontAsset font, float fontSize,
        TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        if (font != null) text.font = font;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        text.enableAutoSizing = true;
        text.fontSizeMin = 8f;
        text.fontSizeMax = fontSize;
        text.overflowMode = TextOverflowModes.Ellipsis;

        return text;
    }

    public static string GetDisplayName(string id)
    {
        if (string.IsNullOrEmpty(id)) return "";

        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null || inventory.Database == null) return id;

        var data = inventory.Database.Get(id);
        return data != null ? data.name : id;
    }
}