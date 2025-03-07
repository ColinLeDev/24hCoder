using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    public GameObject objectToFollow; // L'objet qui suit la souris
    public GameObject objectPrefab;   // L'objet à cloner lors du clic
    public LayerMask groundLayer;     // Le layer du sol
    private Material objectMaterial;  // Matériau de l'objet suivi
        void Start()
    {
        // Récupère une copie du matériau pour éviter les conflits
        objectMaterial = objectToFollow.GetComponent<Renderer>().material;
        SetTransparency(objectMaterial, true); // Rendre l'objet transparent au début
    }
void Update()
{
    // Déplacer l'objet pour suivre la souris
    MoveObjectToMousePosition();

    // Cloner l'objet au clic gauche
    if (Input.GetMouseButtonDown(0))
    {
        GameObject newClone = Instantiate(objectPrefab, objectToFollow.transform.position, objectToFollow.transform.rotation);
        newClone.tag = "obstacle"; // Assigne le tag "Obstacle" au clone
        // Applique le même scale que l'objet qui suit la souris
    newClone.transform.localScale = objectToFollow.transform.localScale;
        Material cloneMaterial = newClone.GetComponent<Renderer>().material;
        SetTransparency(cloneMaterial, false);
    }

    // Rotation de l'objet à 45° en appuyant sur "R"
    if (Input.GetKeyDown(KeyCode.R))
    {
        RotateObject();
    }
}

void RotateObject()
{
    objectToFollow.transform.Rotate(0, 45, 0); // Rotation de 45° sur Y
}

    void MoveObjectToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = objectToFollow.transform.position.y; // Garde la hauteur de l'objet
            objectToFollow.transform.position = targetPosition;
        }
    }

    void CloneObject()
    {
        GameObject newClone = Instantiate(objectPrefab, objectToFollow.transform.position, objectToFollow.transform.rotation);
        newClone.tag = "Obstacle"; // Ajoute le tag "Obstacle"
        Material cloneMaterial = newClone.GetComponent<Renderer>().material;
        SetTransparency(cloneMaterial, false);
    }
   void SetTransparency(Material material, bool transparent)
    {
        Color color = material.color;
        if (transparent)
        {
            color.a = 0.5f; // Semi-transparent
            material.color = color;
            material.SetFloat("_Mode", 3); // Mode Transparent
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
        else
        {
            color.a = 1f; // Opaque
            material.color = color;
            material.SetFloat("_Mode", 0); // Mode Opaque
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
        }
    }
}
