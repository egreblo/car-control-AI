using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator transition;

    public void LoadMenu(){
        SceneManager.LoadScene("MenuScene");
    }
    public void PlayGame(){

        PlayerPrefs.SetInt("usingNeuralNetwork", 0);
        PlayerPrefs.Save(); 
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //SceneManager.LoadScene("GameScene");
        LoadLevelWithName("GameScene");
    }

    public void NeuralNetPlayGame(){

        PlayerPrefs.SetInt("usingNeuralNetwork", 1);
        PlayerPrefs.Save(); 

        SceneManager.LoadScene("GameScene");
    }

    public void LoadLevelWithName(string levelName){
        StartCoroutine(LoadLevel(levelName));
    }
    IEnumerator LoadLevel(string levelName){
        Debug.Log("Loading: " + levelName);
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        Debug.Log("Loaded: " + levelName);
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame(){

        Debug.Log("QUIT");
        Application.Quit();
    }
}
