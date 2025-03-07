using UnityEngine;

public class WrapAround : MonoBehaviour
{
    public GameObject ground;  // Référence au GameObject Sol dans la scène
    private Transform leftWall;
    private Transform rightWall;
    private Transform topWall;
    private Transform bottomWall;
    private float groundWidth;
    private float groundHeight;

    void Start()
    {
        // Vérifiez si le sol est assigné
        if (ground != null)
        {
            // Référencer les murs à partir du GameObject Ground
            leftWall = ground.transform.Find("LeftWall");
            rightWall = ground.transform.Find("RightWall");
            topWall = ground.transform.Find("TopWall");
            bottomWall = ground.transform.Find("BottomWall");

            // Vérifier si tous les murs ont été trouvés
            if (leftWall == null || rightWall == null || topWall == null || bottomWall == null)
            {
                Debug.LogError("Un ou plusieurs murs sont manquants sous Ground !");
            }

            // Calculer la largeur et la hauteur de la zone de jeu
            groundWidth = Mathf.Abs(rightWall.position.x - leftWall.position.x);
            groundHeight = Mathf.Abs(topWall.position.z - bottomWall.position.z);
        }
        else
        {
            Debug.LogError("Le GameObject Sol (Ground) n'est pas assigné !");
        }
    }

    void Update()
    {
        Vector3 pos = transform.position;

        // Vérifie si l'objet dépasse les limites sur l'axe X (gauche/droite)
        if (pos.x > groundWidth / 2)
        {
            pos.x = -groundWidth / 2;  // Téléportation à gauche
            Debug.Log("Sortie à droite → Téléporté à gauche");
        }
        else if (pos.x < -groundWidth / 2)
        {
            pos.x = groundWidth / 2;  // Téléportation à droite
            Debug.Log("Sortie à gauche → Téléporté à droite");
        }

        // Vérifie si l'objet dépasse les limites sur l'axe Z (haut/bas)
        if (pos.z > groundHeight / 2)
        {
            pos.z = -groundHeight / 2;  // Téléportation en bas
            Debug.Log("Sortie en haut → Téléporté en bas");
        }
        else if (pos.z < -groundHeight / 2)
        {
            pos.z = groundHeight / 2;  // Téléportation en haut
            Debug.Log("Sortie en bas → Téléporté en haut");
        }

        // Applique la nouvelle position après téléportation
        transform.position = pos;
    }
}
