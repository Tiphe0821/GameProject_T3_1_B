using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GameFramework.Gameplay;

public class Hotbar : MonoBehaviour
{
    [Header("핫바 설정")]
    [SerializeField] private int slotCount = 9;
    [SerializeField] private bool debugLog = true;           // 핫바 UI 확인용 Console 로그

    [Header("연결")]
    [SerializeField] private PlayerItemInteraction itemInteraction;

    private string[] slots;                                  // 칸마다 아이템 id (빈 칸은 null)
    private int selectedIndex = 0;
    private string lastHeldId = null;

    public int SlotCount => slotCount;
    public int SelectedIndex => selectedIndex;
    public string GetSlotItemId(int index) => slots[index];

    // 핫바 UI가 구독해서 화면을 갱신할 수 있다
    public event Action OnHotbarChanged;

    private void Awake()
    {
        slotCount = Mathf.Max(1, slotCount);
        slots = new string[slotCount];

        if (itemInteraction == null)
            itemInteraction = FindAnyObjectByType<PlayerItemInteraction>();
    }

    private void Update()
    {
        bool changed = SyncWithInventory();

        HandleNumberKeys();
        HandleScroll();

        RefreshHeldItem();

        if (changed) OnHotbarChanged?.Invoke();
    }

    // ===================== 선택 =====================

    public void SelectSlot(int index)
    {
        index = ((index % slotCount) + slotCount) % slotCount;   // 범위를 벗어나면 반대편으로 순환
        if (index == selectedIndex) return;

        selectedIndex = index;
        RefreshHeldItem();
        OnHotbarChanged?.Invoke();
    }

    // ===================== 칸 맞바꾸기 (드래그 앤 드롭용) =====================

    public void SwapSlots(int a, int b)
    {
        if (a < 0 || b < 0 || a >= slotCount || b >= slotCount || a == b) return;

        (slots[a], slots[b]) = (slots[b], slots[a]);

        RefreshHeldItem();              // 들고 있던 칸이 바뀌었으면 손에 든 아이템도 갱신
        OnHotbarChanged?.Invoke();
    }

    private void HandleNumberKeys()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        int keyCount = Mathf.Min(slotCount, 9);
        for (int i = 0; i < keyCount; i++)
        {
            if (keyboard[(Key)((int)Key.Digit1 + i)].wasPressedThisFrame)
            {
                SelectSlot(i);
                return;
            }
        }
    }

    private void HandleScroll()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.leftCtrlKey.isPressed) return;   // Ctrl+휠은 카메라 줌에 쓴다

        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        float scroll = mouse.scroll.ReadValue().y;

        if (scroll < 0f) SelectSlot(selectedIndex + 1);          // 휠 아래로: 다음 칸 (마인크래프트 방식)
        else if (scroll > 0f) SelectSlot(selectedIndex - 1);     // 휠 위로: 이전 칸
    }

    // ===================== 들고 있는 아이템 갱신 =====================

    private void RefreshHeldItem()
    {
        string id = slots[selectedIndex] ?? "";
        if (id == lastHeldId) return;

        lastHeldId = id;

        if (itemInteraction != null)
            itemInteraction.SetHeldItem(id);

        if (debugLog)
            Debug.Log($"[Hotbar] 선택: {selectedIndex + 1}번 칸 / 들고 있는 아이템: {(id == "" ? "없음" : id)}");
    }

    // ===================== 인벤토리와 동기화 =====================

    private bool SyncWithInventory()
    {
        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null) return false;

        bool changed = false;

        // 1. 다 쓴 아이템의 칸을 비운다
        for (int i = 0; i < slots.Length; i++)
        {
            if (string.IsNullOrEmpty(slots[i])) continue;

            if (inventory.GetCount(slots[i]) <= 0)
            {
                slots[i] = null;
                changed = true;
            }
        }

        // 2. 핫바에 없는 새 아이템을 첫 번째 빈 칸에 넣는다
        foreach (ItemStack stack in inventory.Stacks)
        {
            if (stack.count <= 0 || IsInHotbar(stack.itemId)) continue;

            int empty = FindEmptySlot();
            if (empty < 0) break;                                // 빈 칸이 없으면 더 넣지 않는다

            slots[empty] = stack.itemId;
            changed = true;
        }

        return changed;
    }

    private bool IsInHotbar(string itemId)
    {
        for (int i = 0; i < slots.Length; i++)
            if (slots[i] == itemId) return true;
        return false;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
            if (string.IsNullOrEmpty(slots[i])) return i;
        return -1;
    }
}