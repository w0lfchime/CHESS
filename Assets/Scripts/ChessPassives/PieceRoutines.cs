using UnityEngine;
using static Chessboard;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public delegate void PieceRoutine(PieceBoard board, Vector2Int position, Team team);

public class PieceRoutines
{

    public static void Promotion(PieceBoard board, Vector2Int position, Team team)
    {
        if ((team == Team.White && position.y == board.Height - 1) || (team == Team.Black && position.y == board.Height))
        {
            ChessPiece piece = board[position.x, position.y];

            ChessPiece newQueen = CHESSBOARD.SpawnSinglePiece("StandardQueen", team == Team.White ? Team.White : Team.Black);
            board[position.x, position.y] = newQueen;
            CHESSBOARD.PositionSinglePiece(position.x, position.y);

            piece.Capture(board, position, false);

            // piece.AddTag("ToPromote");
        }
    }
    public static void ClearEnPassant(PieceBoard board, Vector2Int position, Team team)
    {
        ChessPiece piece = board[position.x, position.y];
        piece.RemoveTag("EnPassantable");
    }
}