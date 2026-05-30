using UnityEngine;

[ExecuteAlways]
public class MatchFogColor : MonoBehaviour
{
    [SerializeField]
    private bool _deriveZenithFromFog;

    [SerializeField, Range(0.5f, 2f)]
    private float _zenithBrightness = 1.2f;

    [SerializeField, Range(0f, 1f)]
    private float _zenithSaturation = 0.85f;

    private static readonly int HorizonColorId = Shader.PropertyToID("_HorizonColor");
    private static readonly int ZenithColorId = Shader.PropertyToID("_ZenithColor");

    private Renderer _renderer;
    private Material _material;

    private void OnEnable()
    {
        CacheRenderer();
        ClearPropertyBlock();
        ApplyColors();
    }

    private void OnDisable()
    {
        ClearPropertyBlock();
    }

    private void Update()
    {
        ApplyColors();
    }

    private void CacheRenderer()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }

        if (_renderer != null && (_material == null || _material != _renderer.sharedMaterial))
        {
            _material = _renderer.sharedMaterial;
        }
    }

    private void ClearPropertyBlock()
    {
        if (_renderer != null)
        {
            _renderer.SetPropertyBlock(null);
        }
    }

    private void ApplyColors()
    {
        CacheRenderer();
        if (_renderer == null || _material == null)
        {
            return;
        }

        Color horizon = RenderSettings.fogColor;
        _material.SetColor(HorizonColorId, horizon);

        if (_deriveZenithFromFog)
        {
            _material.SetColor(ZenithColorId, DeriveZenithFromFog(horizon));
        }
    }

    private Color DeriveZenithFromFog(Color fog)
    {
        Color.RGBToHSV(fog, out float h, out float s, out float v);
        s = Mathf.Clamp01(s * _zenithSaturation);
        v = Mathf.Clamp01(v * _zenithBrightness);
        Color zenith = Color.HSVToRGB(h, s, v);
        zenith.a = fog.a;
        return zenith;
    }
}
