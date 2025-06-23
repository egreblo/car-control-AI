using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System;

using Random = UnityEngine.Random;
using System.IO;
using Newtonsoft.Json;

public class NNet : MonoBehaviour
{

    public Matrix<float> inputLayer = Matrix<float>.Build.Dense( 1, 3);
    public List<Matrix<float>> hiddenLayers = new();
    public Matrix<float> outputLayer = Matrix<float>.Build.Dense( 1, 2);
    public List<Matrix<float>> weights = new();
    public List<float> biases;
    public float fitness;

    public void Initialise (int hiddenLayerCount, int hiddenNeuronCount){

        biases = new();
        
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();

        for(int i = 0; i < hiddenLayerCount + 1; i++){

            Matrix<float> f = Matrix<float>.Build.Dense(1, hiddenLayerCount);

            hiddenLayers.Add(f);
            biases.Add(Random.Range(-1f, 1f));

            if(i == 0){
                Matrix<float> inputToH1 = Matrix<float>.Build.Dense( 3, hiddenNeuronCount);
                weights.Add(inputToH1);
            }else{
                Matrix<float> hiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronCount, hiddenNeuronCount);
                weights.Add(hiddenToHidden);
            }
        }

        Matrix<float> OutputWeight = Matrix<float>.Build.Dense(hiddenNeuronCount, 2);
        weights.Add(OutputWeight);
        biases.Add(Random.Range(-1f,1f));

        RandomiseWeights();
    }

    public void RandomiseWeights(){

        for(int i = 0; i < weights.Count; i++){
            for(int j = 0; j < weights[i].RowCount; j++){
                for(int k = 0; k < weights[i].ColumnCount; k++){
                    weights[i][j,k] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    //Input sensors
    public (float, float) RunNetwork(float a, float b, float c){

        inputLayer[0,0] = a;
        inputLayer[0,1] = b;
        inputLayer[0,2] = c;

        inputLayer = inputLayer.PointwiseTanh();

        hiddenLayers[0] = ((inputLayer * weights[0]) + biases[0]).PointwiseTanh();

        for(int i = 1; i < hiddenLayers.Count; i++){

            hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[hiddenLayers.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh();

        //acceleration and steering

        return (Sigmoid(outputLayer[0,0]),  (float)Math.Tanh(outputLayer[0,1]));
    }

    private float Sigmoid(float s){

        return (1 / (1 + Mathf.Exp(-s)));
    }

    public NNet InitialiseCopy(int hiddenLayerCount, int hiddenNeuronCount){

        NNet n = new();

        List<Matrix<float>> newWeights = new List<Matrix<float>>();
        for(int i = 0; i < this.weights.Count ; i++){

            Matrix<float> currentWeight = Matrix<float>.Build.Dense(weights[i].RowCount, weights[i].ColumnCount);

            //Copy weights matrix -> OPTIMISE THIS!
            for(int x = 0; x < currentWeight.RowCount; x++){
                for(int y = 0; y < currentWeight.ColumnCount; y++){
                    currentWeight[x,y] = weights[i][x,y];
                }
            }
            newWeights.Add(currentWeight);
        }

        List<float> newBiases = new List<float>();

        newBiases.AddRange(this.biases);

        n.weights = newWeights;
        n.biases = newBiases;

        n.InitialiseHidden(hiddenLayerCount, hiddenNeuronCount);

        return n;
    }

    public void InitialiseHidden(int hiddenLayerCount, int hiddenNeuronCount){

        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();

        for(int i = 0; i < hiddenLayerCount + 1 ; i++){
            Matrix<float> newHiddenLayer = Matrix<float>.Build.Dense(1, hiddenLayerCount);
            hiddenLayers.Add(newHiddenLayer);
        }
    }


        public void SaveNetwork()
    {
        //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string path = Path.Combine(Application.dataPath);

        if (!System.IO.Directory.Exists(path + "\\SavedBrains"))
        {
            Directory.CreateDirectory(path + "\\SavedBrains");
        }
        path = path + "\\SavedBrains";
        string ID = DateTime.Now.Ticks.ToString();
        Directory.CreateDirectory(path + "\\mutation_" + ID);
        path = path + "\\mutation_" + ID;
        string json = JsonConvert.SerializeObject(inputLayer.ToArray());
        File.WriteAllText(path + "\\inputLayer_" + ID + ".txt", json);
        json = JsonConvert.SerializeObject(hiddenLayers.ToArray());
        File.WriteAllText(path + "\\hiddenLayers_" + ID + ".txt", json);
        json = JsonConvert.SerializeObject(outputLayer.ToArray());
        File.WriteAllText(path + "\\outputLayers_" + ID + ".txt", json);
        json = JsonConvert.SerializeObject(weights.ToArray());
        File.WriteAllText(path + "\\weights_" + ID + ".txt", json);
        json = JsonConvert.SerializeObject(biases.ToArray());
        File.WriteAllText(path + "\\biases_" + ID + ".txt", json);
    }

}
