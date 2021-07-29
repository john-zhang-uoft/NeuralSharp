﻿using System;

namespace NeuralSharp
{
    /// <summary>
    /// Abstract layer class used as a template for every type of layer.
    /// </summary>
    public abstract class Layer
    {
        public Matrix Neurons { get; protected set; }
        public (int, int, int) InputShape { get; protected set; }
        public (int, int, int) OutputShape { get; protected set; }

        /// <summary>
        /// Has i rows and j columns, where i is the number of neurons in this layer
        /// and j is the number of neurons in the previous layer.
        /// The [i, j]-th element is the weight between the j-th input neuron to the i-th neuron of this layer.
        /// </summary>
        public Matrix Weights { get; protected set; }

        /// <summary>
        /// Has i rows, where i is the number of neurons in this layer.
        /// </summary>
        public Matrix Biases { get; protected set; }

        /// <summary>
        /// Applied to each neuron in the layer to finish calculating the brightness of each neuron.
        /// </summary>
        protected Func<float, float> ActivationFunction { get; }

        /// <summary>
        /// Stores the gradient of the cost function with respect to each neuron for backpropagation
        /// </summary>
        public Matrix Gradient { get; protected set; }
        
        public Matrix DeltaWeight { get; set; }
        
        /// <summary>
        /// Stores the gradient of the cost function with respect to each bias for backpropagation.
        /// This is equal to the NeuronGradient.
        /// </summary>
        public Matrix DeltaBias { get; set; }

        #region Constructors

        protected Layer()
        {
        } // Empty constructor for inheritance

        protected Layer((int, int, int) outputShape, ActivationFunctions activation)
        {
            OutputShape = outputShape;
            ActivationFunction = activation switch
            {
                ActivationFunctions.Sigmoid => Activations.Sigmoid,
                ActivationFunctions.Tanh => Activations.Tanh,
                ActivationFunctions.ReLU => Activations.ReLU,
                ActivationFunctions.None => Activations.None,
                _ => throw new InvalidOperationException("Unimplemented Activation Function")
            };
        }

        protected Layer((int, int, int) inputShape, (int, int, int) outputShape, ActivationFunctions activation)
        {
            InputShape = inputShape;
            OutputShape = outputShape;
            ActivationFunction = activation switch
            {
                ActivationFunctions.Sigmoid => Activations.Sigmoid,
                ActivationFunctions.Tanh => Activations.Tanh,
                ActivationFunctions.ReLU => Activations.ReLU,
                ActivationFunctions.None => Activations.None,
                _ => throw new InvalidOperationException("Unimplemented Activation Function")
            };
        }
        
        #endregion


        public abstract void FeedForward(Matrix inputs);

        public abstract void BackPropagate(Layer nextLayer, Matrix previousLayerNeurons, Matrix target, 
            Func<Matrix, Matrix, Matrix> dLossFunction);

        public void Connect(Layer previousLayer)
        {
            InputShape = previousLayer.OutputShape;
        }

        public void InitializeRandomWeights(float range)
        {
            Weights = Matrix.RandomMatrix(range, OutputShape.Item1, InputShape.Item1);
        }

        public void InitializeRandomBiases(float range)
        {
            Biases = Matrix.RandomMatrix(range, OutputShape.Item1, 1);
        }

        public void InitializeZeroBiases()
        {
            Biases = new Matrix((OutputShape.Item1, 1));
        }

        /// <summary>
        /// Returns true if there are no negative or zero values in the InputShape.
        /// </summary>
        /// <returns></returns>
        public bool IsValidInputShape()
        {
            return InputShape.Item1 >= 1 && InputShape.Item2 >= 1 && InputShape.Item3 >= 1;
        }

        /// <summary>
        /// Returns true if there are no negative or zero values in the OutputShape.
        /// </summary>
        /// <returns></returns>
        public bool IsValidOutputShape()
        {
            return OutputShape.Item1 >= 1 && OutputShape.Item2 >= 1 && OutputShape.Item3 >= 1;
        }
        
        public void ResetGradients()
        {
            Gradient = new Matrix(OutputShape.Item1, OutputShape.Item2);
            DeltaWeight = new Matrix(Weights.Shape.rows, Weights.Shape.cols);
            DeltaBias = new Matrix(Biases.Shape.rows, Biases.Shape.cols);
        }

        public void UpdateParameters(int batchSize, float alpha, float gamma)
        {
            Weights -= alpha / batchSize * DeltaWeight;
            Biases -= gamma / batchSize * DeltaBias;
        }

    }
}