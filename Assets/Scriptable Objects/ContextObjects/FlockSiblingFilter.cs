using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Flock/Filter/Sibling Flock")]
public class FlockSiblingFilter : ContextFilter
{
    public override List<Transform> Filter(FlockAgent agent, List<Transform> orignal)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (Transform obj in orignal)
        { 
            FlockAgent objAgent = obj.GetComponent< FlockAgent>();

            if (objAgent != null && objAgent.AgentFlock == agent.AgentFlock)
            { 
                filtered.Add(obj);
            }
        }
        return filtered;
    }

    
}
