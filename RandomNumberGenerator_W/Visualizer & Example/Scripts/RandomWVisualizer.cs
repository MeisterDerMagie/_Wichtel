//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Spielplatz.RandomNumberGenerator;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Wichtel;

public class RandomWVisualizer : MonoBehaviour
{
    private enum FloatOrInt {Float, Int}
    
    [SerializeField, BoxGroup("Settings"), Required] private RandomW randomW;
    [SerializeField, BoxGroup("Settings"), Required] private int sampleCount;
    [SerializeField, BoxGroup("Settings"), Required] private FloatOrInt sampleFloatOrInt;

    [SerializeField, FoldoutGroup("References"), Required] private RandomDistributionVisualizer visualizer;

    [SerializeField, FoldoutGroup("Samples"), ReadOnly] private List<float> samples = new List<float>();
    private List<float> samplesMapped = new List<float>();

    private void Awake()
    {
        randomW.Initialize();
        Visualize();
    }

    [Button, HideInEditorMode]
    private void Visualize()
    {
        samples.Clear();
        samplesMapped.Clear();
        
        for (int i = 0; i < sampleCount; i++)
        {
            var nextFloat = (sampleFloatOrInt == FloatOrInt.Float) ? randomW.NextFloat() : randomW.NextInt();
            samples.Add(nextFloat);
            nextFloat = MathW.Remap(nextFloat, randomW.min, randomW.max, 0f, 1f);
            samplesMapped.Add(nextFloat);
        }
        
        visualizer.Visualize(samplesMapped);

        var minValue = samples.Min(t => t);
        var maxValue = samples.Max(t => t);
        Debug.Log("min value is: " + minValue + ", max value is: " + maxValue);
    }
}