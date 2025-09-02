using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Behavior used for chess piece active abilities.
/// <para>
/// INTENDED FUNCTIONALITY: Secondary interact button (right-click) selects piece for active ability use.
/// Secondary interact again to deselect. Primary interact (left-click) square for active ability use.
/// For abilities always centered on the piece, click the piece again.
/// </para>
/// </summary>

public abstract class ChessPiecePower
{
    /// <summary>
    /// Returns whether the active ability can be used.
    /// </summary>
    /// <param name="board">The array of pieces on the board.</param>
    /// <param name="position">The position of the active piece.</param>
    /// <param name="team">The active team.</param>
    /// <returns></returns>
    public virtual bool CanUse(PieceBoard board, Vector2Int position, Team team)
    {
        return GetAvailableTargets(board, position, team).Count > 0;
    }
    /// <summary>
    /// Generates a list of valid target locations for active ability.
    /// </summary>
    /// <param name="board">The array of pieces on the board.</param>
    /// <param name="position">The position of the active piece.</param>
    /// <param name="team">The active team.</param>
    /// <returns></returns>
    public abstract List<Vector2Int> GetAvailableTargets(PieceBoard board, Vector2Int position, Team team);
    /// <summary>
    /// Uses the active ability.
    /// </summary>
    /// <param name="board">The array of pieces on the board.</param>
    /// <param name="position">The position of the moving piece.</param>
    /// <param name="team">The active team.</param>
    /// <param name="targetPosition">The target of the active ability.</param>
    public abstract void Use(PieceBoard board, Vector2Int position, Team team, Vector2Int targetPosition);
}