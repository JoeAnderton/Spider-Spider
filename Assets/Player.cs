using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    bool fired = false;
    bool grounded = false;
    public float speed = 5;
    public GameObject web;
    GameObject currentHook;
    GameObject currentWeb;
    Rigidbody2D rb;
    LineRenderer spiderweb;
    DistanceJoint2D distanceJoint;
    Vector2 playerDirection = new Vector2(0,0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spiderweb = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fired)
        {
            spiderweb.SetPosition(0, GetComponent<Transform>().position);
        }
        if (grounded)
        {
            rb.linearVelocityX = playerDirection.x * speed;
        }
    }
    public void FindClosestHook(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == 1 && !fired)
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Hook");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance && go != currentHook)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            currentHook = closest;
            spiderweb.enabled = true;
            spiderweb.SetPosition(0, GetComponent<Transform>().position);
            distanceJoint = gameObject.AddComponent<DistanceJoint2D>();
            spiderweb.SetPosition(1, closest.GetComponent<Transform>().position);
            distanceJoint.connectedBody = closest.GetComponent<Rigidbody2D>();
            distanceJoint.autoConfigureDistance = false;
            distanceJoint.distance = Vector2.Distance(transform.position, closest.transform.position);
            distanceJoint.enableCollision = true;
            fired = true;
        }
        else if (context.ReadValue<float>() == 0)
        {
            fired = false;
            spiderweb.enabled = false;
            Destroy(distanceJoint);
        }
    }

    public void Movement(InputAction.CallbackContext context)
    {
        playerDirection = context.ReadValue<Vector2>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    grounded = true;

                    if (distanceJoint != null)
                    {
                        distanceJoint.enabled = false;
                    }

                    break;
                }
            }
        }
        if (collision.gameObject.CompareTag("Defeat"))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    grounded = true;

                    if (distanceJoint != null)
                    {
                        distanceJoint.enabled = false;
                    }

                    break;
                }
            }
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            grounded = false;
            if (distanceJoint != null)
            {
                distanceJoint.distance = Vector2.Distance(transform.position, currentHook.transform.position);
                distanceJoint.enabled = true;
            }
        }
    }
}
