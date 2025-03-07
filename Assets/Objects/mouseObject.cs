using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class SpawnObjectOnClick : MonoBehaviour
{
    public GameObject objectToSpawn;
    public LayerMask groundLayer;
    public UIDocument uiDocument;
    private Slider speedSlider, neighborRadiusSlider, separationDistanceSlider;
    private Slider alignmentWeightSlider, cohesionWeightSlider, separationWeightSlider;
    private Toggle distanceInfluenceToggle;

    private VisualElement draggableWindow;
    public float speed = 1.5f;
    public float neighborRadius = 2f;
    public float separationDistance = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1.5f;
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
        distanceInfluenceToggle = root.Q<Toggle>("distanceInfluenceToggle");

        // Initialisation des valeurs
        speedSlider.value = speed;
        neighborRadiusSlider.value = neighborRadius;
        separationDistanceSlider.value = separationDistance;
        alignmentWeightSlider.value = alignmentWeight;
        cohesionWeightSlider.value = cohesionWeight;
        separationWeightSlider.value = separationWeight;
        distanceInfluenceToggle.value = IsDistanceInfluence;

        // Gestion des événements pour mettre à jour les valeurs
        speedSlider.RegisterValueChangedCallback(evt => speed = evt.newValue);
        neighborRadiusSlider.RegisterValueChangedCallback(evt => neighborRadius = evt.newValue);
        separationDistanceSlider.RegisterValueChangedCallback(evt => separationDistance = evt.newValue);
        alignmentWeightSlider.RegisterValueChangedCallback(evt => alignmentWeight = evt.newValue);
        cohesionWeightSlider.RegisterValueChangedCallback(evt => cohesionWeight = evt.newValue);
        separationWeightSlider.RegisterValueChangedCallback(evt => separationWeight = evt.newValue);
        distanceInfluenceToggle.RegisterValueChangedCallback(evt => IsDistanceInfluence = evt.newValue);

        // Ajout du drag
        // draggableWindow.RegisterCallback<PointerDownEvent>(OnPointerDown);
        // draggableWindow.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        // draggableWindow.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }
    private void OnGlobalClick(PointerDownEvent evt)
    {
        Debug.Log("Clic détecté à la position : " + evt.position);
        if (draggableWindow.worldBound.Contains(evt.position))
        {
            Debug.Log("Click sur la fenêtre");
        }
        else
        {
            Debug.Log("Click global");
            Vector3 spawnPosition = GetMouseWorldPosition();
            if (spawnPosition != Vector3.zero)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, 0); // Appliquer une rotation de -90° sur X
                Instantiate(objectToSpawn, spawnPosition + new Vector3(0, 5, 0), rotation);
            }
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic gauche
        {
            if (EventSystem.current.IsPointerOverGameObject()) // Vérifie si le clic est sur un élément UI
            {
                Debug.Log("Clic sur un élément UI");
            }
            else
            {
                Debug.Log("Clic en dehors de l'UI");
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