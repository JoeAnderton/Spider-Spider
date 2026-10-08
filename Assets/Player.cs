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
    public Transform winScreen;
    GameObject currentHook;
    GameObject currentWeb;
    Tornado tornado;
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
        tornado = FindAnyObjectByType<Tornado>();
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
            distanceJoint.maxDistanceOnly = false;
            OnGrappleConnect();
            fired = true;
        }
        else if (context.ReadValue<float>() == 0)
        {
            fired = false;
            spiderweb.enabled = false;
            distanceJoint.enabled = false;
        }
    }
    void OnGrappleConnect()
    {
        Vector2 ropeDirection = ((Vector2)currentHook.gameObject.transform.position - (Vector2)transform.position).normalized;

        // Find the perpendicular vector (the tangent of the circle you will swing on)
        Vector2 tangent = new Vector2(-ropeDirection.y, ropeDirection.x);

        // Project current velocity onto that tangent line
        float currentSpeed = rb.linearVelocity.magnitude;
        float swingDirectionSign = Mathf.Sign(Vector2.Dot(rb.linearVelocity, tangent));

        // Force the player's velocity to perfectly match the swing arc at original speed
        rb.linearVelocity = tangent * currentSpeed * swingDirectionSign;
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
        if (collision.gameObject.CompareTag("Win"))
        {
            winScreen.gameObject.SetActive(true);
            tornado.Stop();
        }
        if (collision.gameObject.CompareTag("Bounce"))
        {
            rb.linearVelocityY = 15f;
            rb.linearVelocityX = 15f;
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

    public void restartLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
