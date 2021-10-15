using System.Collections.Generic;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    PieceManager pieceManager;
    BoardManager boardManager;

    List<PieceBehaviour> allPieces = new List<PieceBehaviour>() { }; //Pieces that will be chosen for filling board
    List<PieceBehaviour> spawnedPieces; //Chosen pieces
    PieceBehaviour spawnedPiece; //Current spawmed piece

    List<PastChoices> pastChoices;
    List<GameObject> instantiatedObjects = new List<GameObject>();

    Cell[,] c;//Cell
    int cellHeight, cellWidth;

    int loopCounter = 0; //To prevent infinite loop
    int secondLoopCounter = 0;
    bool boardFilled = false; //Pieces procedurally generated
    bool infiniteLoop = false;

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
            do
            {
                loopCounter++;
                spawnedPiece = new PieceBehaviour(allPieces[Random.Range(0, allPieces.Count)]); //New piecebehaviour class loaded
                spawnedPiece.location = new Vector2Int(Random.Range(0, cellHeight), Random.Range(0, cellWidth)); //Lets try to give it a location in board
                pastChoices.Add(new PastChoices(spawnedPiece)); //Lets not repeat the same mistakes
            } while (!IsCellsAvailable() && Crashed()); //Lets check if it can fit inside board without any trouble
            if (!infiniteLoop)
            {
                loopCounter = 0;
                spawnedPieces.Add(spawnedPiece); //We will spawn it
                FillCells(); //Fill cells according to spawned piece's filled cells
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

        if (secondLoopCounter > 10 || boardFilled)
        {
            return;
        }
        else
            LoadGame(); //We failed to generate pieces that can fit to board (Maybe it's just because of first piece's wrong location, but we have to try again)

        secondLoopCounter++;
    }

    void Start()
    {
        pieceManager = PieceManager.GetInstance();
        boardManager = BoardManager.GetInstance();
        LoadPieces();
        LoadGame();
    }


    void FillCells()
    {
        for (int i = 0; i < spawnedPiece.filledCellLocations.Count; i++)
        {
            for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
            {
                if ((int)spawnedPiece.filledCellCount[i][j] == 1) //Dont change cell value to 0
                    c[spawnedPiece.location[0] + spawnedPiece.filledCellLocations[i][0], spawnedPiece.location[1] + spawnedPiece.filledCellLocations[i][1]].fillRate[j] = (int)spawnedPiece.filledCellCount[i][j];
            }
        }
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
                    if (c[i, j].fillRate[z] == 0)
                        return false;
            }
        }
        boardFilled = true;
        Debug.Log("Board Filled");
        return true;
    }

    bool AlreadyTried()
    {
        for (int i = 0; i < pastChoices.Count - 1; i++) //Dont check last added choice
        {
            if (pastChoices[i].IsTriedAlready(spawnedPiece))
                return true;
        }
        return false;
    }

    bool IsCellsAvailable()
    {
        if (AlreadyTried()) //We dont need to check for anything else we already tried it
        {
            return false;
        }
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
                if (spawnedPiece.filledCellCount[i][j] == 1) //Check cell filling only if you can also fill there
                {
                    if (c[spawnedPiece.location[0] + spawnedPiece.filledCellLocations[i][0], spawnedPiece.location[1] + spawnedPiece.filledCellLocations[i][1]].fillRate[j] == spawnedPiece.filledCellCount[i][j]) //Cell available
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}