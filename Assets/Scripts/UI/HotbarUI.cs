using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HotbarUI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Configuração")]
    [SerializeField] private int slotCount = 9;

    private SlotUI[] slotUIs;
    private int selectedIndex = 0;

    private InputAction scrollAction;
    private InputAction selectSlotAction;

    void Awake()
{
    var hotbarMap = inputActions.FindActionMap("Hotbar", throwIfNotFound: true);
    scrollAction     = hotbarMap.FindAction("ScrollWheel", throwIfNotFound: true);
    selectSlotAction = hotbarMap.FindAction("SelectSlot",  throwIfNotFound: true);

    inventory.Initialize(slotCount);
    BuildHotbar();
}

void OnSelectSlot(InputAction.CallbackContext ctx)
{
    var control = ctx.control;
    if (control == null) return;

    string controlName = control.name;
    if (int.TryParse(controlName, out int num))
    {
        int index = num - 1;
        if (index >= 0 && index < slotCount)
            SelectSlot(index);
    }
}

    void OnEnable()
    {
        inventory.OnInventoryChanged += RefreshAll;
        scrollAction.Enable();
        selectSlotAction.Enable();

        scrollAction.performed     += OnScroll;
        selectSlotAction.performed += OnSelectSlot;
    }

    void OnDisable()
    {
        inventory.OnInventoryChanged -= RefreshAll;

        scrollAction.performed     -= OnScroll;
        selectSlotAction.performed -= OnSelectSlot;

        scrollAction.Disable();
        selectSlotAction.Disable();
    }

    void OnScroll(InputAction.CallbackContext ctx)
    {
        float y = ctx.ReadValue<Vector2>().y;
        if (y > 0f) SelectSlot((selectedIndex - 1 + slotCount) % slotCount);
        if (y < 0f) SelectSlot((selectedIndex + 1) % slotCount);
    }

    void BuildHotbar()
    {
        foreach (Transform child in slotsContainer)
            Destroy(child.gameObject);

        slotUIs = new SlotUI[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotsContainer);
            go.name = $"Slot_{i}";
            slotUIs[i] = go.GetComponent<SlotUI>();
        }

        SelectSlot(0);
        RefreshAll();
    }

    void SelectSlot(int index)
    {
        if (slotUIs == null) return;
        slotUIs[selectedIndex].SetSelected(false);
        selectedIndex = index;
        slotUIs[selectedIndex].SetSelected(true);
    }

    void RefreshAll()
    {
        for (int i = 0; i < slotCount; i++)
            slotUIs[i].Refresh(inventory.slots[i]);
    }

    public void Resize(int newCount)
    {
        slotCount = newCount;
        inventory.Initialize(newCount);
        BuildHotbar();
    }

    public InventoryManager.InventorySlot GetSelectedSlot()
        => inventory.slots[selectedIndex];
}