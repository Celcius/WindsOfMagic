using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AmoaebaUtils;

public class GameOverController : MonoBehaviour
{
    [SerializeField]
    private GameController controller;
    [SerializeField]
    private FloatVar highScore;

    [SerializeField]
    private FloatVar maxWave;

    [SerializeField]
    private TimelineFloatVar score;

    [SerializeField]
    private FloatVar currentWave;

    [SerializeField]
    private TextMeshProUGUI[] assets;

    [SerializeField]
    private TextMeshProUGUI scoreLabel;

    [SerializeField]
    private TextMeshProUGUI waveLabel;

    [SerializeField]
    private TextMeshProUGUI highScoreLabel;

    [SerializeField]
    private TextMeshProUGUI maxWaveLabel;

    [SerializeField]
    private TextMeshProUGUI anyKeyLabel;

    [SerializeField]
    private TextMeshProUGUI gameOverLabel;

    [SerializeField]
    private Image panel;

    [SerializeField]
    private float scoreIncreaseDuration = 2.0f;

    private bool canDetectKey = false;
    private void OnEnable()
    {
        StopAllCoroutines();
        PrepareAssets();

        StartCoroutine(GameOverRoutine());
    }

    private void PrepareAssets()
    {
        canDetectKey = false;

        foreach(TextMeshProUGUI asset in assets)
        {
            Color c = asset.color;
            c.a = 0;
            asset.color = c;
        }
        highScoreLabel.text = (score.Value > highScore.Value)? "New Record!" : ("Max: " + highScore.Value);
        maxWaveLabel.text = currentWave.Value > maxWave.Value? "New Record!" : ("Max: " + maxWave.Value);
    }

    private void Update()
    {
        if(canDetectKey && Input.anyKeyDown)
        {
            highScore.Value = score.Value;
            maxWave.Value = currentWave.Value;
            canDetectKey = false;
            controller.RestartGame();
        }
    }

    private IEnumerator GameOverRoutine()
    {
        yield return FadeIn(new Graphic[]{panel, gameOverLabel}, 0.5f);
        
        StartCoroutine(FadeIn(new Graphic[]{scoreLabel}, 0.5f));
        StartCoroutine(FillLabel(scoreLabel, score.Value, "Score: ", highScore.Value, highScoreLabel, 1.0f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(FadeIn(new Graphic[]{waveLabel}, 0.5f));
        StartCoroutine(FillLabel(waveLabel, currentWave.Value, "Waves: ", maxWave.Value, maxWaveLabel, 1.0f));
        yield return new WaitForSeconds(0.7f);
        
        StartCoroutine(FadeIn(new Graphic[]{anyKeyLabel}, 0.5f));
        yield return new WaitForSeconds(0.5f);
        canDetectKey = true;
    }

    private IEnumerator FillLabel(TextMeshProUGUI label, float value, string prefix, float highScoreValue, TextMeshProUGUI otherLabel, float duration)
    {
        bool animated = false;
        if(duration > 0)
        {
            float delta = 1.0f / duration;
            float elapsed = 0;
            while(elapsed < duration)
            {
                int newVal = (int)(value * elapsed*delta);
                
                label.text = prefix + newVal;
                if(newVal >= highScoreValue && !animated)
                {
                    SetAlpha(otherLabel, 1.0f);
                    animated = true;
                }
                yield return new WaitForEndOfFrame();
                elapsed += Time.deltaTime;
            }   
        }

        label.text = prefix + value;
        if(!animated)
        {
            SetAlpha(otherLabel, 1.0f);
        }
    }

    private IEnumerator AnimateHighScore(TextMeshProUGUI label)
    {
        while(true)
        {
            float showDuration = 0.05f;
            float elapsed = 0;
            while(elapsed < showDuration)
            {
                float ratio = Mathf.Clamp01(elapsed / showDuration);
                SetAlpha(label, ratio);
                yield return new WaitForEndOfFrame();
                elapsed += Time.deltaTime;
            }
            
            yield return new WaitForSeconds(1.0f);
            SetAlpha(label, 0.0f);
            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator FadeIn(Graphic[] graphics, float fadeInDuration)
    {
        if(fadeInDuration > 0)
        {
            float delta = 1.0f / fadeInDuration;
            float elapsed = 0;
            while(elapsed < fadeInDuration)
            {
                foreach(Graphic graphic in graphics)
                {
                    SetAlpha(graphic, elapsed * delta);
                }
                yield return new WaitForEndOfFrame();
                elapsed += Time.deltaTime;
            }
        }

        foreach(Graphic graphic in graphics)
        {
            Color c = graphic.color;
            c.a = 1.0f;
            graphic.color = c;
        }
    }

    private void SetAlpha(Graphic graphic, float alpha)
    {
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }
}
