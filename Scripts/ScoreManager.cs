using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    
    public TextMeshProUGUI finalScore;
    public TextMeshProUGUI AIfinalScore;
    public TextMeshProUGUI resultText;
    public Button startAIbutton;
    void Update()
    {
        float bestScore = PlayerPrefs.GetFloat("BestScore", 0);
        finalScore.text = "BEST SCORE: " + bestScore.ToString("0");

        float AIbestScore = PlayerPrefs.GetFloat("AIBestScore", 0);
        AIfinalScore.text = "AI SCORE: " + AIbestScore.ToString("0");

        if(AIbestScore > 0){
           if (bestScore > AIbestScore)
            {
                resultText.text = "YOU WON!";
                resultText.color = Color.green;
            }
            else
            {
                resultText.text = "AI WON!";
                resultText.color = Color.red;
            }
            startAIbutton.gameObject.SetActive(false);
    
        } else{
            resultText.text = "No winner yet!";
            resultText.color = Color.white;
            startAIbutton.gameObject.SetActive(true);
        }
    }
}
