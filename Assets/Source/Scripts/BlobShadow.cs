using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[DisallowMultipleComponent]
public class BlobShadow : MonoBehaviour
{
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    [SerializeField] private Material material;
    [SerializeField] private float radius = 2f;
    [SerializeField] private float heightOffset = 0.08f;
    [SerializeField] private float maxRayDistance = 500f;
    [SerializeField] private float heightFadeStart = 0f;
    [SerializeField] private float heightFadeEnd = 12f;
    [SerializeField] private LayerMask groundMask = ~0;

    private Transform blobTransform;
    private MeshRenderer blobRenderer;
    private MaterialPropertyBlock propertyBlock;

    private void OnEnable()
    {
        EnsureBlobExists();
        UpdateBlobPlacement();
    }

    private void OnDisable()
    {
        if (blobTransform != null)
        {
            blobTransform.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        UpdateBlobPlacement();
    }

    private void OnValidate()
    {
        radius = Mathf.Max(0.1f, radius);
        maxRayDistance = Mathf.Max(1f, maxRayDistance);
        heightFadeEnd = Mathf.Max(heightFadeStart + 0.01f, heightFadeEnd);
        EnsureBlobExists();
        UpdateBlobPlacement();
    }

    private void EnsureBlobExists()
    {
        if (material == null)
        {
            return;
        }

        if (blobTransform == null)
        {
            Transform existing = transform.Find("BlobShadow");
            if (existing != null)
            {
                blobTransform = existing;
                blobRenderer = existing.GetComponent<MeshRenderer>();
            }
        }

        if (blobTransform == null)
        {
            GameObject blobObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            blobObject.name = "BlobShadow";
            blobObject.transform.SetParent(transform, false);

            Collider primitiveCollider = blobObject.GetComponent<Collider>();
            if (Application.isPlaying)
            {
                Destroy(primitiveCollider);
            }
            else
            {
                DestroyImmediate(primitiveCollider);
            }

            blobTransform = blobObject.transform;
            blobRenderer = blobObject.GetComponent<MeshRenderer>();
            blobRenderer.shadowCastingMode = ShadowCastingMode.Off;
            blobRenderer.receiveShadows = false;
            blobRenderer.lightProbeUsage = LightProbeUsage.Off;
            blobRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }

        if (blobRenderer.sharedMaterial != material)
        {
            blobRenderer.sharedMaterial = material;
        }

        blobTransform.gameObject.SetActive(true);
    }

    private void UpdateBlobPlacement()
    {
        if (blobTransform == null || material == null)
        {
            return;
        }

        Vector3 origin = transform.position + Vector3.up * 10f;
        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            Vector3.down,
            maxRayDistance + 10f,
            groundMask,
            QueryTriggerInteraction.Ignore);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            if (IsOwnHierarchy(hits[i].collider))
            {
                continue;
            }

            RaycastHit hit = hits[i];
            float heightAboveGround = Vector3.Dot(transform.position - hit.point, hit.normal);
            float heightFade = 1f - Mathf.InverseLerp(heightFadeStart, heightFadeEnd, heightAboveGround);

            if (heightFade <= 0f)
            {
                blobTransform.gameObject.SetActive(false);
                return;
            }

            blobTransform.position = hit.point + hit.normal * heightOffset;
            blobTransform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(90f, 0f, 0f);
            blobTransform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
            ApplyShadowColor(heightFade);
            blobTransform.gameObject.SetActive(true);
            return;
        }

        blobTransform.gameObject.SetActive(false);
    }

    private void ApplyShadowColor(float heightFade)
    {
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        Color baseColor = material.GetColor(ColorId);
        baseColor.a *= heightFade;
        propertyBlock.SetColor(ColorId, baseColor);
        blobRenderer.SetPropertyBlock(propertyBlock);
    }

    private bool IsOwnHierarchy(Collider collider)
    {
        Transform hitTransform = collider.transform;
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }
}
