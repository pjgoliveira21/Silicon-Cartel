using UnityEngine;
using UnityEngine.InputSystem;

public class TestInventory : MonoBehaviour
{
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private ItemData testItem;
    [SerializeField] private InputActionAsset inputActions;

    private InputAction addItemAction;

    void Awake()
    {
        var map = inputActions.FindActionMap("Hotbar", throwIfNotFound: true);

        // Adiciona uma action "AddItem" no teu asset com binding na tecla E
        addItemAction = map.FindAction("AddItem", throwIfNotFound: true);
    }

    void OnEnable()  => addItemAction.Enable();
    void OnDisable() => addItemAction.Disable();

    void Start()
    {
        addItemAction.performed += _ =>
        {
            bool added = inventory.AddItem(testItem);
            Debug.Log(added ? "Item adicionado!" : "Inventário cheio!");
        };
    }
}