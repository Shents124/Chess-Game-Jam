using UnityEngine;
using System.Collections.Generic;
using New;
using UnityEngine.InputSystem;

public class Board2D : MonoBehaviour
{
    public Camera cam;
    public GameObject tilePrefab;
    
    [SerializeField] private int size = 8;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float spacing = 0;      // khoảng cách giữa các cell
    
    public GameObject whitePawnPrefab, blackPawnPrefab;
    // TODO: Add other piece prefabs (rook, knight, etc.)

    private Tile2D[,] tiles = new Tile2D[8, 8];
    private Piece2D[,] board = new Piece2D[8, 8];

    private Dictionary<char, GameObject> piecePrefabs;
    private Piece2D selectedPiece;
    private List<Vector2Int> highlightedMoves = new();
    
    void Start()
    {
        SetupCamera();
        InitPieceDictionary();
        GenerateBoard();
        //LoadPiecesFromCSV("board"); // board.csv trong Assets/Resources
        SpawnRandomRooks();
    }

    void SetupCamera()
    {
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 4.5f;
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }

    void InitPieceDictionary()
    {
        piecePrefabs = new Dictionary<char, GameObject>
        {
            { 'P', whitePawnPrefab },
            { 'p', blackPawnPrefab }
            // TODO: Add other pieces like R, N, B, Q, K etc.
        };
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

                tile.GetComponent<SpriteRenderer>().color = (r + c) % 2 == 0 ? Color.white : Color.blue;
            }
        }
    }

    private Vector2 GetCellCenter(int r, int c)
    {
        return GridHelper.GetCellCenter(size, size, cellSize, spacing,transform.position, r, c, 
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
    
    public GameObject rookPrefab; // gán trong Inspector

    public void SpawnRandomRooks(int count = 3)
    {
        int placed = 0;
        HashSet<Vector2Int> used = new();

        while (placed < count)
        {
            int x = Random.Range(0, 8);
            int y = Random.Range(0, 8);
            Vector2Int pos = new(x, y);

            if (board[x, y] == null && !used.Contains(pos))
            {
                GameObject rookGO = Instantiate(rookPrefab, Vector3.zero, Quaternion.identity, transform);
                Rook2D rook = rookGO.GetComponent<Rook2D>();
            
                // Gán màu (random White hoặc Black)
                rook.colorType = Random.value > 0.5f ? ColorType.White : ColorType.Black;

                var position = GetCellCenter(x, y);
                rook.SetPosition(x, y, position);
                board[x, y] = rook;
                used.Add(pos);

                placed++;
            }
        }
    }
    
    void LoadPiecesFromCSV(string fileName)
    {
        TextAsset file = Resources.Load<TextAsset>(fileName);
        if (file == null)
        {
            Debug.LogError($"Không tìm thấy file Resources/{fileName}.csv");
            return;
        }

        string[] lines = file.text.Split('\n');

        for (int y = 0; y < 8 && y < lines.Length; y++)
        {
            string[] tokens = lines[y].Trim().Split(',');

            for (int x = 0; x < 8 && x < tokens.Length; x++)
            {
                string token = tokens[x].Trim();
                if (string.IsNullOrEmpty(token)) continue;

                char id = token[0];
                if (piecePrefabs.TryGetValue(id, out GameObject prefab))
                {
                    GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
                    Piece2D piece = go.GetComponent<Piece2D>();
                    //piece.isWhite = char.IsUpper(id);
                    //piece.SetPosition(x, y);
                    board[x, y] = piece;
                }
            }
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Mouse mouse = Mouse.current;
            Vector2 worldPos = cam.ScreenToWorldPoint(mouse.position.ReadValue());
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
            Vector2Int target = new(x, y);
            if (highlightedMoves.Contains(target))
            {
                MovePiece(selectedPiece, x, y);
            }

            selectedPiece = null;
            ClearHighlights();
        }
    }

    void MovePiece(Piece2D piece, int newX, int newY)
    {
        if (board[newX, newY] != null)
        {
            var target = board[newX, newY];
            if (target.colorType != piece.colorType)
            {
                Destroy(board[newX, newY].gameObject);
            }
            else
            {
                return;
            }
        }
        
        board[piece.x, piece.y] = null;
        board[newX, newY] = piece;
        var newPosition = GetCellCenterForSize(newX, newY);
        piece.SetPosition(newX, newY, newPosition);
        CheckWin();
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
            Debug.LogWarning("Win");
        }
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
