using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife.GameBoard
{
    public class Board
    {
        public Grid Grid { get; }
        public SolidColorBrush aliveColor { get; private set; }
        public SolidColorBrush deadColor { get; private set; }
        public SolidColorBrush debugColor { get; private set; }
        public int gridsize { get; private set; }

        public Board(int gridsizeint, int squareSize, Grid grid, SolidColorBrush Alive, SolidColorBrush Dead, SolidColorBrush DebugColor)
        {
            gridsize = gridsizeint;
            aliveColor = Alive;
            deadColor = Dead;
            debugColor = DebugColor;
            GridLengthConverter myGridLengthConverter = new GridLengthConverter();
            GridLength side = (GridLength)myGridLengthConverter.ConvertFromString("Auto");

            //Adding Column and Row definitions needed.
            for (int i = 0; i < gridsize; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions[i].Width = side;
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions[i].Height = side;
            }

            Rectangle[,] square = new Rectangle[gridsize, gridsize];

            for (int row = 0; row < gridsize; row++)
            {
                for (int col = 0; col < gridsize; col++)
                {
                    //Creates Square with specific dimensions
                    square[row, col] = new Rectangle() { Name = ("RectangleOnRow" + row.ToString() + "Column" + col.ToString()) };
                    square[row, col].Height = squareSize;
                    square[row, col].Width = squareSize;

                    Grid.SetColumn(square[row, col], col);
                    Grid.SetRow(square[row, col], row);
                    square[row, col].Fill = deadColor;
                    grid.Children.Add(square[row, col]);
                }
            }
            Grid = grid;
        }

        public void SetGridCellState(CellState state, int row, int col)
        {
            var rect = getRectangleElementAtCoord(row, col);
            setRectangleColorByState(state, rect);
        }

        public void setGridCellState(CellState state, Rectangle rect)
        {
            setRectangleColorByState(state, rect);
        }

        public CellState getCellStateAtCoord(int row, int col)
        {
            var rect = getRectangleElementAtCoord(row, col);
            return getCellStateByColor(rect.Fill);
        }

        public CellState getCellStateOfRectangle(Rectangle rect)
        {
            CellState cellstate = getCellStateByColor(rect.Fill);
            return CellState.alive;
        }

        public IList<CellCoordinate> getSurroundigCoordinates(int row, int col, int gridSize)
        {
            IList<CellCoordinate> returnList = new List<CellCoordinate>();
            //Loop through surrounding cells.
            for (int r = row - 1; r <= row + 1; r++)
                for (int c = col - 1; c <= col + 1; c++)
                    if (c >= 0 && r >= 0 && c < gridSize && r < gridSize && !(r == row && c == col))
                    {
                        returnList.Add(new CellCoordinate(r, c));
                    }
            return returnList;
        }

        public int getAliveCountFromCoordinates(ICollection<CellCoordinate> cells)
        {
            int aliveCount = 0;
            foreach (CellCoordinate cell in cells)
            {
                if(getCellStateAtCoord(cell.Row, cell.Column) == CellState.alive)
                {
                    aliveCount++;
                }
            }
            return aliveCount;
        }

        public Boolean DecideIfCellDiesBasedOnNeighboursAlive(int neighbourCellsAlive, CellState cellState)
        {
            //you have to initialise this.
            bool celldies = true;

            if (cellState == CellState.alive)
            {
                if (neighbourCellsAlive <= 1)
                {
                    celldies = true;
                }
                else if (neighbourCellsAlive > 4)
                {
                    celldies = true;
                }
                else if (neighbourCellsAlive == 3 || neighbourCellsAlive == 2)
                {
                    celldies = false;
                }
            }
            else
            {
                if (neighbourCellsAlive == 3)
                {
                    celldies = false;
                }
            }
            return celldies;
        }

        private CellState getCellStateByColor(Brush color)
        {
            CellState returnState = CellState.dead;
            if (color == aliveColor)
            {
                returnState = CellState.alive;
            }
            return returnState;
        }

        private void setRectangleColorByState(CellState state, Rectangle rect)
        {
            if (state == CellState.debug)
            {
                rect.Fill = debugColor;
            }
            else if (state == CellState.alive)
            {
                rect.Fill = aliveColor;
            }
            else
            {
                rect.Fill = deadColor;
            }
        }

        private Rectangle getRectangleElementAtCoord (int row, int col)
        {
            foreach(FrameworkElement fe in Grid.Children)
            {
                if (Grid.GetRow(fe) == row && Grid.GetColumn(fe) == col)
                {
                    return fe as Rectangle;
                }
            }
            return null;
        }

        private CellCoordinate getCoordinateForRectangle (Rectangle rect)
        {
            var row = Grid.GetRow(rect);
            var col = Grid.GetColumn(rect);

            return new CellCoordinate(row,col);
        }
    }
}


