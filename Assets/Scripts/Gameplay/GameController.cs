using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameTime gameTime;

    [SerializeField]
    private InputHandler inputHandler;

    [SerializeField]
    private PauseMenu pauseMenu;

    [SerializeField]
    private PlayTestOptions playTestOptions;

    [SerializeField]
    private FloatVar scoreRepresentation;
    
    [SerializeField]
    private FloatVar roundScoreRepresentation;

    [SerializeField]
    private TimelineFloatVar score;

    [SerializeField]
    private TimelineFloatVar roundScore;
    
    [SerializeField]
    private TimelineFloatVar collectedPickups;
    
    [SerializeField]
    private WaveSpawner spawner;

    [SerializeField]
    private PlayerStatsBalancer balancer;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private RollbackTimer rollbackTimer;
    
    [SerializeField]
    private GameObject recoverableDeathPopup;

    [SerializeField]
    private GameObject endGame;

    private bool IsGameRunning;

    void Start()
    {
        recoverableDeathPopup.SetActive(false);
        endGame.SetActive(false);
        playTestOptions.ResetOptions();

        GameTime.Instance.Start();

        transform.GetComponent<WaveSpawner>();
        spawner.OnWillSpawnWaveEvent += UpdateNewWaveScore;

        score.OnChange += OnScoreChange;
        roundScore.OnChange += OnRoundScoreUpdate;
        collectedPickups.OnChange += OnCollectedPickupsUpdate;
        
        ResetGame();
    }

    private void OnDisable() 
    {
        score.OnChange -= OnScoreChange;
        roundScore.OnChange -= OnRoundScoreUpdate;
        collectedPickups.OnChange -= OnCollectedPickupsUpdate; 
        spawner.OnWillSpawnWaveEvent -= UpdateNewWaveScore;
    }

    public void LesserDeath()
    {
        recoverableDeathPopup.SetActive(true);
    }

    public void EndGame()
    {
        UpdateNewWaveScore();
        GameTime.Instance.Stop();
        IsGameRunning = false;
        if(pauseMenu.IsPaused)
        {
            pauseMenu.SwapPause();
        }
        endGame.SetActive(true);
    }

    public void RestartGame()
    {
        ResetGame();
        SceneManager.LoadScene(0,LoadSceneMode.Single);
    }

    public void ResetGame()
    {
        
        GameTime.Instance.Reset();
        spawner.Reset();
        roundScore.Reset();
        score.Reset();
        scoreRepresentation.Reset();
        roundScoreRepresentation.Reset();
        roundScore.Value = 0;
        score.Value = 0;
        balancer.ResetStats();
        player.StartController();
        rollbackTimer.SetPercentage(playTestOptions.filledTimeBars);
        IsGameRunning = true;
    }
    
    void Update()
    {    
        if(IsGameRunning && inputHandler.IsPauseDown())
        {
            pauseMenu.SwapPause();
        }

        if(GameTime.Instance.IsReversing && recoverableDeathPopup.activeInHierarchy)
        {
            recoverableDeathPopup.SetActive(false);
        }
    }

    private void OnRoundScoreUpdate(float old, float newVal)
    {
        UpdateRoundScore();
    }

    private void OnCollectedPickupsUpdate(float old, float newVal)
    {
        UpdateRoundScore();
    }

    private void OnScoreChange(float old, float newVal)
    {
        scoreRepresentation.Value = score.Value;
    }

    private void UpdateRoundScore()
    {
        roundScoreRepresentation.Value = (int)(roundScore.Value / (collectedPickups.Value + 1));
    }

    public void UpdateNewWaveScore()
    {
        score.Value +=  roundScoreRepresentation.Value;
        collectedPickups.Value = 0;
        roundScore.Value = 0;
    }

}
