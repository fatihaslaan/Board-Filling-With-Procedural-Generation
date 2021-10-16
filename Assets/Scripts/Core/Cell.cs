public class Cell
{
    public int[] fillRate = new int[4] { 0, 0, 0, 0 }; //Sections of cell
    public int[] location = new int[2] { 0, 0 };

    public Cell(int[] l, int[] f)
    {
        location = l;
        fillRate = f;
    }
}