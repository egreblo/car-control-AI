using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public CarController carController;
    public int finishedLapRaward = 50;
    void OnTriggerEnter(){
        
        print("You reached the finish line! +50 fitness!");
        carController.overallFitness += finishedLapRaward;
        carController.SaveCarNetwork();
        carController.Death();
    }
}
