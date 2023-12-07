using System;
using MatchThreeGame._Project.Scripts.Enums;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.GridPiece
{
    [Serializable]
    public class PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }
}