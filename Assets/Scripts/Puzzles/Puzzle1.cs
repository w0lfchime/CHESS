using System.ComponentModel;
using UnityEngine;

public class puzzle1 : puzzle
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // STEP 1
        // Start by defining how many pieces you are going to have on each team. The length of the id arrays should be equal to the amount of
        // arrays in their corrsponding location arrays. The length of all arrays in the location arrays should always be two.

        whitePieceLocations = new int[3, 2];
        blackPieceLocations = new int[1, 2];
        whitePieceIds = new string[3];
        blackPieceIds = new string[1];

        // STEP 2
        // Next, define your pieces. The values in the location variables should be the x, y cords where the piece should start

        // white queen
        whitePieceLocations[0, 0] = 7;
        whitePieceLocations[0, 1] = 0;

        whitePieceIds[0] = "StandardQueen";

        // white king
        whitePieceLocations[1, 0] = 0;
        whitePieceLocations[1, 1] = 0;

        whitePieceIds[1] = "StandardKing";

        // white rook
        whitePieceLocations[2, 0] = 4;
        whitePieceLocations[2, 1] = 0;

        whitePieceIds[2] = "StandardRook";

        // black king
        blackPieceLocations[0, 0] = 0;
        blackPieceLocations[0, 1] = 7;

        blackPieceIds[0] = "StandardKing";

        // STEP 3
        // Now that your pieces are defined, you need to set how the "opponent" will make their moves. This allows for multi-move puzzles
        // Start by defining the aiMoveFrom and aiMoveTo arrays. aiMoveFrom represents the starting locations of the pieces you want to move
        // and aiMoveTo is the locations you want to move that piece too. the values in each array are x, y cords of the board.

        // Make sure to also set aiMoves. This is the amount of moves the ai will make in total.
        aiMoves = 1;

        // set move locations
        aiMoveFrom = new int[aiMoves, 2];
        aiMoveTo = new int[aiMoves, 2];

        // first move, move the black pawn up one
        aiMoveFrom[0, 0] = 0;
        aiMoveFrom[0, 1] = 7;

        aiMoveTo[0, 0] = 1;
        aiMoveTo[0, 1] = 7;

        // STEP 4
        // Next you'll set the players intended movements for the puzzle. This works the same way as it does for the ai,
        // but instead of setting a "moveFrom" var you just put in the id of the piece you want the player to move.

        // whiteMoves works the same as aiMoves
        int whiteMoves = 2;

        // sets the moves the player needs to make
        whiteMoveTo = new int[whiteMoves, 2];
        movedPieces = new string[whiteMoves];

        // must move rook to 2,6 
        whiteMoveTo[0, 0] = 4;
        whiteMoveTo[0, 1] = 6;

        movedPieces[0] = "StandardRook";

        // must move queen to 7,6 
        whiteMoveTo[1, 0] = 7;
        whiteMoveTo[1, 1] = 7;

        movedPieces[1] = "StandardQueen";

        // STEP 5
        // Set the puzzle description and the reward

        // Sets the reward
        reward = "Nothing :P";
        
        // sets puzzle description
        puzzleDesc = "Mate in two";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
