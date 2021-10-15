﻿using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public List<GameObject> allPieces;

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
        }
    }

    public static PieceManager GetInstance()
    {
        return instance;
    }
}