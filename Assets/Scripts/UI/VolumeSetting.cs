using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField]
    private SoundSystem system;

    [SerializeField]
    private Slider slider;

    private void OnEnable() 
    {
        slider.value = system.MainVolume;    
    }
    public void VolumeChange()
    {
        system.MainVolume = slider.value;
    }
}
