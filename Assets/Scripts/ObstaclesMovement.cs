using System.Collections.Generic;
using UnityEngine;

public class ObstaclesMovement : MonoBehaviour
{
    public int numInList;
    public float obstacleRotationSpeed;
    public float obstacleSpeed;
    public List<GameObject> ObstacleList;


    void Update()
    {
        MoveSequence();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 8) { return; }
        obstacleSpeed *= -1;
        numInList =  Random.Range(0, ObstacleList.Count -1);
    }

    void MoveSequence()
    {
        Transform nextObstacleTransform;
        if (numInList == ObstacleList.Count - 1)
            nextObstacleTransform = ObstacleList[0].transform;
        else
            nextObstacleTransform = ObstacleList[numInList + 1].transform;

        Vector2 targetDirection = nextObstacleTransform.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, (obstacleRotationSpeed) * Time.deltaTime);

        transform.Translate(Vector2.right * obstacleSpeed * Time.deltaTime);
    }
}
