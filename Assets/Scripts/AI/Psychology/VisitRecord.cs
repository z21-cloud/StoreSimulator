using System;
using System.Collections.Generic;

[Serializable]
public struct VisitRecord
{
    public int dayIndex;
    public float totalSpent;
    public bool foundAllItems;    
    public PriceReactionType reactionType;
}
