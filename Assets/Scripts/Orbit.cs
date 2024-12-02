using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public ColorsTargetingSystem newColorTarget;
    public ColorsTargetingSystem[] allColorTargets;

    private List<GameObject> orbitObjectedCollidedWith = new List<GameObject>();

    public float speed = 20.0f;
    public float MaxSpeed = 50.0f;
    private float velocityX;
    private float velocityY;
    public float targetGoalRadius = 0.5f;
    public float turningSpeed = 0.01f;
    public float orbitRadius = 1.0f; 
    public float angleSpeed = 20f;

    private Vector2 target;
    public Vector2 startingPosition;
    private Vector2 startVelocity;

    private Rigidbody2D rb;

    private bool timerFinished;
    private bool hasTarget;

    public List<string> colorTags;
    public ColorTags currentColorTag;

    private static List<Vector2> startPositionsList = new List<Vector2>();

    private void Start()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        ApplyRandomDirection();

        startPositionsList.Add(startingPosition);
        target = Vector2.zero;
        startVelocity = rb.velocity;

        colorTags = new List<string> { "Red", "Blue", "Green", "Yellow" };
        currentColorTag = (ColorTags)Random.Range(0, colorTags.Count); 

        StartCoroutine(ChangeColorTag());
    }

    private IEnumerator ChangeColorTag()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            currentColorTag = (ColorTags)(((int)currentColorTag + 1) % colorTags.Count); // Cycle through the color tags
        }
    }

    private void ApplyRandomDirection()
    {
        int randomDirection = Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0:
                rb.velocity = Vector2.left * speed;
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

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * MaxSpeed;
        }

        LerpTowardsTarget();
        if (hasTarget)
        {
            if (Vector2.Distance(transform.position, target) <= targetGoalRadius)
            {
                hasTarget = false; 
            }
        }
    }

    void OnGUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ApplyRandomDirection();
        }
    }


    private void LerpTowardsTarget()
    {
        Vector2 desiredDirection = (target - (Vector2)transform.position).normalized;

        Vector2 orbitPosition = target + new Vector2(Mathf.Cos(Time.time) * orbitRadius, Mathf.Sin(Time.time) * orbitRadius);

        Vector2 currentDirection = rb.velocity.normalized;
        Vector2 newDirection = Vector2.Lerp(currentDirection, (orbitPosition - (Vector2)transform.position).normalized, Time.deltaTime * turningSpeed);

        rb.velocity = newDirection * speed;
    }

    private int CountChildrenWithTag(string tag)
    {
        int count = 0;
        foreach (Transform child in transform)
        {
            if (child.CompareTag(tag))
            {
                count++;
            }
        }
        return count;
    }

    public enum ColorTags
    {
        Red,
        Blue,
        Green,
        Yellow
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("OrbitObject") || collision.gameObject.CompareTag(colorTags[(int)currentColorTag]))
        {
            Debug.Log(collision.gameObject);
            GameObject collidedObject = collision.gameObject; // Define what collidedObject is

            if (!orbitObjectedCollidedWith.Contains(collidedObject))
            {
                orbitObjectedCollidedWith.Add(collidedObject);
            }

            Rigidbody2D rbCollidedWith = collidedObject.GetComponent<Rigidbody2D>(); // Define what rbCollidedWith is

            if (rbCollidedWith != null)
            {
                if (orbitObjectedCollidedWith.Count < 5)
                {
                    GameObject bestTarget = null;
                    int maxDifference = 0;
                    bool foundBetterTarget = false;

                    // Current color counts for this object
                    int thisRedCount = CountChildrenWithTag("Red");
                    int thisBlueCount = CountChildrenWithTag("Blue");
                    int thisGreenCount = CountChildrenWithTag("Green");
                    int thisYellowCount = CountChildrenWithTag("Yellow");

                    foreach (GameObject orbitObject in orbitObjectedCollidedWith)
                    {
                        ColorsTargetingSystem otherColorTargetSystem = orbitObject.GetComponent<ColorsTargetingSystem>();
                        if (otherColorTargetSystem != null)
                        {
                            // Calculate child counts for the collided object
                            int otherRedCount = otherColorTargetSystem.CountChildrenWithTag("Red");
                            int otherBlueCount = otherColorTargetSystem.CountChildrenWithTag("Blue");
                            int otherGreenCount = otherColorTargetSystem.CountChildrenWithTag("Green");
                            int otherYellowCount = otherColorTargetSystem.CountChildrenWithTag("Yellow");

                            int totalDifference = 0;

                            // Calculate differences for each color
                            totalDifference += Mathf.Abs(thisRedCount - otherRedCount);
                            totalDifference += Mathf.Abs(thisBlueCount - otherBlueCount);
                            totalDifference += Mathf.Abs(thisGreenCount - otherGreenCount);
                            totalDifference += Mathf.Abs(thisYellowCount - otherYellowCount);

                            if (totalDifference > maxDifference)
                            {
                                maxDifference = totalDifference;

                                bestTarget = orbitObject; 

                                foundBetterTarget = true;
                            }
                            else if (totalDifference == maxDifference && foundBetterTarget)
                            {
                                if (Random.value < 0.5f)
                                {
                                    bestTarget = orbitObject;
                                }
                            }
                        }
                    }
                    if (orbitObjectedCollidedWith.Count > 3)
                    {
                        target = bestTarget != null ? bestTarget.transform.position : orbitObjectedCollidedWith[orbitObjectedCollidedWith.Count - 2].transform.position;
                        collidedObject.transform.parent = transform.parent;
                    }
                }
            }
        }
    }
}
