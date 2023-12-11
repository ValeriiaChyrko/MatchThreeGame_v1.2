using MatchThreeGame._Project.Scripts.Enums;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.GridPiece
{
    public class Piece : MonoBehaviour
    {
        [SerializeField] private int score;
        public int Score => score;
        private int _x;

        public int X
        {
            get => _x;
            set
            {
                if (IsMovable())
                {
                    _x = value;
                }
            }
        }

        private int _y;

        public int Y
        {
            get => _y;
            set
            {
                if (IsMovable())
                {
                    _y = value;
                }
            }
        }

        public PieceType Type { get; private set; }
        public GridController GridRef { get; private set; }

        public MovablePiece MovableComponent { get; private set; }
        public bool IsMovable() => MovableComponent != null;

        public ColourPiece ColourComponent { get; private set; }
        public bool IsColour() => ColourComponent != null;

        public ClearablePiece ClearableComponent { get; private set; }
        public bool IsClearable() => ClearableComponent != null;

        private void Awake()
        {
            MovableComponent = GetComponent<MovablePiece>();
            ColourComponent = GetComponent<ColourPiece>();
            ClearableComponent = GetComponent<ClearablePiece>();
        }

        public void Init(int x, int y, PieceType type, GridController gridRef)
        {
            X = x;
            Y = y;
            Type = type;
            GridRef = gridRef;
        }

        private void OnMouseEnter()
        {
            GridRef.Destination(this);
        }

        private void OnMouseUp()
        {
            GridRef.Release();
        }

        private void OnMouseDown()
        {
            GridRef.Source(this);
        }

        public void ReturnToPool()
        {
            ObjectPool.Instance.ReturnGameObject(gameObject);
        }

        public Piece CreateFromPool(int x, int y, PieceType type, GridController gridRef)
        {
            var newGameObject = ObjectPool.Instance.GetObject(gameObject);
            var newPiece = newGameObject.GetComponent<Piece>();
            newPiece.Init(x, y, type, gridRef);
            return newPiece;
        }
    }
}