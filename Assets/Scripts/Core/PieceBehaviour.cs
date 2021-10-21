using System.Collections.Generic;
using UnityEngine;

public class PieceBehaviour : MonoBehaviour
{
    [HideInInspector]
    public int id = 0; //For instantiate gameobject from piecemanager
    [HideInInspector]
    public int rotation = 0; //For spawning pieces with different rotation
    [HideInInspector]
    public Vector2Int location = new Vector2Int(); //Piece location

    public List<Vector2Int> filledCellLocations = new List<Vector2Int>(); //Filled cells of current piece
    public List<Vector2Int> triangleCellLocations = new List<Vector2Int>(); //Filled cells of current piece
    public List<Vector4> filledCellCount = new List<Vector4>(); //Filled cell's filling rate

    public List<Vector2Int> bordersOfPiece = new List<Vector2Int>(); //Borders of piece

    public void ChangeMaterial(Material m) //Change materials of childeren objects under this object (center of object)
    {
        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.material = m;
        }
    }
}