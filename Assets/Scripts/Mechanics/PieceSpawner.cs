using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    LevelData levelData;
    List<GameObject> instantiatedObjects = new List<GameObject>();

    void Start()
    {
        levelData = GlobalAttributes.GetLevelData();
        SpawnPieces();
    }

    void SpawnPieces()
    {
        PieceBehaviour tempPiece;
        for (int i = 0; i < levelData.selectedPieceIds.Length; i++)
        {
            instantiatedObjects.Add(Instantiate(PieceManager.GetInstance().allPieces[levelData.selectedPieceIds[i]], new Vector3(Random.Range(-3f, 3f), Random.Range(-2f, -5f), 0), Quaternion.Euler(0, 0, levelData.selectedPieceRotations[i] * -90))); //Spawn pieces to random positions
            instantiatedObjects[instantiatedObjects.Count - 1].AddComponent<PieceLocator>();
            tempPiece = instantiatedObjects[instantiatedObjects.Count - 1].GetComponent<PieceBehaviour>();
            tempPiece.ChangeRotation(levelData.selectedPieceRotations[i]);
            tempPiece.ChangeMaterial(PieceManager.GetInstance().allMaterials[Random.Range(0, PieceManager.GetInstance().allMaterials.Count)]); //Change their material
        }
    }
}
