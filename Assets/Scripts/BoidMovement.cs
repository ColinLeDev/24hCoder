using UnityEngine;
using System.Collections.Generic;

public class BoidMovement3D : MonoBehaviour
{
    public float speed = 1.5f;
    public float neighborRadius = 2f;
    public float separationDistance = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1.5f;

    public bool IsDistanceInfluence = true;
    public Vector3 velocity;

    void Start()
    {
        // Initialisation de la vélocité dans la direction "forward"
        velocity = transform.forward * speed;
    }
List<BoidMovement3D> GetNeighbors()
{
    List<BoidMovement3D> neighbors = new List<BoidMovement3D>();
    foreach (var boid in FindObjectsOfType<BoidMovement3D>())
    {
        if (boid != this && Vector3.Distance(transform.position, boid.transform.position) < neighborRadius)
            neighbors.Add(boid);
    }
    return neighbors;
}

    void Update()
{
    Vector3 acceleration = Vector3.zero;

    List<BoidMovement3D> neighbors = GetNeighbors();
    List<GameObject> obstacles = GetObstacles();

    Vector3 alignment = ComputeAlignment(neighbors);
    Vector3 cohesion = ComputeCohesion(neighbors);
    Vector3 separation = ComputeSeparation(neighbors);
    Vector3 avoidance = AvoidObstacles(obstacles); // Évitement des obstacles

    acceleration += alignment * alignmentWeight;
    acceleration += cohesion * cohesionWeight;
    acceleration += separation * separationWeight;
    acceleration += avoidance * separationWeight; // Appliquer la force d'évitement

    velocity += acceleration * Time.deltaTime;
    velocity = new Vector3(velocity.x, 0, velocity.z).normalized * speed;

    transform.position += velocity * Time.deltaTime;

    if (velocity.sqrMagnitude > 0.001f)
    {
        transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
    }
}


    List<GameObject> GetObstacles()
{
    List<GameObject> obstacles = new List<GameObject>();
    foreach (var obj in GameObject.FindGameObjectsWithTag("obstacle"))
    {
        if (Vector3.Distance(transform.position, obj.transform.position) < neighborRadius)
        {
            obstacles.Add(obj);
        }
    }
    return obstacles;
}
Vector3 AvoidObstacles(List<GameObject> obstacles)
{
    Vector3 avoidanceForce = Vector3.zero;

    foreach (var obj in obstacles)
    {
        float distance = Vector3.Distance(transform.position, obj.transform.position);
        if (distance < separationDistance) // Si trop proche
        {
            Vector3 awayFromObstacle = (transform.position - obj.transform.position).normalized / distance;
            avoidanceForce += awayFromObstacle;
        }
    }

    return new Vector3(avoidanceForce.x, 0, avoidanceForce.z).normalized;
}


    Vector3 ComputeAlignment(List<BoidMovement3D> neighbors)
    {
        if (neighbors.Count == 0)
            return Vector3.zero;

        Vector3 avgVelocity = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            float distance = Vector3.Distance(transform.position, neighbor.transform.position);
            avgVelocity += IsDistanceInfluence ? neighbor.velocity / distance : neighbor.velocity;
        }
        return new Vector3(avgVelocity.x, 0, avgVelocity.z).normalized;
    }

    Vector3 ComputeCohesion(List<BoidMovement3D> neighbors)
    {
        if (neighbors.Count == 0)
            return Vector3.zero;

        Vector3 center = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            float distance = Vector3.Distance(transform.position, neighbor.transform.position);
            center += IsDistanceInfluence ? (neighbor.transform.position / distance) : neighbor.transform.position;
        }
        Vector3 desiredPosition = (center / neighbors.Count) - transform.position;
        return new Vector3(desiredPosition.x, 0, desiredPosition.z).normalized;
    }

    Vector3 ComputeSeparation(List<BoidMovement3D> neighbors)
    {
        Vector3 separation = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            if (Vector3.Distance(transform.position, neighbor.transform.position) < separationDistance)
            {
                float distance = Vector3.Distance(transform.position, neighbor.transform.position);
                separation += IsDistanceInfluence ? (transform.position - neighbor.transform.position) / distance : (transform.position - neighbor.transform.position);
            }
        }
        return new Vector3(separation.x, 0, separation.z).normalized;
    }
}