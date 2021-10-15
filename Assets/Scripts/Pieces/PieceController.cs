using System.Collections.Generic;

public abstract class PieceController
{
    public abstract int id();
    
    public int rotation = 0; //For spawning pieces with different rotation
    public int[] location = new int[2] { 0, 0 }; //Piece location

    public abstract List<Cell> FilledCells(); //Filled cells of current piece

    public abstract List<int[]> Borders(); //Borders of piece
}