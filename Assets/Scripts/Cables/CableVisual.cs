using System;
using UnityEngine;

/// <summary>
/// Renderiza um cabo entre dois pontos usando uma aproximação de catenária.
/// Não usa física real — é puramente matemático, logo é muito performante.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class CableVisual : MonoBehaviour
{
    [Header("Aparência")]
    [SerializeField] private Material cableMaterial;
    [SerializeField] private float cableWidth = 0.04f;
    [SerializeField] private Color cableColor = new Color(0.2f, 0.2f, 0.2f);

    [Header("Simulação de Catenária")]
    [Tooltip("Controla o 'sag' (curvatura) do cabo. Valores maiores = cabo mais tenso.")]
    [SerializeField, Range(0.5f, 10f)] private float catenary_a = 2f;
    
    [Tooltip("Número de segmentos da linha — mais segmentos = curva mais suave")]
    [SerializeField, Range(8, 32)] private int segments = 16;

    private LineRenderer _lineRenderer;
    private Transform _startPoint;
    private Transform _endPoint;

    // Para o cabo "preview" (da porta à mão do player)
    private bool _isPreview = false;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = segments + 1;
        _lineRenderer.startWidth    = cableWidth;
        _lineRenderer.endWidth      = cableWidth;
        _lineRenderer.material      = cableMaterial;
        _lineRenderer.startColor    = cableColor;
        _lineRenderer.endColor      = cableColor;
        
        // Usa world space para que os pontos sejam independentes da hierarquia
        _lineRenderer.useWorldSpace = true;
    }

    /// <summary>
    /// Inicializa o cabo com dois pontos fixos (ligação completa).
    /// </summary>
    public void Initialize(Transform start, Transform end)
    {
        _startPoint = start;
        _endPoint   = end;
        _isPreview  = false;
    }

    /// <summary>
    /// Inicializa o cabo em modo preview — extremidade segue um Transform dinâmico (a mão).
    /// </summary>
    public void InitializePreview(Transform start, Transform dynamicEnd)
    {
        _startPoint = start;
        _endPoint   = dynamicEnd;
        _isPreview  = true;
        
        // Em preview, o cabo fica ligeiramente mais transparente
        Color previewColor = cableColor;
        previewColor.a = 0.6f;
        _lineRenderer.startColor = previewColor;
        _lineRenderer.endColor   = previewColor;
    }

    private void Update()
    {
        if (_startPoint == null || _endPoint == null) return;
        DrawCatenaria(_startPoint.position, _endPoint.position);
    }

    /// <summary>
    /// Calcula e aplica os pontos da curva catenária entre dois pontos do mundo.
    /// A catenária é calculada no plano vertical local entre os dois pontos.
    /// </summary>
    private void DrawCatenaria(Vector3 worldStart, Vector3 worldEnd)
    {
        Vector3[] points = new Vector3[segments + 1];

        // Calcula o vetor horizontal entre os pontos (ignorando Y)
        Vector3 horizontalDiff = worldEnd - worldStart;
        float   horizontalDist = horizontalDiff.magnitude;
        float   verticalDiff   = worldEnd.y - worldStart.y;
        
        // Direção horizontal normalizada (para reconstruir posições 3D)
        Vector3 horizontalDir = horizontalDist > 0.001f 
            ? horizontalDiff / horizontalDist 
            : Vector3.right;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments; // 0 a 1 ao longo do cabo
            
            // Posição horizontal interpolada
            float x = t * horizontalDist;
            
            // Catenária: y = a * cosh(x/a) — centrada para passar pelos dois pontos
            // Deslocamos para que os extremos coincidam com start e end
            float cosh_start = MathF.Cosh(0 / catenary_a);
            float cosh_end   = MathF.Cosh(horizontalDist / catenary_a);
            float cosh_x     = MathF.Cosh((x - horizontalDist * 0.5f) / catenary_a);
            
            // Normaliza o sag para que as extremidades fiquem nos pontos corretos
            float catY = catenary_a * (cosh_x - MathF.Cosh(horizontalDist * 0.5f / catenary_a));
            
            // Interpola o Y linear entre start e end, e adiciona o sag da catenária
            float linearY = Mathf.Lerp(worldStart.y, worldEnd.y, t);
            float finalY  = linearY + catY;

            // Reconstrói a posição 3D
            points[i] = worldStart + horizontalDir * x;
            points[i].y = finalY;
        }

        _lineRenderer.SetPositions(points);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}