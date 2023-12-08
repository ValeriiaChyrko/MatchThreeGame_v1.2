using System;
using MatchThreeGame._Project.Scripts.Enums;

namespace MatchThreeGame._Project.Scripts.GridPiece
{
    [Serializable]
    public class PiecePosition
    {
        public PieceType type;
        public int x;
        public int y;
    }
}