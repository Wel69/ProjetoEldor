using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class StepUpSystem : MonoBehaviour
{
    [Header("StepUp Settings")]
    public float stepUpRange = 0.8f;
    public float maxStepUpHeight = 1.4f;
    public float stepUpDistance = 0.6f;
    public float stepUpDuration = 0.5f;
    public LayerMask stepUpLayer;
    public string stepUpAnimationTrigger = "StepUp";

    [Header("Debug")]
    public bool showGizmos = true;

    private Animator animator;
    private CharacterController controller;
    private PlayerController playerController;
    private bool isStepUp;
    public bool IsStepUp => isStepUp;

    private RaycastHit lastHitObstacle;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isStepUp) return;

        if (Input.GetKeyDown(KeyCode.Space)) // Trocar pro input que você quiser
        {
            TryStepUp();
        }
    }

    void TryStepUp()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, stepUpRange, stepUpLayer))
        {
            float obstacleHeight = hit.point.y - transform.position.y;

            if (obstacleHeight <= maxStepUpHeight)
            {
                StartCoroutine(PerformStepUp(hit));
                return;
            }
        }
    }

    IEnumerator PerformStepUp(RaycastHit hit)
    {
        isStepUp = true;
        lastHitObstacle = hit;

        // Desativa ledge check enquanto faz o StepUp
        if (playerController != null)
            playerController.disableLedgeCheck = true;

        animator.SetTrigger(stepUpAnimationTrigger);
        GetComponent<PlayerController>().enabled = false;

        Vector3 start = transform.position;
        Vector3 end = CalculateTargetPosition(hit);

        float elapsed = 0f;

        while (elapsed < stepUpDuration)
        {
            float t = elapsed / stepUpDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector3.Lerp(start, end, smoothT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        OnStepUpLand();

        GetComponent<PlayerController>().enabled = true;
        isStepUp = false;

        if (playerController != null)
            playerController.disableLedgeCheck = false;
    }

    Vector3 CalculateTargetPosition(RaycastHit hit)
    {
        Collider obstacle = hit.collider;

        Vector3 targetPos = obstacle.bounds.center;
        targetPos.y = obstacle.bounds.max.y; // topo do obstáculo

        // Calcula avanço pra frente mas nunca além do centro da caixa
        Vector3 forwardOffset = transform.forward * stepUpDistance;
        targetPos += forwardOffset;

        // Faz snap no topo correto usando Raycast pra baixo
        if (Physics.Raycast(targetPos + Vector3.up, Vector3.down, out RaycastHit groundHit, 2f, stepUpLayer))
        {
            targetPos.y = groundHit.point.y;
        }

        // 🔥 Checa se tem espaço
        if (Physics.CheckCapsule(targetPos + Vector3.up * 0.5f, targetPos + Vector3.up * 1.8f, 0.3f, stepUpLayer))
        {
            Debug.LogWarning("[STEPUP] Espaço bloqueado no topo, ajustando.");
            // Pega ponto bem na beirada
            targetPos -= forwardOffset * 0.5f;
        }

        return targetPos;
    }

    void OnStepUpLand()
    {
        Debug.Log("[STEPUP] Snap final aplicado.");
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + dir * stepUpRange);
        Gizmos.DrawSphere(origin + dir * stepUpRange, 0.05f);
    }
}
