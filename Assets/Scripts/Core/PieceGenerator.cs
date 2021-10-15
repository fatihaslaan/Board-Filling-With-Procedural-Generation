using System.Collections.Generic;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    PieceManager pieceManager;
    BoardManager boardManager;

    List<PieceBehaviour> allPieces = new List<PieceBehaviour>() { }; //Pieces that will be chosen for filling board
    List<PieceBehaviour> spawnedPieces = new List<PieceBehaviour>(); //Chosen pieces
    PieceBehaviour spawnedPiece; //Current spawmed piece

    Cell[,] c;//Cell
    int cellHeight, cellWidth;

    int crash = 0; //To prevent infinite loop

    void LoadCells() //Load cells with empty fill rate
    {
        cellHeight = boardManager.boardHeight; //Get grid values
        cellWidth = boardManager.boardWidth;

        c = new Cell[cellHeight, cellWidth];
        for (int i = 0; i < cellHeight; i++)
        {
            for (int j = 0; j < cellWidth; j++)
            {
                c[i, j] = new Cell(new int[2] { i, j }, new int[4] { 0, 0, 0, 0 });
            }
        }
    }

    void LoadPieces()
    {
        foreach (GameObject o in pieceManager.allPieces) //Get available pieces to fill the board
            allPieces.Add(o.GetComponent<PieceBehaviour>());
    }

    void ChoosePieces() //Choose correct pieces to fill board with procedural generation
    {
        do
        {
            crash++;
            do
            {
                crash++;
                spawnedPiece = new PieceBehaviour(allPieces[Random.Range(0, allPieces.Count)]); //New piecebehaviour class loaded
                spawnedPiece.location = new Vector2Int(Random.Range(0, cellHeight), Random.Range(0, cellWidth)); //Lets try to give it a location in board
            } while (!IsCellsAvailable() && Crashed("1. crash")); //Lets check if it can fit inside board without any trouble
            spawnedPieces.Add(spawnedPiece); //We will spawn it
            FillCells(); //Fill cells according to spawned piece's filled cells
        } while (!IsBoardFilled() && Crashed("2. crash")); //Repeat until board is filled
        Debug.Log("Board Filled");
    }

    void SpawnPieces()
    {
        GameObject tempObject;
        PieceBehaviour tempPiece;
        foreach (PieceBehaviour piece in spawnedPieces)
        {
            tempObject = Instantiate(pieceManager.allPieces[piece.id], boardManager.GetPositionByLocation(piece.location[0], piece.location[1]), Quaternion.identity); //Spawn pieces to cell's location (This is for test now, pieces won be located at cell's positions)
            tempPiece = tempObject.GetComponent<PieceBehaviour>();
            tempPiece.ChangeValues(piece);
            tempPiece.ChangeMaterial(pieceManager.allMaterials[Random.Range(0, pieceManager.allMaterials.Count)]); //Change their material
        }
    }

    void Start()
    {
        pieceManager = PieceManager.GetInstance();
        boardManager = BoardManager.GetInstance();
        LoadCells();
        LoadPieces();
        ChoosePieces();
        SpawnPieces();
    }


    void FillCells()
    {
        for (int i = 0; i < spawnedPiece.filledCellLocations.Count; i++)
        {
            for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
            {
                c[spawnedPiece.location[0] + spawnedPiece.filledCellLocations[i][0], spawnedPiece.location[1] + spawnedPiece.filledCellLocations[i][1]].fillRate[j] = (int)spawnedPiece.filledCellRates[i][j];
            }
        }
    }

    bool Crashed(string a)
    {
        if (crash > 1000)
        {
            Debug.Log(a);
            return false;
        }
        else
            return true;
    }

    bool IsBoardFilled()
    {
        for (int i = 0; i < cellHeight; i++)
        {
            for (int j = 0; j < cellWidth; j++)
            {
                for (int z = 0; z < GlobalAttributes.cellSectionCount; z++)
                    if (c[i, j].fillRate[z] == 0)
                        return false;
            }
        }
        return true;
    }

    bool IsCellsAvailable()
    {
        for (int i = 0; i < spawnedPiece.bordersOfPiece.Count; i++)
        {
            if (spawnedPiece.bordersOfPiece[i][0] + spawnedPiece.location[0] >= cellHeight || spawnedPiece.bordersOfPiece[i][0] + spawnedPiece.location[0] < 0) //Inside board
            {
                return false;
            }
            if (spawnedPiece.bordersOfPiece[i][1] + spawnedPiece.location[1] >= cellWidth || spawnedPiece.bordersOfPiece[i][1] + spawnedPiece.location[1] < 0)
            {
                return false;
            }
        }
        for (int i = 0; i < spawnedPiece.filledCellLocations.Count; i++)
        {
            for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
            {
                if (c[spawnedPiece.location[0] + spawnedPiece.filledCellLocations[i][0], spawnedPiece.location[1] + spawnedPiece.filledCellLocations[i][1]].fillRate[j] == 1) //Cell available
                {
                    return false;
                }
            }
        }
        return true;
    }
}