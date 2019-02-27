using UnityEngine;

public class MapCameraBehavior : MonoBehaviour {
    [SerializeField] private Camera _cameraToFollow=null;
    [SerializeField] private float _xOffset = 20f;
    [SerializeField] private float _yOffset=0f;

    private void Update() {
        gameObject.transform.position = _cameraToFollow.transform.position + new Vector3(_xOffset, _yOffset);
    }
}