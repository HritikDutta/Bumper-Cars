using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float followSensitivity = 5f;

    private void Update()
    {
        Vector3 targetPosition = target.position - 4f * target.forward;
        targetPosition.y += 2f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSensitivity * Time.deltaTime);
        transform.LookAt(target);
    }
}
