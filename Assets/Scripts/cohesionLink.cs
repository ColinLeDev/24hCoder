using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoidMovement3D))]
public class BoidIndicators : MonoBehaviour
{
    private BoidMovement3D boidMovement;

    // Paramètres du cercle (neighborRadius)
    [Header("Cercle de voisinage")]
    public int circleSegments = 64;
    public Color circleColor = new Color(1f, 1f, 0f, 0.5f); // Jaune semi-transparent
    public float circleLineWidth = 0.05f;
    private GameObject circleObject;
    private LineRenderer circleLineRenderer;

    // Paramètres de la ligne de cohésion
    [Header("Ligne de cohésion")]
    public Color cohesionLineColor = new Color(0f, 1f, 0f, 0.8f); // Vert semi-transparent
    public float cohesionLineWidth = 0.05f;
    private GameObject cohesionLineObject;
    private LineRenderer cohesionLineRenderer;

    void Awake()
    {
        boidMovement = GetComponent<BoidMovement3D>();

        // Création de l'indicateur de cercle comme enfant
        circleObject = new GameObject("NeighborRadiusCircle");
        circleObject.transform.SetParent(transform, false); // En local autour du boid
        circleLineRenderer = circleObject.AddComponent<LineRenderer>();
        circleLineRenderer.loop = true;
        circleLineRenderer.useWorldSpace = false;
        circleLineRenderer.startWidth = circleLineWidth;
        circleLineRenderer.endWidth = circleLineWidth;
        circleLineRenderer.positionCount = circleSegments;
        // On utilise le shader "Sprites/Default" pour la transparence
        Material circleMat = new Material(Shader.Find("Sprites/Default"));
        circleMat.color = circleColor;
        circleLineRenderer.material = circleMat;

        // Création de l'indicateur de cohésion comme enfant
        cohesionLineObject = new GameObject("CohesionLine");
        cohesionLineObject.transform.SetParent(transform, false);
        cohesionLineRenderer = cohesionLineObject.AddComponent<LineRenderer>();
        cohesionLineRenderer.useWorldSpace = true; // Positionnement en coordonnées mondiales
        cohesionLineRenderer.startWidth = cohesionLineWidth;
        cohesionLineRenderer.endWidth = cohesionLineWidth;
        cohesionLineRenderer.positionCount = 2;
        Material cohesionMat = new Material(Shader.Find("Sprites/Default"));
        cohesionMat.color = cohesionLineColor;
        cohesionLineRenderer.material = cohesionMat;
    }

    void Update()
    {
        UpdateCircle();
        UpdateCohesionLine();
    }

    // Met à jour le cercle affichant le neighborRadius (dessiné sur le plan XZ)
    void UpdateCircle()
    {
        float radius = boidMovement.neighborRadius;
        float angle = 0f;
        for (int i = 0; i < circleSegments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            circleLineRenderer.SetPosition(i, new Vector3(x, 0, z));
            angle += 360f / circleSegments;
        }
    }

    // Met à jour la ligne reliant le boid au centre de masse de ses voisins
    void UpdateCohesionLine()
    {
        List<BoidMovement3D> neighbors = boidMovement.GetNeighbors();
        if (neighbors.Count > 0)
        {
            Vector3 center = Vector3.zero;
            foreach (var neighbor in neighbors)
            {
                center += neighbor.transform.position;
            }
            center /= neighbors.Count;
            center.y = transform.position.y; 
            cohesionLineRenderer.enabled = true;
            cohesionLineRenderer.SetPosition(0, transform.position);
            cohesionLineRenderer.SetPosition(1, center);
        }
        else
        {
            cohesionLineRenderer.enabled = false;
        }
    }
}