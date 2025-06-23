# 🏎️ Simulation of Car Control Using Machine Learning

This project is an interactive Unity-based simulation that explores how artificial intelligence can learn to control a car on a racetrack using neural networks and genetic algorithms.

## 🎮 About the Project

The simulation consists of two phases:

1. **Player Phase**:  
   The user controls a car using keyboard input and tries to complete a lap around the racetrack as quickly as possible. The best player score is saved.

2. **AI Phase**:  
   After the player finishes, an AI takes over. It starts with a population of randomly initialized neural networks that control the cars. The AI's goal is to learn how to drive and beat the player's score.

## 🧠 AI Architecture

- **Sensors**: The car is equipped with 3 forward-facing sensors (left, center, right) to detect distances to the track borders.
- **Neural Network**: Takes sensor values as input and outputs two values:
  - Acceleration
  - Steering angle
- **Learning Algorithm**: A **genetic algorithm** evolves the neural networks across generations by:
  - **Selection**: Choosing the best-performing agents
  - **Crossover**: Combining traits of parent agents
  - **Mutation**: Introducing small random changes

## 🛠️ Configuration Options

Users can tune the AI using the in-game Options screen:
- Neural network layer and neuron counts
- Genetic algorithm parameters (population size, mutation rate, elitism)

## 📊 Performance Tracking

The simulation logs performance metrics for each generation, such as:
- Total distance traveled
- Average speed
- Average fitness of the population

These can be exported and analyzed using external tools like MATLAB to visualize AI progress.

## 🧪 Technologies Used

- Unity Game Engine
- C#
- Blender (for modeling)
- Genetic Algorithm

## 📷 Screenshots

_(Add your own screenshots here)_



> Created by Erik Greblo  
> Master's Thesis – Simulation of Car Control Using Machine Learning
