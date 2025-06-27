using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PlayerCutoutSync : MonoBehaviour
{
    private Material material;
    private Transform player;
    private Camera mainCamera;

    [Header("Config")]
    public string sizeProperty = "_Size";
    public float maxDistance = 10f;

    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }

        if (player == null)
        {
            Debug.LogWarning("Player não encontrado! Verifique se tem a tag 'Player'.");
        }
    }

    void Update()
    {
        if (material == null || player == null || mainCamera == null)
            return;

        // Checa se o shader tem o parâmetro necessário
        if (!material.HasProperty(sizeProperty))
        {
            // Só avisa uma vez
            Debug.LogWarning($"O shader do material '{material.name}' não possui o parâmetro '{sizeProperty}'.");
            enabled = false; // Desativa o script para evitar spam
            return;
        }

        // Aqui você poderia sincronizar o valor com base na distância da câmera, por exemplo
        float distance = Vector3.Distance(transform.position, player.position);
        float normalized = Mathf.Clamp01(distance / maxDistance);

        // Exemplo: diminui o size conforme a distância aumenta
        float cutoutSize = Mathf.Lerp(1f, 0.1f, normalized);
        material.SetFloat(sizeProperty, cutoutSize);
    }
}
