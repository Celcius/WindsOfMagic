using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private BoolVar isPaused;

    [SerializeField]
    private GameObject[] enableOnPause;

    [SerializeField]
    private GameObject[] disableOnPause;


    public void SwapPause()
    {
        bool active = !gameObject.activeInHierarchy;
        gameObject.SetActive(active);
        if(active)
        {
            isPaused.Value = true;    
            
            foreach(GameObject obj in enableOnPause)
            {
                obj.SetActive(true);
            }
            
            foreach(GameObject obj in disableOnPause)
            {
                obj.SetActive(false);
            }
        }
    }

    private void OnEnable() 
    {
        isPaused.Value = true;    
    }

    private void OnDisable() 
    {
        isPaused.Value = false;    
    }
}
