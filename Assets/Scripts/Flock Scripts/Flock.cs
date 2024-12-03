using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;

    [Range(10, 500)]
    public int startingCount = 100;
    const float AgentDensity = 0.8f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;

    public float SqaureAvoidanceRadius { get { return squareAvoidanceRadius; } }

    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareAvoidanceRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;


        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(agentPrefab, Random.insideUnitCircle * startingCount * AgentDensity, 
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)), transform);
            newAgent.name = $"Agent{i}";
            newAgent.Init(this);
            agents.Add(newAgent);
        }
    }

    void Update()
    {
        foreach (FlockAgent agent in agents)
        { 
            List<Transform> context = GetNearByObjects(agent);

            Vector2 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed) { move = move.normalized * maxSpeed; }
            agent.Move(move);
        }
    }

    List<Transform> GetNearByObjects(FlockAgent agent)
    { 
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D colliders in contextColliders)
        {
            if (colliders != agent.AgentCollider)
            {
                context.Add(colliders.transform);
            }
        }
        return context;
    }
}
