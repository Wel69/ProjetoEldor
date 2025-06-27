using UnityEngine;
using UnityEngine.InputSystem;

public class AimCursor : MonoBehaviour
{
    public GameObject cursorPrefab;
    private GameObject cursorInstance;

    public Camera mainCamera;
    public LayerMask groundMask;

    private bool isAimingManual;
    private bool isTargetLocked;
    private Transform targetLockTransform;

    private void Start()
    {
        cursorInstance = Instantiate(cursorPrefab);
        cursorInstance.SetActive(false);
    }

    private void Update()
    {
        if (isTargetLocked && targetLockTransform != null)
        {
            Vector3 position = targetLockTransform.position;

            // 🔥 Faz um Raycast pra baixo pra posicionar no chão
            Vector3 rayOrigin = new Vector3(position.x, position.y + 5f, position.z);
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 10f, groundMask))
            {
                cursorInstance.SetActive(true);
                cursorInstance.transform.position = hit.point + Vector3.up * 0.02f;
                cursorInstance.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
            else
            {
                // Se não acertar o chão, usa a posição padrão do alvo
                cursorInstance.SetActive(true);
                cursorInstance.transform.position = targetLockTransform.position;
                cursorInstance.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
        }
        else if (isAimingManual)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
            {
                cursorInstance.SetActive(true);
                cursorInstance.transform.position = hit.point + Vector3.up * 0.02f;
                cursorInstance.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
        }
        else
        {
            cursorInstance.SetActive(false);
        }
    }

    public void SetAimState(bool aiming)
    {
        isAimingManual = aiming;
    }

    public void SetTargetLock(Transform target)
    {
        targetLockTransform = target;
        isTargetLocked = target != null;
    }

    public void ClearTargetLock()
    {
        targetLockTransform = null;
        isTargetLocked = false;
    }


    public bool IsAiming()
    {
        return isAimingManual;
    }

    public Vector3 GetCursorWorldPosition()
{
    return cursorInstance != null ? cursorInstance.transform.position : transform.position;
}

}
