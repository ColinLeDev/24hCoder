using UnityEngine;

public class KeepHeight : MonoBehaviour
{
    public float targetHeight = 5f;

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = targetHeight;
        transform.position = pos;
    }
}