using System.IO;
using UnityEngine;
using UnityEditor;

public static class GlobalAttributes
{
    public static int cellSectionCount = 4; //Cells splitted to sections for filling them with triangles
    public static int currentLevel = 0;
    public static int levelDifficulty = 0;

    public static void FillCells(Cell[,] cell, PieceData piece)
    {
        for (int i = 0; i < piece.filledCellLocations.Count; i++)
        {
            for (int j = 0; j < GlobalAttributes.cellSectionCount; j++)
            {
                if ((int)piece.filledCellCount[i][j] == 1) //Dont change cell value to 0
                {
                    cell[piece.location[0] + piece.filledCellLocations[i][0], piece.location[1] + piece.filledCellLocations[i][1]].fillRate[j] = (int)piece.filledCellCount[i][j];
                }
            }
        }
    }

    public static void EmptyCells(Cell[,] cell)
    {
        for (int i = 0; i < cell.GetLength(0); i++)
            for (int j = 0; j < cell.GetLength(1); j++)
                cell[i, j] = new Cell(new int[2] { i, j }, new Vector4(0, 0, 0, 0));
    }

    public static bool IsInsideBoard(PieceData piece, int width, int height)
    {
        for (int i = 0; i < piece.bordersOfPiece.Count; i++)
        {
            if (piece.bordersOfPiece[i][0] + piece.location[0] >= width || piece.bordersOfPiece[i][0] + piece.location[0] < 0 || piece.location[0] < 0 || piece.location[0] >= width) //Inside board
            {
                return false;
            }
            if (piece.bordersOfPiece[i][1] + piece.location[1] >= height || piece.bordersOfPiece[i][1] + piece.location[1] < 0 || piece.location[1] < 0 || piece.location[1] >= height)
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsBoardFilled(Cell[,] cell)
    {
        for (int i = 0; i < cell.GetLength(0); i++)
        {
            for (int j = 0; j < cell.GetLength(1); j++)
            {
                for (int z = 0; z < GlobalAttributes.cellSectionCount; z++)
                    if (cell[i, j].fillRate[z] == 0)
                        return false;
            }
        }
        return true;
    }

    public static LevelData GetLevelData() //Read json file
    {
        string path = Application.dataPath + "/Resources/LevelData" + currentLevel + ".json";
        #if UNITY_EDITOR
            path = "Assets/Resources/LevelData" + currentLevel + ".json";
        #endif
        string level;
        using (StreamReader sr = new StreamReader(path))
        {
            level=sr.ReadToEnd();
        }
        LevelData data = JsonUtility.FromJson<LevelData>(level);
        return data;  //Return the .json data
    }

    public static void SaveLevelData(LevelData level) //Save level
    {
        string path = Application.dataPath + "/Resources/LevelData" + currentLevel + ".json";
        #if UNITY_EDITOR
            path = "Assets/Resources/LevelData" + currentLevel + ".json";
        #endif
        string str = JsonUtility.ToJson(level);
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(str);
            }
        }
        #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
        #endif
    }
}