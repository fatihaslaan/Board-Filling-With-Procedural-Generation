using System.Collections.Generic;

public class Piece_1 : PieceController
{
    /* Piece Looks Like
        _
       | |
       '''
    */

    public override List<int[]> Borders()
    {
        List<int[]> borders = new List<int[]>();
        borders.Add(new int[2] { 0, 0 });
        return borders;
    }

    public override List<Cell> FilledCells()
    {
        List<Cell> cells = new List<Cell>();
        cells.Add(new Cell(new int[2] { 0, 0 }, new int[4] { 1, 1, 1, 1 }));
        return cells;
    }

    public override int id()
    {
        return 0;
    }
}