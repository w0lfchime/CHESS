using System;
using System.Data;
using UnityEngine;

public class puzzle : MonoBehaviour
{
    // Just fill out the lists with the types of the pieces you want :D
    // Has to be filled out in another script in the Start() function
    public string[] whitePieceIds;
    public string[] blackPieceIds;

    // Fill these lists with where you want those pieces to go. USE -1 IN INDEX 0 FOR NO PIECE
    // Has to filled out in another script in the Start() function
    public int[,] whitePieceLocations;
    public int[,] blackPieceLocations;

    // This is the string with the description of the puzzle
    public string puzzleDesc = "";

    // Preprogrammed moves for the ais moves
    // Basically, aiMoves s how many times the ai will have to move, moveFrom is a 2D array that will be filled with all the locations on the board for pieces that need to be moved and moveTo is where they end
    // aiMoveOn should not be set anywhere, used in the chessboard script
    // Fill these out in another script
    public int aiMoveOn = 0;
    public int aiMoves;
    public int[,] aiMoveFrom;
    public int[,] aiMoveTo;

    // These vars hold the locations where there should be pieces when YOU move and what piece they sould be
    // Each index is another move
    // Fill them out in another script
    public int[,] whiteMoveTo;
    public string[] movedPieces;

    // This is for whats unlocked after you beat the puzzle. IDK what kinda var it should be yet so for now its a string
    // Fill out in another script
    public string reward = "";

    // Put in a var for which map to use later when we decide how we want those to work
}

