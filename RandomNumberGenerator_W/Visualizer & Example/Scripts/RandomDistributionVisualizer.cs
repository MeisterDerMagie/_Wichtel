//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Wichtel;
using Wichtel.Extensions;
using Random = UnityEngine.Random;

namespace _Spielplatz.RandomNumberGenerator {
public class RandomDistributionVisualizer : MonoBehaviour
{
    public RawImage rawImage;
    [SerializeField, BoxGroup("Settings")] public int resolution = 400;
    [SerializeField, BoxGroup("Settings")] public int sampleHeight = 100;
    [SerializeField, BoxGroup("Settings")] public Color backgroundColor = Color.clear, sampleColor = Color.green, borderColor = Color.cyan;
    
    [SerializeField] private List<float> numbers = new List<float>();
    private float low = 0f, high = 1f;

    public void Visualize(List<float> _numbers, float _low = 0f, float _high = 1f)
    {
        //copy List
        numbers = new List<float>(_numbers);
        
        int width = resolution;
        int height = resolution;//Mathf.Max(numbers);
        Texture2D tex = new Texture2D(width, height);
        
        //clear texture
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                tex.SetPixel(x, y, backgroundColor);
            }
        }

        //draw samples
        foreach (var number in numbers)
        {
            var xPosition = MathW.Remap(number, _low, _high, 1f, width - 2f);
            int xPositionInt = Mathf.RoundToInt(xPosition);
            
            for (int i = -sampleHeight/2; i < sampleHeight/2; i++)
            {
                var currentPixelAlpha = tex.GetPixel(xPositionInt, height / 2 + i).a; 
                tex.SetPixel(xPositionInt, height/2 + i, sampleColor.With(a: currentPixelAlpha + sampleColor.a));
            }
        }
        
        //draw border
        for (int i = -sampleHeight; i < sampleHeight; i++)
        {
            tex.SetPixel(0, height/2 + i, borderColor);
            tex.SetPixel(width-1, height/2 + i, borderColor);
        }

        // apply some settings to make tex look better
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        rawImage.texture = tex;
    }

    [Button]
    private void Refresh()
    {
        Visualize(numbers);
    }
}
}