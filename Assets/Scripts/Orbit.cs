using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    // List of collided objects
    private List<GameObject> collidedObjects = new List<GameObject>();

    private float speed = 4.0f;
    private float numClicks = 0.0f;
    private float maxSpeed = 10.0f;

    private Vector2 target;
    public Vector2 startingPosition;

    private Rigidbody2D rb;
    private Rigidbody2D rbCollidedWith;
    private Vector2 startVelocity;

    private bool timerFinished;
    private bool hasTarget;
    private bool isFrozen = false;

    public float targetGoalRadius;


    // Static list to hold starting positions across all instances
    private static List<Vector2> availablePositions = new List<Vector2>();


    private void Start()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        SetInitialDirection();

        target = Vector2.zero;
        hasTarget = false;

        availablePositions.Add(startingPosition);
        startVelocity = rb.velocity;
        StartCoroutine(ManagePositions());
    }

    private void SetInitialDirection()
    {
        int randomDirection = Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0:
                target = 
                rb.velocity = Vector2.left * (speed);
                break;
            case 1:
                rb.velocity = Vector2.right * speed;
                break;
            case 2:
                rb.velocity = Vector2.up * speed;
                break;
            case 3:
                rb.velocity = Vector2.down * speed;
                break;
        }
    }

    private void Update()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed; // Clamp the velocity to maxSpeed
        }

        if (hasTarget && !isFrozen)
        {
            MoveTowardsTarget();

            if (Vector2.Distance(transform.position, target) <= targetGoalRadius)
            {
                ReverseDirection();
                hasTarget = false; // clear target when reached
            }
        }
        Debug.Log("Current Position: " + transform.position);
        FreezeCheck();
    }

    void OnGUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            numClicks++;
            speed = speed + numClicks;
            SetInitialDirection();
        }
    }

    private IEnumerator ManagePositions()
    {
        yield return new WaitForSeconds(5);
        startVelocity = rb.velocity;
        timerFinished = true;
    }

    private void FreezeCheck()
    {
        if (!isFrozen && availablePositions.Count > 0 && timerFinished)
        {
            Vector2 currentPosition = transform.position;

            foreach (Vector2 position in availablePositions)
            {
                if (Vector2.Distance(currentPosition, position) < 0.05f) // Doesn't need to be exact just very close
                {
                    rb.velocity = Vector2.zero;
                    //rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY; // Freeze position
                    isFrozen = true;
                    availablePositions.Remove(position); // Remove to prevent conflict
                    break;
                }
            }
        }
    }

    private void MoveTowardsTarget()
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void ReverseDirection()
    {
        rb.velocity = -rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OrbitObject") && !isFrozen)
        {
            GameObject collidedObject = collision.gameObject;

            if (!collidedObjects.Contains(collidedObject))
            {
                collidedObjects.Add(collidedObject);
                isFrozen = false;
            }

            Rigidbody2D rbCollidedWith = collidedObject.GetComponent<Rigidbody2D>();

            if (rbCollidedWith != null)
            {
                if (collidedObjects.Count < 5 && !hasTarget)
                {
                    target = collidedObjects[collidedObjects.Count].transform.position;

                    collidedObject.transform.parent = transform;
                }     
                   
                if (collidedObjects.Count >= 5 && !hasTarget)
                {
                   // var redTarget // find object in collidedObjects list with tag red
                }
            }
        }
    }
}