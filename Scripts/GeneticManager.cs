using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System.Linq;

public class GeneticManager : MonoBehaviour
{
    
    [Header("References")]
    public CarController controller;

    [Header("Controls")]
    public int initalPopulation = 12;

    [Range(0.0f, 1.0f)]
    public float mutationRate = 0.055f;

    [Header("Crossover Control")]
    public int bestAgentSelection = 8;
    public int worstAgentSelection = 3;
    public int numberToCrossover;
    
    public List<int> genePool = new List<int>();
    private int naturallySelected;
    private NNet[] population;

    [Header("Public View")]
    public int currentGeneration;
    public int currentGenome;
    public List<float> generationAvgFitnessList = new();
    public List<float> fitnessList = new();
    public float averageGenerationFitness;

    private void Start(){
        currentGenome = 0;
        averageGenerationFitness = 0.0f;
        CreatePopulation();
    }

    private void CreatePopulation(){

        population = new NNet[initalPopulation];
        FillPopulationWithRandomValues(population, 0);
        ResetToCurrentGenome();
    }
    
    private void ResetToCurrentGenome(){

        controller.ResetWithNetwork(population[currentGenome]);
    }

    private void FillPopulationWithRandomValues(NNet[] newPopulation, int startingIndex ){

        while(startingIndex < initalPopulation){

            newPopulation[startingIndex] = new();
            newPopulation[startingIndex].Initialise(controller.layers, controller.neurons);
            startingIndex++;
        }
    }

    public void Death ( float fitness, NNet net){

        
        //Debug.Log(currentGenome + " : " + population.Length);
        if (currentGenome < population.Length - 1){
            
            //Debug.Log("+1");
            population[currentGenome].fitness = fitness;
            currentGenome = currentGenome + 1;
            ResetToCurrentGenome();
        }else{
            //Debug.Log("Repopulate");
            Repopulate();
        }
    }
    public void Repopulate(){

        AverageFitnessLogic();

        genePool.Clear();
        currentGeneration++;
        naturallySelected = 0;
        averageGenerationFitness = 0;
        SortPopulation();

        NNet[] newPopulation = PickBestPopulation();

        Crossover(newPopulation);
        Mutate(newPopulation);

        FillPopulationWithRandomValues(newPopulation, naturallySelected);

        population = newPopulation;

        currentGenome = 0;
        ResetToCurrentGenome();

    }

    private void Crossover(NNet[] newPopulation){

        for(int i = 0; i < numberToCrossover ; i += 2 ){

            int A_Index = i;
            int B_Index = i + 1;

            if(genePool.Count >= 1){
                for(int j = 0; j < 100; j++){
                    A_Index = genePool[Random.Range(0, genePool.Count)];
                    B_Index = genePool[Random.Range(0, genePool.Count)];

                    if(A_Index != B_Index){
                        break;
                    }
                }
            }

            NNet child1 = new();
            NNet child2 = new();

            child1.Initialise(controller.layers, controller.neurons);
            child2.Initialise(controller.layers, controller.neurons);

            child1.fitness = 0;
            child2.fitness = 0;

            for(int w = 0; w < child1.weights.Count ; w++){
                if(Random.Range(0.0f, 1.0f) < 0.5f){
                    child1.weights[w] = population[A_Index].weights[w];
                    child2.weights[w] = population[B_Index].weights[w];
                }else{
                    child1.weights[w] = population[B_Index].weights[w];
                    child2.weights[w] = population[A_Index].weights[w];
                }
            }

            for(int w = 0; w < child1.biases.Count ; w++){
                if(Random.Range(0.0f, 1.0f) < 0.5f){
                    child1.biases[w] = population[A_Index].biases[w];
                    child2.biases[w] = population[B_Index].biases[w];
                }else{
                    child1.biases[w] = population[B_Index].biases[w];
                    child2.biases[w] = population[A_Index].biases[w];
                }
            }

            newPopulation[naturallySelected] = child1;
            naturallySelected++;
            newPopulation[naturallySelected] = child2;
            naturallySelected++;
            

        }
    }

    private void Mutate(NNet[] newPopulation){

        for(int i = 0; i < naturallySelected; i++){
            for(int j = 0; j < newPopulation[i].weights.Count; j++){
                if(Random.Range(0.0f, 1.0f) < mutationRate){

                    newPopulation[i].weights[j] = MutateMatrix(newPopulation[i].weights[j]);

                }
            }
        }

    }


    public Matrix<float> MutateMatrix(Matrix<float> A){

        int randomPoints = Random.Range(1 , (A.RowCount * A.ColumnCount) / 7);

        Matrix<float> C = A;

        for(int i = 0; i < randomPoints; i++){
            int randomColumn = Random.Range(0, C.ColumnCount);
            int randomRow = Random.Range(0, C.RowCount);

            C[randomRow,randomColumn] = Mathf.Clamp( C[randomRow,randomColumn] + Random.Range(0.0f, 1.0f), -1.0f, 1.0f);
        }

        return C;
    }

    private void SortPopulation(){
        for(int i = 0; i< population.Length - 1 ; i++){
            for(int j = i + 1 ; j < population.Length ; j++){
                if(population[i].fitness < population[j].fitness){
                    NNet temp = population[i];
                    population[i] = population[j];
                    population[j] = temp;
                }
            }
        }
    }

    public NNet[] PickBestPopulation(){

        NNet[] newPopulation = new NNet[initalPopulation];

        for(int i = 0; i < bestAgentSelection ; i++){

            newPopulation[naturallySelected] = population[i].InitialiseCopy(controller.layers, controller.neurons);
            newPopulation[naturallySelected].fitness = 0;
            naturallySelected++;

            int f = Mathf.RoundToInt(population[i].fitness * 10);
            for(int j = 0; j < f; j++){
                genePool.Add(i);
            }
        }

        for(int i = 0; i < worstAgentSelection - 1; i++){

            int last = population.Length - 1;
            last -= i;

            int f = Mathf.RoundToInt(population[i].fitness * 10);
            for(int j = 0; j < f; j++){
                genePool.Add(last);
            }
        }

        return newPopulation;
    }

    public void CalculateAvgFitness(float overallFitness,NNet nnet){
        fitnessList.Add(overallFitness);
        averageGenerationFitness = fitnessList.Sum() / fitnessList.Count;
        //Debug.Log("avg: " +averageGenerationFitness);
    }

    public void AverageFitnessLogic(){ 

        generationAvgFitnessList.Add(averageGenerationFitness);
        fitnessList.Clear();

    }
}
