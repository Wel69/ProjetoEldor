using UnityEngine;

public class LedgeChecker : MonoBehaviour
{
    [Header("Ledge Check Settings")]
    public Transform groundCheckPoint;
    public float ledgeCheckDistance = 0.5f;
    public float raycastLength = 1.5f;
    public LayerMask groundLayer;

    [Header("Debug")]
    public bool showGizmos = true;

    public bool IsOnLedge()
    {
        Vector3 origin = groundCheckPoint.position + transform.forward * ledgeCheckDistance;
        bool hasGround = Physics.Raycast(origin, Vector3.down, raycastLength, groundLayer);

        Debug.DrawRay(origin, Vector3.down * raycastLength, hasGround ? Color.green : Color.red);
        return !hasGround;
    }

    public bool IsNearLedge()
    {
        Vector3[] offsets = {
            transform.forward * ledgeCheckDistance,
            -transform.forward * ledgeCheckDistance,
            transform.right * ledgeCheckDistance,
            -transform.right * ledgeCheckDistance
        };

        foreach (var offset in offsets)
        {
            if (!HasGround(offset))
            {
                Debug.DrawRay(groundCheckPoint.position + offset, Vector3.down * raycastLength, Color.red);
                return true;
            }
        }

        return false;
    }

    private bool HasGround(Vector3 offset)
    {
        Vector3 origin = groundCheckPoint.position + offset;
        bool hit = Physics.Raycast(origin, Vector3.down, raycastLength, groundLayer);
        Debug.DrawRay(origin, Vector3.down * raycastLength, hit ? Color.green : Color.red);
        return hit;
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || groundCheckPoint == null) return;

        Vector3[] offsets = {
            transform.forward * ledgeCheckDistance,
            -transform.forward * ledgeCheckDistance,
            transform.right * ledgeCheckDistance,
            -transform.right * ledgeCheckDistance
        };

        foreach (var offset in offsets)
        {
            Vector3 origin = groundCheckPoint.position + offset;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, origin + Vector3.down * raycastLength);
            Gizmos.DrawSphere(origin, 0.05f);
        }
    }
}