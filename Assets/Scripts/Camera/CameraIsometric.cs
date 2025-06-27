using UnityEngine;

public class CameraIsometric : MonoBehaviour
{
    [Header("Configuração")]
    public Transform target;
    public AimCursor aimCursor;

    [Header("Offsets")]
    public Vector3 normalOffset = new Vector3(3f, 5f, 3f);
    public Vector3 aimOffset = new Vector3(5f, 10f, -5f);
    public float smoothSpeed = 5f;

    private Vector3 currentOffset;

    void Start()
    {
        currentOffset = normalOffset;
    }

    void LateUpdate()
    {
        if (target == null || aimCursor == null) return;

        // Alterna o offset com base na mira
        Vector3 desiredOffset = aimCursor.IsAiming() ? aimOffset : normalOffset;
        currentOffset = Vector3.Lerp(currentOffset, desiredOffset, Time.deltaTime * smoothSpeed);

        // Atualiza posição e rotação da câmera
        transform.position = target.position + currentOffset;
        transform.LookAt(target.position);
    }
}
