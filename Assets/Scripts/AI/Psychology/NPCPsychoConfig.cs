using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCConfig", menuName = "Configs/NPC Psycho Config")]
public class NPCPsychoConfig : ScriptableObject
{
    [Header("Hunger Settings")]
    public List<NeedsThreshold> hungerThreshold;
    
    [Header("Water Settings")]
    public List<NeedsThreshold> thirstThreshold;

    [Header("Loyalty settings")]
    public float minLoyaltyToVisit = 20f;
}
