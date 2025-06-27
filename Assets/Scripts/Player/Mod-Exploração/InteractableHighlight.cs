using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    public Material highlightMat;
    private Material originalMat;
    private MeshRenderer rend;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        originalMat = rend.material;
    }

    public void Highlight()
    {
        rend.material = highlightMat;
    }

    public void RemoveHighlight()
    {
        rend.material = originalMat;
    }

    public void Interact()
    {
        Debug.Log($"🟢 Interação feita no objeto: {gameObject.name}");
        // 🔥 Aqui você coloca sua lógica (abrir porta, pegar item, etc.)
    }
}
