﻿using System;

namespace NeuralSharp
{
    public partial class Model
    {
        public void InitializeParametersXavier()
        {

            float range;
            for (int i = _layers.Count - 1; i > 0; i--)
            {
                range = (float) Math.Sqrt(6f / (_layers[i].InputShape.Item1 + _layers[i].OutputShape.Item1));
                
                _layers[i].Connect(_layers[i - 1]);
                _layers[i].InitializeRandomWeights(range);
                _layers[i].InitializeZeroBiases();
            }
            
            range = (float) Math.Sqrt(6f / (_layers[0].InputShape.Item1 + _layers[0].OutputShape.Item1));

            _layers[0].InitializeRandomWeights(range);
            _layers[0].InitializeZeroBiases();
        }
    }
}