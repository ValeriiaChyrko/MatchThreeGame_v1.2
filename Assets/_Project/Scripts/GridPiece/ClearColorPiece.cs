using MatchThreeGame._Project.Scripts.Enums;

namespace MatchThreeGame._Project.Scripts.GridPiece
{
    public class ClearColorPiece : ClearablePiece
    {
        public ColourType Colour { get; set; }

        public override void Clear()
        {
            base.Clear();
            Piece.GridRef.ClearColour(Colour);
        }
    }
}
