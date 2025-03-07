using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(BoidMovement3D))]
public class NeighborRadiusCircle : MonoBehaviour
{
    private BoidMovement3D boidMovement;
    private LineRenderer lineRenderer;
    public SpawnObjectOnClick ctrlObject;
    
    // Nombre de segments pour dessiner le cercle (plus c'est élevé, plus le cercle sera lisse)
    public int segments = 64;
    
    // Couleur et largeur du cercle
    public Color circleColor = new Color(1f, 1f, 0f, 0.5f); // Jaune semi-transparent
    public float lineWidth = 0.05f;
    
    void Awake()
    {
        boidMovement = GetComponent<BoidMovement3D>();
        lineRenderer = GetComponent<LineRenderer>();
        
        // Configuration du LineRenderer
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false; // Pour que le cercle soit dessiné en local autour du boid
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = segments;
        
        // On utilise le shader "Sprites/Default" qui gère bien la transparence
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = circleColor;
        lineRenderer.material = mat;
    }
    
    void Update()
    {
        if (boidMovement == null)
            return;
        if (!ctrlObject.showNeighborRadius)
            return;
            
        float radius = boidMovement.neighborRadius;
        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            // Dessiner le cercle sur le plan XZ
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
            angle += 360f / segments;
        }
    }
}