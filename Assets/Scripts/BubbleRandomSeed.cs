using UnityEngine;
using UnityEngine.UI;

public class BubbleRandomSeed : MonoBehaviour, IMeshModifier
{
    [SerializeField] private float seed = 1f;

    public void ModifyMesh(VertexHelper vh)
    {
        UIVertex vertex = new UIVertex();

        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            vertex.uv1 = new Vector4(seed, 0, 0, 0);
            vh.SetUIVertex(vertex, i);
        }
    }

    public void ModifyMesh(Mesh mesh) { }
}