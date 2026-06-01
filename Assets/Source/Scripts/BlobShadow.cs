using UnityEngine;
using UnityEngine.Rendering;

namespace Igrushka.VehicleGame
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class BlobShadow : MonoBehaviour
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private const float MaxRayDistance = 50f;
        private const float RayStartHeightOffset = 1f;

        [SerializeField] private Material _material;
        [SerializeField] private float _radius = 2f;
        [SerializeField] private float _heightOffsetFromGround = 0.1f;
        [SerializeField] private float _heightFadeStart = 0f;
        [SerializeField] private float _heightFadeEnd = 12f;
        [SerializeField] private LayerMask _groundMask = ~0;

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
            EnsureBlobExists();
            UpdateBlobPlacement();
        }

        private static Mesh _quadMesh;

        private void EnsureBlobExists()
        {
            if (_material == null)
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
                GameObject blobObject = new GameObject("BlobShadow");
                blobObject.transform.SetParent(transform, false);

                blobObject.AddComponent<MeshFilter>().sharedMesh = GetQuadMesh();
                blobRenderer = blobObject.AddComponent<MeshRenderer>();

                blobTransform = blobObject.transform;
                blobRenderer.shadowCastingMode = ShadowCastingMode.Off;
                blobRenderer.receiveShadows = false;
                blobRenderer.lightProbeUsage = LightProbeUsage.Off;
                blobRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }

            if (blobRenderer.sharedMaterial != _material)
            {
                blobRenderer.sharedMaterial = _material;
            }

            blobTransform.gameObject.SetActive(true);
        }

        private Mesh GetQuadMesh()
        {
            if (_quadMesh != null)
            {
                return _quadMesh;
            }

            _quadMesh = new Mesh();
            _quadMesh.name = "BlobShadowQuad";
            _quadMesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(0.5f, -0.5f, 0),
                new Vector3(0.5f, 0.5f, 0),
                new Vector3(-0.5f, 0.5f, 0)
            };
            _quadMesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
            _quadMesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };
            _quadMesh.RecalculateNormals();
            return _quadMesh;
        }

        private void UpdateBlobPlacement()
        {
            if (blobTransform == null || _material == null)
            {
                return;
            }

            Vector3 origin = transform.position + Vector3.up * RayStartHeightOffset;
            RaycastHit[] hits = Physics.RaycastAll(
                origin,
                Vector3.down,
                MaxRayDistance + RayStartHeightOffset,
                _groundMask,
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
                float heightFade = 1f - Mathf.InverseLerp(_heightFadeStart, _heightFadeEnd, heightAboveGround);

                if (heightFade <= 0f)
                {
                    blobTransform.gameObject.SetActive(false);
                    return;
                }

                blobTransform.position = hit.point + hit.normal * _heightOffsetFromGround;
                blobTransform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(90f, 0f, 0f);
                blobTransform.localScale = new Vector3(_radius * 2f, _radius * 2f, 1f);
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

            Color baseColor = _material.GetColor(ColorId);
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
}
