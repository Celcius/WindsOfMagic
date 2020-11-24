using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private BoolVar isPaused;
    public bool IsPaused => isPaused.Value;

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

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
