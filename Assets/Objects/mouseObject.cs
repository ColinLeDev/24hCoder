using UnityEngine;
using UnityEngine.UIElements;

public class SpawnObjectOnClick : MonoBehaviour
{
    public GameObject objectToSpawn;
    public LayerMask groundLayer;
    private UIDocument uiDocument;
    private bool isPointerOverUI;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();

        if (uiDocument != null)
        {
            var root = uiDocument.rootVisualElement;

            // Ajouter un gestionnaire d'événement de pointer (souris)
            root.RegisterCallback<PointerEnterEvent>(evt => isPointerOverUI = true);
            root.RegisterCallback<PointerLeaveEvent>(evt => isPointerOverUI = false);
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic gauche
        {
            if (!isPointerOverUI)
            {
                Vector3 spawnPosition = GetMouseWorldPosition();
                if (spawnPosition != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.Euler(0, 0, 0); // Appliquer une rotation de -90° sur X
                    Instantiate(objectToSpawn, spawnPosition + new Vector3(0, 5, 0), rotation);
                }
                Debug.Log("Clic en dehors de l'UI !");
            }
            else
            {
                Debug.Log("Clic sur l'UI !");
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


