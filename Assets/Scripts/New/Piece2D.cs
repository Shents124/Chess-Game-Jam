using System.Collections.Generic;
using UnityEngine;

namespace New
{
    public enum ColorType
    {
        White,
        Blue,
        Yellow,
        Red,
        Green
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
        public SpriteRenderer spriteRenderer;
        public ColorType colorType;
        public int x, y;
        public PieceType pieceType;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(int x, int y, ColorType colorType, PieceType pieceType, Sprite sprite, Vector2 position)
        {
            this.x = x;
            this.y = y;
            this.colorType = colorType;
            spriteRenderer.sprite = sprite;
            this.pieceType = pieceType;
            SetColor();
            transform.position = position;
        }

        protected virtual void SetColor()
        {
            switch (colorType)
            {
                case ColorType.White:
                    spriteRenderer.color = Color.white;
                    break;
                case ColorType.Blue:
                    spriteRenderer.color = Color.blue;
                    break;
                case ColorType.Yellow:
                    spriteRenderer.color = Color.yellow;
                    break;
                case ColorType.Red:
                    spriteRenderer.color = Color.red;
                    break;
                case ColorType.Green:
                    spriteRenderer.color = Color.green;
                    break;
            }
        }
        
        
        public void SetPosition(int newX, int newY, Vector2 newPosition)
        {
            x = newX;
            y = newY;

            transform.position = newPosition;
        }

        public virtual List<MoveData> GetAvailableMoves(Piece2D[,] board)
        {
            return GridHelper.GetAvailableMoves(colorType, pieceType, board, x, y);
        }

        protected bool IsInside(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }
    }
}