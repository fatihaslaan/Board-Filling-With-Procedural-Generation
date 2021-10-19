using System.Collections.Generic;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    PieceManager pieceManager;
    BoardManager boardManager;

    List<PieceBehaviour> allPieces = new List<PieceBehaviour>() { }; //Pieces that will be chosen for filling board
    List<PieceBehaviour> piecesWithTriangle = new List<PieceBehaviour>() { }; //Pieces with triangle shape
    List<PieceBehaviour> spawnedPieces; //Chosen pieces
    List<PieceBehaviour> tempSpawnedPieces = new List<PieceBehaviour>(); //Chosen pieces
    PieceBehaviour spawnedPiece; //Current spawmed piece

    List<PastChoices> pastChoices;
    List<GameObject> instantiatedObjects = new List<GameObject>();

    Cell[,] cell;//Cell
    Cell[,] tempCell;
    int cellHeight, cellWidth;

    static int triangleLoopCounter = 0;
    int loopCounter = 0; //To prevent infinite loop
    int secondLoopCounter = 0;
    bool boardFilled = false; //Pieces procedurally generated
    bool infiniteLoop = false;
    bool cellsAvailable = false;

    void LoadCells() //Load cells with empty fill rate
    {
        cellHeight = boardManager.boardHeight; //Get grid values
        cellWidth = boardManager.boardWidth;

        cell = new Cell[cellWidth, cellHeight];
        tempCell = new Cell[cellWidth, cellHeight];
        for (int i = 0; i < cellWidth; i++)
        {
            for (int j = 0; j < cellHeight; j++)
            {
                cell[i, j] = new Cell(new int[2] { i, j }, new Vector4(0, 0, 0, 0));
                tempCell[i, j] = new Cell(new int[2] { i, j }, new Vector4(0, 0, 0, 0));
            }
        }
    }

    void CopyCells()
    {
        for (int i = 0; i < cellWidth; i++)
        {
            for (int j = 0; j < cellHeight; j++)
            {
                tempCell[i, j] = new Cell(new int[2] { i, j }, new Vector4(cell[i, j].fillRate[0], cell[i, j].fillRate[1], cell[i, j].fillRate[2], cell[i, j].fillRate[3]));
            }
        }
    }

    void LoadPieces()
    {
        PieceBehaviour temp;
        foreach (GameObject o in pieceManager.allPieces) //Get available pieces to fill the board
        {
            temp = o.GetComponent<PieceBehaviour>();
            allPieces.Add(temp);
            if (temp.triangleCellLocations.Count > 0)
                piecesWithTriangle.Add(temp);
        }
    }

    void ChoosePieces() //Choose correct pieces to fill board with procedural generation
    {
        do
        {
            do
            {
                loopCounter++;
                spawnedPiece = new PieceBehaviour(allPieces[Random.Range(0, allPieces.Count)]); //New piecebehaviour class loaded
                spawnedPiece.location = new Vector2Int(Random.Range(0, cellWidth), Random.Range(0, cellHeight)); //Lets try to give it a location in board
                spawnedPiece.ChangeRotation(Random.Range(0, 4));
                pastChoices.Add(new PastChoices(spawnedPiece)); //Lets not repeat the same mistakes
            } while (!IsPiecePlaceable() && Crashed()); //Lets check if it can fit inside board without any trouble
            if (!infiniteLoop)
            {
                loopCounter = 0;
                if (spawnedPiece.triangleCellLocations.Count == 0)
                {
                    spawnedPieces.Add(spawnedPiece); //We will spawn it
                    GlobalAttributes.FillCells(cell, spawnedPiece); //Fill cells according to spawned piece's filled cells
                }
            }
            if (spawnedPieces.Count >= 12)
                infiniteLoop = true;
        } while (!IsBoardFilled() && !infiniteLoop); //Repeat until board is filled
    }

    void SpawnPieces()
    {
        PieceBehaviour tempPiece;
        foreach (PieceBehaviour piece in spawnedPieces)
        {
            instantiatedObjects.Add(Instantiate(pieceManager.allPieces[piece.id], new Vector3(Random.Range(-3f, 3f), Random.Range(-2f, -5f), 0) /*boardManager.GetPositionByLocation(piece.location[0], piece.location[1])*/, Quaternion.Euler(0, 0, piece.rotation * -90))); //Spawn pieces to cell's location (This is for test for now, pieces wont be located at cell's positions)
            instantiatedObjects[instantiatedObjects.Count - 1].AddComponent<PieceLocator>();
            tempPiece = instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<PieceBehaviour>();
            tempPiece.ChangeValues(piece);
            tempPiece.ChangeMaterial(pieceManager.allMaterials[Random.Range(0, pieceManager.allMaterials.Count)]); //Change their material
        }
    }

    void LoadGame()
    {
        loopCounter = 0;
        infiniteLoop = false;

        pastChoices = new List<PastChoices>();
        spawnedPieces = new List<PieceBehaviour>();

        LoadCells();
        ChoosePieces();

        secondLoopCounter++;
        if (secondLoopCounter > 50 || boardFilled)
        {
            SpawnPieces();
            return;
        }
        else
            LoadGame(); //We failed to generate pieces that can fit to board (Maybe it's just because of first piece's wrong location, but we have to try again)
    }

    void Start()
    {
        pieceManager = PieceManager.GetInstance();
        boardManager = BoardManager.GetInstance();
        LoadPieces();
        LoadGame();
    }

    bool AlreadyConnectedWithPiece(PieceBehaviour piece, int index)
    {
        for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
        {
            if (piece.filledCellCount[index + (piece.filledCellLocations.Count - piece.triangleCellLocations.Count)][j] == 0)
            {
                if (tempCell[piece.location[0] + piece.triangleCellLocations[index][0], piece.location[1] + piece.triangleCellLocations[index][1]].fillRate[j] == 0)
                {
                    return false;
                }
            }
            else
            {
                if (cell[piece.location[0] + piece.triangleCellLocations[index][0], piece.location[1] + piece.triangleCellLocations[index][1]].fillRate[j] == 1)
                    return false;
            }
        }
        return true;
    }

    bool IsItConnectsWithSelectedPiece(PieceBehaviour piece, int index)
    {
        PieceBehaviour temp;
        for (int j = 0; j < piecesWithTriangle.Count; j++)
        {
            for (int z = 0; z < piecesWithTriangle[j].triangleCellLocations.Count; z++)
            {
                for (int r = 0; r < 4; r++)
                {
                    temp = new PieceBehaviour(piecesWithTriangle[j]);
                    temp.ChangeRotation(r);
                    temp.location = new Vector2Int(piece.location[0] + piece.triangleCellLocations[index][0] - temp.triangleCellLocations[z][0], piece.location[1] + piece.triangleCellLocations[index][1] - temp.triangleCellLocations[z][1]); //Try to connect pieces
                    if (IsPieceWithTrianglePlaceable(temp)) //Check if that piece can also have triangles and that triangles can fit with other pieces
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    bool IsPieceWithTrianglePlaceable(PieceBehaviour piece)
    {
        int counter = 0;
        if (IsCellsAvailable(tempCell, piece))
        {
            for (int i = 0; i < piece.triangleCellLocations.Count; i++)
            {
                if (AlreadyConnectedWithPiece(piece, i))
                {
                    counter++;
                }
                else
                {
                    GlobalAttributes.FillCells(tempCell, piece);
                    pastChoices.Add(new PastChoices(piece));
                    if (IsItConnectsWithSelectedPiece(piece, i))
                    {
                        counter++;
                    }
                }

            }
            if (counter == piece.triangleCellLocations.Count)
            {
                tempSpawnedPieces.Add(piece);
                return true;
            }
        }
        return false;
    }

    bool Crashed()
    {
        if (loopCounter > 1500)
        {
            infiniteLoop = true;
            return false;
        }
        else
            return true;
    }

    bool IsBoardFilled()
    {
        if (GlobalAttributes.IsBoardFilled(cell))
        {
            boardFilled = true;
            Debug.Log("Pieces That Can Fill The Board Is Generated");
            return true;
        }
        return false;
    }

    bool AlreadyTried(PieceBehaviour piece)
    {
        for (int i = 0; i < pastChoices.Count - 1; i++) //Dont check last added choice
        {
            if (pastChoices[i].IsTriedAlready(piece))
                return true;
        }
        return false;
    }

    bool IsPiecePlaceable()
    {
        if (spawnedPiece.triangleCellLocations.Count > 0)
        {
            CopyCells();
            if (!IsPieceWithTrianglePlaceable(spawnedPiece))
            {
                tempSpawnedPieces = new List<PieceBehaviour>();
                return false;
            }
            else
            {
                if (spawnedPieces.Count + tempSpawnedPieces.Count <= 12)
                {
                    foreach (PieceBehaviour piece in tempSpawnedPieces)
                    {
                        GlobalAttributes.FillCells(cell, piece);
                        spawnedPieces.Add(piece);
                    }
                }
                tempSpawnedPieces = new List<PieceBehaviour>();
            }
        }
        else if (!IsCellsAvailable(cell, spawnedPiece))
            return false;
        return true;
    }

    bool IsCellsAvailable(Cell[,] c, PieceBehaviour piece)
    {
        if (AlreadyTried(piece)) //We dont need to check for anything else we already tried it
        {
            return false;
        }
        if (!GlobalAttributes.IsInsideBoard(piece, cellWidth, cellHeight))
        {
            return false;
        }
        for (int i = 0; i < piece.filledCellLocations.Count; i++)
        {
            for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
            {
                if (piece.filledCellCount[i][j] == 1) //Check cell filling only if you can also fill there
                {
                    if (c[piece.location[0] + piece.filledCellLocations[i][0], piece.location[1] + piece.filledCellLocations[i][1]].fillRate[j] == 1) //Cell unavailable
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}