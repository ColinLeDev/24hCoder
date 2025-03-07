using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using System;

public class SpawnObjectOnClick : MonoBehaviour
{
    public GameObject objectToSpawn;
    public LayerMask groundLayer;
    public UIDocument uiDocument;
    private Slider speedSlider, neighborRadiusSlider, separationDistanceSlider;
    private Slider alignmentWeightSlider, cohesionWeightSlider, separationWeightSlider;
    private Slider obsAvoidanceDistanceSlider, safeDistanceSlider, obsAvoidanceWeightSlider,obsRepusionCoeffSlider, turnSpeedSlider;
    private Toggle distanceInfluenceToggle, toggleNeighborRadius, toggleCohesionLines, toggleModeObstacle;

    private VisualElement draggableWindow;
    private Vector2 dragOffset;
    private bool isDragging = false;
    public bool canSpawn = true;
    public float speed = 1.5f;
    public float neighborRadius = 2f;
    public float separationDistance = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1.5f;
    public float obstacleAvoidanceDistance = 10f;
    public float safeDistance = 0.5f;
    public float obstacleAvoidanceWeight = 2f;
    public float obstacleRepulsionCoefficient = 100f;
    public float turnSpeed = 360f;
    public bool cohesionLines = false;
    public bool showNeighborRadius = false;
    public bool modeObstacle = false;
    public bool IsDistanceInfluence = true;
    private void OnEnable()
    {
        // uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        while (root.parent != null)
        {
            root = root.parent;
        }
        Debug.Log(root);

        // Récupération des éléments
        draggableWindow = root.Q<VisualElement>("DraggableWindow");

        // Ajout de la détection des clics globaux
        // root.RegisterCallback<PointerDownEvent>(OnGlobalClick);

        speedSlider = root.Q<Slider>("speedSlider");
        neighborRadiusSlider = root.Q<Slider>("neighborRadiusSlider");
        separationDistanceSlider = root.Q<Slider>("separationDistanceSlider");
        alignmentWeightSlider = root.Q<Slider>("alignmentWeightSlider");
        cohesionWeightSlider = root.Q<Slider>("cohesionWeightSlider");
        separationWeightSlider = root.Q<Slider>("separationWeightSlider");
        obsAvoidanceDistanceSlider = root.Q<Slider>("obsAvoidanceDistanceSlider");
        safeDistanceSlider = root.Q<Slider>("safeDistanceSlider");
        obsAvoidanceWeightSlider = root.Q<Slider>("obsAvoidanceWeightSlider");
        obsRepusionCoeffSlider = root.Q<Slider>("obsRepusionCoeffSlider");
        turnSpeedSlider = root.Q<Slider>("turnSpeedSlider");
        // toggleNeighborRadius = root.Q<Toggle>("toggleNeighborRadius");
        // toggleCohesionLines = root.Q<Toggle>("toggleCohesionLines");
        toggleModeObstacle = root.Q<Toggle>("toggleModeObstacle");
        distanceInfluenceToggle = root.Q<Toggle>("distanceInfluenceToggle");

        // Initialisation des valeurs
        speedSlider.value = speed;
        neighborRadiusSlider.value = neighborRadius;
        separationDistanceSlider.value = separationDistance;
        alignmentWeightSlider.value = alignmentWeight;
        cohesionWeightSlider.value = cohesionWeight;
        separationWeightSlider.value = separationWeight;
        obsAvoidanceDistanceSlider.value = obstacleAvoidanceDistance;
        safeDistanceSlider.value = safeDistance;
        obsAvoidanceWeightSlider.value = obstacleAvoidanceWeight;
        obsRepusionCoeffSlider.value = obstacleRepulsionCoefficient;
        turnSpeedSlider.value = turnSpeed;
        // toggleNeighborRadius.value = showNeighborRadius;
        // toggleCohesionLines.value = cohesionLines;
        toggleModeObstacle.value = modeObstacle;
        distanceInfluenceToggle.value = IsDistanceInfluence;

        // Gestion des événements pour mettre à jour les valeurs
        speedSlider.RegisterValueChangedCallback(evt => speed = evt.newValue);
        neighborRadiusSlider.RegisterValueChangedCallback(evt => neighborRadius = evt.newValue);
        separationDistanceSlider.RegisterValueChangedCallback(evt => separationDistance = evt.newValue);
        alignmentWeightSlider.RegisterValueChangedCallback(evt => alignmentWeight = evt.newValue);
        cohesionWeightSlider.RegisterValueChangedCallback(evt => cohesionWeight = evt.newValue);
        separationWeightSlider.RegisterValueChangedCallback(evt => separationWeight = evt.newValue);
        obsAvoidanceDistanceSlider.RegisterValueChangedCallback(evt => obstacleAvoidanceDistance = evt.newValue);
        safeDistanceSlider.RegisterValueChangedCallback(evt => safeDistance = evt.newValue);
        obsAvoidanceWeightSlider.RegisterValueChangedCallback(evt => obstacleAvoidanceWeight = evt.newValue);
        obsRepusionCoeffSlider.RegisterValueChangedCallback(evt => obstacleRepulsionCoefficient = evt.newValue);
        turnSpeedSlider.RegisterValueChangedCallback(evt => turnSpeed = evt.newValue);
        // toggleNeighborRadius.RegisterValueChangedCallback(evt => showNeighborRadius = evt.newValue);
        // toggleCohesionLines.RegisterValueChangedCallback(evt => cohesionLines = evt.newValue);
        toggleModeObstacle.RegisterValueChangedCallback(evt => modeObstacle = evt.newValue);
        distanceInfluenceToggle.RegisterValueChangedCallback(evt => IsDistanceInfluence = evt.newValue);

        // Ajout du drag
        draggableWindow.RegisterCallback<PointerDownEvent>(OnPointerDown);
        draggableWindow.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        draggableWindow.RegisterCallback<PointerUpEvent>(OnPointerUp);

        // Ajout de la détection de l'entrée dans l'objet (hover)
        draggableWindow.RegisterCallback<MouseEnterEvent>(evt => canSpawn = false);
        draggableWindow.RegisterCallback<MouseLeaveEvent>(evt => canSpawn = true);
    }
    private void OnPointerDown(PointerDownEvent evt)
    {
        isDragging = true;
        dragOffset = (Vector2)evt.position - draggableWindow.layout.position;
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (isDragging)
        {
            draggableWindow.style.left = evt.position.x - dragOffset.x;
            draggableWindow.style.top = evt.position.y - dragOffset.y;
        }
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        isDragging = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canSpawn && !modeObstacle) // Clic gauche
        {
            Vector3 spawnPosition = GetMouseWorldPosition();
            if (spawnPosition != Vector3.zero)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, 0); // Appliquer une rotation de -90° sur X

                var newObj = Instantiate(objectToSpawn, spawnPosition + new Vector3(0, 5, 0), rotation);
                BoidMovement3D boidMovement = newObj.GetComponent<BoidMovement3D>();
                if (boidMovement != null)
                {
                    boidMovement.speed = speed;
                    boidMovement.neighborRadius = neighborRadius;
                    boidMovement.separationDistance = separationDistance;
                    boidMovement.alignmentWeight = alignmentWeight;
                    boidMovement.cohesionWeight = cohesionWeight;
                    boidMovement.separationWeight = separationWeight;
                    boidMovement.IsDistanceInfluence = IsDistanceInfluence;
                    boidMovement.obstacleAvoidanceDistance = obstacleAvoidanceDistance;
                    boidMovement.safeDistance = safeDistance;
                    boidMovement.obstacleAvoidanceWeight = obstacleAvoidanceWeight;
                    boidMovement.obstacleRepulsionCoefficient = obstacleRepulsionCoefficient;
                    boidMovement.turnSpeed = turnSpeed;
                }
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}