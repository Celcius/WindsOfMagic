using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReverseTutorial : MonoBehaviour
{
    [SerializeField]
    private InputSchemeVar currentScheme;

    [SerializeField]
    private TextMeshProUGUI label;
    
    private Color storedColor;

    [SerializeField]
    private float fadeInDuration = 0.5f;

    [SerializeField]
    private float stayInScreenDuration = 2.0f;

    [SerializeField]
    private float fadeInDelay = 0.5f;

    private void Awake() 
    {
        storedColor = label.color;    
    }
    private void OnEnable() 
    {
        StopAllCoroutines();
        StartCoroutine(AnimateLabel());
    }
    
    private IEnumerator AnimateLabel()
    {
        while (true)
        {
            label.text = "Press " + currentScheme.Value.ReverseTimeKey + "\nto Rewind Time...";
            Color invisColor = label.color;
            invisColor.a = 0;
            label.color = invisColor;

            yield return new WaitForSeconds(fadeInDelay);

            float elapsed = fadeInDuration;
            float delta = storedColor.a / fadeInDuration;
            Color curColor = label.color;
            while(elapsed > 0)
            {
                curColor.a += delta * Time.deltaTime;
                yield return new WaitForEndOfFrame();
                elapsed -= Time.deltaTime;
            }

            label.color = storedColor;
            yield return new WaitForSeconds(stayInScreenDuration);
        }
    }

}
