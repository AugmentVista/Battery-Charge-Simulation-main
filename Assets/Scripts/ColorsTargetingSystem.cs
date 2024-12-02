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
        StartCoroutine(CheckForUnparentedCircles());
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

    private IEnumerator CheckForUnparentedCircles()
    {
        while (true)
        {
            foreach (Transform child in transform)
            {
                if (child == null || child.parent == null) 
                {
                    AssignCircleToClosestOrbit(child.gameObject);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void AssignCircleToClosestOrbit(GameObject circle)
    {
        // Logic to find the nearest Orbit object based on circle position and assign it
        ColorsTargetingSystem[] allOrbitSystems = FindObjectsOfType<ColorsTargetingSystem>();
        ColorsTargetingSystem closestSystem = null;
        float closestDistance = float.MaxValue;

        foreach (var orbitSystem in allOrbitSystems)
        {
            float distance = Vector2.Distance(circle.transform.position, orbitSystem.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSystem = orbitSystem;
            }
        }

        if (closestSystem != null)
        {
            if (circle.CompareTag(closestSystem.gameObject.tag))
            {
                circle.transform.SetParent(closestSystem.OrbitObject.transform, false);
            }
        }
    }

    private void AssignChildBasedOnCount(GameObject childObject, int currentCount, int otherCount)
    {
        lock (this)
        {
            if (currentCount < otherCount)
            {
                childObject.transform.SetParent(OrbitObject.transform.parent, false);
            }
            else if (currentCount > otherCount)
            {
                ColorsTargetingSystem otherColorSystem = childObject.GetComponent<ColorsTargetingSystem>();
                childObject.transform.SetParent(otherColorSystem.OrbitObject.transform.parent, false);
            }
            else
            {
                if (Random.value < 0.5f)
                {
                    childObject.transform.SetParent(OrbitObject.transform.parent, false);
                }
                else
                {
                    ColorsTargetingSystem otherColorSystem = childObject.GetComponent<ColorsTargetingSystem>();
                    childObject.transform.SetParent(otherColorSystem.OrbitObject.transform.parent, false);
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
