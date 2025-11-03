using System;
using System.Collections.Generic;
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

        public static List<MoveData> GetAvailableMoves(ColorType colorType, PieceType pieceType, Piece2D[,] board, int x, int y)
        {
            switch (pieceType)
            {
                case PieceType.Pawn: 
                    return GetPawnMoves(colorType, board, x, y);
                case PieceType.Rook:
                    return GetRookMoves(colorType, board, x, y);
                case PieceType.Knight:
                    return GetKnightMoves(colorType, board, x, y);
                case PieceType.Bishop:
                    return GetBishopMoves(colorType, board, x, y);
                case PieceType.Queen:
                    return GetQueenMoves(colorType, board, x, y);
                case PieceType.King:
                    return GetKingMoves(colorType, board, x, y);
                // case PieceType.Bomb:
                //     break;
                default:
                    return new List<MoveData>();
            }
        }

        private static List<MoveData> GetRookMoves(ColorType colorType, Piece2D[,] board, int x, int y)
        {
            List<MoveData> moves = new();

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            for (int dir = 0; dir < 4; dir++)
            {
                int nx = x;
                int ny = y;

                while (true)
                {
                    nx += dx[dir];
                    ny += dy[dir];
                    if (!IsInside(nx, ny)) break;

                    if (board[nx, ny] == null)
                    {
                        moves.Add(new MoveData(nx, ny));
                    }
                    else
                    {
                        if (CanEat(colorType, board[nx, ny].colorType))
                            moves.Add(new MoveData(nx, ny, true));
                       
                        break;
                    }
                }
            }
            
            return moves;
        }
        
        private static List<MoveData> GetPawnMoves(ColorType colorType, Piece2D[,] board, int x, int y)
        {
            List<MoveData> moves = new();
            int dir = colorType == ColorType.White ? 1 : -1;

            int forwardX = x + dir;
            
            // Ăn chéo
            foreach (int dy in new int[] { -1, 1 })
            {
                int ny = y + dy;
                if (IsInside(forwardX, ny))
                {
                    var target = board[forwardX, ny];
                    if (target == null)
                    {
                        moves.Add(new MoveData(forwardX, ny));
                    }
                    else
                    {
                        if (CanEat(colorType, target.colorType))
                            moves.Add(new MoveData(forwardX, ny, true));
                    }
                }
            }

            return moves;
        }
        
        private static List<MoveData> GetKnightMoves(ColorType colorType, Piece2D[,] board, int x, int y)
        {
            List<MoveData> moves = new();
            int[] dx = { 1, 2, 2, 1, -1, -2, -2, -1 };
            int[] dy = { 2, 1, -1, -2, -2, -1, 1, 2 };

            for (int i = 0; i < 8; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];
                if (!IsInside(nx, ny)) continue;

                var target = board[nx, ny];
                if (target == null)
                {
                    moves.Add(new MoveData(nx, ny));
                }
                else
                {
                    if (CanEat(colorType, target.colorType))
                    {
                        moves.Add(new MoveData(nx, ny, true));
                    }
                }
            }

            return moves;
        }
        
        private static List<MoveData> GetBishopMoves(ColorType colorType, Piece2D[,] board, int x, int y)
        {
            List<MoveData> moves = new();
            Vector2Int[] dirs = new Vector2Int[]
            {
                new Vector2Int(1, 1),
                new Vector2Int(-1, 1),
                new Vector2Int(-1, -1),
                new Vector2Int(1, -1)
            };

            AddLinearMoves(colorType, board, moves, dirs, x, y);
            return moves;
        }

        private static List<MoveData> GetQueenMoves(ColorType colorType, Piece2D[,] board, int x, int y)
        {
            List<MoveData> moves = new();
            Vector2Int[] dirs = new Vector2Int[]
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(1, 1), new Vector2Int(-1, 1),
                new Vector2Int(-1, -1), new Vector2Int(1, -1)
            };

            AddLinearMoves(colorType, board, moves, dirs, x, y);
            return moves;
            
        }

        private static List<MoveData> GetKingMoves(ColorType colorType, Piece2D[,] board, int x, int y)
        {
            List<MoveData> moves = new();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (IsInside(nx, ny))
                    {
                        var target = board[nx, ny];
                        if (target == null)
                        {
                            moves.Add(new MoveData(nx, ny));
                        }
                        else
                        {
                            if (CanEat(colorType, target.colorType))
                            {
                                moves.Add(new MoveData(nx, ny, true));
                            }
                        }
                    }
                }
            }

            return moves;
        }
        
        private static void AddLinearMoves(ColorType colorType, Piece2D[,] board, List<MoveData> moves, Vector2Int[] directions, int x, int y)
        {
            foreach (var dir in directions)
            {
                int nx = x;
                int ny = y;

                while (true)
                {
                    nx += dir.x;
                    ny += dir.y;
                    if (!IsInside(nx, ny)) break;

                    var target = board[nx, ny];
                    if (target == null)
                    {
                        moves.Add(new MoveData(nx, ny));
                    }
                    else
                    {
                        if (CanEat(colorType, target.colorType))
                            moves.Add(new MoveData(nx, ny, true)); // có thể ăn

                        break; // gặp quân nào cũng phải dừng
                    }
                }
            }
        }
        
        public static bool CanEat(ColorType currentColor, ColorType otherColor)
        {
            switch (currentColor)
            {
                case ColorType.White:
                case ColorType.Blue:
                    return currentColor != otherColor;
                case ColorType.Yellow:
                case ColorType.Red:
                case ColorType.Green:
                default:
                    return true;
            }
        }
        
        public static bool IsInside(int x, int y)
        {
            return x is >= 0 and < 8 && y is >= 0 and < 8;
        }
    }
}