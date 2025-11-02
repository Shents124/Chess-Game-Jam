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

        public void Initialize(int x, int y, ColorType colorType, PieceType pieceType, Color color, Sprite sprite, Vector2 position)
        {
            this.x = x;
            this.y = y;
            this.colorType = colorType;
            spriteRenderer.sprite = sprite;
            this.pieceType = pieceType;
            SetColor(color);
            transform.position = position;
        }

        protected virtual void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }


        public void TurnInto(PieceType newPieceType, Sprite newSprite)
        {
            pieceType = newPieceType;
            spriteRenderer.sprite = newSprite;
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
    }
}