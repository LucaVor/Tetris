    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public List<Dictionary<string, float>> multipliers = new List<Dictionary<string, float>>();
    public Dictionary<int, int> scores = new Dictionary<int, int>();
    public int populationSize;
    public int generation = 0;

    [Range(0,100)]
    public int mutationChance = 50;
    [Range(10,100)]
    public int mutationRange = 10;

    public static GeneticAlgorithm instance;
    public bool initializedAgents = false;

    public int pointInPopulation = 1;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AgentUpdate()
    {
        if(!initializedAgents) {
            SetRandomModifiers();
            Debug.Log("Set new modifiers.");
        }

        bool newGeneration = pointInPopulation == populationSize;
        if(newGeneration) {
            generation += 1;
            Debug.Log("New Generation.");
            pointInPopulation = 1;
            
            int highestPoint = 0;
            int highestFitness = 0;

            foreach(KeyValuePair<int, int> score in scores) {
                if(score.Value > highestFitness) {
                    highestFitness = score.Value;
                    highestPoint = score.Key;
                }
            }
            Debug.Log("Got best agent. Point in population: " + highestPoint + $" Modifiers: " + 
                multipliers[highestPoint-1]["Holes"] + " : " + 
                multipliers[highestPoint-1]["Height"] + " : " + 
                multipliers[highestPoint-1]["Lines"] + " : " + 
                multipliers[highestPoint-1]["Bump"]
                );

            Dictionary<string, float> bestModifiers = multipliers[highestPoint-1];

            multipliers = new List<Dictionary<string, float>>();
            for(int i = 0; i < populationSize; i++) {
                multipliers.Add(bestModifiers);
            }

            Debug.Log("Replicated.");

            MututateAllModifiers();
            Debug.Log("Mutated new generation!");

            scores = new Dictionary<int, int>();

        } else {
            Debug.Log("New agent in generation! Adding score and updating modifiers.");
            scores.Add(pointInPopulation, TetrisGame.instance.linesCleared);

            TetrisGame.instance.holeMulti = multipliers[pointInPopulation - 1]["Holes"];
            TetrisGame.instance.largeHeightMulti = multipliers[pointInPopulation - 1]["Height"];
            TetrisGame.instance.linesValue = multipliers[pointInPopulation - 1]["Lines"];
            TetrisGame.instance.bumpMulti = multipliers[pointInPopulation - 1]["Bump"];

            pointInPopulation ++;
        }

        initializedAgents = true;
    }

    void SetRandomModifiers()
    {
        multipliers = new List<Dictionary<string, float>>();
        for(int i = 0; i < populationSize; i++) {
            Dictionary<string, float> modifiers = new Dictionary<string, float>();
            modifiers.Add("Holes", Random.Range(1, 100));
            modifiers.Add("Height", Random.Range(1, 100));
            modifiers.Add("Lines", Random.Range(-100, -1));
            modifiers.Add("Bump", Random.Range(1, 90));

            multipliers.Add(modifiers);
        }
    }

    void MututateAllModifiers()
    {
        for(int i = 0; i < populationSize; i++) {
            Dictionary<string, float> modifiers = multipliers[i];
            if(Random.Range(0,100) < mutationChance) modifiers["Holes"] += Random.Range((-mutationRange)/2, mutationRange/2);
            if(Random.Range(0,100) < mutationChance) modifiers["Height"] += Random.Range((-mutationRange)/2, mutationRange/2);
            if(Random.Range(0,100) < mutationChance) modifiers["Lines"] += Random.Range((-mutationRange)/2, mutationRange/2);
            if(Random.Range(0,100) < mutationChance) modifiers["Bump"] += Random.Range((-mutationRange)/2, mutationRange/2);

            modifiers["Holes"] = Mathf.Clamp(modifiers["Holes"], 1, 100);
            modifiers["Height"] = Mathf.Clamp(modifiers["Height"], 1, 100);
            modifiers["Lines"] = Mathf.Clamp(modifiers["Lines"], -100, -1);
            modifiers["Bump"] = Mathf.Clamp(modifiers["Bump"], 1, 90);

            multipliers[i] = modifiers;
        }
    }


}
