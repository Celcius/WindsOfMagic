using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickupText : MonoBehaviour
{
    [SerializeField]
    private ColorScheme currentColors;

    [SerializeField]
    private ColorType topColor;
    
    [SerializeField]
    private ColorType botColor;

    [SerializeField]
    private TextMeshPro topLabel;

    [SerializeField]
    private TextMeshPro botLabel;

    [SerializeField]
    private float moveDistance = 10;

    [SerializeField]
    private float moveTime = 3.0f;

    [SerializeField]
    private float fadeOutStartRatio = 0.85f;

    [SerializeField]
    private float fadeInEndRatio = 0.1f;

    [SerializeField]
    private AnimationCurve moveProfile;


   private void Start()
    {
        if(currentColors != null)
        {
            currentColors.OnChange -= this.ApplyColor;  
            currentColors.OnChange += this.ApplyColor;
        }
        ApplyColor();

        StartCoroutine(MoveRoutine());
    }

    public void SetLabels(string topText, string botText)
    {
        SetLabel(topLabel, topText);
        SetLabel(botLabel, botText);
    }

    private void SetLabel(TextMeshPro label, string text)
    {
        if(text == null || string.Empty == text)
        {
            label.gameObject.SetActive(false);
        }
        label.text = text;
    }

    private IEnumerator MoveRoutine()
    {
        float elapsed = 0;
        float fadeInStart = fadeInEndRatio * moveTime;
        float fadeOutStart = fadeOutStartRatio * moveTime;
        Vector3 position = transform.position;

        while (elapsed < moveTime)
        {
            float moveRatio = Mathf.Clamp01(elapsed/moveTime);

            float alphaRatio = elapsed <= fadeInStart ? Mathf.Clamp01(elapsed / fadeInStart) :
                               elapsed >= fadeOutStart ? 1.0f - Mathf.Clamp01((elapsed - fadeOutStart) / (moveTime - fadeOutStart)) 
                               : 1.0f;
            transform.position = position+ Vector3.up * moveProfile.Evaluate(moveRatio) * moveDistance;
            ApplyColor(alphaRatio);

            yield return new WaitForEndOfFrame();
            elapsed += GameTime.Instance.DeltaTime;
        }
        Destroy(this.gameObject);
        
    }

    private void OnDestroy() 
    {
        currentColors.OnChange -= this.ApplyColor;    
    }

    private void ApplyColor()
    {
        ApplyColor(1.0f);
    }
    private void ApplyColor(float alpha)
    {
        ApplyColorToLabel(topLabel, topColor, alpha);
        ApplyColorToLabel(botLabel, botColor, alpha);
    }

    private void ApplyColorToLabel(TextMeshPro label, ColorType color, float alpha)
    {
        Color applyTopColor = currentColors.GetColor(color);
        applyTopColor.a = alpha;
        label.color = applyTopColor;
    }

#if UNITY_EDITOR
    private void OnAwake()
    {
        currentColors.OnChange -= this.ApplyColor;  
        currentColors.OnChange += this.ApplyColor;
        ApplyColor();
    }

    private void OnValidate ()
    {
        ApplyColor();
    }
#endif
}
