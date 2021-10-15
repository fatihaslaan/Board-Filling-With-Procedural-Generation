using System.Collections.Generic;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    static List<PieceController> allPieces = new List<PieceController>() { new Piece_1()}; //Pieces that will be chosen for filling board
    List<PieceController> spawnedPieces = new List<PieceController>(); //Chosen pieces
    PieceController spawnedPiece; //Current spawmed piece

    Cell[,] c = new Cell[3, 3]; //Cell

    int crash = 0; //To prevent infinite loop
    string log=""; //To see chosen pieces

    void LoadCells() //Load cells with empty fill rate
    {
        Vector4 a=new Vector4(0,3,0,0);
        float ab=a[1];
        Debug.Log(ab);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                c[i, j] = new Cell(new int[2] { i, j }, new int[4] { 0, 0, 0, 0 });
            }
        }
    }

    void ChoosePieces() //Choose correct pieces to fill board with procedural generation
    {
        do
        {
            crash++;
            do
            {
                crash++;
                spawnedPiece = allPieces[Random.Range(0, allPieces.Count)];
                spawnedPiece.location = new int[2] { Random.Range(0, 3), Random.Range(0, 3) };
            } while (!IsCellsAvailable() && Crashed("1. crash")); //Piece added to board
            spawnedPieces.Add(spawnedPiece);
            FillCells(); //Fill cells according to spawned piece's filled cells
            log+="\n name "+ spawnedPiece.id()+" locx: "+spawnedPiece.location[0]+" locy: "+spawnedPiece.location[1];
            Debug.Log(log);
        } while (!IsBoardFilled() && Crashed("2. crash"));
        Debug.Log("Board Filled");
    }

    void SpawnPieces()
    {

    }

    void Start()
    {
        LoadCells();
        ChoosePieces();
        SpawnPieces();
    }


    void FillCells()
    {
        for (int i = 0; i < spawnedPiece.FilledCells().Count; i++)
        {
            for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
            {
                c[spawnedPiece.location[0] + spawnedPiece.FilledCells()[i].location[0], spawnedPiece.location[1] + spawnedPiece.FilledCells()[i].location[1]].fillRate[j] = spawnedPiece.FilledCells()[i].fillRate[j];
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
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
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
        for (int i = 0; i < spawnedPiece.Borders().Count; i++)
        {
            if (spawnedPiece.Borders()[i][0] + spawnedPiece.location[0] >= 3 || spawnedPiece.Borders()[i][0] + spawnedPiece.location[0] < 0) //Inside board
            {
                return false;
            }
            if (spawnedPiece.Borders()[i][1] + spawnedPiece.location[1] >= 3 || spawnedPiece.Borders()[i][1] + spawnedPiece.location[1] < 0)
            {
                return false;
            }
        }
        for (int i = 0; i < spawnedPiece.FilledCells().Count; i++)
        {
            for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
            {
                if (c[spawnedPiece.location[0] + spawnedPiece.FilledCells()[i].location[0], spawnedPiece.location[1] + spawnedPiece.FilledCells()[i].location[1]].fillRate[j] == 1) //Cell available
                {
                    return false;
                }
            }
        }
        return true;
    }
}