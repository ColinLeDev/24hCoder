using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject boidPrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic gauche
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            Instantiate(boidPrefab, mousePosition, Quaternion.identity);
        }
    }
}