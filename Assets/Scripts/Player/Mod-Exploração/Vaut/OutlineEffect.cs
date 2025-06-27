using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public Material outlineMaterial;
    public float outlineScale = 1.02f;

    private GameObject outlineObj;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
            outlineObj = new GameObject("Outline");
            outlineObj.transform.parent = transform;
            outlineObj.transform.localPosition = Vector3.zero;
            outlineObj.transform.localRotation = Quaternion.identity;
            outlineObj.transform.localScale = Vector3.one * outlineScale;

            MeshRenderer rend = outlineObj.AddComponent<MeshRenderer>();
            rend.material = outlineMaterial;

            MeshFilter mf = outlineObj.AddComponent<MeshFilter>();
            mf.mesh = meshFilter.mesh;
        }
    }

    public void EnableOutline(bool enable)
    {
        if (outlineObj != null)
            outlineObj.SetActive(enable);
    }
}
