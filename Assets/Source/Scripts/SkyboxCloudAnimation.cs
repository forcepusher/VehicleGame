using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class SkyboxCloudAnimation : MonoBehaviour
{
    private static readonly int CloudTimeId = Shader.PropertyToID("_CloudTime");

    private Renderer _renderer;
    private MaterialPropertyBlock _propertyBlock;

    private void OnEnable()
    {
        Cache();
        ApplyTime();
    }

    private void Update()
    {
        ApplyTime();
    }

    private void Cache()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }
    }

    private void ApplyTime()
    {
        Cache();
        if (_renderer == null)
        {
            return;
        }

        _renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(CloudTimeId, GetAnimationTime());
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    private static float GetAnimationTime()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return (float)UnityEditor.EditorApplication.timeSinceStartup;
        }
#endif
        return Time.time;
    }
}
