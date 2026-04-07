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

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (playerInputHandler == null)
            playerInputHandler = FindAnyObjectByType<PlayerInputHandler>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (inventoryManager == null)
            inventoryManager = FindAnyObjectByType<InventoryManager>();

        if (playerAnimator == null)
            playerAnimator = player.GetComponentInChildren<Animator>();

        if (playerController == null)
            playerController = player.GetComponent<CharacterController>();

        if (mainCanvas == null)
            mainCanvas = FindAnyObjectByType<Canvas>();
    }
}