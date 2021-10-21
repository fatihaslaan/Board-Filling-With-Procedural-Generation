using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    PieceManager pieceManager;
    BoardManager boardManager;

    List<PieceBehaviour> piecesWithoutTriangle = new List<PieceBehaviour>() { }; //Pieces that will be chosen for filling board
    List<PieceBehaviour> piecesWithTriangle = new List<PieceBehaviour>() { }; //Pieces with triangle shape
    List<PieceData> spawnedPieces; //Chosen pieces
    List<PieceData> tempSpawnedPieces = new List<PieceData>(); //Chosen pieces
    PieceData spawnedPiece; //Current spawmed piece

    List<PastChoices> pastChoices;
    List<GameObject> instantiatedObjects = new List<GameObject>();

    Cell[,] cell;//Cell
    Cell[,] tempCell;
    int cellHeight, cellWidth;

    int loopCounter = 0; //To prevent infinite loop
    int secondLoopCounter = 0;
    bool boardFilled = false; //Pieces procedurally generated
    bool infiniteLoop = false;
    int randomPiece = 0;

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
            if (temp.triangleCellLocations.Count > 0)
                piecesWithTriangle.Add(temp);
            else
                piecesWithoutTriangle.Add(temp);

        }
    }

    void ChoosePieces() //Choose correct pieces to fill board with procedural generation
    {
        do
        {
            do
            {
                loopCounter++;
                if (Random.Range(0, 10) < 3)
                {
                    randomPiece = Random.Range(0, piecesWithoutTriangle.Count);
                    spawnedPiece = new PieceData(piecesWithoutTriangle[randomPiece]); //New piecebehaviour class loaded
                }
                else
                {
                    do
                    {
                        randomPiece = Random.Range(0, piecesWithTriangle.Count);
                    } while (piecesWithTriangle[randomPiece].filledCellCount.Count <= 1); //Try bigger pieces
                    spawnedPiece = new PieceData(piecesWithTriangle[randomPiece]);
                }
                spawnedPiece.location = new Vector2Int(Random.Range(0, cellWidth), Random.Range(0, cellHeight)); //Lets try to give it a location in board
                spawnedPiece.ChangeRotation(Random.Range(0, 4));
            } while (!IsPiecePlaceable() && Crashed()); //Lets check if it can fit inside board without any trouble
            if (!infiniteLoop)
            {
                loopCounter = 0;
                if (spawnedPiece.triangleCellLocations.Count == 0) //Only add pieces without triangle faces (We already added pieces with triangles)
                {
                    spawnedPieces.Add(spawnedPiece); //We will spawn it
                    GlobalAttributes.FillCells(cell, spawnedPiece); //Fill cells according to spawned piece's filled cells
                }
            }
            if (spawnedPieces.Count >= boardManager.maxPieceCount)
            {
                Debug.Log("BOARD IS TOO BIG FOR THE PIECES WE CREATED");
                infiniteLoop = true;
            }
        } while (!IsBoardFilled() && !infiniteLoop); //Repeat until board is filled
    }

    void SaveSelectedPieces()
    {
        LevelData newLevel = new LevelData();
        newLevel.selectedPieceIds = new int[spawnedPieces.Count];
        newLevel.selectedPieceRotations = new int[spawnedPieces.Count];
        for (int i = 0; i < spawnedPieces.Count; i++)
        {
            newLevel.selectedPieceIds[i] = spawnedPieces[i].id;
            newLevel.selectedPieceRotations[i] = spawnedPieces[i].rotation;
        }
        GlobalAttributes.SaveLevelData(newLevel);
        pieceManager.pieceSpawner.SetActive(true);

        //TEST TO SEE IF BOARD FILLED CORRECTLY

        // PieceBehaviour tempPiece;
        // foreach (PieceData piece in spawnedPieces)
        // {
        //     instantiatedObjects.Add(Instantiate(pieceManager.allPieces[piece.id], boardManager.GetPositionByCoordinates(piece.location[0], piece.location[1]), Quaternion.Euler(0, 0, piece.rotation * -90))); //Spawn pieces to cell's location (This is for test for now, pieces wont be located at cell's positions)
        //     instantiatedObjects[instantiatedObjects.Count - 1].AddComponent<PieceLocator>();
        //     tempPiece = instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<PieceBehaviour>();
        //     tempPiece.ChangeMaterial(pieceManager.allMaterials[Random.Range(0, pieceManager.allMaterials.Count)]); //Change their material
        // }
    }

    void LoadGame()
    {
        loopCounter = 0;
        infiniteLoop = false;
        boardFilled = false;

        pastChoices = new List<PastChoices>();
        spawnedPieces = new List<PieceData>();

        LoadCells();
        ChoosePieces();

        secondLoopCounter++;
        if (secondLoopCounter > 200)
            return;
        if (boardFilled)
        {
            if (spawnedPieces.Count < boardManager.minPieceCount)
            {
                LoadGame();
            }
            else
            {
                SaveSelectedPieces();
                return;
            }
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

    bool AlreadyConnectedWithPiece(PieceData piece, int index)
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

    bool IsItConnectsWithSelectedPiece(PieceData piece, int index)
    {
        PieceData temp;
        for (int j = 0; j < piecesWithTriangle.Count; j++) //Check all pieces with triangles
        {
            for (int z = 0; z < piecesWithTriangle[j].triangleCellLocations.Count; z++)
            {
                for (int r = 0; r < 4; r++) //Try with different rotation
                {
                    temp = new PieceData(piecesWithTriangle[j]);
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

    bool IsPieceWithTrianglePlaceable(PieceData piece)
    {
        int connectedTriangleFaceCounter = 0;
        if (IsCellsAvailable(tempCell, piece)) //Piece doesnt fit inside board
        {
            for (int i = 0; i < piece.triangleCellLocations.Count; i++)
            {
                if (AlreadyConnectedWithPiece(piece, i)) //This triangle face is already connected 
                {
                    GlobalAttributes.FillCells(tempCell, piece);
                    connectedTriangleFaceCounter++;
                }
                else
                {
                    GlobalAttributes.FillCells(tempCell, piece);
                    if (IsItConnectsWithSelectedPiece(piece, i)) //Can this piece connect with other pieces?
                    {
                        connectedTriangleFaceCounter++;
                    }
                }

            }
            if (connectedTriangleFaceCounter == piece.triangleCellLocations.Count) //All triangle faces connected
            {
                tempSpawnedPieces.Add(piece);
                return true;
            }
        }
        return false;
    }

    bool Crashed()
    {
        if (loopCounter > 750)
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

    bool AlreadyTried(PieceData piece)
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
            piecesWithTriangle = piecesWithTriangle.OrderByDescending(o => System.Guid.NewGuid()).ToList(); //Shuffle triangle pieces list to make different connections
            if (!IsPieceWithTrianglePlaceable(spawnedPiece)) //Can this piece connect with other triangle pieces and fit inside board?
            {
                tempSpawnedPieces = new List<PieceData>();
                pastChoices.Add(new PastChoices(spawnedPiece));
                return false;
            }
            else
            {
                if (spawnedPieces.Count + tempSpawnedPieces.Count <= boardManager.maxPieceCount)
                {
                    foreach (PieceData piece in tempSpawnedPieces)
                    {
                        GlobalAttributes.FillCells(cell, piece);
                        spawnedPieces.Add(piece);
                    }
                }
                else
                {
                    Debug.Log("BOARD IS TOO BIG FOR THE PIECES WE CREATED");
                }
                tempSpawnedPieces = new List<PieceData>();
            }
        }
        else if (!IsCellsAvailable(cell, spawnedPiece)) //We dont have to worry about connections if piece doesnt have triangles
        {
            pastChoices.Add(new PastChoices(spawnedPiece));
            return false;
        }
        return true;
    }

    bool IsCellsAvailable(Cell[,] c, PieceData piece)
    {
        if (AlreadyTried(piece)) //We dont need to check for anything else we already tried it
        {
            return false;
        }
        if (!GlobalAttributes.IsInsideBoard(piece, cellWidth, cellHeight)) //Is piece can fit inside board borders?
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