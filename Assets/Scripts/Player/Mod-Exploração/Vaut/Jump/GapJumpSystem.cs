using UnityEngine;
using System.Collections;

public class GapJumpSystem : MonoBehaviour
{
    [Header("Gap Jump Settings")]
    public float jumpCheckDistance = 2.5f;
    public float jumpDuration = 0.6f;
    public float jumpForwardOffset = 0.4f;
    public float jumpArcHeight = 1.2f;
    public LayerMask jumpLayer;
    public string jumpAnimationTrigger = "JumpToNext";

    private Animator animator;
    private CharacterController controller;
    private PlayerController playerController;
    private bool isJumping = false;
    public bool IsJumping => isJumping;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isJumping) return;

        if (Input.GetKeyDown(KeyCode.Space)) // ou teu input custom
        {
            TryGapJump();
        }
    }

    void TryGapJump()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, jumpCheckDistance, jumpLayer))
        {
            Debug.Log("[GAP JUMP] Próxima caixa detectada.");
            StartCoroutine(PerformGapJump(hit));
        }
        else
        {
            Debug.Log("[GAP JUMP] Nenhuma caixa detectada.");
        }
    }

    IEnumerator PerformGapJump(RaycastHit hit)
    {
        isJumping = true;

        if (playerController != null)
            playerController.disableLedgeCheck = true;

        animator.SetTrigger(jumpAnimationTrigger);
        GetComponent<PlayerController>().enabled = false;

        Vector3 start = transform.position;

        Vector3 end = hit.collider.bounds.center;
        end.y = hit.collider.bounds.max.y;
        end += transform.forward * jumpForwardOffset;

        if (Physics.Raycast(end + Vector3.up, Vector3.down, out RaycastHit groundHit, 2f, jumpLayer))
        {
            end.y = groundHit.point.y;
        }

        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            float t = elapsed / jumpDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);

            // Interpolação Horizontal
            Vector3 horizontalPos = Vector3.Lerp(start, end, smoothT);

            // Interpolação Vertical (Arco)
            float arc = Mathf.Sin(smoothT * Mathf.PI) * jumpArcHeight;
            horizontalPos.y += arc;

            transform.position = horizontalPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        Debug.Log("[GAP JUMP] Chegou na próxima caixa.");

        GetComponent<PlayerController>().enabled = true;
        isJumping = false;

        if (playerController != null)
            playerController.disableLedgeCheck = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + transform.forward * jumpCheckDistance);
        Gizmos.DrawSphere(origin + transform.forward * jumpCheckDistance, 0.05f);
    }
}
