using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class NoiseEffect : MonoBehaviour
{
    public RawImage rawImage;
    private float minX = 0f; // Minimum X offset for UV
    private float maxX = 1f; // Maximum X offset for UV
    private float minY = 0f; // Minimum Y offset for UV
    private float maxY = 1f; // Maximum Y offset for UV

    private void Update()
    {
        if (rawImage == null)
        {
            return;
        }

        // Get the current uvRect to preserve W and H
        Rect currentRect = rawImage.uvRect;
        // Assign random X and Y, keep W and H unchanged
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        rawImage.uvRect = new Rect(randomX, randomY, currentRect.width, currentRect.height);
    }
}
