using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AmoaebaUtils;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private Image grainImage;

    [SerializeField]
    private Image timeRatioImage;

    [SerializeField]
    private ColorScheme currentColors;
    
    [SerializeField]
    private FloatVar timeRatio;

    [SerializeField]
    private FloatVar timeVoyageRatio;

    [SerializeField]
    private PlayerStats currentPlayerStats;

    [SerializeField]
    private TextMeshProUGUI[] gameSpeedUIs;

    [SerializeField]
    private RectTransform ratioLine;

    private List<RectTransform> ratioLines = new List<RectTransform>();

    [SerializeField]
    private int linesPreloadedCount = 3;

    [SerializeField]
    private Image healthImage;

    [SerializeField]
    private FloatVar healthVar;

    [SerializeField]
    private Image timeVoyageImage;

    private void Awake() 
    {
        CreateLines(linesPreloadedCount);    
    }

    private void Start()
    {
        timeRatio.OnChange += OnTimeRatioChange;
        timeVoyageRatio.OnChange += OnTimeVoyageChange;
        GameTime.Instance.OnSpeedChangeEvent += OnSpeedChange;
        currentPlayerStats.OnChangeEvent += OnStatsChange;
        healthVar.OnChange += OnHealthChange;
        currentColors.OnChange += OnChangeColors;
        OnStatsChange();
    }

    private void OnDestroy() 
    {
        currentColors.OnChange -= OnChangeColors;
        timeRatio.OnChange -= OnTimeRatioChange;  
        timeVoyageRatio.OnChange -= OnTimeVoyageChange;
         GameTime.Instance.OnSpeedChangeEvent -= OnSpeedChange;
         currentPlayerStats.OnChangeEvent -= OnStatsChange;
         healthVar.OnChange -= OnHealthChange;
    }

    private void OnChangeColors()
    {
        UpdateHealth();
        UpdateClockLines();
    }

    private void OnTimeRatioChange(float oldVal, float newVal)
    {
        UpdateRollbackClock();
        bool isVoyage = GameTime.Instance.IsReversing;
        timeVoyageImage.gameObject.SetActive(isVoyage);
    }

    private void OnTimeVoyageChange(float oldVal, float newVal)
    {

    }

    private void OnHealthChange(float oldVal, float newVal)
    {
        UpdateHealth();
    }

    private void UpdateRollbackClock()
    {
        float totalTime = currentPlayerStats.RollbackCount * currentPlayerStats.RollbackTime;
        timeRatioImage.fillAmount = timeRatio.Value / totalTime;

        Color newColor = currentColors.GetColor(ColorType.RewindColor);
        newColor.a = timeRatioImage.color.a;
        timeRatioImage.color = newColor;
    }

    private Color GetColor(Color[] colors, float ratio)
    {
        if(colors.Length == 0)
        {
            return Color.magenta;
        }

        ratio = Mathf.Clamp01(ratio);
    
        if(ratio == 1)
        {
            return colors[colors.Length-1];
        }
        
        ratio = ratio * colors.Length;
        int lowIndex = (int)ratio;
        int highIndex = (lowIndex <= colors.Length-2) ? lowIndex+1 : lowIndex;
        return Color.Lerp(colors[lowIndex], colors[highIndex], ratio - lowIndex);
    }

    private void OnSpeedChange()
    {
        bool stopped = GameTime.Instance.IsStopped;
        bool reversing = GameTime.Instance.IsReversing;

        grainImage.gameObject.SetActive((reversing || stopped) && GameTime.Instance.HasStarted);
        
        gameSpeedUIs[0].gameObject.SetActive(stopped);
        gameSpeedUIs[1].gameObject.SetActive(!stopped);
        gameSpeedUIs[1].text = stopped ? " ▌▌" :
                               reversing ? "«" : "»";
    }


    private void OnStatsChange()
    {
        UpdateRollbackClock();
        OnSpeedChange();
        UpdateClockLines();
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        float ratio = Mathf.Clamp01(healthVar.Value / currentPlayerStats.Health);
        healthImage.fillAmount = ratio;
        Color newColor = currentColors.GetColor(ColorType.HealthColor);
        newColor.a = healthImage.color.a;
        healthImage.color = newColor;
    }


    private void CreateClockLine()
    {

    }

    private void UpdateClockLines()
    {
        int count = Mathf.Max(0, (int)currentPlayerStats.RollbackCount - 1);
        
        CreateLines(Mathf.Max(count - ratioLines.Count, 0));


        for(int i = 0; i < ratioLines.Count; i++)
        {
            if(i >= count)
            {
                ratioLines[i].gameObject.SetActive(false);
            }
            else
            {
                float curRatio = (float)(i+1) / (count+1);
                ratioLines[i].gameObject.SetActive(true);
                RectTransform lineTransform = ratioLines[i];
                RectTransform parentTransform = lineTransform.parent.GetComponent<RectTransform>();
                lineTransform.anchoredPosition = new Vector2(lineTransform.anchoredPosition.x,
                                                             parentTransform.rect.size.y * curRatio);
                
            }
        }
        
    }

    private void CreateLines(int toCreate)
    {
        for(int i = 0; i < toCreate; i++)
        {
            RectTransform newLine = Instantiate<RectTransform>(ratioLine, ratioLine.parent);
            newLine.gameObject.SetActive(true);
            ratioLines.Add(newLine);
        }
    }
}
