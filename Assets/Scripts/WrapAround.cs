using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapAround : MonoBehaviour
{
    private Rigidbody2D rb;
    public Camera gameCamera;

    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Get the object's size based on its renderer bounds
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        objectWidth = renderer.bounds.size.x ;  // Half the width
        objectHeight = renderer.bounds.size.y ; // Half the height
    }

    void FixedUpdate()
    {
        HandleScreenWrap();
    }

    void HandleScreenWrap()
    {
        Vector3 thisObjectsPosition = transform.position;

        // Parameters of physical camera size
        float camHeight = gameCamera.orthographicSize * 2f;
        float camWidth = camHeight * gameCamera.aspect;
        float leftBound = gameCamera.transform.position.x - camWidth / 2;
        float rightBound = gameCamera.transform.position.x + camWidth / 2;
        float floorBound = gameCamera.transform.position.y - camHeight / 2;
        float topBound = gameCamera.transform.position.y + camHeight / 2;

        // Check if the object is outside the camera's bounds + object size
        bool isWrappingX = false;
        bool isWrappingY = false;

        if (thisObjectsPosition.x > rightBound + objectWidth)
        {
            thisObjectsPosition.x = leftBound - objectWidth; // Wrap around to the left
            isWrappingX = true;
        }
        else if (thisObjectsPosition.x < leftBound - objectWidth)
        {
            thisObjectsPosition.x = rightBound + objectWidth; // Wrap around to the right
            isWrappingX = true;
        }

        if (thisObjectsPosition.y > topBound + objectHeight)
        {
            thisObjectsPosition.y = floorBound - objectHeight; // Wrap around to the bottom
            isWrappingY = true;
        }
        else if (thisObjectsPosition.y < floorBound - objectHeight)
        {
            thisObjectsPosition.y = topBound + objectHeight; // Wrap around to the top
            isWrappingY = true;
        }

        if (isWrappingX || isWrappingY)
        {
            transform.position = thisObjectsPosition;
        }
    }
}
