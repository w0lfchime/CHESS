using UnityEngine;
using static Chessboard;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public delegate void PieceRoutine(PieceBoard board, Vector2Int position, Team team);

public class PieceRoutines
{

    public static readonly PieceRoutine PROMOTION;
    public static readonly PieceRoutine CLEAR_EN_PASSANT;



    static PieceRoutines()
    {
        PROMOTION = (board, position, team) =>
        {
            if ((team == Team.White && position.y == board.Height - 1) || (team == Team.Black && position.y == board.Height))
            {
                ChessPiece piece = board[position.x, position.y];

                ChessPiece newQueen = team == Team.White ? CHESSBOARD.SpawnSinglePiece(ChessPieceID.StandardQueen, Team.White) : CHESSBOARD.SpawnSinglePiece(ChessPieceID.StandardQueen, Team.Black);
                board[position.x, position.y] = newQueen;
                CHESSBOARD.PositionSinglePiece(position.x, position.y);

                piece.Capture(board, position, false);
            }
        };

        CLEAR_EN_PASSANT = (board, position, team) =>
        {
            ChessPiece piece = board[position.x, position.y];
            piece.RemoveTag("EnPassantable");
        };
    }
}