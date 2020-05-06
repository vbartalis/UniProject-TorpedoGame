using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using torpedo2.data;
using torpedo2.json;

namespace Torpedo.pages
{
    /// <summary>
    /// Interaction logic for SinglePlayer.xaml
    /// </summary>
    public partial class SinglePlayer : Page
    {
        private bool gameOver;
        private const int GameSize = 10;
        private GameBoard gameBoard;
        private readonly AI ai = new AI();
        private IList<GameOutcome> gameOutcomes;
        private bool player1Won;


        public SinglePlayer()
        {
            InitializeComponent();
            GameBoardGrid.Visibility = Visibility.Collapsed;
            PlayerNameGrid.Visibility = Visibility.Visible;
            GameOverGrid.Visibility = Visibility.Collapsed;
        }


        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlayerNameTextBox.Text.Length > 0)
            {
                string playerName = PlayerNameTextBox.Text;
                gameBoard = new GameBoard(playerName, true, "computer", false);
                GameBoardGrid.Visibility = Visibility.Visible;
                PlayerNameGrid.Visibility = Visibility.Collapsed;
                GameOverGrid.Visibility = Visibility.Collapsed;
                gameOver = false;
                RenderGameState();
                Focus_OnGrid();
            }
        }

        private void DoubleClickOnPlayerNameLabel(object sender, MouseButtonEventArgs e)
        {
            GameBoardGrid.Visibility = Visibility.Collapsed;
            PlayerNameGrid.Visibility = Visibility.Visible;
            GameOverGrid.Visibility = Visibility.Collapsed;
        }

        private void RestartButttonClick(object sender, RoutedEventArgs e)
        {
            string playerName = PlayerNameTextBox.Text;
            GameOverGrid.Visibility = Visibility.Collapsed;
            gameBoard = new GameBoard(playerName, true, "computer", false);
            gameOver = false;
            RenderGameState();

        }

        private void RenderGameState()
        {
            PlayerCanvas.Children.Clear();
            EnemyCanvas.Children.Clear();

            for (int x = 0; x < GameSize; x++)
            {
                for (int y = 0; y < GameSize; y++)
                {
                    DrawPlayerPosition(x, y, gameBoard.GameMap1.Positions[x, y], PlayerCanvas);
                    DrawEnemyPosition(x, y, gameBoard.GameMap2.Positions[x, y], EnemyCanvas);
                }
            }

            PlayerNameLabel.Content = gameBoard.Player1.Name;
            RemainingEnemyShipsLabel.Content = gameBoard.Player2.Ships.RemainingShips().ToString();
            EnemyShipsHitLabel.Content = gameBoard.Player1.Hits.ToString();
            FiredTorpedosLabel.Content = gameBoard.Player1.Fires.ToString();
            RoundLabel.Content = gameBoard.Turn.ToString();

        }

        private void DrawPlayerPosition(int x, int y, Position position, Canvas canvas)
        {
            var shape = new Rectangle();

            //no ship, no hit in position
            if (position.Ship == 0 && !position.Hit)
            {
                shape.Fill = Brushes.Aqua;
            }
            //no ship, position was hit
            else if (position.Ship == 0 && position.Hit)
            {
                shape.Fill = Brushes.YellowGreen;
            }
            //ship in position, not hit
            else if (position.Ship != 0 && !position.Hit)
            {
                shape.Fill = Brushes.Gray;
            }
            //ship in position, it was hit
            else if (position.Ship != 0 && position.Hit)
            {
                shape.Fill = Brushes.DarkRed;
            }

            var unitX = canvas.Width / GameSize;
            var unitY = canvas.Height / GameSize;
            shape.Width = unitX;
            shape.Height = unitY;
            Canvas.SetTop(shape, y * unitY);
            Canvas.SetLeft(shape, x * unitX);
            canvas.Children.Add(shape);
        }

        private void DrawEnemyPosition(int x, int y, Position position, Canvas canvas)
        {
            var shape = new Rectangle();

            //no ship, no hit in position
            if (position.Ship == 0 && !position.Hit)
            {
                shape.Fill = Brushes.SkyBlue;
            }
            //no ship, position was hit
            else if (position.Ship == 0 && position.Hit)
            {
                shape.Fill = Brushes.YellowGreen;
            }
            //ship in position, not hit
            else if (position.Ship != 0 && !position.Hit)
            {
                shape.Fill = Brushes.SkyBlue;
            }
            //ship in position, it was hit
            else if (position.Ship != 0 && position.Hit)
            {
                shape.Fill = Brushes.DarkRed;
            }

            var unitX = canvas.Width / GameSize;
            var unitY = canvas.Height / GameSize;
            shape.Width = unitX;
            shape.Height = unitY;
            Canvas.SetTop(shape, y * unitY);
            Canvas.SetLeft(shape, x * unitX);
            canvas.Children.Add(shape);

        }

        private void EnemyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            Point point = Mouse.GetPosition(EnemyCanvas);
            if (gameBoard.TurnStarts && !gameOver)
            {
                Position position = GetAttackedPosition(point);
                var alreadyHit = position.Hit;
                if (!alreadyHit)
                {
                    gameBoard.FireTorpedo(position, gameBoard.Player1, gameBoard.Player2);
                    RenderGameState();
                    if (gameBoard.Player2.Ships.RemainingShips() == 0)
                    {
                        player1Won = true;
                        GameOver();
                    }
                    position = ai.SearchForTarget(GameSize ,gameBoard.GameMap1.Positions);
                    gameBoard.FireTorpedo(position, gameBoard.Player2, gameBoard.Player1);
                    RenderGameState();
                    if (gameBoard.Player1.Ships.RemainingShips() == 0)
                    {
                        player1Won = false;
                        GameOver();
                    }

                }
            }
        }


        private Position GetAttackedPosition(Point point)
        {
            var unitX = EnemyCanvas.Width / GameSize;
            var unitY = EnemyCanvas.Height / GameSize;
            var x = (int)(point.X / unitX);
            var y = (int)(point.Y / unitY);
            return gameBoard.GameMap2.Positions[x, y];
        }

        private void GameOver()
        {
            gameOver = true;
            GameOverGrid.Visibility = Visibility.Visible;
            if (player1Won)
            {
                GameOverLabel.Content = gameBoard.Player1.Name + " Won";
            }
            else
            {
                GameOverLabel.Content = gameBoard.Player2.Name + " Won";
            }
            DataWriter dataWriter = new DataWriter();
            GameOutcome newGameOutcome = new GameOutcome(gameBoard.Player1, gameBoard.Player2, player1Won, gameBoard.Turn);
            gameOutcomes = dataWriter.ReadJson();
            gameOutcomes.Add(newGameOutcome);
            dataWriter.UpdateJson(gameOutcomes);
        }

        private void Focus_OnGrid()
        {
            GameBoardGrid.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.H)
            {
                Help();
            }
        }

        private void HelpButttonClick(object sender, RoutedEventArgs e)
        {
            Help();
        }

        private void Help()
        {
            EnemyCanvas.Children.Clear();
            for (int x = 0; x < GameSize; x++)
            {
                for (int y = 0; y < GameSize; y++)
                {
                    DrawPlayerPosition(x, y, gameBoard.GameMap2.Positions[x, y], EnemyCanvas);
                }
            }
        }
    }

}
