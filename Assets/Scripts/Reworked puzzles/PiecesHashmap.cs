using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Collections;

public class PiecesHashmap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static readonly Dictionary<string, string> PieceIds = new Dictionary<string, string>
    {
        {"00", "None"},
        {"sb", "StandardBishop"},
        {"sp", "StandardPawn"},
        {"sn", "StandardKnight"},
        {"sr", "StandardRook"},
        {"sq", "StandardQueen"},
        {"sk", "StandardKing"},
    };
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
