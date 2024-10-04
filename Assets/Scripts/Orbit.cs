using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public ColorsTargetingSystem newColorTarget;
    public ColorsTargetingSystem[] allColorTargets;

    private List<GameObject> orbitObjectedCollidedWith = new List<GameObject>();

    private float speed = 20.0f;
    private float numClicks = 1.0f;
    private float MaxSpeed = 40.0f;
    private float velocityX;
    private float velocityY;
    public float targetGoalRadius = 0.05f;
    public float turningSpeed = 0.5f;

    private Vector2 target;
    public Vector2 startingPosition;
    private Vector2 startVelocity;

    private Rigidbody2D rb;

    private bool timerFinished;
    private bool hasTarget;

    public List<string> colorTags; // Adjusted to a List for flexibility
    public ColorTags currentColorTag;

    private static List<Vector2> startPositionsList = new List<Vector2>();

    private void Start()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        ApplyRandomDirection();

        startPositionsList.Add(startingPosition); // add each orbit object's start pos to the collective static list
        target = Vector2.zero;
        startVelocity = rb.velocity;

        colorTags = new List<string> { "Red", "Blue", "Green", "Yellow" };
        currentColorTag = (ColorTags)Random.Range(0, colorTags.Count); // Choose a random starting color tag

        // Start the coroutine to change the color tag
        StartCoroutine(ChangeColorTag());
    }

    private IEnumerator ChangeColorTag()
    {
        while (true)
        {
            // Wait for 4 seconds
            yield return new WaitForSeconds(4.0f);

            // Change to the next color tag
            currentColorTag = (ColorTags)(((int)currentColorTag + 1) % colorTags.Count); // Cycle through the color tags
        }
    }

    private void ApplyRandomDirection()
    {
        int randomDirection = Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0:
                rb.velocity = Vector2.left * (speed + numClicks);
                break;
            case 1:
                rb.velocity = Vector2.right * (speed + numClicks);
                break;
            case 2:
                rb.velocity = Vector2.up * (speed + numClicks);
                break;
            case 3:
                rb.velocity = Vector2.down * (speed + numClicks);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * MaxSpeed;
        }

        if (hasTarget)
        {
            LerpTowardsTarget();

            if (Vector2.Distance(transform.position, target) <= targetGoalRadius)
            {
                hasTarget = false; // clear target when reached
            }
        }
        OccupyingAnyStartPosition();
    }

    void OnGUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            numClicks++;
            speed = speed + numClicks;
            ApplyRandomDirection();
        }
    }

    private void OccupyingAnyStartPosition()
    {
        if (startPositionsList.Count > 0)
        {
            Vector2 currentPosition = transform.position;

            foreach (Vector2 position in startPositionsList)
            {
                if (Vector2.Distance(currentPosition, position) < 0.05f) // Doesn't need to be exact just very close
                {
                    startPositionsList.Remove(position);
                    break;
                }
            }
        }
    }

    private void LerpTowardsTarget()
    {
        Vector2 desiredDirection = (target - (Vector2)transform.position).normalized;

        Vector2 currentDirection = rb.velocity.normalized;

        Vector2 newDirection = Vector2.Lerp(currentDirection, desiredDirection, Time.deltaTime * turningSpeed); // Lerp between the Vector2's to simulate a turning radius

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

                            // Check if the total difference is the greatest found so far
                            if (totalDifference > maxDifference)
                            {
                                maxDifference = totalDifference;
                                bestTarget = orbitObject; // Set best target
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
                    }

                    collidedObject.transform.parent = transform.parent;
                }
            }
        }
    }
}
