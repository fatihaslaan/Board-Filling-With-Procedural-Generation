using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public List<GameObject> allPieces; //Piece objects
    public List<Material> allMaterials; //Materials for objects

    public GameObject pieceSpawner;

    static PieceManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            ChangeIds();
        }
    }

    void ChangeIds()
    {
        for(int i=0;i<allPieces.Count;i++)
        {
            allPieces[i].GetComponent<PieceBehaviour>().id=i; //Change ids according to object order
        }
    }

    public static PieceManager GetInstance()
    {
        return instance;
    }
}