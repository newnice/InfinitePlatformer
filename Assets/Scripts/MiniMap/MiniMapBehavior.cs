using UnityEngine;

public class MiniMapBehavior : MonoBehaviour {
    [SerializeField] private Camera _mapCamera;

    private void Awake() {
        var rect = GetComponent<RectTransform>();
        if (rect != null) {
            rect.sizeDelta = new Vector2(_mapCamera.scaledPixelWidth, _mapCamera.scaledPixelHeight);
           Debug.Log(rect.sizeDelta);
        }
    }
}