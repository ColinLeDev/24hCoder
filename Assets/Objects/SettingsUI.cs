using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
  public GameObject objectToSpawn;
  public LayerMask groundLayer;
  private UIDocument uiDocument;

  private VisualElement draggableWindow;
  private Vector2 dragOffset;
  private bool isDragging = false;

  private Slider speedSlider, neighborRadiusSlider, separationDistanceSlider;
  private Slider alignmentWeightSlider, cohesionWeightSlider, separationWeightSlider;
  private Toggle distanceInfluenceToggle;

  public float speed = 1.5f;
  public float neighborRadius = 2f;
  public float separationDistance = 1f;
  public float alignmentWeight = 1f;
  public float cohesionWeight = 1f;
  public float separationWeight = 1.5f;
  public bool IsDistanceInfluence = true;

  private void OnEnable()
  {
    var root = uiDocument.rootVisualElement;

    // Récupération des éléments
    draggableWindow = root.Q<VisualElement>("DraggableWindow");

    // Ajout de la détection des clics globaux
    root.RegisterCallback<PointerDownEvent>(OnGlobalClick);

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
    draggableWindow.RegisterCallback<PointerDownEvent>(OnPointerDown);
    draggableWindow.RegisterCallback<PointerMoveEvent>(OnPointerMove);
    draggableWindow.RegisterCallback<PointerUpEvent>(OnPointerUp);
  }

  // Fonction appelée à chaque clic global
  private void OnGlobalClick(PointerDownEvent evt)
  {
    // Vérifie si le clic est hors de la fenêtre draggable
    if (!draggableWindow.worldBound.Contains(evt.position))
    {
      Debug.Log("Clic en dehors de la fenêtre UI !");

      Vector3 spawnPosition = GetMouseWorldPosition();
      if (spawnPosition != Vector3.zero)
      {
        Quaternion rotation = Quaternion.Euler(0, 0, 0); // Appliquer une rotation de -90° sur X
        Instantiate(objectToSpawn, spawnPosition + new Vector3(0, 5, 0), rotation);
      }
      // Tu peux cacher la fenêtre ou exécuter une action ici
    }
    else // Clic dans la fenêtre draggable
    {
      Debug.Log("Clic sur la fenêtre UI !");
    }
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
