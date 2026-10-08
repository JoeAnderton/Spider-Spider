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
    public float webDistanceMax = 50;
    public GameObject web;
    GameObject currentHook;
    GameObject currentWeb;
    GameObject closestHook;
    GameObject prevHook;
    Rigidbody2D rb;
    LineRenderer spiderweb;
    DistanceJoint2D distanceJoint;
    Vector2 playerDirection = new Vector2(0,0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spiderweb = GetComponent<LineRenderer>();
        distanceJoint = gameObject.AddComponent<DistanceJoint2D>();
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
        closestHook = findNextHook();
        if (closestHook != null)
        {
            closestHook.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else
        {
            spiderweb.enabled = false;
            distanceJoint.enabled = false;
        }
        if (prevHook != closestHook && prevHook != null)
        {
            prevHook.GetComponent<SpriteRenderer>().color = new Color(239f / 255f, 91f / 255f, 115f / 255f);
        }
    }
    public void FindClosestHook(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == 1 && !fired && closestHook != null)
        {
            currentHook = closestHook;
            spiderweb.enabled = true;
            distanceJoint.enabled = true;
            spiderweb.SetPosition(0, GetComponent<Transform>().position);
            spiderweb.SetPosition(1, closestHook.GetComponent<Transform>().position);
            distanceJoint.connectedBody = closestHook.GetComponent<Rigidbody2D>();
            distanceJoint.autoConfigureDistance = false;
            distanceJoint.distance = Vector2.Distance(transform.position, closestHook.transform.position);
            distanceJoint.enableCollision = true;
            fired = true;
        }
        else if (context.ReadValue<float>() == 0)
        {
            fired = false;
            spiderweb.enabled = false;
            distanceJoint.enabled = false;
        }
    }
    public GameObject findNextHook()
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
            if (curDistance < distance && go && curDistance <= webDistanceMax)
            {
                closest = go;
                distance = curDistance;
            }
        }
        prevHook = closestHook;
        return closest;
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
            if (distanceJoint != null && currentHook != null)
            {
                distanceJoint.distance = Vector2.Distance(transform.position, currentHook.transform.position);
                distanceJoint.enabled = true;
            }
        }
    }
}
