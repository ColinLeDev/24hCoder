using UnityEngine;
using System.Collections.Generic;

public class BoidMovement3D : MonoBehaviour
{
    [Header("Paramètres de base")]
    public float speed = 1.5f;
    public float neighborRadius = 2f;
    public float separationDistance = 1f;
    
    [Header("Poids des comportements")]
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1.5f;
    
    [Header("Évitement d'obstacles")]
    public float obstacleAvoidanceDistance = 10f; // Rayon de détection des obstacles
    public float safeDistance = 0.5f;             // Distance minimale autorisée entre les surfaces
    public float obstacleAvoidanceWeight = 2f;    // Poids global de la force d'évitement
    public float obstacleRepulsionCoefficient = 100f; // Coefficient de force d'urgence (ajustez pour augmenter la puissance d'évitement)

    [Header("Rotation")]
    public float turnSpeed = 360f; // Vitesse de rotation en degrés par seconde
    
    [Header("Steering")]
    public float maxAcceleration = 5f; // Accélération maximale pour un steering naturel

    public bool IsDistanceInfluence = true;
    public Vector3 velocity;

    private float y;

    void Start()
    {
        // Initialisation avec la direction forward du transform
        velocity = transform.forward * speed;

        y = transform.position.y;
    }

    // Récupère les voisins dans un rayon défini
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

    // Récupère les obstacles (objets tagués "obstacle") dont la distance effective est inférieure à obstacleAvoidanceDistance
    List<GameObject> GetObstacles()
    {
        List<GameObject> obstacles = new List<GameObject>();
        Collider boidCollider = GetComponent<Collider>();
        float boidSize = (boidCollider != null) ? boidCollider.bounds.extents.magnitude : 0f;
        
        foreach (var obj in GameObject.FindGameObjectsWithTag("obstacle"))
        {
            Collider col = obj.GetComponent<Collider>();
            if (col != null)
            {
                // On calcule le point le plus proche sur la surface de l'obstacle
                Vector3 closestPoint = col.ClosestPoint(transform.position);
                float distance = Vector3.Distance(transform.position, closestPoint);
                // La distance effective entre la surface du boid et celle de l'obstacle
                float effectiveDistance = distance - boidSize;
                // Debug.Log("Distance effective à " + obj.name + " : " + effectiveDistance);
                if (effectiveDistance < obstacleAvoidanceDistance)
                {
                    obstacles.Add(obj);
                }
            }
        }
        return obstacles;
    }

    // Calcule la force d'évitement en fonction de la distance effective à chaque obstacle
    Vector3 AvoidObstacles(List<GameObject> obstacles)
    {
        Vector3 totalAvoidance = Vector3.zero;
        Collider boidCollider = GetComponent<Collider>();
        float boidSize = (boidCollider != null) ? boidCollider.bounds.extents.magnitude : 0f;

        foreach (var obj in obstacles)
        {
            Collider col = obj.GetComponent<Collider>();
            if (col != null)
            {
                Vector3 closestPoint = col.ClosestPoint(transform.position);
                Vector3 diff = transform.position - closestPoint;
                float distance = diff.magnitude;
                float effectiveDistance = distance - boidSize;

                if (effectiveDistance < safeDistance && effectiveDistance > 0f)
                {
                    // Zone d'urgence : force repulsive très forte
                    float forceMagnitude = (safeDistance - effectiveDistance) * obstacleRepulsionCoefficient * 20f;
                    totalAvoidance += diff.normalized * forceMagnitude;
                }
                else if (effectiveDistance > 0f && effectiveDistance < obstacleAvoidanceDistance)
                {
                    float forceScale = (obstacleAvoidanceDistance - effectiveDistance) / obstacleAvoidanceDistance;
                    totalAvoidance += diff.normalized * forceScale;
                }
            }
        }
        return totalAvoidance;
    }

    void Update()
{
    // Forcer le y à rester constant (pour rester sur le plan horizontal)
    transform.position = new Vector3(transform.position.x, y, transform.position.z);

    Vector3 acceleration = Vector3.zero;
    Collider boidCollider = GetComponent<Collider>();
    float boidSize = (boidCollider != null) ? boidCollider.bounds.extents.magnitude : 0f;

    List<BoidMovement3D> neighbors = GetNeighbors();
    List<GameObject> obstacles = GetObstacles();

    Vector3 alignment = ComputeAlignment(neighbors);
    Vector3 cohesion = ComputeCohesion(neighbors);
    Vector3 separation = ComputeSeparation(neighbors);
    Vector3 avoidance = AvoidObstacles(obstacles);

    // Détecter si un obstacle est dans la zone d'urgence
    bool emergency = false;
    foreach (var obj in obstacles)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col != null)
        {
            Vector3 closestPoint = col.ClosestPoint(transform.position);
            float effectiveDistance = Vector3.Distance(transform.position, closestPoint) - boidSize;
            if (effectiveDistance < safeDistance)
            {
                emergency = true;
                break;
            }
        }
    }

    if (emergency)
    {
        Vector3 desiredDirection = Vector3.Slerp(velocity.normalized, avoidance.normalized, 0.8f);
        Vector3 desiredVelocity = desiredDirection * speed;
        Vector3 steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxAcceleration);
        acceleration = steering;
    }
    else
    {
        acceleration += alignment * alignmentWeight;
        acceleration += cohesion * cohesionWeight;
        acceleration += separation * separationWeight;
        acceleration += avoidance * obstacleAvoidanceWeight;
        acceleration = Vector3.ClampMagnitude(acceleration, maxAcceleration);
    }

    velocity += acceleration * Time.deltaTime;
    // Contrainte sur le plan XZ
    velocity = new Vector3(velocity.x, 0, velocity.z).normalized * speed;
    transform.position += velocity * Time.deltaTime;

    if (velocity.sqrMagnitude > 0.001f)
    {
        Quaternion targetRotation = Quaternion.LookRotation(velocity, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}

    Vector3 ComputeAlignment(List<BoidMovement3D> neighbors)
    {
        if (neighbors.Count == 0)
            return Vector3.zero;
        Vector3 avgVelocity = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            float d = Vector3.Distance(transform.position, neighbor.transform.position);
            avgVelocity += IsDistanceInfluence ? neighbor.velocity / d : neighbor.velocity;
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
            float d = Vector3.Distance(transform.position, neighbor.transform.position);
            center += IsDistanceInfluence ? neighbor.transform.position / d : neighbor.transform.position;
        }
        Vector3 desired = (center / neighbors.Count) - transform.position;
        return new Vector3(desired.x, 0, desired.z).normalized;
    }

    Vector3 ComputeSeparation(List<BoidMovement3D> neighbors)
    {
        Vector3 separation = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            float d = Vector3.Distance(transform.position, neighbor.transform.position);
            if (d < separationDistance && d > 0f)
            {
                separation += IsDistanceInfluence ? (transform.position - neighbor.transform.position) / d : (transform.position - neighbor.transform.position);
            }
        }
        return new Vector3(separation.x, 0, separation.z).normalized;
    }
}