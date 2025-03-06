using UnityEngine;
using System.Collections.Generic;

public class BoidMovement : MonoBehaviour
{
    public float speed = 1.5f;
    public float neighborRadius = 2f;
    public float separationDistance = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1.5f;

    private Vector2 velocity;

    void Start()
    {
        // Initialisation du mouvement dans la direction du haut du prefab (l'avant)
        velocity = transform.up * speed;
    }

    void Update()
    {
        Vector2 acceleration = Vector2.zero;

        List<BoidMovement> neighbors = GetNeighbors();
        Vector2 alignment = ComputeAlignment(neighbors);
        Vector2 cohesion = ComputeCohesion(neighbors);
        Vector2 separation = ComputeSeparation(neighbors);

        acceleration += alignment * alignmentWeight;
        acceleration += cohesion * cohesionWeight;
        acceleration += separation * separationWeight;

        velocity += acceleration * Time.deltaTime;
        velocity = velocity.normalized * speed;

        // Déplacer le boid dans la direction du mouvement
        transform.position += (Vector3)velocity * Time.deltaTime;

        // Faire tourner le boid pour que le haut (l'avant) suive la direction de déplacement
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    List<BoidMovement> GetNeighbors()
    {
        List<BoidMovement> neighbors = new List<BoidMovement>();
        foreach (var boid in FindObjectsOfType<BoidMovement>())
        {
            if (boid != this && Vector2.Distance(transform.position, boid.transform.position) < neighborRadius)
                neighbors.Add(boid);
        }
        return neighbors;
    }

    Vector2 ComputeAlignment(List<BoidMovement> neighbors)
    {
        if (neighbors.Count == 0) return Vector2.zero;
        Vector2 avgVelocity = Vector2.zero;
        foreach (var neighbor in neighbors)
            avgVelocity += neighbor.velocity;
        return (avgVelocity / neighbors.Count).normalized;
    }

    Vector2 ComputeCohesion(List<BoidMovement> neighbors)
    {
        if (neighbors.Count == 0) return Vector2.zero;
        Vector2 center = Vector2.zero;
        foreach (var neighbor in neighbors)
            center += (Vector2)neighbor.transform.position;
        return ((center / neighbors.Count) - (Vector2)transform.position).normalized;
    }

    Vector2 ComputeSeparation(List<BoidMovement> neighbors)
    {
        Vector2 separation = Vector2.zero;
        foreach (var neighbor in neighbors)
        {
            if (Vector2.Distance(transform.position, neighbor.transform.position) < separationDistance)
                separation += (Vector2)(transform.position - neighbor.transform.position);
        }
        return separation.normalized;
    }
}