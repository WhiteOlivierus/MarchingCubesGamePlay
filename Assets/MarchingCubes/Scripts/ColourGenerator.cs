using UnityEngine;

[ExecuteInEditMode]
public class ColourGenerator : MonoBehaviour
{
    public Material mat;
    public Gradient gradient;
    public float normalOffsetWeight;

    Texture2D texture;
    const int TEXTURERESOLUTION = 50;

    void Init()
    {
        if (texture != null && texture.width == TEXTURERESOLUTION)
            return;

        texture = new Texture2D(TEXTURERESOLUTION, 1, TextureFormat.RGBA32, false);
    }

    void Update()
    {
        Init();
        UpdateTexture();

        MeshGenerator m = FindObjectOfType<MeshGenerator>();

        float boundsY = m.boundsSize * m.numChunks.y;

        mat.SetFloat("boundsY", boundsY);
        mat.SetFloat("normalOffsetWeight", normalOffsetWeight);

        mat.SetTexture("ramp", texture);
    }

    void UpdateTexture()
    {
        if (gradient == null)
            return;

        Color[] colours = new Color[texture.width];

        for (int i = 0; i < TEXTURERESOLUTION; i++)
        {
            Color gradientCol = gradient.Evaluate(i / (TEXTURERESOLUTION - 1f));
            colours[i] = gradientCol;
        }

        texture.SetPixels(colours);
        texture.Apply();
    }
}
