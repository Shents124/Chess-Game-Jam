using UnityEngine;

namespace New
{
    public static class GridHelper
    {
        /// <summary>
        /// Trả về vị trí góc trái dưới (bottom-left) của grid dựa vào center.
        /// </summary>
        private static Vector2 GetBottomLeft(int rows, int cols, float cellSize, float spacing, Vector2 center)
        {
            float totalWidth = cols * cellSize + (cols - 1) * spacing;
            float totalHeight = rows * cellSize + (rows - 1) * spacing;
            return center - new Vector2(totalWidth / 2f, totalHeight / 2f);
        }

        /// <summary>
        /// Tính tâm cell (row, col) trên mặt phẳng XY
        /// </summary>
        public static Vector2 GetCellCenter(int rows, int cols, float cellSize, float spacing, Vector2 center, int row,
            int col, float cellStep)
        {
            Vector2 bottomLeft = GetBottomLeft(rows, cols, cellSize, spacing, center);

            float x = bottomLeft.x + col * cellStep + cellSize / 2f;
            float y = bottomLeft.y + row * cellStep + cellSize / 2f;

            return new Vector2(x, y);
        }

        public static Vector2Int GetCellIndex(int rows, int cols, float cellSize, float spacing, Vector2 center,
            float cellStep, Vector2 position)
        {
            Vector2 bottomLeft = GetBottomLeft(rows, cols, cellSize, spacing, center);

            float dx = position.x - bottomLeft.x;
            float dy = position.y - bottomLeft.y;

            int col = Mathf.FloorToInt(dx / cellStep);
            int row = Mathf.FloorToInt(dy / cellStep);

            return new Vector2Int(row, col);
        }
    }
}