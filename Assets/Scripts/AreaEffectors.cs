using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaEffectors : MonoBehaviour
{
    private AreaEffector2D areaEffector;

    
    public Image slider;

    private float minForceMagnitude = -0.0f;
    private float maxForceMagnitude = 200.0f;

    void Start()
    {
        areaEffector = GetComponent<AreaEffector2D>();
    }

    void Update()
    {
        areaEffector.forceMagnitude = Mathf.Lerp(minForceMagnitude, maxForceMagnitude, slider.fillAmount);
    }

    
}

