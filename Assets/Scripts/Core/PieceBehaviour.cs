using System.Collections.Generic;
using UnityEngine;

public class PieceBehaviour : MonoBehaviour
{
    public int id = 0; //For instantiate gameobject from piecemanager

    public int rotation = 0; //For spawning pieces with different rotation

    public Vector2Int location = new Vector2Int(); //Piece location

    public List<Vector2Int> filledCellLocations = new List<Vector2Int>(); //Filled cells of current piece
    public List<Vector2Int> triangleCellLocations = new List<Vector2Int>(); //Filled cells of current piece
    public List<Vector4> filledCellCount = new List<Vector4>(); //Filled cells of current piece

    public List<Vector2Int> bordersOfPiece = new List<Vector2Int>(); //Borders of piece

    public PieceBehaviour(PieceBehaviour p) //Cloning a class from selected object
    {
        id = p.id;
        rotation = p.rotation;
        location = p.location;
        for (int i = 0; i < p.filledCellLocations.Count; i++)
            filledCellLocations.Add(new Vector2Int(p.filledCellLocations[i].x, p.filledCellLocations[i].y));
        for (int i = 0; i < p.triangleCellLocations.Count; i++)
            triangleCellLocations.Add(new Vector2Int(p.triangleCellLocations[i].x, p.triangleCellLocations[i].y));
        for (int i = 0; i < p.bordersOfPiece.Count; i++)
            bordersOfPiece.Add(new Vector2Int(p.bordersOfPiece[i].x, p.bordersOfPiece[i].y));
        for (int i = 0; i < p.filledCellCount.Count; i++)
            filledCellCount.Add(new Vector4(p.filledCellCount[i][0], p.filledCellCount[i][1], p.filledCellCount[i][2], p.filledCellCount[i][3]));
        //ChangeValues(p);
    }

    public void ChangeValues(PieceBehaviour p)
    {
        id = p.id;
        rotation = p.rotation;
        location = p.location;
        for (int i = 0; i < p.filledCellLocations.Count; i++)
            filledCellLocations[i] = new Vector2Int(p.filledCellLocations[i].x, p.filledCellLocations[i].y);
        for (int i = 0; i < p.triangleCellLocations.Count; i++)
            triangleCellLocations[i] = new Vector2Int(p.triangleCellLocations[i].x, p.triangleCellLocations[i].y);
        for (int i = 0; i < p.bordersOfPiece.Count; i++)
            bordersOfPiece[i] = new Vector2Int(p.bordersOfPiece[i].x, p.bordersOfPiece[i].y);
        for (int i = 0; i < p.filledCellCount.Count; i++)
            filledCellCount[i] = new Vector4(p.filledCellCount[i][0], p.filledCellCount[i][1], p.filledCellCount[i][2], p.filledCellCount[i][3]);
    }

    public void ChangeRotation(int rotate) //Changing values according to rotation
    {
        if (rotate % 4 == 0)
            return;
        rotation = (rotate) % 4;
        for (int r = 0; r < rotate; r++)
        {
            for (int i = 0; i < filledCellLocations.Count; i++)
            {
                filledCellLocations[i] = new Vector2Int(filledCellLocations[i].y, -filledCellLocations[i].x);
            }
            for (int i = 0; i < triangleCellLocations.Count; i++)
            {
                triangleCellLocations[i] = new Vector2Int(triangleCellLocations[i].y, -triangleCellLocations[i].x);
            }
            for (int i = 0; i < bordersOfPiece.Count; i++)
            {
                bordersOfPiece[i] = new Vector2Int(bordersOfPiece[i].y, -bordersOfPiece[i].x);
            }
            for (int i = 0; i < filledCellCount.Count; i++)
            {

                filledCellCount[i] = new Vector4(filledCellCount[i][3], filledCellCount[i][0], filledCellCount[i][1], filledCellCount[i][2]);
            }
        }
    }

    public void ChangeMaterial(Material m) //Change materials of childeren objects under this object (center of object)
    {
        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.material = m;
        }
    }
}