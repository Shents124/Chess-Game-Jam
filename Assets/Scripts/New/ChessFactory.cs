using System.Collections.Generic;
using UnityEngine;

namespace New
{
    public class ChessFactory : MonoBehaviour
    {
        public GameObject prefab;
        public GameObject bombPrefab;
        
        public List<Sprite> sprites;
        public List<Color> colors;
        
        public Piece2D GetPiece(PieceType pieceType, ColorType colorType, int x, int y, Vector2 position)
        {
            Piece2D piece = null;
            
            switch (pieceType)
            {
                case PieceType.Pawn:
                case PieceType.Rook:
                case PieceType.Knight:
                case PieceType.Bishop:
                case PieceType.Queen:
                case PieceType.King:
                    var clone = Instantiate(prefab);
                    clone.AddComponent<Piece2D>();
                    piece = clone.GetComponent<Piece2D>();
                    break;
                case PieceType.Bomb:
                    var bomb = Instantiate(bombPrefab);
                    piece = bomb.GetComponent<Piece2D>();
                    break;
            }
            
            var index = (int)pieceType;
            var sprite = sprites[index];
            var color = colors[(int)colorType];
            if (piece)
                piece.Initialize(x, y, colorType, pieceType, color, sprite, position);

            return piece;
        }

        public Sprite GetSprite(PieceType spriteType)
        {
            return sprites[(int)spriteType];
        }
    }
}