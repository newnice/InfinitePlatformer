using UnityEngine;
using UnityEngine.UI;

public class MiniMapUpdater : MonoBehaviour {
    [SerializeField] private float _maxXScale = 1.5f;
    [SerializeField] private float _maxYScale = 1.5f;
    [SerializeField] private Camera _camera;
    private Image _mapCanvas;
    private Vector3 _mapSize;

    private Matrix4x4 _toMapTransform;


    public void DrawToken(RectTransform tokenTransform, Vector3 objectWorldPosition) {
        var viewPos = _camera.WorldToViewportPoint(objectWorldPosition);
        var mapPos = _toMapTransform.MultiplyPoint3x4(viewPos);

        if (mapPos.x < 0) mapPos.x = 0;
        if (mapPos.y < 0) mapPos.y = 0;
        tokenTransform.localPosition = mapPos - _mapSize;
    }

    private void Awake() {
        _mapCanvas = GetComponent<Image>();
        _mapSize = _mapCanvas.rectTransform.rect.size;
        _toMapTransform = new Matrix4x4(new Vector4(_mapSize.x / _maxXScale, 0),
            new Vector4(0, _mapSize.y / _maxYScale), Vector4.zero,
            Vector4.zero);
    }
}