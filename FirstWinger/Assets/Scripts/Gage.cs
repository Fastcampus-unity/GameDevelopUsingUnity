using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gage : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHP(float currentValue, float maxValue)
    {
        if (currentValue > maxValue)
            currentValue = maxValue;

        slider.value = currentValue / maxValue;
    }
}
