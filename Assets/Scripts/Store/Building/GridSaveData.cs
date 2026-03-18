using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridSaveData
{
    public List<CellSaveData> cells = new List<CellSaveData>();
}

[System.Serializable]
public class CellSaveData
{
    public int x;
    public int y;
    public string buildableID;
}
