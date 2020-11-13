using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private PlayerStats basePlayerStats;

    [SerializeField]
    private ProjectileStats projectileStats;

    [SerializeField]
    private ProjectileStats baseProjectileStats;

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
    
    void Start()
    {
        playTestOptions.ResetOptions();
        playerStats.SetPlayerStats(basePlayerStats);
        projectileStats.SetProjectileStats(baseProjectileStats);
        GameTime.Instance.Start();

        transform.GetComponent<WaveSpawner>();
        spawner.OnWillSpawnWaveEvent += OnWillSpawnWave;

        score.OnChange += OnScoreChange;
        roundScore.OnChange += OnRoundScoreUpdate;
        collectedPickups.OnChange += OnCollectedPickupsUpdate;
    }

    private void OnDisable() 
    {
        score.OnChange -= OnScoreChange;
        roundScore.OnChange -= OnRoundScoreUpdate;
        collectedPickups.OnChange -= OnCollectedPickupsUpdate; 
        spawner.OnWillSpawnWaveEvent -= OnWillSpawnWave;
    }

    public void EndGame()
    {
        SceneManager.LoadScene(0,LoadSceneMode.Single);
        GameTime.Instance.Reset();
        roundScore.Reset();
        score.Reset();
        scoreRepresentation.Reset();
        roundScoreRepresentation.Reset();
        roundScore.Value = 0;
        score.Value = 0;
    }
    
    void Update()
    {    
        if(inputHandler.IsPauseDown())
        {
            pauseMenu.SwapPause();
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

    private void OnWillSpawnWave()
    {
        score.Value +=  roundScoreRepresentation.Value;
        collectedPickups.Value = 0;
        roundScore.Value = 0;
    }

}
