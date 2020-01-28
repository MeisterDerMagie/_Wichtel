//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using SimpleRNGLibrary;

namespace Wichtel {
[CreateAssetMenu(fileName = "Random", menuName = "Scriptable Objects/Random", order = 0)]
public class RandomW : ScriptableObject
{
    private enum SeedType {GenerateSeed, CustomSeed}
    private enum ProbabilityDistribution {Uniform, Custom}

    [SerializeField, BoxGroup("Seed Settings")] private SeedType seedType;
    [SerializeField, ShowIf("seedTypeIsCustom"), BoxGroup("Seed Settings")] private uint customSeed;

    [SerializeField, BoxGroup("Range Settings")] public float min = 0f;
    [SerializeField, BoxGroup("Range Settings")] public float max = 1f;
    
    [SerializeField, BoxGroup("Low Discrepancy Settings"), LabelText("Low Discrepancy (Grid & Jitter)")] private bool lowDiscrepancy;
    [SerializeField, ShowIf("lowDiscrepancy"), BoxGroup("Low Discrepancy Settings")] private int sections = 16;
    [SerializeField, ShowIf("lowDiscrepancy"), BoxGroup("Low Discrepancy Settings"), Range(0f, 1f)] private float jitterAmount = 1f;
    [SerializeField, ShowIf("lowDiscrepancy"), BoxGroup("Low Discrepancy Settings")] private List<float> lowDiscrepancyValues = new List<float>();

    [SerializeField, BoxGroup("Probability Distribution Settings")] private ProbabilityDistribution probabilityDistribution;
    [SerializeField, ShowIf("probabilityDistributionIsCustom"), BoxGroup("Probability Distribution Settings")] private AnimationCurve probabilityCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    

    //- for Odin Inspector: -
    private bool seedTypeIsCustom => (seedType == SeedType.CustomSeed);
    private bool probabilityDistributionIsCustom => (probabilityDistribution == ProbabilityDistribution.Custom);
    //- -
    
    [SerializeField, ReadOnly, BoxGroup("SimpleRNG Seeds")] private SimpleRNG simpleRng;
    private AnimationCurveSampler animationCurveSampler;
    
    
    [Button, HideInEditorMode]
    public void Initialize()
    {
        simpleRng = new SimpleRNG();
        animationCurveSampler = new AnimationCurveSampler(probabilityCurve);
        
        //Clear LowDiscrepancyList
        lowDiscrepancyValues.Clear();
        
        //Seed
        switch (seedType)
        {
            case SeedType.GenerateSeed: //generate random seed
                simpleRng.SetSeedFromSystemTime();
                break;
            case SeedType.CustomSeed: //use custom seed
                simpleRng.SetSeed(customSeed);
                break;
        }
    }


    #region Public Methods
    
    [Button, HideInEditorMode]
    public float NextFloat() //exclusive
    {
        var nextFloat = Next();
        //Debug.Log("nextFloat = " + nextFloat);
        return nextFloat;
    }
    
    [Button, HideInEditorMode]
    public int NextInt() //inclusive
    {
        var nextInt = Mathf.RoundToInt(Next());
        //Debug.Log("nextInt = " + nextInt);
        return nextInt;
    }
    
    #endregion

    private float Next()
    {
        float next = 0f;
        
        //-- Discrepancy --
        //- High Discrepancy ("normal" random numbers / white noise) -
        if(!lowDiscrepancy)
        {
            next = simpleRng.GetFloatRange(0f, 1f);
        }
        //- Low Discrepancy (Grid & Jitter) -
        else
        {
            next = NextLowDiscrepancyFloat();
        }

        //-- Probability Distribution --
        //- Custom Distribution -
        if (probabilityDistribution == ProbabilityDistribution.Custom)
        {
            next = animationCurveSampler.Sample(next);
        }
        //- else: Uniform Probability Distribution -

        //-- Min Max Range --
        next = MathW.Remap(next, 0f, 1f, min, max);
        
        //-- Return final value --
        return next;
    }
    
    private float NextLowDiscrepancyFloat()
    {
        if(lowDiscrepancyValues.Count == 0) GenerateLowDiscrepancyFloatPool();
        float nextValue = lowDiscrepancyValues[0];
        lowDiscrepancyValues.RemoveAt(0);
        return nextValue;
    }

    private void GenerateLowDiscrepancyFloatPool()
    {
        //create grid
        lowDiscrepancyValues.Clear();
        for (int i = 0; i < sections; i++)
        {
            float value = 1f / sections * (i + 0.5f);
            lowDiscrepancyValues.Add(value);
        }

        //add jitter
        float sampleSpace = 1f / sections; //space between the samples ("distance")
        for (int i = 0; i < lowDiscrepancyValues.Count; i++)
        {
            lowDiscrepancyValues[i] += simpleRng.GetFloatRange(-sampleSpace/2f, sampleSpace/2f) * jitterAmount;
        }
        
        //shuffle
        ShuffleList(lowDiscrepancyValues);
    }

    private void ShuffleList<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = simpleRng.GetIntRange(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
}