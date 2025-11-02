using System;
using UnityEngine;
using System.Collections.Generic;
using New;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Vfx;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class Board2D : MonoBehaviour
{
    public Camera cam;
    public GameObject tilePrefab;
    public ChessFactory chessFactory;

    public Color blackGrid;

    [SerializeField] private TextMeshProUGUI levelTxt;
    [SerializeField] private Button reloadBtn;

    [SerializeField] private LevelConfigData levelConfigData;
    [SerializeField] private int size = 8;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float spacing = 0; // khoảng cách giữa các cell
    [SerializeField] private VfxScaleFlyUp vfxCompleted;

    // TODO: Add other piece prefabs (rook, knight, etc.)

    private Tile2D[,] tiles = new Tile2D[8, 8];
    private Piece2D[,] board = new Piece2D[8, 8];

    private Dictionary<char, GameObject> piecePrefabs;
    private Piece2D selectedPiece;
    private List<Vector2Int> highlightedMoves = new();

    [SerializeField]
    private int _currentLevel = 1;
    private bool _canInput = false;

    private void Awake()
    {
        reloadBtn.onClick.AddListener(LoadLevel);
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        TouchSimulation.Disable();
    }

    void Start()
    {
        
        GenerateBoard();
        LoadLevel();
    }

    void GenerateBoard()
    {
        for (int r = 0; r < size; r++) // từ hàng dưới lên
        {
            for (int c = 0; c < size; c++) // từ trái sang phải
            {
                Vector2 pos = GetCellCenter(r, c);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                Tile2D tileScript = tile.GetComponent<Tile2D>();
                tileScript.SetPosition(r, c);
                tiles[r, c] = tileScript;

                tile.GetComponent<SpriteRenderer>().color = (r + c) % 2 == 1 ? Color.white : blackGrid;
            }
        }
    }

    private void LoadLevel()
    {
        levelTxt.text = "Level " + _currentLevel;
        ClearBoard();
        ClearHighlights();
        var configs = levelConfigData.GetPieceConfigs(_currentLevel);
        foreach (var config in configs)
        {
            var x = config.x;
            var y = config.y;
            var position = GetCellCenter(x, y);
            var piece = chessFactory.GetPiece(config.pieceType, config.colorType, x, y, position);
            if (piece == null)
                continue;

            board[x, y] = piece;
        }

        _canInput = true;
    }

    private void ClearBoard()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var piece = board[i, j];
                if (piece == null)
                    continue;

                Destroy(piece.gameObject);
                board[i, j] = null;
            }
        }
    }

    private Vector2 GetCellCenter(int r, int c)
    {
        return GridHelper.GetCellCenter(size, size, cellSize, spacing, transform.position, r, c,
            cellSize);
    }

    private Vector3 GetCellCenterForSize(int row, int col)
    {
        return GetCellCenter(row, col);
    }

    private Vector2Int GetCellIndex(Vector2 position)
    {
        return GridHelper.GetCellIndex(size, size, cellSize, spacing, transform.position, cellSize,
            position);
    }


    void Update()
    {
        if (_canInput == false)
            return;

        var touchCount = Touch.activeTouches.Count;
        if (touchCount == 0)
            return;
        
        var touch = Touch.activeTouches[0];
        if (touch.valid && touch.phase == TouchPhase.Began)
        {
            Vector2 worldPos = cam.ScreenToWorldPoint(touch.screenPosition);
            var index = GetCellIndex(worldPos);

            if (IsInside(index.x, index.y))
                OnTileClicked(index);
        }
    }

    void OnTileClicked(Vector2Int index)
    {
        var x = index.x;
        var y = index.y;
        Piece2D clickedPiece = board[x, y];

        if (selectedPiece == null)
        {
            if (clickedPiece != null)
            {
                selectedPiece = clickedPiece;
                ShowMoves(selectedPiece.GetAvailableMoves(board));
            }
        }
        else
        {
            Vector2Int targetIndex = new(x, y);

            if (highlightedMoves.Contains(targetIndex))
            {
                var piece = board[x, y];

                if (piece != null && piece.colorType != selectedPiece.colorType)
                    MovePiece(selectedPiece, piece, x, y);
            }

            selectedPiece = null;
            ClearHighlights();
        }
    }

    void MovePiece(Piece2D piece, Piece2D targetPiece, int newX, int newY)
    {
        board[piece.x, piece.y] = null;

        if (piece.colorType == ColorType.Red)
        {
            Destroy(piece.gameObject);
            board[newX, newY] = null;
        }
        else
        {
            board[newX, newY] = piece;
            var newPosition = GetCellCenterForSize(newX, newY);
            piece.SetPosition(newX, newY, newPosition);
            
            if (piece.colorType == ColorType.Green)
            {
                var newPiceType = targetPiece.pieceType;
                var newSprite = chessFactory.GetSprite(newPiceType);
                piece.TurnInto(newPiceType, newSprite);
            }
        }

        if (targetPiece.pieceType == PieceType.Bomb)
        {
            DestroyPieceByBomb(targetPiece.x, targetPiece.y);
        }

        Destroy(targetPiece.gameObject);
        
        CheckWin();
    }

    private void DestroyPieceByBomb(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (GridHelper.IsInside(nx, ny))
                {
                    var piece = board[nx, ny];
                    if (piece != null)
                    {
                        Destroy(piece.gameObject);
                        board[nx, ny] = null;
                    }
                }
                
            }
        }
    }

    private void CheckWin()
    {
        var count = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var piece = board[i, j];
                if (piece != null)
                {
                    count++;
                }
            }
        }

        if (count == 1)
        {
            _currentLevel++;
            _canInput = false;
            ShowVfx();
        }
    }

    private void ShowVfx()
    {
        vfxCompleted.transform.position = Vector3.zero;
        vfxCompleted.PlayEffect(LoadLevel);
    }

    void ShowMoves(List<MoveData> moves)
    {
        ClearHighlights();
        foreach (var move in moves)
        {
            var x = move.Index.x;
            var y = move.Index.y;
            tiles[x, y].SetHighlight(true, move.CanEat);
            highlightedMoves.Add(move.Index);
        }
    }

    void ClearHighlights()
    {
        foreach (var pos in highlightedMoves)
            tiles[pos.x, pos.y].SetHighlight(false);

        highlightedMoves.Clear();
    }

    bool IsInside(int x, int y) => x >= 0 && x < 8 && y >= 0 && y < 8;
}