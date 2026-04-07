using UnityEngine;

public class GameReferences : MonoBehaviour
{
    public static GameReferences Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private CharacterController playerController;

    [Header("Systems")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Canvas mainCanvas;

    public GameObject Player => player;
    public PlayerInputHandler PlayerInputHandler => playerInputHandler;
    public Camera PlayerCamera => playerCamera;
    public InventoryManager InventoryManager => inventoryManager;
    public Canvas MainCanvas => mainCanvas;
    public Animator PlayerAnimator => playerAnimator;
    public CharacterController PlayerController => playerController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        player ??= GameObject.FindWithTag("Player");

        playerInputHandler ??= FindAnyObjectByType<PlayerInputHandler>();

        playerCamera ??= Camera.main;

        inventoryManager ??= FindAnyObjectByType<InventoryManager>();

        playerAnimator ??= player.GetComponentInChildren<Animator>();

        playerController ??= player.GetComponent<CharacterController>();

        mainCanvas ??= FindAnyObjectByType<Canvas>();
    }
}