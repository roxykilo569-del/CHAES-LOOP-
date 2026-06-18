using UnityEngine;
using UnityEngine.UI;

public class ScanlineScroll : MonoBehaviour
{
    public RawImage rawImage;

    [Header("Scroll")]
    public float scrollSpeed = 0.02f;

    [Header("Tiling")]
    public float tilingX = 1f;
    public float tilingY = 1f;

    private Rect uvRect;

    private void Start()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }

        uvRect = rawImage.uvRect;
        uvRect.width = tilingX;
        uvRect.height = tilingY;
        rawImage.uvRect = uvRect;
    }

    private void Update()
    {
        uvRect.y -= scrollSpeed * Time.deltaTime;
        rawImage.uvRect = uvRect;
    }
}