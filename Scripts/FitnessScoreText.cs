using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class FitnessScoreText : MonoBehaviour
{

    public CarController carController;
    public TextMeshProUGUI fitnessText;
    public TextMeshProUGUI geneticText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI genAvgFitnessText;
    public TextMeshProUGUI timeRemainingText;
    public GeneticManager geneticManager;
    public SceneChangeTimer sceneChangeTimer;

    void Update()
    {
        fitnessText.text = carController.overallFitness.ToString("0");
        timeRemainingText.text = sceneChangeTimer.timeRemaining.ToString("0");
        bestScoreText.text = "Top: " + carController.bestScore.ToString("0");

        geneticText.text = "";
        genAvgFitnessText.text = "";
        if(carController.useNeuralNetwork){
            
            float avgFitness = geneticManager.averageGenerationFitness;
            int generation = geneticManager.currentGeneration;
            int genome = geneticManager.currentGenome;
            geneticText.text = "Gen / gen : " + generation + " / " + genome;
            geneticText.text += "\nAvg fitness: " + avgFitness.ToString("F1");

            List<float> genAverageFitness = geneticManager.generationAvgFitnessList;
            genAvgFitnessText.text = null;
            for(int i = 0; i < genAverageFitness.Count; i++){
                genAvgFitnessText.text += "gen " + i + " : " + genAverageFitness[i].ToString("F2") + "\n";
            }

        }
    }
}
