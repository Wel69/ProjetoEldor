using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class VaultSystem : MonoBehaviour
{
    [Header("Vault Settings")]
    public float vaultRange = 0.8f;
    public float maxVaultHeight = 1.1f;
    public float vaultDistance = 0.6f;
    public float vaultDuration = 1.1f;
    public LayerMask vaultLayer;
    public string vaultAnimationTrigger = "Vault";

    [Header("Target Matching")]
    public AvatarTarget matchTarget = AvatarTarget.RightHand;
    public Vector3 handOffset = new Vector3(0, 0.9f, 0.05f);
    public float matchStart = 0.1f;
    public float matchEnd = 0.9f;
    public bool enableTargetMatching = true;

    private Animator animator;
    private CharacterController controller;
    private bool isVaulting;
    public bool IsVaulting => isVaulting;

    private RaycastHit lastHitObstacle;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isVaulting) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryVault();
        }
    }

    void TryVault()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * vaultRange, Color.red, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, vaultRange, vaultLayer))
        {
            float obstacleHeight = hit.point.y - transform.position.y;

            if (obstacleHeight <= maxVaultHeight)
            {
                Debug.Log("[VAULT] Executando vault");
                StartCoroutine(PerformVault(hit));
                return;
            }
        }

        Debug.Log("[VAULT] Sem vault válido — executando pulo normal");
        GetComponent<PlayerController>().TryJump();
    }

    IEnumerator PerformVault(RaycastHit hit)
    {
        isVaulting = true;
        lastHitObstacle = hit;
        animator.SetTrigger(vaultAnimationTrigger);

        GetComponent<PlayerController>().enabled = false;

        Vector3 start = transform.position;
        Vector3 end = hit.point + transform.forward * vaultDistance;

        float elapsed = 0f;

        while (elapsed < vaultDuration)
        {
            float t = elapsed / vaultDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector3.Lerp(start, end, smoothT);

            // 🔥 Target Matching na mão
            if (enableTargetMatching)
            {
                Vector3 targetPos = hit.point + transform.TransformDirection(handOffset);
                animator.MatchTarget(
                    targetPos,
                    Quaternion.identity,
                    matchTarget,
                    new MatchTargetWeightMask(Vector3.one, 0),
                    matchStart,
                    matchEnd
                );
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        GetComponent<PlayerController>().enabled = true;
        isVaulting = false;

        Debug.Log("[VAULT] Finalizado com sucesso");
    }

    public void OnVaultLand()
    {
        Debug.Log("[VAULT] Evento OnVaultLand chamado.");

        if (lastHitObstacle.collider != null)
        {
            Collider obstacleCol = lastHitObstacle.collider;
            Vector3 obstacleCenter = obstacleCol.bounds.center;
            Vector3 obstacleSize = obstacleCol.bounds.size;

            Vector3 targetPos = obstacleCenter;
            targetPos.y = obstacleCol.bounds.max.y;
            targetPos += transform.forward * 0.5f;

            if (Physics.Raycast(targetPos + Vector3.up, Vector3.down, out RaycastHit groundHit, 2f, vaultLayer))
            {
                targetPos.y = groundHit.point.y;
            }

            controller.enabled = false;
            transform.position = targetPos;
            controller.enabled = true;
        }

        GetComponent<PlayerController>().enabled = true;
        isVaulting = false;
    }

    // 🔥 GIZMOS — Debug visual do Vault e Target Matching
    void OnDrawGizmos()
    {
        if (lastHitObstacle.collider != null)
        {
            Gizmos.color = Color.red;
            Vector3 targetPos = lastHitObstacle.point + transform.TransformDirection(handOffset);
            Gizmos.DrawSphere(targetPos, 0.05f);
        }
    }
}
