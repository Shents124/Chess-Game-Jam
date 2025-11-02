using System.Collections.Generic;
using UnityEngine;

namespace New
{
    public class BombPiece : Piece2D
    {
        protected override void SetColor(Color color)
        {
            
        }

        public override List<MoveData> GetAvailableMoves(Piece2D[,] board)
        {
            return null;
        }
    }
}