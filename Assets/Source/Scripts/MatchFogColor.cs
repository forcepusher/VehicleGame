using UnityEngine;

[ExecuteAlways]
public class MatchFogColor : MonoBehaviour
{
    [SerializeField]
    private string _colorPropertyName = "_Color";

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer == null) return;
        }

        if (_propBlock == null)
        {
            _propBlock = new MaterialPropertyBlock();
        }

        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(_colorPropertyName, RenderSettings.fogColor);
        _renderer.SetPropertyBlock(_propBlock);
    }
}
