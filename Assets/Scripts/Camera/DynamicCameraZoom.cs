using UnityEngine;
using UnityEngine.InputSystem;

public class DynamicCameraZoom : MonoBehaviour
{
    [Header("Referências de Posição")]
    public Transform normalView;  // Posição padrão (exploração / melee)
    public Transform rangedView;  // Posição de combate a distância

    [Header("Configurações")]
    public float zoomSpeed = 5f; // Velocidade de interpolação

    [Header("Input")]
    public PlayerInput playerInput;

    private Transform cameraTransform;
    private bool isAiming;

    private void Start()
    {
        cameraTransform = Camera.main.transform;

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        // Conecta a ação
        playerInput.actions["Aim"].performed += ctx => SetAimState(true);
        playerInput.actions["Aim"].canceled += ctx => SetAimState(false);
    }

    private void LateUpdate()
    {
        if (normalView == null || rangedView == null)
            return;

        Transform targetView = isAiming ? rangedView : normalView;

        // Move suavemente a câmera entre os pontos
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetView.position, Time.deltaTime * zoomSpeed);
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetView.rotation, Time.deltaTime * zoomSpeed);
    }

    private void SetAimState(bool aiming)
    {
        isAiming = aiming;
    }
}
