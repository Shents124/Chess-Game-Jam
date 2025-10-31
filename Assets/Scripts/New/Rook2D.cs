using System.Collections.Generic;
using UnityEngine;

namespace New
{
    public class Rook2D : Piece2D
    {
        public override List<MoveData> GetAvailableMoves(Piece2D[,] board)
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
                        if (board[nx, ny].colorType != colorType)
                            moves.Add(new MoveData(nx, ny, true));
                        break;
                    }
                }
            }
            
            return moves;
        }
    }
}