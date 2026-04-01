using System.Collections.Generic;
using UnityEngine;

namespace StoreSimulator.BuildingSystem
{
    public static class PlacementSystem
    {
        // all building grid math 
        public static Vector3 CalculateVisualPositions(
                BuildingGridCell baseCell,
                Vector2Int size,
                Vector2 slotSize
            )
        {
            Vector3 offset = new Vector3
            (
                (size.x - 1) * slotSize.x / 2f,
                0,
                (size.y - 1) * slotSize.y / 2f
            );

            return baseCell.worldPosittion + offset;
        }

        public static List<Vector2Int> GetFootprint(
                Vector2Int origin,
                Vector2Int size
            )
        {
            List<Vector2Int> cells = new();

            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                {
                    cells.Add(origin + new Vector2Int(x, y));
                }

            return cells;
        }

        public static Vector2Int GetRotatedSize(IBuildable buildable, float currentRotationY)
        {
            if (currentRotationY % 180 == 0) return buildable.Size;

            return new Vector2Int(buildable.Size.y, buildable.Size.x);
        }
    }
}