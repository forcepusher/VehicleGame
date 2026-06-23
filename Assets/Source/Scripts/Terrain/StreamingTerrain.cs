using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace BananaParty.VehicleGame
{
    public class StreamingTerrain : MonoBehaviour
    {
#if UNITY_EDITOR
        private const string BundleRoot = "Standalone/SceneTiles";
#else
        private const string BundleRoot = "WebGL/SceneTiles";
#endif

        [SerializeField]
        private Transform _streamSource;

        [SerializeField]
        private GameObject _loadingOverlay;

        private float _unloadTtlSeconds = 300f;

        private readonly Dictionary<TileCoordinate, TileSceneEntry> _entries = new Dictionary<TileCoordinate, TileSceneEntry>();
        private readonly HashSet<TileCoordinate> _currentRequiredTiles = new HashSet<TileCoordinate>();
        private int _activeTileSceneActivationCount;
        private float _timeScaleBeforePause;

        public bool AllRequiredTilesLoaded
        {
            get
            {
                foreach (TileCoordinate tile in _currentRequiredTiles)
                {
                    if (!IsTileReady(tile))
                        return false;
                }

                return _currentRequiredTiles.Count > 0;
            }
        }

        private void Awake()
        {
            _loadingOverlay.SetActive(false);
        }

        private void Update()
        {
            UpdateRequiredTiles();
        }

        private void BeginSceneActivationPause()
        {
            if (_activeTileSceneActivationCount == 0)
            {
                _timeScaleBeforePause = Time.timeScale;
                Time.timeScale = 0f;
                _loadingOverlay.SetActive(true);
            }

            _activeTileSceneActivationCount++;
        }

        private void EndSceneActivationPause()
        {
            _activeTileSceneActivationCount--;

            if (_activeTileSceneActivationCount > 0)
                return;

            _activeTileSceneActivationCount = 0;
            Time.timeScale = _timeScaleBeforePause;
            _loadingOverlay.SetActive(false);
        }

        private void UpdateRequiredTiles()
        {
            Vector3 worldPosition = _streamSource.position;
            TileCoordinate anchor = TerrainTileGrid.GetAnchor(worldPosition);

            _currentRequiredTiles.Clear();
            foreach (TileCoordinate tile in TerrainTileGrid.GetWindow(anchor))
                _currentRequiredTiles.Add(tile);

            foreach (TileCoordinate tile in _currentRequiredTiles)
                RequestLoad(tile);

            ScheduleUnloads();
            ProcessExpiredUnloads();
        }

        private void RequestLoad(TileCoordinate tile)
        {
            TileSceneEntry entry = GetOrCreateEntry(tile);
            if (entry.IsLoaded || entry.IsLoading)
                return;

            entry.IsLoading = true;
            entry.UnloadAfterTime = 0f;
            StartCoroutine(LoadTileScene(entry));
        }

        private IEnumerator LoadTileScene(TileSceneEntry entry)
        {
            string bundleUrl = GetBundleUrl(entry.Coordinate);
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new IOException($"Failed to load tile bundle {entry.Coordinate}: {request.error}");

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
            request.Dispose();

            string scenePath = bundle.GetAllScenePaths()[0];

            BeginSceneActivationPause();
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            yield return loadOperation;
            EndSceneActivationPause();

            entry.Bundle = bundle;
            entry.Scene = SceneManager.GetSceneByPath(scenePath);
            entry.IsLoaded = true;
            entry.IsLoading = false;
        }

        private void ScheduleUnloads()
        {
            foreach (KeyValuePair<TileCoordinate, TileSceneEntry> pair in _entries)
            {
                TileSceneEntry entry = pair.Value;
                if (!entry.IsLoaded || entry.IsLoading)
                    continue;

                if (_currentRequiredTiles.Contains(pair.Key))
                {
                    entry.UnloadAfterTime = 0f;
                    continue;
                }

                if (entry.UnloadAfterTime <= 0f)
                    entry.UnloadAfterTime = Time.time + _unloadTtlSeconds;
            }
        }

        private void ProcessExpiredUnloads()
        {
            List<TileCoordinate> expiredTiles = new List<TileCoordinate>();

            foreach (KeyValuePair<TileCoordinate, TileSceneEntry> pair in _entries)
            {
                TileSceneEntry entry = pair.Value;
                if (entry.UnloadAfterTime > 0f && Time.time >= entry.UnloadAfterTime)
                    expiredTiles.Add(pair.Key);
            }

            foreach (TileCoordinate tile in expiredTiles)
                StartCoroutine(UnloadTileScene(_entries[tile]));
        }

        private IEnumerator UnloadTileScene(TileSceneEntry entry)
        {
            if (entry.IsLoading)
                yield break;

            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(entry.Scene);
            yield return unloadOperation;

            if (entry.Bundle != null)
                entry.Bundle.Unload(true);

            _entries.Remove(entry.Coordinate);
        }

        private TileSceneEntry GetOrCreateEntry(TileCoordinate tile)
        {
            if (_entries.TryGetValue(tile, out TileSceneEntry entry))
                return entry;

            entry = new TileSceneEntry { Coordinate = tile };
            _entries[tile] = entry;
            return entry;
        }

        private bool IsTileReady(TileCoordinate tile)
        {
            return _entries.TryGetValue(tile, out TileSceneEntry entry) && entry.IsLoaded && !entry.IsLoading;
        }

        private static string GetBundleUrl(TileCoordinate tile)
        {
            return Path.Combine(Application.streamingAssetsPath, BundleRoot, tile.SceneName)
                .Replace('\\', '/');
        }
    }
}
