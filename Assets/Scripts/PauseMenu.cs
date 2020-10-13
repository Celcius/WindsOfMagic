using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private BoolVar isPaused;

    private void OnEnable() 
    {
        isPaused.Value = true;    
    }

    private void OnDisable() 
    {
        isPaused.Value = false;    
    }
}
