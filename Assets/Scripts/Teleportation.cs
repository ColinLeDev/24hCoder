using UnityEngine;

public class ScreenWrap2D : MonoBehaviour
{
    private Camera mainCamera;
    private float screenWidth;
    private float screenHeight;

    void Start()
    {
        mainCamera = Camera.main;
        screenHeight = mainCamera.orthographicSize;
        screenWidth = screenHeight * mainCamera.aspect;
    }

    void Update()
    {
        Vector3 newPosition = transform.position;

        // Vérifie si l'objet dépasse les bords de l'écran et le fait réapparaître de l'autre côté
        if (transform.position.x > screenWidth) 
            newPosition.x = -screenWidth;
        else if (transform.position.x < -screenWidth) 
            newPosition.x = screenWidth;

        if (transform.position.y > screenHeight) 
            newPosition.y = -screenHeight;
        else if (transform.position.y < -screenHeight) 
            newPosition.y = screenHeight;

        transform.position = newPosition;
    }
}
