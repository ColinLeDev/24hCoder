using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.EventSystems;

public class SpawnObjectOnClick : MonoBehaviour, IPointerClickHandler
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
            root.RegisterCallback<PointerDownEvent>(evt => isPointerOverUI = true);
            root.RegisterCallback<PointerUpEvent>(evt => isPointerOverUI = false);

        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic gauche
        {
            if (true)
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
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        Debug.Log(name + " Game Object Clicked!");
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