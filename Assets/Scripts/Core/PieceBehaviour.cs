using System.Collections.Generic;
using UnityEngine;

public class PieceBehaviour : MonoBehaviour
{
    public int id = 0; //For instantiate gameobject from piecemanager

    public int rotation = 0; //For spawning pieces with different rotation

    public Vector2Int location = new Vector2Int(); //Piece location

    public List<Vector2Int> filledCellLocations = new List<Vector2Int>(); //Filled cells of current piece
    public List<Vector4> filledCellCount = new List<Vector4>(); //Filled cells of current piece

    public List<Vector2Int> bordersOfPiece = new List<Vector2Int>(); //Borders of piece

    public PieceBehaviour(PieceBehaviour p) //Cloning a class from selected object
    {
        ChangeValues(p);
    }

    public void ChangeValues(PieceBehaviour p)
    {
        id = p.id;
        rotation = p.rotation;
        location = p.location;
        filledCellLocations = p.filledCellLocations;
        filledCellCount = p.filledCellCount;
        bordersOfPiece = p.bordersOfPiece;
    }

    public void ChangeMaterial(Material m) //Change materials of childeren objects under this object (center of object)
    {
        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.material = m;
        }
    }
}
