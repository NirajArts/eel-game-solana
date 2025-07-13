using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    public Vector3 smoothSpeed = new Vector3(5f, 5f, 5f);
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;

    void Start()
    {
        if (target)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (!target) return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = currentPosition;
        if (followX) newPosition.x = Mathf.Lerp(currentPosition.x, desiredPosition.x, smoothSpeed.x * Time.deltaTime);
        if (followY) newPosition.y = Mathf.Lerp(currentPosition.y, desiredPosition.y, smoothSpeed.y * Time.deltaTime);
        if (followZ) newPosition.z = Mathf.Lerp(currentPosition.z, desiredPosition.z, smoothSpeed.z * Time.deltaTime);
        transform.position = newPosition;
    }
}
