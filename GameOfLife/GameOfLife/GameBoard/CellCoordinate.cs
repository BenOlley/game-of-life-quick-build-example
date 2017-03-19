namespace GameOfLife.GameBoard
{
    public class CellCoordinate
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public CellCoordinate( int row, int col)
        {
            Row = row;
            Column = col;
        }
    }
}
