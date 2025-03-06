using UnityEngine;

public class CameraMovements : MonoBehaviour
{
    [Header("Déplacement clavier")]
    public float moveSpeed = 10f;       // Vitesse de déplacement via ZQSD

    [Header("Zoom")]
    public float zoomSpeed = 20f;       // Vitesse de zoom via la molette
    public float minZoomDistance = 5f;  // Distance minimale de zoom
    public float maxZoomDistance = 50f; // Distance maximale de zoom

    [Header("Panning par drag")]
    public float dragSpeed = 0.5f;      // Vitesse de déplacement lors du drag (panning)

    [Header("Rotation par clic droit")]
    public float rotationSpeed = 5f;    // Vitesse de rotation lors du clic droit

    private Vector3 dragOrigin;         // Point de départ du drag
    private bool isDragging = false;    // Indique si le drag est en cours

    private float yaw = 0f;             // Angle de rotation sur l'axe Y
    private float pitch = 0f;           // Angle de rotation sur l'axe X

    void Start()
    {
        // On initialise les angles de rotation actuels à partir de l'orientation de la caméra
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        // 1. Déplacement clavier (Z, Q, S, D)
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            move += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            move += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            move += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            move += Vector3.right;
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        // 2. Zoom avec la molette de la souris
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // On déplace la caméra le long de son axe forward
            Vector3 zoom = transform.forward * scroll * zoomSpeed;
            Vector3 newPosition = transform.position + zoom;
            // Optionnel : limiter le zoom en fonction de la distance à l'origine
            float distance = Vector3.Distance(newPosition, Vector3.zero);
            if (distance >= minZoomDistance && distance <= maxZoomDistance)
            {
                transform.position = newPosition;
            }
        }

        // 3. Panning par drag (clic central)
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }
        if (isDragging)
        {
            Vector3 difference = Input.mousePosition - dragOrigin;
            // On déplace la caméra en espace monde dans la direction opposée au mouvement de la souris
            Vector3 pan = new Vector3(-difference.x, 0, -difference.y) * dragSpeed * Time.deltaTime;
            transform.Translate(pan, Space.World);
            dragOrigin = Input.mousePosition;
        }

        // 4. Rotation par clic droit
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            // Limiter l'angle de pitch pour éviter un retournement de la caméra
            pitch = Mathf.Clamp(pitch, -89f, 89f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }
    }
}