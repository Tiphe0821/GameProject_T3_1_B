using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HotbarSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public static bool IsDragging { get; private set; }

    private static readonly Color NormalColor = new Color(0f, 0f, 0f, 0.55f);
    private static readonly Color SelectedColor = new Color(0.25f, 0.55f, 0.9f, 0.9f);

    public int Index { get; private set; }

    private Hotbar hotbar;
    private Canvas rootCanvas;
    private Image background;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI countText;

    private GameObject ghost;

    private bool firstRefresh = true;
    private string lastId = "";
    private int lastCount = -1;
    private bool lastSelected;

    public void Init(Hotbar hotbar, int index, Canvas rootCanvas, Image background, TextMeshProUGUI nameText, TextMeshProUGUI countText)
    {
        this.hotbar = hotbar;
        Index = index;
        this.rootCanvas = rootCanvas;
        this.background = background;
        this.nameText = nameText;
        this.countText = countText;
    }

    // 값이 바뀐 경우에만 화면을 갱신한다
    public void Refresh(string id, int count, bool selected)
    {
        id = id ?? "";

        bool idChanged = firstRefresh || id != lastId;
        if (!idChanged && count == lastCount && selected == lastSelected) return;

        if (idChanged) nameText.text = HotbarUI.GetDisplayName(id);
        countText.text = (id != "" && count > 1) ? count.ToString() : "";   // 2개 이상일 때만 개수 표시
        background.color = selected ? SelectedColor : NormalColor;
        transform.localScale = selected ? Vector3.one * 1.1f : Vector3.one;

        firstRefresh = false;
        lastId = id;
        lastCount = count;
        lastSelected = selected;
    }

    // ===================== 클릭 =====================

    public void OnPointerClick(PointerEventData eventData)
    {
        hotbar.SelectSlot(Index);
    }

    // ===================== 드래그 앤 드롭 =====================

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 빈 칸은 드래그할 수 없다
        if (string.IsNullOrEmpty(hotbar.GetSlotItemId(Index))) return;

        ghost = new GameObject("HotbarDragGhost", typeof(RectTransform), typeof(Image));
        ghost.transform.SetParent(rootCanvas.transform, false);
        ghost.transform.SetAsLastSibling();

        ghost.GetComponent<RectTransform>().sizeDelta = ((RectTransform)transform).sizeDelta;

        Image image = ghost.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.6f);
        image.raycastTarget = false;                 // 드롭 대상 감지를 막지 않도록

        TextMeshProUGUI text = HotbarUI.CreateText(ghost.transform, "Name", nameText.font, 16f,
            TextAlignmentOptions.Center, Vector2.zero, Vector2.one);
        text.text = nameText.text;
        text.color = Color.black;

        ghost.transform.position = eventData.position;
        IsDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghost != null) ghost.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CleanupGhost();
    }

    public void OnDrop(PointerEventData eventData)
    {
        HotbarSlotUI from = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponent<HotbarSlotUI>() : null;
        if (from == null || from == this) return;

        hotbar.SwapSlots(from.Index, Index);
    }

    private void OnDisable()
    {
        CleanupGhost();
    }

    private void CleanupGhost()
    {
        if (ghost != null) Destroy(ghost);
        ghost = null;
        IsDragging = false;
    }
}