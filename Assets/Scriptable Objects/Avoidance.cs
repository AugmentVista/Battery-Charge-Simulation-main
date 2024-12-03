using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "FlockBehavior_Avoidance")]
public class Avoidance : FlockBehavior
{
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0) { return Vector2.zero; }
        int avoidCount = 0;

        Vector2 avoidanceMove = Vector2.zero;
        foreach (Transform item in context)
        {
            if (Vector2.SqrMagnitude(item.position - agent.transform.position) < flock.SqaureAvoidanceRadius)
            {
                avoidCount++;
                avoidanceMove += (Vector2)(agent.transform.position - item.position);
            }
            avoidanceMove += (Vector2)item.transform.up;
        }
        if (avoidCount > 0)
        { 
            avoidanceMove /= avoidCount;
        }
        return avoidanceMove;
    }
}
