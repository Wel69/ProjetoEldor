using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    [Header("Configurações")]
    public float interactRadius = 3f;
    public LayerMask interactMask;
    public Camera mainCamera;

    [Header("UI")]
    public GameObject interactUI;
    public Text interactText;

    [Header("Referências")]
    public AimCursor aimCursor; // ← Referência do AimCursor.cs

    private InteractableHighlight currentHighlight;
    private PlayerInput input;

    private void OnEnable()
    {
        input = GetComponent<PlayerInput>();
        input.actions["Interact"].performed += OnInteract;
    }

    private void OnDisable()
    {
        input.actions["Interact"].performed -= OnInteract;
    }

    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void Update()
    {
        float currentRadius = (aimCursor != null && aimCursor.IsAiming()) ? interactRadius * 2f : interactRadius;

        Vector3 rayOrigin;
        Vector3 rayDirection;

        if (aimCursor != null && aimCursor.IsAiming())
        {
            rayOrigin = transform.position + Vector3.up * 1.5f;
            Vector3 cursorPos = aimCursor.GetCursorWorldPosition();
            rayDirection = (cursorPos - rayOrigin).normalized;

            Debug.DrawLine(rayOrigin, cursorPos, Color.yellow);
        }
        else
        {
            rayOrigin = transform.position + Vector3.up * 1.5f;
            rayDirection = transform.forward;

            Debug.DrawRay(rayOrigin, rayDirection * currentRadius, Color.green);
        }

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, currentRadius, interactMask))
        {
            var interactable = hit.collider.GetComponent<InteractableHighlight>();
            if (interactable != null)
            {
                if (currentHighlight != interactable)
                {
                    currentHighlight?.RemoveHighlight();
                    interactable.Highlight();
                    currentHighlight = interactable;

                    Debug.Log($"✨ Highlight ON: {interactable.gameObject.name}");

                    if (interactUI != null)
                    {
                        interactUI.SetActive(true);
                        if (interactText != null)
                            interactText.text = $"[E] Interagir com {interactable.gameObject.name}";
                    }
                }
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentHighlight != null)
            {
                Debug.Log($"🟢 Interagindo com: {currentHighlight.gameObject.name}");
                currentHighlight.Interact();
            }
            else
            {
                Debug.Log("⚠️ Nenhum objeto interativo na mira.");
            }
        }
    }

    private void ClearHighlight()
    {
        if (currentHighlight != null)
        {
            Debug.Log($"❌ Highlight OFF: {currentHighlight.gameObject.name}");
            currentHighlight.RemoveHighlight();
            currentHighlight = null;
        }

        if (interactUI != null)
            interactUI.SetActive(false);
    }
}
