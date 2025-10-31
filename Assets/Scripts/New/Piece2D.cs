using System.Collections.Generic;
using UnityEngine;

namespace New
{
    public enum ColorType
    {
        White,
        Black,
        Gold,
    }

    public struct MoveData
    {
        public readonly Vector2Int Index;
        public readonly bool CanEat;

        public MoveData(int x, int y, bool canEat = false)
        {
            Index = new Vector2Int(x, y);
            CanEat = canEat;
        }
        
        public MoveData(Vector2Int index, bool canEat)
        {
            Index = index;
            CanEat = canEat;
        }
    }
    
    public class Piece2D : MonoBehaviour
    {
        public ColorType colorType;
        public int x, y;
        
        public void SetPosition(int newX, int newY, Vector2 newPosition)
        {
            x = newX;
            y = newY;

            transform.position = newPosition;
        }

        public virtual List<MoveData> GetAvailableMoves(Piece2D[,] board)
        {
            return new List<MoveData>();
        }

        protected bool IsInside(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }
    }
}