/*
 * ============================================================================
 * 15 Puzzle (Loyd's 15) Game - Application Entry Point
 * ============================================================================
 *
 * This file contains the Application class which serves as the entry point
 * for the WPF application. It initializes the application and sets up
 * the main window.
 *
 * Author: [Your Name]
 * Date: January 2026
 * Assignment: Computer Science - GUI Programming with OOP (WPF/C#)
 */

using System.Windows;

namespace Puzzle15
{
    /// <summary>
    /// The main Application class for the 15 Puzzle game.
    /// This class is automatically instantiated by the WPF framework
    /// when the application starts.
    /// </summary>
    public partial class App : Application
    {
        // The App class uses partial class definition because
        // the other part is auto-generated from App.xaml
        // The StartupUri in App.xaml specifies MainWindow.xaml
        // as the first window to open
    }
}
