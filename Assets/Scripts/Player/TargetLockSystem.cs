using UnityEngine;
using UnityEngine.InputSystem;

public class TargetLockSystem : MonoBehaviour
{
    [Header("Configurações")]
    public float targetRadius = 8f;
    public LayerMask targetMask; // Layer dos inimigos

    [Header("Referências")]
    public Transform player;
    public Camera mainCamera;
    public AimCursor aimCursor;

    private Transform currentTarget;
    private PlayerInput input;

    void OnEnable()
    {
        input = GetComponent<PlayerInput>();
        input.actions["TargetLock"].performed += HandleTargetLock;
    }

    void OnDisable()
    {
        input.actions["TargetLock"].performed -= HandleTargetLock;
    }

    void Update()
    {
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(player.position, currentTarget.position);

            // 🔥 Se saiu do raio, destrava
            if (dist > targetRadius)
            {
                Debug.Log($"⚠️ Alvo {currentTarget.name} saiu do raio. Destravando.");
                ClearTarget();
                return;
            }

            // 🔥 Verifica se tem obstáculo entre o player e o alvo
            Vector3 dir = currentTarget.position - player.position;
            Vector3 origin = player.position + Vector3.up * 1.5f; // 🔥 Simula visão da cabeça

            int enemyLayer = LayerMask.NameToLayer("Enemy");
            int enemyLayerMask = 1 << enemyLayer;
            int obstacleMask = ~enemyLayerMask;

            if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dir.magnitude, obstacleMask))
            {
                Debug.Log($"⚠️ Obstáculo bloqueando {currentTarget.name}. Destravando.");
                ClearTarget();
                return;
            }

            // 🔥 Mantém olhando para o alvo
            dir.y = 0;
            if (dir.magnitude > 0.5f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                player.rotation = Quaternion.Lerp(player.rotation, lookRot, Time.deltaTime * 10f);
            }

            Debug.DrawLine(origin, currentTarget.position, Color.red);

            if (aimCursor != null)
                aimCursor.SetTargetLock(currentTarget);
        }
    }

    private void HandleTargetLock(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentTarget == null)
            {
                FindTarget();
            }
            else
            {
                ClearTarget();
            }
        }
    }

    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(player.position, targetRadius, targetMask);
        Debug.Log($"🎯 Procurando alvos... Encontrados: {hits.Length}");

        if (hits.Length > 0)
        {
            Transform closest = null;
            float minDist = Mathf.Infinity;

            int enemyLayer = LayerMask.NameToLayer("Enemy");
            int enemyLayerMask = 1 << enemyLayer;
            int obstacleMask = ~enemyLayerMask;

            foreach (var hit in hits)
            {
                float dist = Vector3.Distance(player.position, hit.transform.position);
                Vector3 dir = hit.transform.position - player.position;
                Vector3 origin = player.position + Vector3.up * 1.5f;

                bool hasObstacle = Physics.Raycast(origin, dir.normalized, dir.magnitude, obstacleMask);

                if (hasObstacle)
                {
                    Debug.Log($"❌ {hit.gameObject.name} bloqueado por obstáculo.");
                    continue;
                }

                Debug.Log($"👀 Verificando {hit.gameObject.name} → Distância: {dist}");

                if (dist < minDist)
                {
                    minDist = dist;
                    closest = hit.transform;
                }
            }

            if (closest != null)
            {
                currentTarget = closest;
                Debug.Log($"🔒 Travou alvo: {currentTarget.name}");

                if (aimCursor != null)
                    aimCursor.SetTargetLock(currentTarget);
            }
            else
            {
                Debug.Log("⚠️ Nenhum alvo válido encontrado dentro do raio e sem obstáculos.");
            }
        }
        else
        {
            Debug.Log("⚠️ Nenhum inimigo dentro do raio.");
        }
    }

    private void ClearTarget()
    {
        Debug.Log($"🔓 Destravou alvo: {(currentTarget != null ? currentTarget.name : "Nenhum")}");
        currentTarget = null;

        if (aimCursor != null)
            aimCursor.ClearTargetLock();
    }

    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }
}
