using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject boardBackground; //Board background object
    public int boardWidth, boardHeight; //Change board width and height
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
            LoadCells();
        }
    }

    void LoadCells()
    {
        cellPositions = new Vector3[boardHeight, boardWidth];
        for (int i = -boardHeight / 2; i < boardHeight - (boardHeight / 2); i++)
        {
            for (int j = -boardWidth / 2; j < boardWidth - (boardWidth / 2); j++)
            {
                cellPositions[i + (boardHeight / 2), j + (boardWidth / 2)] = new Vector3((1.25f * i), 3.75f + (j * 1.25f), 0.1f);
                Instantiate(boardBackground, cellPositions[i + (boardHeight / 2), j + (boardWidth / 2)], Quaternion.identity); //Instantiate cells to scene
            }
        }
    }

    public static BoardManager GetInstance()
    {
        return instance;
    }

    public Vector3 GetPositionByLocation(int x, int y)
    {
        return new Vector3(cellPositions[x, y].x, cellPositions[x, y].y, 0);
    }
}