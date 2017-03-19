using GameOfLife.GameBoard;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int squareSize = 50;
        private int gridsize = 20;
        private Board Board;
        private SolidColorBrush AliveColor = new SolidColorBrush(Color.FromRgb(102, 204, 0));
        private SolidColorBrush DebugColor = new SolidColorBrush(Color.FromRgb(204, 0, 224));
        private SolidColorBrush DeadColor = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        private bool isSimulationRunning;

        public MainWindow()
        {
            InitializeComponent();
            Board = new Board(gridsize, squareSize, GameGrid, AliveColor, DeadColor, DebugColor);

            GameGrid.MouseLeftButtonUp += new MouseButtonEventHandler(GameGrid_LeftMouseButtonUp);
            GameGrid.MouseRightButtonUp += new MouseButtonEventHandler(GameGrid_RightMouseButtonUp);

            StartButton.Click += (s, e) => { MessageBox.Show("Starting Simulation"); isSimulationRunning = true; };
            StopButton.Click += (s, e) => { MessageBox.Show("Stopping Simulation"); isSimulationRunning = false; };

            isSimulationRunning = false;

            var cts = new CancellationTokenSource();
            var processGenerationTask = ProcessGenerationsAsync(cts.Token);
        }

        async Task ProcessGenerationsAsync(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                await Dispatcher.Yield(DispatcherPriority.Background);

                if (isSimulationRunning == true)
                {
                    //await Task.Delay(50, token);
                    for (int row = 0; row < gridsize; row++)
                    {
                        for (int col = 0; col < gridsize; col++)
                        {
                            var coordinates = Board.getSurroundigCoordinates(row, col, gridsize);
                            var aliveCount = Board.getAliveCountFromCoordinates(coordinates);
                                
                            CellState cellstate = Board.getCellStateAtCoord(row, col);

                            if (Board.DecideIfCellDiesBasedOnNeighboursAlive(aliveCount, cellstate))
                            {
                                if (cellstate != CellState.dead)
                                {
                                    Board.SetGridCellState(CellState.dead, row, col);
                                }
                            }
                            else
                            {
                                if (cellstate != CellState.alive)
                                {
                                    Board.SetGridCellState(CellState.alive, row, col);
                                }
                            }
                        }
                    }
                }                
            }
        }

        private void GameGrid_LeftMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                Rectangle rect = e.Source as Rectangle;
                Board.setGridCellState(CellState.dead, rect);
            }
        }

        private void GameGrid_RightMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                Rectangle rect = e.Source as Rectangle;
                Board.setGridCellState(CellState.alive, rect);
            }
        }

        private void StartButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isSimulationRunning = true;
        }

        private void StopButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isSimulationRunning = false;
        }
    }
}
