using UnityEngine;

public class KillZone : MonoBehaviour
{
    public Transform mainCamera;
    public float killDepth = -10f;
	
	void Update ()
    {
        Vector3 newPosition = mainCamera.transform.position;
        newPosition.y= killDepth;
        transform.position = newPosition;
	}
}
