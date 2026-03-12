using UnityEngine;

[System.Serializable]
public struct Gauge
{
    public float Max;
    public float Current
    {
        get
        {
            return mCurrent;
        }
        set
        {
            value = Mathf.Clamp(value, 0, Max);
            mCurrent = value;
        }
    }

    public float Normalize
    {
        get
        {
            if(Max == 0)
            {
                return 0;
            }
            return Current / Max;
        }
    }

    private float mCurrent;

    public Gauge(float current, float max)
    {
        Max = max;
        current = Mathf.Clamp(current, 0, Max);
        mCurrent = current;
    }

    public Gauge(Gauge gauge) : this(gauge.Current, gauge.Max)
    {

    }

    public Gauge(float current, Gauge guage) : this(current, guage.Max)
    {

    }

    public Gauge(Gauge gauge, float max) : this(gauge.Current, max)
    {
    }

    public Gauge AddGuage(float amount)
    {
        Current += amount;
        return new Gauge(this);
    }
     public Gauge MaximizeGauge()
    {
        Current = Max;
        return new Gauge(this);
    }
}
