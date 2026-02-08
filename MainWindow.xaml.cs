/*
 * ============================================================================
 * 15 Puzzle (Loyd's 15) Game - Main Window Code-Behind
 * ============================================================================
 *
 * Author: Jan Holy and Dan Anikyn
 * Date: January 2026
 */

using System.Windows;
using System.Windows.Controls;

namespace Puzzle15
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    ///
    /// This class serves as the "View" in a simplified MVC/MVVM pattern:
    ///   PuzzleBoard (Model): Contains game state and logic
    ///   MainWindow (View): Displays the game and handles user input
    /// The window maintains a 2D array of Button references that correspond
    /// to the PuzzleBoard's grid. When the board state changes, we update
    /// these buttons to show the new tile arrangement.
    /// </summary>
    public partial class MainWindow : Window
    {
        // ====================================================================
        // PRIVATE FIELDS
        // ====================================================================

        /// <summary>
        /// The game logic handler. Contains the current state of the puzzle
        /// and methods to manipulate it.
        /// </summary>
        private readonly PuzzleBoard _board;

        /// <summary>
        /// 2D array storing references to all 16 tile buttons.
        /// _tileButtons[row, col] corresponds to _board's grid[row, col].
        /// This allows us to quickly update specific buttons when needed.
        /// </summary>
        private readonly Button[,] _tileButtons;

        /// <summary>
        /// Counts the number of moves the player has made in the current game.
        /// Resets to 0 when starting a new game.
        /// </summary>
        private int _moveCount;

        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================

        /// <summary>
        /// Initializes the main window and sets up the game.
        /// </summary>
        public MainWindow()
        {
            // Initialize XAML components (required for WPF)
            // This loads the UI defined in MainWindow.xaml
            InitializeComponent();

            // Create the game logic instance
            _board = new PuzzleBoard();

            // Initialize the 2D array for button references
            _tileButtons = new Button[PuzzleBoard.GridSize, PuzzleBoard.GridSize];

            // Initialize move counter
            _moveCount = 0;

            // Create the 16 tile buttons and add them to the grid
            CreateTileButtons();

            // Start the first game (shuffle the board)
            StartNewGame();
        }

        // ====================================================================
        // UI CREATION METHODS
        // ====================================================================

        /// <summary>
        /// Creates all 16 tile buttons and adds them to the PuzzleGrid.
        ///
        /// Each button is:
        ///   Sized to 80x80 pixels
        ///   Given a margin for spacing
        ///   Styled using the TileButtonStyle from App.xaml
        ///   Connected to the TileButton_Click event handler
        ///
        /// </summary>
        private void CreateTileButtons()
        {
            // UniformGrid fills left-to-right, top-to-bottom
            // So we add buttons in row-major order
            for (int row = 0; row < PuzzleBoard.GridSize; row++)
            {
                for (int col = 0; col < PuzzleBoard.GridSize; col++)
                {
                    // Create a new button for this grid position
                    Button tileButton = new Button
                    {
                        // Set fixed size for consistent appearance
                        Width = 75,
                        Height = 75,

                        // Margin creates spacing between tiles
                        Margin = new Thickness(4),

                        // Apply our custom style from App.xaml resources
                        Style = (Style)Application.Current.Resources["TileButtonStyle"],

                        // Store the position in the Tag property
                        // We use a ValueTuple (int, int) for easy access
                        Tag = (row, col)
                    };

                    // Connect the click event handler
                    // All tiles use the same handler; we identify which one
                    // was clicked using the Tag property
                    tileButton.Click += TileButton_Click;

                    // Add to the UniformGrid in the XAML
                    // UniformGrid automatically positions children
                    PuzzleGrid.Children.Add(tileButton);

                    // Store reference in our 2D array for quick access
                    _tileButtons[row, col] = tileButton;
                }
            }
        }

        // ====================================================================
        // DISPLAY UPDATE METHODS
        // ====================================================================

        /// <summary>
        /// Updates all tile buttons to reflect the current board state.
        /// </summary>
        private void UpdateDisplay()
        {
            for (int row = 0; row < PuzzleBoard.GridSize; row++)
            {
                for (int col = 0; col < PuzzleBoard.GridSize; col++)
                {
                    // Get the tile value from the logic board
                    int value = _board.GetValue(row, col);

                    // Get the corresponding button
                    Button button = _tileButtons[row, col];

                    if (value == PuzzleBoard.EmptyTileValue)
                    {
                        // This is the empty space
                        // Show no text and disable the button
                        // The disabled state triggers the style change to dark background
                        button.Content = "";
                        button.IsEnabled = false;
                    }
                    else
                    {
                        // This is a numbered tile
                        // Show the number and enable clicking
                        button.Content = value.ToString();
                        button.IsEnabled = true;
                    }
                }
            }

            // Update the move counter display
            MoveCounterText.Text = $"Moves: {_moveCount}";
        }

        // ====================================================================
        // EVENT HANDLERS
        // ====================================================================

        /// <summary>
        /// Handles click events on tile buttons.
        ///   Extract the (row, col) position from the button's Tag
        ///   Attempt to move the tile at that position
        ///   If successful: increment counter and update display
        ///   Check for win condition
        /// </summary>
        /// The button that was clicked.
        /// Event arguments (not used).
        private void TileButton_Click(object sender, RoutedEventArgs e)
        {
            // Cast sender to Button to access its properties
            Button clickedButton = (Button)sender;

            // Retrieve the stored position from the Tag property
            // The Tag was set as a ValueTuple (int, int) in CreateTileButtons
            (int row, int col) = ((int, int))clickedButton.Tag;

            // Attempt to move this tile
            // MoveTile returns true if the move was valid and executed
            bool moveSuccessful = _board.MoveTile(row, col);

            if (moveSuccessful)
            {
                // Move was valid - increment the counter
                _moveCount++;

                // Refresh the visual display
                UpdateDisplay();

                // Check if the player has won
                if (_board.IsSolved())
                {
                    ShowWinMessage();
                }
            }
            // If move was invalid (tile not adjacent to empty), nothing happens
            // This provides implicit feedback - only valid tiles respond to clicks
        }

        /// <summary>
        /// Handles the "New Game" button click.
        /// Starts a fresh game with a newly shuffled board.
        /// </summary>
        /// The New Game button.
        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        /// <summary>
        /// Handles the "Quit" button click.
        /// Closes the application.
        /// </summary>
        /// The Quit button.
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window, which ends the application
            // (since this is the main window)
            Close();
        }

        // ====================================================================
        // GAME CONTROL METHODS
        // ====================================================================

        /// <summary>
        /// Starts a new game by shuffling the board and resetting the counter.
        ///
        /// Actions performed:
        ///   Shuffle the board (1000 random moves from solved state)
        ///   Reset the move counter to 0
        ///   Update the display to show the new arrangement
        /// </summary>
        private void StartNewGame()
        {
            // Shuffle performs 1000 random valid moves from the solved state
            // This guarantees the puzzle is solvable
            _board.Shuffle(1000);

            // Reset the move counter
            _moveCount = 0;

            // Update the UI to show the new board state
            UpdateDisplay();
        }

        /// <summary>
        /// Displays a congratulations message when the puzzle is solved.
        /// Uses WPF's MessageBox for a native Windows dialog appearance.
        /// </summary>
        private void ShowWinMessage()
        {
            // Create a congratulatory message with the move count
            string message = $"Congratulations!\n\n" +
                           $"You solved the puzzle in {_moveCount} moves!\n\n" +
                           $"Click OK to continue.";

            // Show the message box with an info icon
            MessageBox.Show(
                message,                           // Message text
                "You Win!",                        // Title bar text
                MessageBoxButton.OK,               // Only OK button
                MessageBoxImage.Information        // Info icon
            );
        }
    }
}
