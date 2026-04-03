using System;


[Serializable]
public struct NeedsThreshold
{
    public float threshold;         // if value > threshold, use this state 
    public string stateName;        // debugging
    public float storeProbability;  // probability go to store
    public float needsChance;
    public int criticalityLevel;
}
