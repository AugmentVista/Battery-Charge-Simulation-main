using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Physics Layer")]
public class PhsyicsFilter : ContextFilter
{
    public LayerMask physicsMask;

    public override List<Transform> Filter(FlockAgent agent, List<Transform> orignal)
    {
        List<Transform> filtered = new List<Transform>();
        foreach (Transform obj in orignal)
        {
            if (physicsMask == (physicsMask | (1 << obj.gameObject.layer)))
            { 
                filtered.Add(obj);
            }
        }
        return filtered;
    }
}
