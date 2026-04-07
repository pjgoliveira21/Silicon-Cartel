using UnityEngine;

/// <summary>
/// Representa uma ligação completa entre dois CablePort.
/// Guarda referências aos dois lados e ao visual do cabo.
/// </summary>
public class CableConnection
{
    public CablePort    PortA       { get; private set; }
    public CablePort    PortB       { get; private set; }
    public CableVisual  Visual      { get; private set; }
    public CableType    CableType   { get; private set; }

    public CableConnection(CablePort a, CablePort b, CableVisual visual, CableType type)
    {
        PortA     = a;
        PortB     = b;
        Visual    = visual;
        CableType = type;

        // Regista a ligação em ambas as portas
        a.AddConnection(this);
        b.AddConnection(this);
    }

    /// <summary>
    /// Remove a ligação de ambas as portas e destrói o visual.
    /// </summary>
    public void Disconnect()
    {
        PortA.RemoveConnection(this);
        PortB.RemoveConnection(this);
        Visual.Destroy();
    }
}