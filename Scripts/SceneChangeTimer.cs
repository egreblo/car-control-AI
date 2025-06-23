using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneChangeTimer : MonoBehaviour
{
    public float timerDuration = 30.0f;
    private float elapsedTime = 0.0f;
    public float timeMultiplier = 20;
    public float timeRemaining;
    public CarController controller;

    void Start(){
        if(controller.useNeuralNetwork){
            timerDuration *= timeMultiplier;
            Time.timeScale = timeMultiplier;
        }else{
             Time.timeScale = 1;
        }
    }

    void Update()
    {

        elapsedTime += Time.deltaTime;
        timeRemaining = timerDuration - elapsedTime;

        if(controller.useNeuralNetwork) timeRemaining /= timeMultiplier;

        if (elapsedTime >= timerDuration)
        {
            FindObjectOfType<CarController>().Death();
            FindObjectOfType<CarController>().SaveBestScore();

            if(controller.useNeuralNetwork) Time.timeScale = 1;

            SceneManager.LoadScene("ScoreScene");
        }
    }
}
