using UnityEngine;

public class Tornado : MonoBehaviour
{
    public float speed = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        transform.position = new Vector3(speed,0,0)*Time.deltaTime + transform.position;
    }

    public void Stop()
    {
        speed = 0;
    }
}
