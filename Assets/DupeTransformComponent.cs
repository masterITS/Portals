using UnityEngine;

public class DupeTransformComponent : MonoBehaviour
{

    public Transform target; // The target GameObject to copy from

    private void Update()
    {
        if (!target) return;
        // Copy position and rotation from the target
        transform.position = target.position;
        transform.rotation = target.rotation;

    }
}