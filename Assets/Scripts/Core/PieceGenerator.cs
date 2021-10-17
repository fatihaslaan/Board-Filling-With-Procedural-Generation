using System.Collections.Generic;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    PieceManager pieceManager;
    BoardManager boardManager;

    List<PieceBehaviour> allPieces = new List<PieceBehaviour>() { }; //Pieces that will be chosen for filling board
    List<PieceBehaviour> piecesWithTriangle = new List<PieceBehaviour>() { }; //Pieces with triangle shape
    List<PieceBehaviour> spawnedPieces; //Chosen pieces
    PieceBehaviour spawnedPiece; //Current spawmed piece

    List<PastChoices> pastChoices;
    List<GameObject> instantiatedObjects = new List<GameObject>();

    Cell[,] cell;//Cell
    Cell[,] tempCell;
    int cellHeight, cellWidth;

    int loopCounter = 0; //To prevent infinite loop
    int secondLoopCounter = 0;
    bool boardFilled = false; //Pieces procedurally generated
    bool infiniteLoop = false;

    void LoadCells() //Load cells with empty fill rate
    {
        cellHeight = boardManager.boardHeight; //Get grid values
        cellWidth = boardManager.boardWidth;

        cell = new Cell[cellWidth, cellHeight];
        tempCell = new Cell[cellWidth, cellHeight];
        for (int i = 0; i < cellHeight; i++)
        {
            for (int j = 0; j < cellWidth; j++)
            {
                cell[j, i] = new Cell(new int[2] { j, i }, new int[4] { 0, 0, 0, 0 });
                tempCell[j, i] = new Cell(new int[2] { j, i }, new int[4] { 0, 0, 0, 0 });
            }
        }
    }

    void CopyCells()
    {
        for (int i = 0; i < cellHeight; i++)
        {
            for (int j = 0; j < cellWidth; j++)
            {
                tempCell[j, i] = new Cell(new int[2] { j, i }, new int[4] { cell[j, i].fillRate[0], cell[j, i].fillRate[1], cell[j, i].fillRate[2], cell[j, i].fillRate[3] });
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
                pastChoices.Add(new PastChoices(spawnedPiece)); //Lets not repeat the same mistakes
            } while (!IsPiecePlaceable() && Crashed()); //Lets check if it can fit inside board without any trouble
            if (!infiniteLoop)
            {
                loopCounter = 0;
                spawnedPieces.Add(spawnedPiece); //We will spawn it
                FillCells(cell, spawnedPiece); //Fill cells according to spawned piece's filled cells
            }
        } while (!IsBoardFilled() && !infiniteLoop); //Repeat until board is filled
    }

    void SpawnPieces()
    {
        PieceBehaviour tempPiece;
        foreach (PieceBehaviour piece in spawnedPieces)
        {
            instantiatedObjects.Add(Instantiate(pieceManager.allPieces[piece.id], boardManager.GetPositionByLocation(piece.location[0], piece.location[1]), Quaternion.identity)); //Spawn pieces to cell's location (This is for test for now, pieces wont be located at cell's positions)
            tempPiece = instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<PieceBehaviour>();
            tempPiece.ChangeValues(piece);
            tempPiece.ChangeMaterial(pieceManager.allMaterials[Random.Range(0, pieceManager.allMaterials.Count)]); //Change their material
        }
    }

    void LoadGame()
    {
        loopCounter = 0;
        infiniteLoop = false;
        foreach (GameObject o in instantiatedObjects) //Remove old objects from scene
        {
            Destroy(o);
        }

        instantiatedObjects = new List<GameObject>();
        pastChoices = new List<PastChoices>();
        spawnedPieces = new List<PieceBehaviour>();

        LoadCells();
        ChoosePieces();
        SpawnPieces();

        secondLoopCounter++;
        if (secondLoopCounter > 100 || boardFilled)
        {
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

    void FillCells(Cell[,] c, PieceBehaviour piece)
    {
        for (int i = 0; i < piece.filledCellLocations.Count; i++)
        {
            for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
            {
                if ((int)piece.filledCellCount[i][j] == 1) //Dont change cell value to 0
                    c[piece.location[0] + piece.filledCellLocations[i][0], piece.location[1] + piece.filledCellLocations[i][1]].fillRate[j] = (int)piece.filledCellCount[i][j];
            }
        }
    }

    bool AlreadyConnectedWithPiece(Cell[,] c, PieceBehaviour piece, int index)
    {
        /* TEST */
        int counter = 0;
        for (int i = 0; i < GlobalAttributes.cellSectionCount; i++)
        {
            if (c[piece.location[0] + piece.triangleCellLocations[index][0], piece.location[1] + piece.triangleCellLocations[index][1]].fillRate[i] == 1)
            {
                counter++;
            }
        }
        if (counter == 4)
        {
            return true;
        }
        for (int i = 0; i < GlobalAttributes.cellSectionCount; i++)
        {
            if (c[piece.location[0] + piece.triangleCellLocations[index][0], piece.location[1] + piece.triangleCellLocations[index][1]].fillRate[i] == piece.filledCellCount[index + (piece.filledCellLocations.Count - piece.triangleCellLocations.Count)][i])
            {
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
                temp = new PieceBehaviour(piecesWithTriangle[j]);
                temp.ChangeValues(piecesWithTriangle[j]);
                temp.location = new Vector2Int(piece.location[0] + piece.triangleCellLocations[index][0] - temp.triangleCellLocations[z][0], piece.location[1] + piece.triangleCellLocations[index][1] - temp.triangleCellLocations[z][1]); //Try to connect pieces
                if (IsCellsAvailable(tempCell, temp)) //No overlap
                {
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
        FillCells(tempCell, piece);
        for (int i = 0; i < piece.triangleCellLocations.Count; i++)
        {
            if (AlreadyConnectedWithPiece(tempCell, piece, i))
            {
                counter++;
                continue;
            }
            if (IsItConnectsWithSelectedPiece(piece, i))
            {
                counter++;
            }
        }
        if (counter == piece.triangleCellLocations.Count)
        {
            return true;
        }
        CopyCells();
        return false;
    }

    bool Crashed()
    {
        if (loopCounter > 100)
        {
            infiniteLoop = true;
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
                    if (cell[i, j].fillRate[z] == 0)
                        return false;
            }
        }
        boardFilled = true;
        Debug.Log("Board Filled");
        return true;
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
        if (!IsCellsAvailable(cell, spawnedPiece))
            return false;
        if (spawnedPiece.triangleCellLocations.Count > 0)
        {
            CopyCells();
            if (!IsPieceWithTrianglePlaceable(spawnedPiece))
            {
                return false;
            }
        }
        return true;
    }

    bool IsCellsAvailable(Cell[,] c, PieceBehaviour piece)
    {
        if (AlreadyTried(piece)) //We dont need to check for anything else we already tried it
        {
            return false;
        }
        for (int i = 0; i < piece.bordersOfPiece.Count; i++)
        {
            if (piece.bordersOfPiece[i][0] + piece.location[0] >= cellWidth || piece.bordersOfPiece[i][0] + piece.location[0] < 0 || piece.location[0] < 0 || piece.location[0] >= cellWidth) //Inside board
            {
                return false;
            }
            if (piece.bordersOfPiece[i][1] + piece.location[1] >= cellHeight || piece.bordersOfPiece[i][1] + piece.location[1] < 0 || piece.location[1] < 0 || piece.location[1] >= cellHeight)
            {
                return false;
            }
        }
        for (int i = 0; i < piece.filledCellLocations.Count; i++)
        {
            for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
            {
                if (piece.filledCellCount[i][j] == 1) //Check cell filling only if you can also fill there
                {
                    if (c[piece.location[0] + piece.filledCellLocations[i][0], piece.location[1] + piece.filledCellLocations[i][1]].fillRate[j] == piece.filledCellCount[i][j]) //Cell available
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}