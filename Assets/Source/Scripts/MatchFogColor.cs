using UnityEngine;

[ExecuteAlways]
public class MatchFogColor : MonoBehaviour
{
    [SerializeField]
    private Color _zenithColor = new Color(0.55f, 0.35f, 0.3f, 1f);

    [SerializeField]
    private bool _deriveZenithFromFog = true;

    [SerializeField, Range(0.5f, 2f)]
    private float _zenithBrightness = 1.2f;

    [SerializeField, Range(0f, 1f)]
    private float _zenithSaturation = 0.85f;

    private static readonly int HorizonColorId = Shader.PropertyToID("_HorizonColor");
    private static readonly int ZenithColorId = Shader.PropertyToID("_ZenithColor");

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    private void OnEnable()
    {
        CacheRenderer();
        ApplyColors();
    }

    private void Awake()
    {
        CacheRenderer();
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

        if (_propBlock == null)
        {
            _propBlock = new MaterialPropertyBlock();
        }
    }

    private void ApplyColors()
    {
        CacheRenderer();
        if (_renderer == null)
        {
            return;
        }

        Color horizon = RenderSettings.fogColor;
        Color zenith = _deriveZenithFromFog ? DeriveZenithFromFog(horizon) : _zenithColor;

        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(HorizonColorId, horizon);
        _propBlock.SetColor(ZenithColorId, zenith);
        _renderer.SetPropertyBlock(_propBlock);
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
