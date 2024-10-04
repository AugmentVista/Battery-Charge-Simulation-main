using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorsTargetingSystem : MonoBehaviour
{
    public GameObject[] Circles; // Array to hold child circles
    public GameObject OrbitObject; // Reference to the parent OrbitObject

    private Orbit parentController; // Reference to the Orbit component

    void Start()
    {
        parentController = OrbitObject.GetComponent<Orbit>();
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Red") ||
            collision.gameObject.CompareTag("Blue") ||
            collision.gameObject.CompareTag("Green") ||
            collision.gameObject.CompareTag("Yellow"))
        {
            // Determine which color tag was hit
            string collidedTag = collision.gameObject.tag;
            ColorsTargetingSystem otherColorSystem = collision.gameObject.GetComponent<ColorsTargetingSystem>();

            if (otherColorSystem != null)
            {
                int currentCount = CountChildrenWithTag(collidedTag);
                int otherCount = otherColorSystem.CountChildrenWithTag(collidedTag);

                AssignChildBasedOnCount(collision.gameObject, currentCount, otherCount);
            }
        }
    }

    private void AssignChildBasedOnCount(GameObject childObject, int currentCount, int otherCount)
    {
        lock (this)
        {
            // Randomly decide in case of a tie
            if (currentCount < otherCount)
            {
                // Assign the child to this object's parent
                childObject.transform.parent = OrbitObject.transform;
            }
            else if (currentCount > otherCount)
            {
                // Assign the child to the other object's parent
                ColorsTargetingSystem otherColorSystem = childObject.GetComponent<ColorsTargetingSystem>();
                childObject.transform.parent = otherColorSystem.OrbitObject.transform;
            }
            else 
            {
                if (Random.value < 0.5f) // 50% chance to decide which parent to assign to
                {
                    childObject.transform.parent = OrbitObject.transform;
                }
                else
                {
                    ColorsTargetingSystem otherColorSystem = childObject.GetComponent<ColorsTargetingSystem>();
                    childObject.transform.parent = otherColorSystem.OrbitObject.transform;
                }
            }
        }
    }

    public int CountChildrenWithTag(string tag)
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
}
