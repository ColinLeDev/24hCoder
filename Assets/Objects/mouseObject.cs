using UnityEngine;

public class SpawnObjectOnClick : MonoBehaviour
{
    public GameObject objectToSpawn;
    public LayerMask groundLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic gauche
        {
            Vector3 spawnPosition = GetMouseWorldPosition();
            if (spawnPosition != Vector3.zero)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, 0); // Appliquer une rotation de -90° sur X
                Instantiate(objectToSpawn, spawnPosition + new Vector3(0, 5, 0), rotation);
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
