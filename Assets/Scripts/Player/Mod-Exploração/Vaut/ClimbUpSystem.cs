using UnityEngine;

public class ClimbUpSystem : MonoBehaviour
{
    public Transform checkPoint;
    public float checkDistance = 1.5f;
    public float moveSpeed = 2.5f;
    public LayerMask obstacleLayer;
    public Animator animator;
    public CharacterController controller;

    private bool isClimbing = false;
    private bool startMoving = false;
    private Vector3 targetPos;
    private RaycastHit lastObstacleHit;

    void Update()
    {
        if (isClimbing && startMoving)
        {
            PerformMove();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryClimbUp();
        }
    }

    void TryClimbUp()
    {
        if (Physics.Raycast(checkPoint.position, transform.forward, out RaycastHit hit, checkDistance, obstacleLayer))
        {
            lastObstacleHit = hit;
            targetPos = hit.point + transform.forward * 0.7f;
            targetPos.y = transform.position.y + 1.5f;

            StartClimbUp();
        }
    }

    void StartClimbUp()
    {
        isClimbing = true;
        startMoving = false;
        controller.enabled = false;

        animator.SetBool("IsVaulting", true);
        animator.SetTrigger("ClimbUp");
    }

    public void OnClimbUpStartMove()
    {
        startMoving = true;
    }

    public void OnClimbUpLand()
    {
        Vector3 snapPos = targetPos;
        snapPos.y = lastObstacleHit.point.y;

        transform.position = snapPos;
        EndClimbUp();
    }

    void PerformMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    void EndClimbUp()
    {
        isClimbing = false;
        startMoving = false;
        controller.enabled = true;

        animator.SetBool("IsVaulting", false);
    }
}