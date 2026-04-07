using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gere o estado global do sistema de cabos.
/// Deteta portas via raycast a partir da câmara do player,
/// e orquestra o fluxo: hover → primeiro clique → segundo clique → ligação.
/// </summary>
public class CableManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject cableVisualPrefab;
    [Tooltip("Transform na mão do player — o cabo preview segue este ponto")]
    [SerializeField] private Transform  playerHandTransform;

    [Header("Deteção de Portas")]
    [SerializeField] private float      raycastRange    = 3f;
    [SerializeField] private LayerMask  portLayerMask;

    // Estado interno do sistema
    private CablePort   _hoveredPort  = null;
    private CablePort   _selectedPort = null; // porta onde o player já clicou
    private CableVisual _previewCable = null;  // cabo em construção

    private readonly List<CableConnection> _allConnections = new();

    // Eventos para UI/Áudio reagirem sem acoplar ao CableManager
    public static event System.Action<CablePort>       OnPortSelected;
    public static event System.Action<CableConnection> OnConnectionMade;
    public static event System.Action<CableConnection> OnConnectionRemoved;

    private PlayerInputHandler _inputHandler;
    private Camera             _playerCamera;

    private void Start()
    {
        _inputHandler = GameReferences.Instance.PlayerInputHandler;
        _playerCamera = GameReferences.Instance.PlayerCamera;

        _inputHandler.OnInteractPressed += HandleInteractPressed;
        CablePort.OnPortHovered         += HandlePortHovered;
        CablePort.OnPortUnhovered       += HandlePortUnhovered;
    }

    private void OnDisable()
    {
        if (_inputHandler != null)
            _inputHandler.OnInteractPressed -= HandleInteractPressed;
        
        CablePort.OnPortHovered   -= HandlePortHovered;
        CablePort.OnPortUnhovered -= HandlePortUnhovered;
    }

    private void HandlePortHovered(CablePort port)  => _hoveredPort = port;
    private void HandlePortUnhovered(CablePort port) 
    { 
        if (_hoveredPort == port) _hoveredPort = null;
    }

    /// <summary>
    /// Lógica principal de interação — chamada pelo evento de input.
    /// </summary>
    private void HandleInteractPressed()
    {
        // Verifica se o player está a olhar para uma porta via raycast
        CablePort targetPort = GetPortUnderCrosshair();
        if (targetPort == null) 
        {
            // Clique no vazio cancela o cabo em construção
            CancelPreview();
            return;
        }

        // --- PRIMEIRO CLIQUE: seleciona a porta de origem ---
        if (_selectedPort == null)
        {
            if (!targetPort.IsAvailable)
            {
                Debug.Log($"[CableManager] Porta '{targetPort.portLabel}' já está cheia.");
                return;
            }

            _selectedPort = targetPort;
            SpawnPreviewCable(targetPort.transform);
            OnPortSelected?.Invoke(targetPort);
            return;
        }

        // --- SEGUNDO CLIQUE: tenta fechar a ligação ---
        if (targetPort == _selectedPort)
        {
            // Clicou na mesma porta — cancela
            CancelPreview();
            return;
        }

        TryConnectPorts(_selectedPort, targetPort);
    }

    /// <summary>
    /// Faz raycast a partir do centro do ecrã para detetar CablePorts.
    /// </summary>
    private CablePort GetPortUnderCrosshair()
    {
        Ray ray = _playerCamera.ScreenPointToRay(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, portLayerMask))
            return hit.collider.GetComponent<CablePort>();

        return null;
    }

    /// <summary>
    /// Instancia o cabo de preview que segue a mão do player.
    /// </summary>
    private void SpawnPreviewCable(Transform startTransform)
    {
        GameObject go = Instantiate(cableVisualPrefab);
        _previewCable = go.GetComponent<CableVisual>();
        _previewCable.InitializePreview(startTransform, playerHandTransform);
    }

    /// <summary>
    /// Valida a ligação e, se válida, cria a CableConnection definitiva.
    /// </summary>
    private void TryConnectPorts(CablePort portA, CablePort portB)
    {
        // Validação de compatibilidade de tipo
        if (portA.acceptedType != portB.acceptedType)
        {
            Debug.Log($"[CableManager] Tipos incompatíveis: {portA.acceptedType} ↔ {portB.acceptedType}");
            CancelPreview();
            return;
        }

        if (!portB.IsAvailable)
        {
            Debug.Log($"[CableManager] Porta '{portB.portLabel}' está cheia.");
            CancelPreview();
            return;
        }

        // Destrói o preview e cria o cabo definitivo
        Destroy(_previewCable.gameObject);
        _previewCable = null;

        GameObject go = Instantiate(cableVisualPrefab);
        CableVisual visual = go.GetComponent<CableVisual>();
        visual.Initialize(portA.transform, portB.transform);

        CableConnection connection = new CableConnection(portA, portB, visual, portA.acceptedType);
        _allConnections.Add(connection);

        OnConnectionMade?.Invoke(connection);
        _selectedPort = null;

        Debug.Log($"[CableManager] Cabo ligado: {portA.portLabel} ↔ {portB.portLabel}");
    }

    private void CancelPreview()
    {
        if (_previewCable != null)
        {
            Destroy(_previewCable.gameObject);
            _previewCable = null;
        }
        _selectedPort = null;
    }
}