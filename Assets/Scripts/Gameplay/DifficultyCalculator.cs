using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using Facet.Combinatorics;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DifficultyCalculator : ScriptableObject
{
    public class WaveCountSet
    {
        public string WaveName = "";
        public int[] EntityCount = new int[0];

        public int Score = 0;

        public WaveCountSet(int[] combinations, ScoreAwarder[] enemies)
        {
            EntityCount = new int[enemies.Length];
            for(int i = 0; i < enemies.Length; i++)
            {
                EntityCount[i] = 0;
                foreach(int j in combinations)
                {
                    if(i == j)
                    {
                        EntityCount[i]++;
                    }
                }
            }

            Setup(enemies);
        }

        public WaveCountSet(ScoreAwarder[] enemyList, ScoreAwarder[] enemies)
        {
            EntityCount = new int[enemies.Length];
            for(int i = 0; i < enemies.Length; i++)
            {
                EntityCount[i] = 0;

                for(int j = 0; j < enemyList.Length; j++)
                {
                    if(enemyList[j] == enemies[i])
                    {
                        EntityCount[i]++;
                    }
                }
            }
            
            Setup(enemies);
        }

        private void Setup(ScoreAwarder[] enemies)
        {
            Score = GetScore(enemies);

            WaveName = "[" + Score + "] ";
            for(int i = 0; i < EntityCount.Length; i++)
            {
                if(EntityCount[i] <= 0)
                {
                    continue;
                }
                WaveName += enemies[i].name + "x" + EntityCount[i];
                if(i < EntityCount.Length-1)
                {
                    WaveName +=  ", ";
                }
            }
        }

        public override bool Equals(object obj) 
        {
            return this.WaveName.Equals(((WaveCountSet)obj).WaveName);
        }

        public override int GetHashCode() 
        {
            return WaveName.GetHashCode();
        }

        private int GetScore(ScoreAwarder[] enemies)
        {
            int score = 0;
            int len = Mathf.Min(enemies.Length, EntityCount.Length);
            
            for(int i = 0; i < len; i++)
            {
                score += (int)enemies[i].Score * EntityCount[i];
            }

            return score;
        }

        public List<ScoreAwarder> ConvertToList(ScoreAwarder[] enemies)
        {
            List<ScoreAwarder> awarders = new List<ScoreAwarder>();
            int len = Mathf.Min(enemies.Length, EntityCount.Length);
            for(int i = 0; i < len; i++)
            {
                for(int n = 0; n < EntityCount[i]; n++)
                {
                    awarders.Add(enemies[i]);
                }
            }

            return awarders;
        }
    }

    private class WaveCountSetComparer : IComparer<WaveCountSet>
    {

        private ScoreAwarder[] enemies;   
   
        public WaveCountSetComparer(ScoreAwarder[] enemies)
        {
            this.enemies = enemies;
        }

        public int Compare(WaveCountSet x, WaveCountSet y)
        {
            return y.Score - x.Score;
        }

    }

    
   public ScoreAwarder[] enemies;

    //public int maxEnemiesPerWave = 10;
    public int maxDifficultyCutOff = 800;   

    public AnimationCurve DifficultyMultiplierPerEnemy;
    public AnimationCurve DifficultyTierPerScore;

    public List<ScoreAwarder> GetNewDifficultyTierWave(int currentWave)
    {
        int score = EvaluateIntCurve(DifficultyTierPerScore, currentWave);
        Debug.Log($"Generating Wave - {currentWave} - Score {score}");
        return GenerateDifficultyTierWave(currentWave).ConvertToList(enemies);
    }

    public WaveCountSet GenerateDifficultyTierWave(int currentWave)
    {
        int score = EvaluateIntCurve(DifficultyTierPerScore, currentWave);
        return GenerateScoreWave(score);
    }

    private static int EvaluateIntCurve(AnimationCurve wave, int access)
    {
        var lastKey = wave.keys[wave.keys.Length-1];
        float toEval = Mathf.Clamp(access, 0, lastKey.time);
        return (int)wave.Evaluate(toEval);
    }
    
    public List<ScoreAwarder> GetNewScoreWave(int maxScore)
    {
        return GenerateScoreWave(maxScore).ConvertToList(enemies);
    }

    

    public WaveCountSet GenerateScoreWave(int maxScore)
    {
        float multiplier = 1.0f;
        int availableScore = Mathf.Min(maxScore, maxDifficultyCutOff);
        List<ScoreAwarder> chosen = new List<ScoreAwarder>();

        List<ScoreAwarder> possibleEnemies = new List<ScoreAwarder>();
        possibleEnemies.AddRange(enemies);

        
        Predicate<ScoreAwarder> checkScore = (ScoreAwarder awarder) => {return awarder.Score * multiplier <= availableScore; };
        possibleEnemies = UpdateScoreList(possibleEnemies, checkScore);

        while(possibleEnemies.Count > 0)
        {
            multiplier = EvaluateIntCurve(DifficultyMultiplierPerEnemy, chosen.Count+1);
            ScoreAwarder chosenAwarder = possibleEnemies[TimedBoundRandom.RandomInt(0, possibleEnemies.Count)];
            chosen.Add(chosenAwarder);
            availableScore -= (int)(chosenAwarder.Score*multiplier);
            
            possibleEnemies = UpdateScoreList(possibleEnemies, checkScore);
        }
        
        return new WaveCountSet(chosen.ToArray(), enemies);
    }

    public List<ScoreAwarder> UpdateScoreList(List<ScoreAwarder> awarders, Predicate<ScoreAwarder> validPredicate)
    {
        List<ScoreAwarder> toReturn = new List<ScoreAwarder>();
        foreach(ScoreAwarder awarder in awarders)
        {
            if(validPredicate(awarder))
            {
                toReturn.Add(awarder);
            }
        }

        return toReturn;
    }
    /*
    public void CalculateCombinations()
    {
        WaveCountSet[] combinations = GetEnemyCombinations();

        if(combinations.Length != 0)
        {
            CalculatedCombinations = GetEnemyCombinations();
            calculatedMaxEnemyWaves = maxEnemiesPerWave;
            calculatedFor = enemies;
        }
        

          Debug.Log("==================");
        foreach(WaveCountSet set in combinations)
        {
            Debug.Log(set.WaveName);
        }
        Debug.Log("==================");
    }

    private WaveCountSet[] GetEnemyCombinations()
    {
        int size = enemies.Length * enemies.Length * maxEnemiesPerWave;
        if(maxEnemiesPerWave > 5)
        {
            Debug.LogError("Cannot compute " + size + " number of combinations in time");
            return new WaveCountSet[0];
        }
        
        if(size == 0)
        {
            return new WaveCountSet[0];
        }

            List<int[]> indexList = new List<int[]>();

        int[] indexes = new int[enemies.Length];
        for(int i = 0; i < enemies.Length; i++)
        {
            indexes[i] = i;
        }

        for(int i = 1; i <= maxEnemiesPerWave; i++)
        {
            Variations<int> variations = new Variations<int>(indexes, maxEnemiesPerWave, GenerateOption.WithRepetition);
            foreach(IList<int> variation in variations)
            {
                List<int> temp = new List<int>(variation);
                    indexList.Add(temp.ToArray());
            }
        }

        HashSet<WaveCountSet> reducedCombinations = new HashSet<WaveCountSet>();
        foreach(int[] combination in indexList)
        {
            WaveCountSet set = new WaveCountSet(combination, enemies);
            
            if(set.Score >= maxDifficultyCutOff)
            {
                continue;
            }

            reducedCombinations.Add(set);
        }
        
        List<WaveCountSet> sortedCombinations = new List<WaveCountSet>(reducedCombinations);
        sortedCombinations.Sort(new WaveCountSetComparer(enemies));
        return sortedCombinations.ToArray();
    }
    */
}

#if UNITY_EDITOR
[CustomEditor(typeof(DifficultyCalculator))]
public class DifficultyCalculatorEditor : Editor
{
        DifficultyCalculator DCtarget;

        protected void OnEnable()
        {
            DCtarget = (DifficultyCalculator)target;
        }

        float scoreTest = 0;
        int waveGeneration = 1;

        public override void OnInspectorGUI ()
        {
           base.OnInspectorGUI();
          
           EditorGUILayout.Space();

           /*if(GUILayout.Button("Calculate Combinations"))
           {
               DCtarget.CalculateCombinations();
           }*/

           scoreTest = EditorGUILayout.FloatField("ScoreToGenerate", scoreTest);
           
           if(GUILayout.Button("Generate Score Wave"))
           {
               DifficultyCalculator.WaveCountSet wave = DCtarget.GenerateScoreWave((int)scoreTest);
               Debug.Log("======= Generated for score " + scoreTest + "=======");
               Debug.Log(wave.WaveName);
               
               Debug.Log("=============="); 
               
           }

            waveGeneration = EditorGUILayout.IntField("Tier Wave Generation", waveGeneration);

             if(GUILayout.Button("Generate Tier Wave"))
           {
               DifficultyCalculator.WaveCountSet wave = DCtarget.GenerateDifficultyTierWave(waveGeneration);
               Debug.Log("======= Generated for wave " + waveGeneration + "=======");
               Debug.Log(wave.WaveName);
               Debug.Log("=============="); 
               
           }
        }
}
#endif
