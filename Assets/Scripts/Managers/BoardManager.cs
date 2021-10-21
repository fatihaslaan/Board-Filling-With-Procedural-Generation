using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject boardBackground; //Board background object
    [HideInInspector]
    public int boardWidth, boardHeight; //Change board width and height

    //public int maxBoardWidth, maxBoardHeight;
    //public int minBoardWidth, minBoardHeight;
    public int maxPieceCount, minPieceCount;

    public Cell[,] boardCells;
    public List<PieceLocator> selectedPieces = new List<PieceLocator>();

    Vector3[,] cellPositions; //Real positions of cells

    static BoardManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;

            //DIFFICULTY
            GlobalAttributes.levelDifficulty = Random.Range(0, 3);
            if (GlobalAttributes.levelDifficulty == 2) //Hard
            {
                boardHeight = 5;
                boardWidth = 5;
            }
            else
            {
                boardHeight = 4;
                boardWidth = 4;
                if (GlobalAttributes.levelDifficulty == 1) //Medium
                    minPieceCount = 10;
                else
                    maxPieceCount = 10; //Easy
            }

            //boardWidth = Random.Range(minBoardWidth, maxBoardWidth + 1);
            //boardHeight = Random.Range(minBoardHeight, maxBoardHeight + 1);
            
            LoadCells();
        }
    }

    void LoadCells() //Instantiate cell objects
    {
        cellPositions = new Vector3[boardWidth, boardHeight];
        boardCells = new Cell[boardWidth, boardHeight];
        for (int i = -boardWidth / 2; i < boardWidth - (boardWidth / 2); i++)
        {
            for (int j = -boardHeight / 2; j < boardHeight - (boardHeight / 2); j++)
            {
                cellPositions[i + (boardWidth / 2), j + (boardHeight / 2)] = new Vector3((1.25f * i), 3.75f + (j * 1.25f), 0.1f);
                Instantiate(boardBackground, cellPositions[i + (boardWidth / 2), j + (boardHeight / 2)], Quaternion.identity); //Instantiate cells to scene
            }
        }
    }

    public void FillCells() //Fill cells with pieces that located by player
    {
        foreach (PieceLocator p in selectedPieces)
        {
            if (p.located)
            {
                GlobalAttributes.FillCells(boardCells, p.pieceData);
            }
        }
    }

    public static BoardManager GetInstance()
    {
        return instance;
    }

    public Vector3 GetPositionByCoordinates(int x, int y) //Retun the real position
    {
        return new Vector3(cellPositions[x, y].x, cellPositions[x, y].y, 0);
    }
}