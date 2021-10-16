﻿using System.Collections.Generic;
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
    int cellHeight, cellWidth;

    int loopCounter = 0; //To prevent infinite loop
    int secondLoopCounter = 0;
    bool boardFilled = false; //Pieces procedurally generated
    bool infiniteLoop = false;

    void LoadCells() //Load cells with empty fill rate
    {
        cellHeight = boardManager.boardHeight; //Get grid values
        cellWidth = boardManager.boardWidth;

        cell = new Cell[cellHeight, cellWidth];
        for (int i = 0; i < cellHeight; i++)
        {
            for (int j = 0; j < cellWidth; j++)
            {
                cell[i, j] = new Cell(new int[2] { i, j }, new int[4] { 0, 0, 0, 0 });
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
                spawnedPiece.location = new Vector2Int(Random.Range(0, cellHeight), Random.Range(0, cellWidth)); //Lets try to give it a location in board
                pastChoices.Add(new PastChoices(spawnedPiece)); //Lets not repeat the same mistakes
            } while (!IsPiecePlaceable() && Crashed()); //Lets check if it can fit inside board without any trouble
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
            Debug.Log("id " + piece.id + " loc " + piece.location);
            instantiatedObjects.Add(Instantiate(pieceManager.allPieces[piece.id], boardManager.GetPositionByLocation(piece.location[0], piece.location[1]), Quaternion.identity)); //Spawn pieces to cell's location (This is for test for now, pieces wont be located at cell's positions)
            tempPiece = instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<PieceBehaviour>();
            tempPiece.ChangeValues(piece);
            tempPiece.ChangeMaterial(pieceManager.allMaterials[Random.Range(0, pieceManager.allMaterials.Count)]); //Change their material
        }
    }

    void LoadGame()
    {
        Debug.Log("********************");
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


    void FillCells()
    {
        for (int i = 0; i < spawnedPiece.filledCellLocations.Count; i++)
        {
            for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
            {
                if ((int)spawnedPiece.filledCellCount[i][j] == 1) //Dont change cell value to 0
                    cell[spawnedPiece.location[0] + spawnedPiece.filledCellLocations[i][0], spawnedPiece.location[1] + spawnedPiece.filledCellLocations[i][1]].fillRate[j] = (int)spawnedPiece.filledCellCount[i][j];
            }
        }
    }

    bool AlreadyConnectedWithPiece(int index)
    {
        for (int i = 0; i < GlobalAttributes.cellSectionCount; i++)
        {
            if (cell[spawnedPiece.location[0] + spawnedPiece.triangleCellLocations[index][0], spawnedPiece.location[1] + spawnedPiece.triangleCellLocations[index][1]].fillRate[i] == spawnedPiece.filledCellCount[index + (spawnedPiece.filledCellLocations.Count - spawnedPiece.triangleCellLocations.Count)][i])
            {
                return false;
            }
        }
        return true;
    }

    bool IsPieceWithTrianglePlaceable()
    {
        PieceBehaviour temp;
        int counter = 0;
        for (int i = 0; i < spawnedPiece.triangleCellLocations.Count; i++)
        {
            if (AlreadyConnectedWithPiece(i))
            {
                counter++;
                continue;
            }
            for (int j = 0; j < piecesWithTriangle.Count; j++)
            {
                for (int z = 0; z < piecesWithTriangle[j].triangleCellLocations.Count; z++)
                {
                    temp = new PieceBehaviour(piecesWithTriangle[j]);
                    temp.ChangeValues(piecesWithTriangle[j]);
                    temp.location = new Vector2Int(spawnedPiece.location[0] + spawnedPiece.triangleCellLocations[i][0] - temp.triangleCellLocations[z][0], spawnedPiece.location[1] + spawnedPiece.triangleCellLocations[i][1] - temp.triangleCellLocations[z][1]);
                    if (IsCellsAvailable(temp))
                    {
                        /*               TEST                   */
                        for (int a = 0; a < temp.filledCellLocations.Count; a++)
                        {
                            for (int b = 0; b < spawnedPiece.filledCellLocations.Count; b++)
                            {
                                if (temp.filledCellLocations[a] + temp.location == spawnedPiece.filledCellLocations[b] + spawnedPiece.location)
                                {
                                    for (int c = 0; c < GlobalAttributes.cellSectionCount; c++)
                                    {
                                        if (temp.filledCellCount[a][c] == 1)
                                        {
                                            if (spawnedPiece.filledCellCount[b][c] == temp.filledCellCount[a][c])
                                            {
                                                goto a;
                                                //return false;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        counter++;
                        goto endloop;
                    a:;
                    }
                }
            }
            if (counter <= i)
                break;
            endloop:;
        }
        if (counter == spawnedPiece.triangleCellLocations.Count)
        {
            return true;
        }
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
        if (!IsCellsAvailable(spawnedPiece))
            return false;
        if (spawnedPiece.triangleCellLocations.Count > 0)
        {
            if (!IsPieceWithTrianglePlaceable())
                return false;
        }
        return true;
    }

    bool IsCellsAvailable(PieceBehaviour piece)
    {
        if (AlreadyTried(piece)) //We dont need to check for anything else we already tried it
        {
            return false;
        }
        for (int i = 0; i < piece.bordersOfPiece.Count; i++)
        {
            if (piece.bordersOfPiece[i][0] + piece.location[0] >= cellHeight || piece.bordersOfPiece[i][0] + piece.location[0] < 0 || piece.location[0] < 0 || piece.location[0] >= cellHeight) //Inside board
            {
                return false;
            }
            if (piece.bordersOfPiece[i][1] + piece.location[1] >= cellWidth || piece.bordersOfPiece[i][1] + piece.location[1] < 0 || piece.location[1] < 0 || piece.location[1] >= cellHeight)
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
                    if (cell[piece.location[0] + piece.filledCellLocations[i][0], piece.location[1] + piece.filledCellLocations[i][1]].fillRate[j] == piece.filledCellCount[i][j]) //Cell available
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}