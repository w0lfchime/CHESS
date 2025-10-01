// using UnityEngine;
// using System;
// using System.Collections.Generic;

// public class FixedHighlightChessboard : MonoBehaviour
// {
//     [Header("Art Settings")]
//     [SerializeField] private Material tileMaterial;
//     [SerializeField] private float tileSize = 1f;
//     [SerializeField] private float yOffset = 0.2f;
//     [SerializeField] private Vector3 boardCenter = Vector3.zero;
//     [SerializeField] private float deathSize = 0.3f;
//     [SerializeField] private float deathSpacing = 0.3f;
//     [SerializeField] private float deathHeight = -0.1f; // Height at which dead pieces are placed
//     [SerializeField] private float dragOffset = 1.5f; // Offset for dragging pieces above the tile
//     [SerializeField] private GameObject victoryScreen;

//     [Header("Prefabs And Materials")]
//     [SerializeField] private GameObject[] prefabs;
//     [SerializeField] private Material[] teamMaterials;

//     //Code I added for piece spawning offset
//     [Header("Piece Placement")]
//     [SerializeField] private float pieceOffset = 0.1f; // Offset for piece placement above the tile]

//     //Logic
//     private ChessPiece[,] chessPieces;
//     public ChessPiece currentlyDragging;
//     public ChessPiece previouslyHovering;
//     public ChessPiece currentlyHovering;
//     private const int TILE_COUNT_X = 8;
//     private const int TILE_COUNT_Y = 8;
//     private GameObject[,] tiles;
//     private Camera currentCamera;
//     private Vector2Int currentHover;
//     private Vector3 bounds;
//     private List<ChessPiece> deadWhitePieces = new List<ChessPiece>();
//     private List<ChessPiece> deadBlackPieces = new List<ChessPiece>();
//     public List<Vector2Int> availableMoves = new List<Vector2Int>();
//     public List<Vector2Int> specialMoves = new List<Vector2Int>();
//     public bool isWhiteTurn;
//     private SpecialMove specialMove;
//     private List<Vector2Int[]> moveList = new List<Vector2Int[]>();

//     private void Awake()
//     {
//         isWhiteTurn = true; // Start with white's turn
//         GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
//         SpawnAllPieces();
//         PositionAllPieces();
//     }

//     private void Update()
//     {
//         if (!currentCamera)
//         {
//             currentCamera = Camera.main;
//             return;
//         }

//         RaycastHit info;
//         Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
//         if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight", "OpponentHighlight", "SpecialHighlight")))
//         {
//             Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

//             if (currentHover == -Vector2Int.one)
//             {
//                 currentHover = hitPosition;
//                 tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
//             }
//             else if (currentHover != hitPosition)
//             {
//                 tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
//                 currentHover = hitPosition;
//                 tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
//             }

//             //If not dragging a piece
//             if (currentlyDragging == null)
//             {
//                 //If hovering on piece
//                 if (chessPieces[hitPosition.x, hitPosition.y] != null)
//                 {
//                     currentlyHovering = chessPieces[hitPosition.x, hitPosition.y];

//                     if ((previouslyHovering == null && currentlyHovering == null) || (previouslyHovering != currentlyHovering))
//                     {
//                         RemoveHighlightTiles();

//                         //List of available moves
//                         availableMoves = currentlyHovering.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
//                         //Get list of special moves
//                         specialMove = currentlyHovering.GetSpecialMoves(ref chessPieces, ref moveList, ref specialMoves);

//                         for (int i = 0; i < specialMoves.Count; i++)
//                         {
//                             availableMoves.Add(specialMoves[i]);
//                         }

//                         PreventCheck();
//                         HighlightTiles();
//                     }

//                     //Is it our turn?
//                     if ((chessPieces[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn) || (chessPieces[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn))
//                     {
//                         //If press down on Mouse
//                         if (Input.GetMouseButtonDown(0))
//                         {
//                             currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];
//                         }
//                     }
//                 }
//                 else
//                 {
//                     currentlyHovering = null;
//                     previouslyHovering = null;
//                     RemoveHighlightTiles();
//                 }
//             }

//             //If releasing mouse
//             if (currentlyDragging != null && Input.GetMouseButtonUp(0))
//             {
//                 Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

//                 bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);

//                 if (!validMove)
//                 {
//                     currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
//                     currentlyDragging = null;
//                 }
//                 else
//                 {
//                     currentlyDragging = null;
//                 }
//                 RemoveHighlightTiles();
//             }

//         }
//         else
//         {
//             // Not hovering any tile, reset previous hover
//             if (currentHover != -Vector2Int.one)
//             {
//                 tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
//                 currentHover = -Vector2Int.one;
//                 currentlyHovering = null;
//                 previouslyHovering = null;
//                 RemoveHighlightTiles();
//             }

//             if (currentlyDragging && Input.GetMouseButtonUp(0))
//             {
//                 // If we release the mouse while dragging, reset the piece position
//                 currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
//                 currentlyHovering = null;
//                 previouslyHovering = null;
//                 currentlyDragging = null;
//                 //RemoveHighlightTiles();
//             }
//         }

//         //During dragging piece
//         if (currentlyDragging)
//         {
//             Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset); //if we have normal board, if tilted may need to adjust
//             float distance = 0.0f;
//             if (horizontalPlane.Raycast(ray, out distance))
//             {
//                 currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
//             }
//         }
//     }



//     //Generating Board
//     private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
//     {
//         yOffset += transform.position.y;
//         bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;

//         tiles = new GameObject[tileCountX, tileCountY];

//         for (int x = 0; x < tileCountX; x++)
//         {
//             for (int y = 0; y < tileCountY; y++)
//             {
//                 tiles[x, y] = GenerateSingleTile(tileSize, x, y);
//             }
//         }
//     }

//     private GameObject GenerateSingleTile(float tileSize, int x, int y)
//     {
//         GameObject tileObject = new GameObject(string.Format("X:{0} Y:{1}", x, y));
//         tileObject.transform.parent = transform;

//         Mesh mesh = new Mesh();
//         tileObject.AddComponent<MeshFilter>().mesh = mesh;
//         tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

//         Vector3[] vertices = new Vector3[4];
//         vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
//         vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
//         vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
//         vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

//         int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

//         mesh.vertices = vertices;
//         mesh.triangles = tris;

//         mesh.RecalculateNormals();

//         tileObject.layer = LayerMask.NameToLayer("Tile");

//         tileObject.AddComponent<BoxCollider>();

//         return tileObject;
//     }

//     // Spawning of Pieces
//     // private void SpawnAllPieces()
//     // {
//     //     chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

//     //     int whiteTeam = 0;
//     //     int blackTeam = 1;

//     //     //White Team
//     //     chessPieces[0, 0] = SpawnSinglePiece(ChessPieceID.StandardRook, whiteTeam);
//     //     chessPieces[1, 0] = SpawnSinglePiece(ChessPieceID.StandardKnight, whiteTeam);
//     //     chessPieces[2, 0] = SpawnSinglePiece(ChessPieceID.StandardBishop, whiteTeam);
//     //     chessPieces[3, 0] = SpawnSinglePiece(ChessPieceID.StandardQueen, whiteTeam);
//     //     chessPieces[4, 0] = SpawnSinglePiece(ChessPieceID.StandardKing, whiteTeam);
//     //     chessPieces[5, 0] = SpawnSinglePiece(ChessPieceID.StandardBishop, whiteTeam);
//     //     chessPieces[6, 0] = SpawnSinglePiece(ChessPieceID.StandardKnight, whiteTeam);
//     //     chessPieces[7, 0] = SpawnSinglePiece(ChessPieceID.StandardRook, whiteTeam);
//     //     for (int i = 0; i < TILE_COUNT_X; i++)
//     //     {
//     //         chessPieces[i, 1] = SpawnSinglePiece(ChessPieceID.StandardPawn, whiteTeam);
//     //     }

//     //     //Black Team
//     //     chessPieces[0, 7] = SpawnSinglePiece(ChessPieceID.StandardRook, blackTeam);
//     //     chessPieces[1, 7] = SpawnSinglePiece(ChessPieceID.StandardKnight, blackTeam);
//     //     chessPieces[2, 7] = SpawnSinglePiece(ChessPieceID.StandardBishop, blackTeam);
//     //     chessPieces[3, 7] = SpawnSinglePiece(ChessPieceID.StandardQueen, blackTeam);
//     //     chessPieces[4, 7] = SpawnSinglePiece(ChessPieceID.StandardKing, blackTeam);
//     //     chessPieces[5, 7] = SpawnSinglePiece(ChessPieceID.StandardBishop, blackTeam);
//     //     chessPieces[6, 7] = SpawnSinglePiece(ChessPieceID.StandardKnight, blackTeam);
//     //     chessPieces[7, 7] = SpawnSinglePiece(ChessPieceID.StandardRook, blackTeam);
//     //     for (int i = 0; i < TILE_COUNT_X; i++)
//     //     {
//     //         chessPieces[i, 6] = SpawnSinglePiece(ChessPieceID.StandardPawn, blackTeam);
//     //     }
//     // }

//     // private ChessPiece SpawnSinglePiece(ChessPieceID type, int team)
//     // {
//     //     ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
//     //     cp.team = team;
//     //     cp.ID = type;
//     //     cp.GetComponent<MeshRenderer>().material = teamMaterials[team];


//     //     Vector3 pos = cp.transform.localPosition;
//     //     pos.y = yOffset + pieceOffset; // Set the y position to be above the tile
//     //     cp.transform.localPosition = pos;

//     //     return cp;
//     // }


//     //Positioning Pieces
//     private void PositionAllPieces()
//     {
//         for (int i = 0; i < TILE_COUNT_X; i++)
//         {
//             for (int j = 0; j < TILE_COUNT_Y; j++)
//             {
//                 if (chessPieces[i, j] != null)
//                 {
//                     PositionSinglePiece(i, j, true);
//                 }
//             }
//         }
//     }
//     private void PositionSinglePiece(int x, int y, bool force = false)
//     {
//         chessPieces[x, y].currentX = x;
//         chessPieces[x, y].currentY = y;
//         chessPieces[x, y].SetPosition(GetTileCenter(x, y), 0, force);
//     }
//     private Vector3 GetTileCenter(int x, int y)
//     {
//         return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
//     }


//     private void HighlightTiles()
//     {
//         for (int i = 0; i < availableMoves.Count; i++)
//         {
//             if ((currentlyHovering.team == 0 && isWhiteTurn) || (currentlyHovering.team == 1 && !isWhiteTurn))
//             {
//                 if (!specialMoves.Contains(availableMoves[i]))
//                 {
//                     tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
//                 }
//                 else
//                 {
//                     tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("SpecialHighlight");
//                 }
//             }
//             else
//             {
//                 tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("OpponentHighlight");
//             }
//         }
//         previouslyHovering = currentlyHovering;
//     }

//     private void RemoveHighlightTiles()
//     {
//         for (int i = 0; i < availableMoves.Count; i++)
//         {
//             tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
//         }

//         availableMoves.Clear();
//         specialMoves.Clear();
//     }


//     //Checkmate
//     private void CheckMate(int team)
//     {
//         DisplayVictory(team);
//     }
//     private void DisplayVictory(int winningTeam)
//     {
//         victoryScreen.SetActive(true);
//         victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
//     }
//     public void OnResetButton()
//     {
//         //UI
//         victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
//         victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
//         victoryScreen.SetActive(false);

//         //Fields reset
//         currentlyDragging = null;
//         currentlyHovering = null;
//         availableMoves.Clear();
//         moveList.Clear();

//         //Clean up
//         for (int x = 0; x < TILE_COUNT_X; x++)
//         {
//             for (int y = 0; y < TILE_COUNT_Y; y++)
//             {
//                 if (chessPieces[x, y] != null)
//                 {
//                     Destroy(chessPieces[x, y].gameObject);
//                     chessPieces[x, y] = null;
//                 }
//             }
//         }

//         for (int i = 0; i < deadWhitePieces.Count; i++)
//         {
//             Destroy(deadWhitePieces[i].gameObject);
//         }
//         deadWhitePieces.Clear();

//         for (int i = 0; i < deadBlackPieces.Count; i++)
//         {
//             Destroy(deadBlackPieces[i].gameObject);
//         }
//         deadBlackPieces.Clear();

//         SpawnAllPieces();
//         PositionAllPieces();
//         isWhiteTurn = true; // Reset to white's turn
//     }
//     public void OnExitButton()
//     {
//         Application.Quit();
//     }

//     //Special Moves
//     private void ProcessSpecialMove()
//     {
//         // Process special moves based on the current state of the game
//         if (specialMove == SpecialMove.EnPassant)
//         {
//             // Handle En Passant logic here
//             var newMove = moveList[moveList.Count - 1];
//             ChessPiece myPawn = chessPieces[newMove[1].x, newMove[1].y];
//             var targetPawnPosition = moveList[moveList.Count - 2];
//             ChessPiece enemyPawn = chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];

//             if (myPawn.currentX == enemyPawn.currentX)
//             {
//                 if (myPawn.currentY == enemyPawn.currentY - 1 || myPawn.currentY == enemyPawn.currentY + 1)
//                 {
//                     if (enemyPawn.team == 0)
//                     {
//                         deadWhitePieces.Add(enemyPawn);
//                         enemyPawn.SetScale(Vector3.one * deathSize);
//                         enemyPawn.SetPosition(
//                             new Vector3(8 * tileSize, yOffset + deathHeight, -1 * tileSize) - bounds
//                             + new Vector3((tileSize / 2), 0, (tileSize / 2))
//                             + (Vector3.forward * deathSpacing) * deadWhitePieces.Count
//                         );
//                     }
//                     else
//                     {
//                         deadBlackPieces.Add(enemyPawn);
//                         enemyPawn.SetScale(Vector3.one * deathSize);
//                         enemyPawn.SetPosition(
//                             new Vector3(-1 * tileSize, yOffset + deathHeight, 8 * tileSize) - bounds
//                             + new Vector3((tileSize / 2), 0, (tileSize / 2))
//                             + (Vector3.back * deathSpacing) * deadBlackPieces.Count
//                         );
//                     }
//                     chessPieces[enemyPawn.currentX, enemyPawn.currentY] = null; // Remove the enemy pawn from the board
//                 }
//             }
//         }
//         if (specialMove == SpecialMove.Castling)
//         {
//             // Handle Castling logic here
//             Vector2Int[] lastMove = moveList[moveList.Count - 1];

//             //left rook
//             if (lastMove[1].x == 2) // Left Castling
//             {
//                 if (lastMove[1].y == 0) // White team
//                 {
//                     ChessPiece rook = chessPieces[0, 0];
//                     chessPieces[3, 0] = rook;
//                     PositionSinglePiece(3, 0);
//                     chessPieces[0, 0] = null;
//                 }
//                 else if (lastMove[1].y == 7) // Black team
//                 {
//                     ChessPiece rook = chessPieces[0, 7];
//                     chessPieces[3, 7] = rook;
//                     PositionSinglePiece(3, 7);
//                     chessPieces[0, 7] = null;
//                 }
//             }

//             //right rook
//             else if (lastMove[1].x == 6) // Right Castling
//             {
//                 if (lastMove[1].y == 0) // White team
//                 {
//                     ChessPiece rook = chessPieces[7, 0];
//                     chessPieces[5, 0] = rook;
//                     PositionSinglePiece(5, 0);
//                     chessPieces[7, 0] = null;
//                 }
//                 else if (lastMove[1].y == 7) // Black team
//                 {
//                     ChessPiece rook = chessPieces[7, 7];
//                     chessPieces[5, 7] = rook;
//                     PositionSinglePiece(5, 7);
//                     chessPieces[7, 7] = null;
//                 }
//             }

//         }
//         if (specialMove == SpecialMove.Promotion)
//         {
//             // Handle Promotion logic here
//             Vector2Int[] lastMove = moveList[moveList.Count - 1];
//             ChessPiece targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];

//             if (targetPawn.ID == ChessPieceID.StandardPawn)
//             {
//                 // Check if the pawn has reached the promotion row
//                 if ((targetPawn.team == 0 && targetPawn.currentY == 7))
//                 {
//                     ChessPiece newQueen = SpawnSinglePiece(ChessPieceID.StandardQueen, 0);
//                     newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position; // Place the new queen at the same position
//                     Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
//                     chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
//                     PositionSinglePiece(lastMove[1].x, lastMove[1].y);
//                 }
//                 else if ((targetPawn.team == 1 && targetPawn.currentY == 0))
//                 {
//                     ChessPiece newQueen = SpawnSinglePiece(ChessPieceID.StandardQueen, 1);
//                     newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position; // Place the new queen at the same position
//                     Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
//                     chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
//                     PositionSinglePiece(lastMove[1].x, lastMove[1].y);
//                 }
//             }
//         }

//         // Reset special move after processing
//         specialMove = SpecialMove.None;
//     }
//     private void PreventCheck()
//     {
//         ChessPiece targetKing = null;
//         for (int x = 0; x < TILE_COUNT_X; x++)
//         {
//             for (int y = 0; y < TILE_COUNT_Y; y++)
//             {
//                 if (chessPieces[x, y] != null && chessPieces[x, y].ID == ChessPieceID.StandardKing && chessPieces[x, y].team == currentlyHovering.team)
//                 {
//                     targetKing = chessPieces[x, y];
//                     break;
//                 }
//             }
//         }
//         SimulateMoveForSinglePiece(currentlyHovering, ref availableMoves, targetKing);

//     }
//     private void SimulateMoveForSinglePiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetKing)
//     {
//         //Save the current values, to reset after function call
//         int actualX = cp.currentX;
//         int actualY = cp.currentY;
//         List<Vector2Int> movesToRemove = new List<Vector2Int>();

//         //Go through all moves, simulate, and check if in check
//         for (int i = 0; i < moves.Count; i++)
//         {
//             int simX = moves[i].x;
//             int simY = moves[i].y;

//             Vector2Int kingPositionThisSim = new Vector2Int(targetKing.currentX, targetKing.currentY);
//             //Did we sim king move
//             if (cp.ID == ChessPieceID.StandardKing)
//             {
//                 kingPositionThisSim = new Vector2Int(simX, simY);
//             }

//             //Copy 2D array and not ref
//             ChessPiece[,] simulation = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
//             List<ChessPiece> simAttackingPieces = new List<ChessPiece>();
//             for (int x = 0; x < TILE_COUNT_X; x++)
//             {
//                 for (int y = 0; y < TILE_COUNT_Y; y++)
//                 {
//                     if (chessPieces[x, y] != null)
//                     {
//                         simulation[x, y] = chessPieces[x, y];
//                         if (simulation[x, y].team != cp.team)
//                         {
//                             simAttackingPieces.Add(simulation[x, y]);
//                         }
//                     }
//                 }
//             }

//             //Simulate that move
//             simulation[actualX, actualY] = null; // Remove from previous position
//             cp.currentX = simX;
//             cp.currentY = simY;
//             simulation[simX, simY] = cp; // Place at new position

//             //Did a piece get taken down during sim
//             var deadPiece = simAttackingPieces.Find(p => p.currentX == simX && p.currentY == simY);
//             if (deadPiece != null)
//             {
//                 simAttackingPieces.Remove(deadPiece);
//             }

//             //Get all simulated moves for all pieces
//             List<Vector2Int> simulatedMoves = new List<Vector2Int>();
//             for (int a = 0; a < simAttackingPieces.Count; a++)
//             {
//                 var pieceMoves = simAttackingPieces[a].GetAvailableMoves(ref simulation, TILE_COUNT_X, TILE_COUNT_Y);
//                 for (int b = 0; b < pieceMoves.Count; b++)
//                 {
//                     simulatedMoves.Add(pieceMoves[b]);
//                 }
//             }

//             //Check if king is in check
//             if (ContainsValidMove(ref simulatedMoves, kingPositionThisSim))
//             {
//                 //If king is in check, remove this move
//                 movesToRemove.Add(moves[i]);
//             }

//             //Reset the simulated piece position
//             cp.currentX = actualX;
//             cp.currentY = actualY;

//         }

//         //Remove from current move list
//         for (int i = 0; i < movesToRemove.Count; i++)
//         {
//             moves.Remove(movesToRemove[i]);
//         }
//     }
//     private bool CheckForCheckmate()
//     {
//         var lastMove = moveList[moveList.Count - 1];
//         int targetTeam = (chessPieces[lastMove[1].x, lastMove[1].y].team == 0) ? 1 : 0; // Get the team of the last moved piece


//         List<ChessPiece> attackingPieces = new List<ChessPiece>();
//         List<ChessPiece> defendingPieces = new List<ChessPiece>();
//         ChessPiece targetKing = null;
//         for (int x = 0; x < TILE_COUNT_X; x++)
//         {
//             for (int y = 0; y < TILE_COUNT_Y; y++)
//             {
//                 if (chessPieces[x, y] != null)
//                 {
//                     if (chessPieces[x, y].team == targetTeam)
//                     {
//                         defendingPieces.Add(chessPieces[x, y]);
//                         if (chessPieces[x, y].ID == ChessPieceID.StandardKing)
//                         {
//                             targetKing = chessPieces[x, y];
//                         }
//                     }
//                     else
//                     {
//                         attackingPieces.Add(chessPieces[x, y]);
//                     }
//                 }
//             }
//         }

//         //Is king attacked right now
//         List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();
//         for (int i = 0; i < attackingPieces.Count; i++)
//         {
//             var pieceMoves = attackingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
//             for (int j = 0; j < pieceMoves.Count; j++)
//             {
//                 currentAvailableMoves.Add(pieceMoves[j]);
//             }
//         }

//         if (ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
//         {
//             //StandardKing is in check
//             //Check if we can move any piece to prevent check
//             for (int i = 0; i < defendingPieces.Count; i++)
//             {
//                 List<Vector2Int> pieceMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
//                 SimulateMoveForSinglePiece(defendingPieces[i], ref pieceMoves, targetKing);

//                 if (pieceMoves.Count != 0)
//                 {
//                     //If we have any valid move, we can prevent check
//                     return false;
//                 }
//             }

//             return true; // No valid moves to prevent check, so it's checkmate

//         }

//         return false;
//     }

//     //Operations
//     private bool MoveTo(ChessPiece cp, int x, int y)
//     {
//         if (!ContainsValidMove(ref availableMoves, new Vector2Int(x, y)))
//         {
//             return false; // Invalid move
//         }

//         Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

//         //If another piece on target position
//         if (chessPieces[x, y] != null)
//         {
//             ChessPiece ocp = chessPieces[x, y];

//             if (cp.team == ocp.team)
//             {
//                 return false;
//             }

//             //If enemey piece, remove it
//             if (ocp.team == 0)
//             {
//                 if (ocp.ID == ChessPieceID.StandardKing)
//                 {
//                     CheckMate(1);
//                 }

//                 deadWhitePieces.Add(ocp);
//                 ocp.SetScale(Vector3.one * deathSize);
//                 ocp.SetPosition(
//                     new Vector3(8 * tileSize, yOffset + deathHeight, -1 * tileSize) - bounds
//                     + new Vector3((tileSize / 2), 0, (tileSize / 2))
//                     + (Vector3.forward * deathSpacing) * deadWhitePieces.Count
//                 );
//             }
//             else
//             {
//                 if (ocp.ID == ChessPieceID.StandardKing)
//                 {
//                     CheckMate(0);
//                 }

//                 deadBlackPieces.Add(ocp);
//                 ocp.SetScale(Vector3.one * deathSize);
//                 ocp.SetPosition(
//                     new Vector3(-1 * tileSize, yOffset + deathHeight, 8 * tileSize) - bounds
//                     + new Vector3((tileSize / 2), 0, (tileSize / 2))
//                     + (Vector3.back * deathSpacing) * deadBlackPieces.Count
//                 );
//             }

//         }

//         chessPieces[x, y] = cp;
//         chessPieces[previousPosition.x, previousPosition.y] = null;

//         PositionSinglePiece(x, y);

//         isWhiteTurn = !isWhiteTurn; // Switch turn

//         moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });

//         ProcessSpecialMove();

//         if (CheckForCheckmate())
//         {
//             CheckMate(cp.team);
//         }

//         return true;
//     }
//     private Vector2Int LookupTileIndex(GameObject hitInfo)
//     {
//         for (int x = 0; x < TILE_COUNT_X; x++)
//         {
//             for (int y = 0; y < TILE_COUNT_Y; y++)
//             {
//                 if (tiles[x, y] == hitInfo)
//                 {
//                     return new Vector2Int(x, y);
//                 }
//             }
//         }

//         return -Vector2Int.one;

//     }
//     private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
//     {
//         for (int i = 0; i < moves.Count; i++)
//         {
//             if (moves[i].x == pos.x && moves[i].y == pos.y)
//             {
//                 return true;
//             }
//         }
//         return false;
//     }


// }
