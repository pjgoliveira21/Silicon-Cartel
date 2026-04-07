using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representa um ponto de entrada ou saída de cabo num componente de hardware.
/// Coloca este componente em qualquer Transform que sirva de porta de ligação.
/// </summary>
public class CablePort : MonoBehaviour
{
    [Header("Configuração da Porta")]
    [Tooltip("Identificador visual para o jogador (ex: 'PCIe x16', 'PSU +12V')")]
    public string portLabel = "Port";
    
    [Tooltip("Tipo de cabo aceite — usado para validar ligações incompatíveis")]
    public CableType acceptedType = CableType.Power;
    
    [Tooltip("Máximo de cabos que podem ser ligados a esta porta")]
    public int maxConnections = 1;

    // Lista de cabos atualmente ligados a esta porta
    private readonly List<CableConnection> _connections = new();

    public bool IsAvailable => _connections.Count < maxConnections;
    public IReadOnlyList<CableConnection> Connections => _connections;

    // Eventos para sistemas externos (UI de highlight, áudio, simulação elétrica)
    public static event System.Action<CablePort> OnPortHovered;
    public static event System.Action<CablePort> OnPortUnhovered;

    public void AddConnection(CableConnection connection)
    {
        _connections.Add(connection);
    }

    public void RemoveConnection(CableConnection connection)
    {
        _connections.Remove(connection);
    }

    private void OnMouseEnter() => OnPortHovered?.Invoke(this);
    private void OnMouseExit()  => OnPortUnhovered?.Invoke(this);
}

/// <summary>
/// Tipos de cabo — permite validar que uma PSU não liga a uma porta de rede, por exemplo.
/// </summary>
public enum CableType { Power, Data, Network }