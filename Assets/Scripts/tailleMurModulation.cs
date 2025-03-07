using UnityEngine;

public class CubeScaler : MonoBehaviour
{
    public Vector3 scaleChange = new Vector3(0.1f, 0.1f, 0.1f); // Valeur de changement
    private Vector3 initialScale; // Taille de départ du cube

    void Start()
    {
        initialScale = transform.localScale; // Sauvegarde de la taille initiale
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.T)) // Augmenter la taille
        {
            transform.localScale += scaleChange * Time.deltaTime * 5;
        }
        if (Input.GetKey(KeyCode.G)) // Réduire la taille
        {
            transform.localScale -= scaleChange * Time.deltaTime * 5;
            transform.localScale = new Vector3(
                Mathf.Max(transform.localScale.x, 0.1f), 
                Mathf.Max(transform.localScale.y, 0.1f), 
                Mathf.Max(transform.localScale.z, 0.1f)
            ); // Empêche une taille négative
        }
        if (Input.GetKeyDown(KeyCode.H)) // Réinitialiser la taille
        {
            transform.localScale = initialScale;
        }
    }
}
