using UnityEngine;
using UnityEngine.UIElements;

public class CameraScript : MonoBehaviour
{
    float cameraY = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        cameraY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, cameraY, transform.position.z);
    }
}
