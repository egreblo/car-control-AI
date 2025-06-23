using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using Newtonsoft;
using System;
using Newtonsoft.Json;
using System.Linq;

//[RequireComponent(typeof(NNet))]
public class CarController : MonoBehaviour
{
    public Vector3 startPosition, startRotation;
    private NNet network;
    [Range(-1f,1f)]
    public float acceleration,turn;
    private float friction = 0.1f;
    public float timeSinceStart = 0f;

    [Header("Fitness")]
    public float overallFitness;
    public float distanceMultiplier = 1.4f;
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;
    public float bestScore = 0.0f;

    [Header("Network options")]
    public bool useNeuralNetwork = false;
    public int layers = 1;
    public int neurons = 10;


    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    private float aSensor, bSensor, cSensor;



    private void Awake(){

        int check = PlayerPrefs.GetInt("usingNeuralNetwork", 0);
        useNeuralNetwork = check == 1;

        PlayerPrefs.SetFloat("AIBestScore", 0);
        PlayerPrefs.Save();

        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        lastPosition = startPosition;

        network = GetComponent<NNet>();
    }

    public void Reset(){
        timeSinceStart = 0f;
        totalDistanceTravelled = 0f;
        avgSpeed = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;

        acceleration = 0;
        turn = 0;
    }

    private void OnCollisionEnter(Collision collision){
        //Reset();
        //print("Collision: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Road hitbox")){
            Death();
        }
    }
    private void FixedUpdate() {

        InputSensors();
        lastPosition = transform.position;

        if( useNeuralNetwork == true) { 
            (acceleration, turn) = network.RunNetwork(aSensor, bSensor, cSensor);
            MoveCar( acceleration, turn);
            
        }else{
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");
            float accDamping = 0.3f;

            acceleration += verticalInput  * Time.deltaTime * accDamping;
            if (true || Mathf.Approximately(verticalInput, 0f)) {
                ApplyFriction();
            }
            if(acceleration > 1 ) acceleration = 1;
            if(acceleration > 0.2){
                turn = RandomWiggle(horizontalInput, acceleration);
            }else{
                turn = horizontalInput;
            }
            
            MoveCar( acceleration, turn);
        }
        timeSinceStart += Time.deltaTime;
        CalculateFitness();
    }
    private float currentWiggle = 0f;
    private float targetWiggle = 0f;
    private float wiggleSmoothTime = 0.25f;
    float RandomWiggle(float input, float acceleration) {
        float maxWiggle = 0.6f; 

        currentWiggle = Mathf.Lerp(currentWiggle, targetWiggle, Time.deltaTime / wiggleSmoothTime) * acceleration;

        if (Mathf.Abs(currentWiggle - targetWiggle) < 0.01f) {
            targetWiggle = UnityEngine.Random.Range(-maxWiggle, maxWiggle);
        }

        return input + currentWiggle;
    }

    void ApplyFriction() {
        acceleration -= friction * Time.deltaTime;

        if (acceleration < 0) {
            acceleration = 0;
        }
    }
    private void CalculateFitness(){
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled /timeSinceStart;

        overallFitness = (totalDistanceTravelled * distanceMultiplier + avgSpeed * avgSpeedMultiplier);

        if(timeSinceStart > 20 && overallFitness < 40){
            Death();
        }
    }

    private void InputSensors(){

        Vector3 a = transform.forward + transform.right;
        Vector3 b = transform.forward;
        Vector3 c = transform.forward - transform.right;
        
        Vector3 offset = new Vector3(-0.02f, 0, 0.02f); 
        Vector3 rayOrigin = transform.TransformPoint(offset);
        Ray r = new Ray(rayOrigin,a);

        //print("Ray : " + r);

        if (Physics.Raycast(r, out RaycastHit hit))
        {
            aSensor = hit.distance / 100;
            Debug.DrawLine(r.origin, hit.point, Color.green);
            Debug.DrawLine(hit.point, r.GetPoint(5), Color.red);
            //print("A : " + aSensor);
        }

        r.direction = b;

        if(Physics.Raycast(r, out hit)){
            bSensor = hit.distance;///100;
            Debug.DrawLine(r.origin, hit.point, Color.green);
            Debug.DrawLine(hit.point, r.GetPoint(20), Color.red);
            //print("B : " + bSensor);
        }

        r.direction = c;

        if(Physics.Raycast(r, out hit)){
            cSensor = hit.distance/100;
            Debug.DrawLine(r.origin, hit.point, Color.green);
            Debug.DrawLine(hit.point, r.GetPoint(5), Color.red);
            //print("C : " + cSensor);
        }

    }


    private Vector3 input;
    public void MoveCar(float v, float h){
        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, v * 11.4f), 0.02f);
        input = transform.TransformDirection(input);
        transform.position += input;

        transform.eulerAngles += new Vector3(0 , h * 90 * 0.02f, 0);
    }

    public void ResetWithNetwork(NNet net){
        network = net;
        Reset();
    }
    public void SaveCarNetwork(){
        network.SaveNetwork();
    }

    public void Death(){
        if(bestScore < overallFitness) bestScore = overallFitness;

        
        FindObjectOfType<AudioManager>().Play("CarCrash");
        GameObject.FindObjectOfType<GeneticManager>().CalculateAvgFitness(overallFitness, network);
        GameObject.FindObjectOfType<GeneticManager>().Death(overallFitness, network);
    }

    public void SaveBestScore()
    {
        if(useNeuralNetwork == true){
            PlayerPrefs.SetFloat("AIBestScore", bestScore);
            PlayerPrefs.Save();
        }
        else{   
            
            PlayerPrefs.SetFloat("BestScore", bestScore);
            PlayerPrefs.Save();
        }
    }
}
