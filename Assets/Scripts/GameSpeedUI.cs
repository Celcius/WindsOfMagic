using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSpeedUI : MonoBehaviour
{

    [SerializeField]
    private float[] speeds;

    [SerializeField]
    private TextMeshProUGUI[] speedLabels;
    
    [SerializeField]
    private Slider slider;
    
    private void Start()
    {
        slider.onValueChanged.AddListener(OnEventChanged);
        SetLabels();
        
        for(int i = 0; i < speeds.Length; i++)
        {
            if(Mathf.Approximately(speeds[i], GameTime.Instance.SpeedModifier))
            {
                slider.value = i;
                break;
            }
        }
    }

    private void OnDestroy() 
    {
        slider.onValueChanged.RemoveListener(OnEventChanged);
    }

    private void OnEventChanged(float val)
    {
        int index = (int)val;
        GameTime.Instance.SpeedModifier = speeds[index];
    }



    private void SetLabels()
    {
        int count = Mathf.Min(speeds.Length, speedLabels.Length);

        for(int i = 0; i < count; i++)
        {
            speedLabels[i].text = "" + speeds[i] + "x";
        }
    }
}
