using UnityEngine;

public class Cell
{
    public Vector4 fillRate = new Vector4(0,0,0,0); //Sections of cell
    //public int[] fillRate = new int[4] { 0, 0, 0, 0 }; //Sections of cell
    public int[] location = new int[2] { 0, 0 };

    public Cell(int[] l, Vector4 f)
    {
        location = l;
        fillRate = f;
    }
}