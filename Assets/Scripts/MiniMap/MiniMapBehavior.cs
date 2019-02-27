using UnityEngine;

public class MiniMapBehavior : MonoBehaviour {
    [SerializeField] private Camera _mapCamera=null;

    private void Awake() {
        var rect = GetComponent<RectTransform>();
        if (rect != null) {
            rect.sizeDelta = new Vector2(_mapCamera.scaledPixelWidth, _mapCamera.scaledPixelHeight);
        }
    }
}