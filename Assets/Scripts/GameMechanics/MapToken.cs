using System;
using UnityEngine;
using UnityEngine.UI;

[Obsolete]
public class MapToken : MonoBehaviour {
    private MiniMapUpdater _mapUpdater;

    [SerializeField] private Sprite _sprite;
    [SerializeField] private float _maxSize = 5;
    private GameObject _object;
    private RectTransform _rectTransform;


    private void Awake() {
        _mapUpdater = FindObjectOfType<MiniMapUpdater>();
        InitObject();
    }

    private void Start() {
        _rectTransform = _object.GetComponent<RectTransform>();
        _rectTransform.sizeDelta = new Vector2(_maxSize, _maxSize);
    }

    private void InitObject() {
        _object = new GameObject("MapToken");
        var image = _object.AddComponent<Image>();
        image.sprite = _sprite;

        image.transform.SetParent(_mapUpdater.transform);
        _object.SetActive(false);
    }

    private void LateUpdate() {
        _mapUpdater.DrawToken(_rectTransform, gameObject.transform.position);
    }

    private void OnDestroy() {
        Destroy(_object);
    }

    private void OnDisable() {
        if (_object != null)
            _object.SetActive(false);
    }

    private void OnEnable() {
        _object.SetActive(true);
    }
}