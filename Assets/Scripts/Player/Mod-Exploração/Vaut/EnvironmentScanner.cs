using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    public float checkDistance = 1.5f;
    public LayerMask obstacleLayer;
    public Transform checkPoint;

    void OnDrawGizmos()
    {
        if (checkPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(checkPoint.position, checkPoint.position + transform.forward * checkDistance);
        }
    }
}