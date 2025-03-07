using UnityEngine;

public class RaycastDeleteOnDeleteKey : MonoBehaviour
{
    public float rayLength = 10f; // Longueur du rayon
    public LayerMask coneLayer; // Layer de l'objet Cone
    public LayerMask groundLayer;

    void Update()
    {
        // Vérifie si la touche Suppr est pressée
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            // Get les coordonnées du curseur de la souris
            Vector3 mousePosition = GetMouseWorldPosition();
            mousePosition.y = 5f;
            Debug.Log(mousePosition);
            // Cherche un objet avec un tag "Cone" très proche de la position de la souris, sans ray
            Collider[] colliders = Physics.OverlapSphere(mousePosition, 0.1f, coneLayer);
            if (colliders.Length > 0)
            {
                // Si un objet est trouvé, le détruit
                Destroy(colliders[0].gameObject);
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
