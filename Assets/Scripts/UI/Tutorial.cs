using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    public BoolVar shownTutorial;
    
    [SerializeField]
    private GameObject[] objectsToHide;

    [SerializeField]
    private GameController controller;
    
    [SerializeField]
    private float fadeDuration = 5.0f;

    [SerializeField]
    private float startDuration = 0.5f;

    [SerializeField]
    private Transform instructionsLabel;

    [SerializeField]
    private RollbackTimer rollbackTimer;

    private bool hasDestroyed = false;

#if UNITY_EDITOR
    [SerializeField]
    private bool hideTutorial = false;
#endif

    void Start()
    {
#if UNITY_EDITOR
        if(hideTutorial)
        {
            shownTutorial.Value = true;
        }
#endif

        if(!shownTutorial.Value)
        {
            ChangeHiddenObjects(false);
            instructionsLabel.gameObject.SetActive(false);
        }

    }

    private void ChangeHiddenObjects(bool active)
    {
        foreach(GameObject obj in objectsToHide)
        {
            obj.SetActive(active);
        }
    }

    void Update()
    {
        if(shownTutorial.Value && !hasDestroyed)
        {
            shownTutorial.Value = true;
            ChangeHiddenObjects(true);
            gameObject.SetActive(false);
            hasDestroyed = true;
            controller.StartSpawn();
            Destroy(this.gameObject);
            return;
        }
        
        if(shownTutorial.Value && hasDestroyed)
        {
            return;
        }
        startDuration = Mathf.Clamp(startDuration - Time.deltaTime, 0, startDuration);

        if(startDuration <= 0)
        {
            instructionsLabel.gameObject.SetActive(true);

            if(Input.GetKeyDown(KeyCode.Return))
            {
                EndTutorial();
            }
        } 
    }

    private void EndTutorial()
    {
        StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        TextMeshPro[] texts = GetComponentsInChildren<TextMeshPro>();
        TextMeshProUGUI[] textsUI = GetComponentsInChildren<TextMeshProUGUI>();
        float initialAlpha = sprites[0].color.a;
        
        float elapsed = fadeDuration;
        while(elapsed > 0)
        {
            float alpha = elapsed / fadeDuration * initialAlpha;
            foreach(SpriteRenderer rend in sprites)
            {
                Color color = rend.color;
                color.a = alpha;
                rend.color = color;
            }

            foreach(TextMeshPro text in texts)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }

            foreach(TextMeshProUGUI text in textsUI)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }

            yield return new WaitForEndOfFrame();
            elapsed -= Time.deltaTime;
        }
        shownTutorial.Value = true;
        hasDestroyed = true;
        controller.StartSpawn();
        ChangeHiddenObjects(true);
        Destroy(this.gameObject);
    }
}
