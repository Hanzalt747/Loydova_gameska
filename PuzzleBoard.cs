/*
 * ============================================================================
 * 15 Puzzle (Loyd's 15) Game - Game Logic Class
 * ============================================================================
 *
 * This file contains the PuzzleBoard class which handles all the game logic
 * for the 15 Puzzle. It is completely independent of the UI, following the
 * principle of separation of concerns.
 *
 * The puzzle consists of a 4x4 grid containing tiles numbered 1-15 and one
 * empty space. The goal is to arrange tiles in numerical order by sliding
 * them into the empty space.
 *
 * Author: [Your Name]
 * Date: January 2026
 * Assignment: Computer Science - GUI Programming with OOP (WPF/C#)
 */

using System;
using System.Collections.Generic;

namespace Puzzle15
{
    /// <summary>
    /// Represents the logical state and operations of the 15 Puzzle board.
    ///
    /// <para>
    /// The board is stored as a 2D array where:
    /// <list type="bullet">
    ///   <item>grid[row, col] gives the value at position (row, col)</item>
    ///   <item>Row 0 is the TOP row, Row 3 is the BOTTOM row</item>
    ///   <item>Col 0 is the LEFT column, Col 3 is the RIGHT column</item>
    ///   <item>Value 0 represents the empty space</item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// Coordinate System Visualization:
    /// <code>
    ///        col 0   col 1   col 2   col 3
    ///      +-------+-------+-------+-------+
    /// row 0|   1   |   2   |   3   |   4   |
    ///      +-------+-------+-------+-------+
    /// row 1|   5   |   6   |   7   |   8   |
    ///      +-------+-------+-------+-------+
    /// row 2|   9   |  10   |  11   |  12   |
    ///      +-------+-------+-------+-------+
    /// row 3|  13   |  14   |  15   |   0   |  &lt;- 0 is empty
    ///      +-------+-------+-------+-------+
    /// </code>
    /// The above shows the SOLVED state of the puzzle.
    /// </para>
    /// </summary>
    public class PuzzleBoard
    {
        // ====================================================================
        // CONSTANTS
        // ====================================================================

        /// <summary>
        /// The size of the grid (4x4 for the classic 15 puzzle).
        /// </summary>
        public const int GridSize = 4;

        /// <summary>
        /// The value used to represent the empty space in the grid.
        /// We use 0 because it's not a valid tile number (tiles are 1-15).
        /// </summary>
        public const int EmptyTileValue = 0;

        // ====================================================================
        // PRIVATE FIELDS
        // ====================================================================

        /// <summary>
        /// The 2D array storing the current state of the puzzle.
        /// Each element contains a tile value (1-15) or 0 for empty.
        /// </summary>
        private int[,] _grid;

        /// <summary>
        /// Random number generator for shuffling.
        /// We keep one instance to ensure good randomization.
        /// </summary>
        private readonly Random _random;

        // ====================================================================
        // PROPERTIES
        // ====================================================================

        /// <summary>
        /// Gets the current row position of the empty tile.
        /// This is tracked separately to avoid searching the grid each time.
        /// </summary>
        public int EmptyRow { get; private set; }

        /// <summary>
        /// Gets the current column position of the empty tile.
        /// This is tracked separately to avoid searching the grid each time.
        /// </summary>
        public int EmptyCol { get; private set; }

        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================

        /// <summary>
        /// Initializes a new instance of the PuzzleBoard class.
        /// The board starts in the solved state.
        /// </summary>
        public PuzzleBoard()
        {
            // Initialize the 2D array with the correct dimensions
            _grid = new int[GridSize, GridSize];

            // Create the random number generator
            _random = new Random();

            // Set up the board in solved state
            ResetToSolved();
        }

        // ====================================================================
        // PUBLIC METHODS
        // ====================================================================

        /// <summary>
        /// Resets the board to the solved (goal) state.
        ///
        /// <para>
        /// The solved state has:
        /// <list type="bullet">
        ///   <item>Numbers 1-15 arranged sequentially left-to-right, top-to-bottom</item>
        ///   <item>The empty space (0) in the bottom-right corner at position (3, 3)</item>
        /// </list>
        /// </para>
        ///
        /// <para>
        /// Algorithm:
        /// <list type="number">
        ///   <item>Iterate through each cell row by row, column by column</item>
        ///   <item>Assign values 1, 2, 3, ... 15 sequentially</item>
        ///   <item>Assign 0 to the last cell (bottom-right corner)</item>
        /// </list>
        /// </para>
        /// </summary>
        public void ResetToSolved()
        {
            // Counter for filling in tile values (1 through 15)
            int value = 1;

            // Fill the grid row by row
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    // Check if this is the last cell (bottom-right corner)
                    bool isLastCell = (row == GridSize - 1) && (col == GridSize - 1);

                    if (isLastCell)
                    {
                        // Last cell gets the empty tile marker
                        _grid[row, col] = EmptyTileValue;
                    }
                    else
                    {
                        // All other cells get sequential numbers 1-15
                        _grid[row, col] = value;
                        value++;
                    }
                }
            }

            // Update empty tile position tracking
            // In solved state, empty is at bottom-right (row=3, col=3)
            EmptyRow = GridSize - 1;
            EmptyCol = GridSize - 1;
        }

        /// <summary>
        /// Gets the tile value at a specific grid position.
        /// </summary>
        /// <param name="row">Row index (0 to 3, where 0 is top).</param>
        /// <param name="col">Column index (0 to 3, where 0 is left).</param>
        /// <returns>The tile value at that position (0-15, where 0 is empty).</returns>
        /// <example>
        /// In solved state:
        /// <code>
        /// GetValue(0, 0) returns 1
        /// GetValue(1, 2) returns 7
        /// GetValue(3, 3) returns 0 (empty)
        /// </code>
        /// </example>
        public int GetValue(int row, int col)
        {
            return _grid[row, col];
        }

        /// <summary>
        /// Checks if the tile at position (row, col) can be moved.
        ///
        /// <para>
        /// A move is VALID if and only if the tile is directly adjacent
        /// (horizontally or vertically) to the empty space.
        /// Diagonal moves are NOT allowed in the 15 puzzle.
        /// </para>
        ///
        /// <para>
        /// We use Manhattan distance to determine adjacency:
        /// <code>
        /// Manhattan distance = |row_diff| + |col_diff|
        /// </code>
        /// If the distance equals 1, the tiles are adjacent.
        /// </para>
        ///
        /// <para>
        /// Visual representation of valid positions (X) relative to empty (0):
        /// <code>
        ///         +---+
        ///         | X |  &lt;- one row above
        /// +---+---+---+---+
        /// | X |   | 0 |   |  &lt;- X is one col left
        /// +---+---+---+---+
        ///         | X |  &lt;- one row below
        ///         +---+
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="row">Row of the tile to check.</param>
        /// <param name="col">Column of the tile to check.</param>
        /// <returns>True if the tile can move, false otherwise.</returns>
        public bool IsValidMove(int row, int col)
        {
            // Calculate how far the tile is from the empty space
            int rowDifference = Math.Abs(row - EmptyRow);
            int colDifference = Math.Abs(col - EmptyCol);

            // Manhattan distance formula
            int manhattanDistance = rowDifference + colDifference;

            // Adjacent tiles have Manhattan distance of exactly 1
            // This means: same row and 1 column apart, OR same column and 1 row apart
            return manhattanDistance == 1;
        }

        /// <summary>
        /// Attempts to move the tile at position (row, col) into the empty space.
        ///
        /// <para>
        /// This performs a SWAP operation:
        /// <list type="number">
        ///   <item>The tile's value moves to where the empty space was</item>
        ///   <item>The tile's old position becomes the new empty space</item>
        /// </list>
        /// </para>
        ///
        /// <para>
        /// Visual example (moving tile 6 into empty space):
        /// <code>
        /// BEFORE:                    AFTER:
        /// +---+---+---+---+         +---+---+---+---+
        /// | 1 | 2 | 3 | 4 |         | 1 | 2 | 3 | 4 |
        /// +---+---+---+---+         +---+---+---+---+
        /// | 5 | 6 | 0 | 8 |   -->   | 5 | 0 | 6 | 8 |
        /// +---+---+---+---+         +---+---+---+---+
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="row">Row of the tile to move.</param>
        /// <param name="col">Column of the tile to move.</param>
        /// <returns>True if the move was successful, false if it was invalid.</returns>
        public bool MoveTile(int row, int col)
        {
            // First, check if this move is allowed
            if (!IsValidMove(row, col))
            {
                return false;
            }

            // === PERFORM THE SWAP ===

            // Step 1: Save the value of the tile we're moving
            int tileValue = _grid[row, col];

            // Step 2: Place that value where the empty space currently is
            _grid[EmptyRow, EmptyCol] = tileValue;

            // Step 3: Mark the tile's old position as empty
            _grid[row, col] = EmptyTileValue;

            // Step 4: Update our tracking of where the empty space is
            // The empty space is now where the tile used to be
            EmptyRow = row;
            EmptyCol = col;

            return true;
        }

        /// <summary>
        /// Shuffles the board by performing random valid moves.
        ///
        /// <para>
        /// *** THIS IS THE KEY TO ENSURING SOLVABILITY ***
        /// </para>
        ///
        /// <para>
        /// <strong>WHY THIS APPROACH WORKS:</strong>
        /// </para>
        ///
        /// <para>
        /// Not all arrangements of tiles 1-15 are solvable! In fact, exactly
        /// HALF of all possible permutations (16!/2 arrangements) are unsolvable.
        /// This is related to the mathematical concept of "inversions" and parity.
        /// </para>
        ///
        /// <para>
        /// Instead of understanding the complex math, we use a simple trick:
        /// <list type="number">
        ///   <item>Start from the SOLVED state (which is obviously solvable)</item>
        ///   <item>Make only LEGAL moves (sliding tiles into empty space)</item>
        ///   <item>Each legal move is REVERSIBLE (can always undo it)</item>
        ///   <item>Therefore, any state reached this way can be solved by
        ///         reversing the sequence of moves</item>
        /// </list>
        /// </para>
        ///
        /// <para>
        /// By making 1000 random moves, we thoroughly scramble the puzzle
        /// while mathematically guaranteeing it remains solvable.
        /// </para>
        ///
        /// <para>
        /// <strong>OPTIMIZATION:</strong>
        /// We track the last move to avoid immediately undoing it (e.g., moving
        /// a tile right then immediately left), which would waste shuffling effort.
        /// </para>
        /// </summary>
        /// <param name="numberOfMoves">Number of random moves to perform (default 1000).</param>
        public void Shuffle(int numberOfMoves = 1000)
        {
            // Always start from the solved state before shuffling
            ResetToSolved();

            // Track the position a tile was moved FROM in the last move
            // This helps us avoid immediately undoing the previous move
            int lastMovedFromRow = -1;
            int lastMovedFromCol = -1;

            for (int moveNumber = 0; moveNumber < numberOfMoves; moveNumber++)
            {
                // Get all tiles that could potentially move (adjacent to empty)
                List<(int row, int col)> adjacentTiles = GetAdjacentPositions();

                // Remove the last moved-from position to prevent back-and-forth
                // Example: if we just moved a tile from (1,2) to (1,3),
                // we don't want to immediately move it back from (1,3) to (1,2)
                List<(int row, int col)> filteredMoves = new List<(int, int)>();

                foreach (var position in adjacentTiles)
                {
                    if (position.row != lastMovedFromRow || position.col != lastMovedFromCol)
                    {
                        filteredMoves.Add(position);
                    }
                }

                // Edge case: if filtering removed all options, use the unfiltered list
                if (filteredMoves.Count == 0)
                {
                    filteredMoves = adjacentTiles;
                }

                // Randomly select one of the valid tiles to move
                int randomIndex = _random.Next(filteredMoves.Count);
                var chosenPosition = filteredMoves[randomIndex];

                // Remember where the empty space currently is
                // (this becomes the "moved from" position for the tile)
                lastMovedFromRow = EmptyRow;
                lastMovedFromCol = EmptyCol;

                // Execute the move
                MoveTile(chosenPosition.row, chosenPosition.col);
            }
        }

        /// <summary>
        /// Checks if the puzzle is in the winning (solved) state.
        ///
        /// <para>
        /// The puzzle is solved when:
        /// <list type="bullet">
        ///   <item>Tiles 1-15 are in sequential order (left-to-right, top-to-bottom)</item>
        ///   <item>The empty space (0) is in the bottom-right corner</item>
        /// </list>
        /// </para>
        ///
        /// <para>
        /// We check each position against its expected value:
        /// <code>
        /// Position (0,0) should have 1
        /// Position (0,1) should have 2
        /// ...
        /// Position (3,2) should have 15
        /// Position (3,3) should have 0
        /// </code>
        /// </para>
        /// </summary>
        /// <returns>True if the puzzle is solved, false otherwise.</returns>
        public bool IsSolved()
        {
            // The expected value starts at 1 and increments for each position
            int expectedValue = 1;

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    // Check if this is the very last cell (bottom-right corner)
                    bool isLastCell = (row == GridSize - 1) && (col == GridSize - 1);

                    if (isLastCell)
                    {
                        // Last cell must be the empty tile
                        if (_grid[row, col] != EmptyTileValue)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // All other cells must have the expected sequential value
                        if (_grid[row, col] != expectedValue)
                        {
                            return false;
                        }
                        expectedValue++;
                    }
                }
            }

            // All checks passed - puzzle is solved!
            return true;
        }

        // ====================================================================
        // PRIVATE HELPER METHODS
        // ====================================================================

        /// <summary>
        /// Gets all grid positions that are adjacent to the empty space.
        ///
        /// <para>
        /// These are the positions from which a tile could legally move.
        /// We check all four directions (up, down, left, right) and
        /// return only those that fall within the grid boundaries.
        /// </para>
        /// </summary>
        /// <returns>A list of (row, col) tuples for valid adjacent positions.</returns>
        /// <example>
        /// If empty is at (1, 2):
        /// Returns: [(0,2), (2,2), (1,1), (1,3)] - all four neighbors
        ///
        /// If empty is at (0, 0) (top-left corner):
        /// Returns: [(1,0), (0,1)] - only two valid neighbors
        /// </example>
        private List<(int row, int col)> GetAdjacentPositions()
        {
            List<(int row, int col)> adjacentPositions = new List<(int, int)>();

            // Define the four cardinal directions as (rowChange, colChange)
            // Up:    row decreases by 1, col stays same
            // Down:  row increases by 1, col stays same
            // Left:  row stays same, col decreases by 1
            // Right: row stays same, col increases by 1
            (int rowDelta, int colDelta)[] directions = new[]
            {
                (-1, 0),  // Up
                (1, 0),   // Down
                (0, -1),  // Left
                (0, 1)    // Right
            };

            foreach (var (rowDelta, colDelta) in directions)
            {
                // Calculate the potential adjacent position
                int newRow = EmptyRow + rowDelta;
                int newCol = EmptyCol + colDelta;

                // Check if this position is within the grid boundaries
                bool rowInBounds = (newRow >= 0) && (newRow < GridSize);
                bool colInBounds = (newCol >= 0) && (newCol < GridSize);

                if (rowInBounds && colInBounds)
                {
                    adjacentPositions.Add((newRow, newCol));
                }
            }

            return adjacentPositions;
        }
    }
}
