using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SurfaceTextureCreator : MonoBehaviour
{
    // Noise Settings
    [Header("NOISE SETTINGS")]
    [Range(1, 500)]
    public int resolution = 10;

    public Vector3 offset;

    public float frequency = 1f;

    [Range(1, 15)]
    public int octaves = 1;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Range(1, 3)]
    public int dimensions = 3;

    public NoiseMethodType type;
    private NoiseMethod method;

    private MeshRenderer mRender;
    private Texture2D surfaceTexture;

    // Each color in this will be mapped to a biome
    [Header("BIOME SETTINGS")]
    public Gradient BiomeBlend;

    // Each color in this will be used to blend between cliff/grass/snow in each biome
    public List<Gradient> BiomeColorSchemes;

    // Start is called before the first frame update
    void Start()
    {
        surfaceTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);

        mRender = gameObject.GetComponent<MeshRenderer>();
        method = Noise.methods[(int)type][dimensions - 1];
        DrawBiomeMask();
    }

    /// <summary>
    /// Draws masks that we can use to define biome regions
    /// </summary>
    void DrawBiomeMask()
    {
        for (int v = 0, y = 0; y <= (resolution); y++)
        {
            for (int x = 0; x <= (resolution); x++, v++)
            {
                Vector2 point = new Vector2(x, y + offset.y);
                NoiseSample sample = Noise.Sum(method, new Vector3(point.x + offset.x, point.y + offset.y), frequency, octaves, lacunarity, persistence);
                Color color = BiomeBlend.Evaluate(sample.value);
                for (int i = 0; i < BiomeBlend.colorKeys.Length; i++)
                {
                    // Code to render biome colors
                }

                surfaceTexture.SetPixel((int)point.x, (int)point.y, color);
            }
        }

        // Apply the biome mask to the terrain as an example
        // will be removed once we get the biome colors working
        surfaceTexture.Apply();
        surfaceTexture.filterMode = FilterMode.Point;
        mRender.material.SetTexture("_BaseMap", surfaceTexture);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // DISABLE THIS IN PRODUCTION
    void OnValidate()
    {
        if (Application.isPlaying == true)
        {
            DrawBiomeMask();
        }
    }
}
