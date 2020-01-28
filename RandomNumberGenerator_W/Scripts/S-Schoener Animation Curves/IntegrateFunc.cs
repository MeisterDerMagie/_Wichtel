using UnityEngine;

/// <summary>
/// Provides a numerically integrated version of a function.
/// </summary>
public class IntegrateFunc {
    private System.Func<float, float> _func;
    private float[] _values;
    private float _from, _to;

    /// <summary>
    /// Integrates a function on an interval. Use the steps parameter to control
    /// the precision of the numerical integration. Larger step values lead to
    /// better precision.
    /// </summary>
    public IntegrateFunc(System.Func<float, float> func,
        float from, float to, int steps) {
        _values = new float[steps + 1];
        _func = func;
        _from = from;
        _to = to;
        ComputeValues();
    }

    private void ComputeValues() {
        int n = _values.Length;
        float segment = (_to - _from) / (n - 1);
        float lastY = _func(_from);
        float sum = 0;
        _values[0] = 0;
        for (int i = 1; i < n; i++) {
            float x = _from + i * segment;
            float nextY = _func(x);
            sum += segment * (nextY + lastY) / 2;
            lastY = nextY;
            _values[i] = sum;
        }
    }

    /// <summary>
    /// Evaluates the integrated function at any point in the interval.
    /// </summary>
    public float Evaluate(float x) {
        Debug.Assert(_from <= x && x <= _to);
        float t = Mathf.InverseLerp(_from, _to, x);
        int lower = (int) (t * _values.Length);
        int upper = (int) (t * _values.Length + .5f);
        if (lower == upper || upper >= _values.Length)
            return _values[lower];
        float innerT = Mathf.InverseLerp(lower, upper, t * _values.Length);
        return (1 - innerT) * _values[lower] + innerT * _values[upper];
    }

    /// <summary>
    /// Returns the total value integrated over the whole interval.
    /// </summary>
    public float Total {
        get {
            return _values[_values.Length - 1];
        }
    }
}