using System.Collections.Generic;
using MatchThreeGame._Project.Scripts.Enums;
using MatchThreeGame._Project.Scripts.GridPiece;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private int xDim;
        [SerializeField] private int yDim;
        
        [SerializeField] private PiecePrefab[] piecePrefabs;
        [SerializeField] private GameObject backgroundPrefab;

        private Dictionary<PieceType, GameObject> _piecePrefabDictionary;
        private Piece[,] _pieces;

        private void Start()
        {
            _piecePrefabDictionary = new Dictionary<PieceType, GameObject>();

            foreach (var t in piecePrefabs)
                _piecePrefabDictionary.TryAdd(t.type, t.prefab);

            for (var i = 0; i < xDim; i++)
            {
                for (var j = 0; j < yDim; j++)
                {
                    var background = Instantiate(backgroundPrefab, GetWorldPosition(i, j), Quaternion.identity);
                    background.transform.parent = transform;
                }
            }

            _pieces = new Piece[xDim, yDim];
            for (var i = 0; i < xDim; i++)
            {
                for (var j = 0; j < yDim; j++)
                {
                    var newPiece = Instantiate(_piecePrefabDictionary[PieceType.NORMAL], Vector3.zero,
                        Quaternion.identity);
                    newPiece.name = "Piece (" + i +',' + j + ')';
                    newPiece.transform.parent = transform;

                    _pieces[i, j] = newPiece.GetComponent<Piece>();
                    _pieces[i, j].Init(i, j, PieceType.NORMAL,this);

                    if (_pieces[i, j].IsMovable())
                        _pieces[i, j].MovablePiece.Move(i, j);
                    
                    if (_pieces[i, j].IsColour())
                        _pieces[i, j].ColourPiece.ColourType = (ColourType)Random.Range(0, _pieces[i, j].ColourPiece.ColourAmount);
                }
            }
        }

        public Vector2 GetWorldPosition(int x, int y)
        {
            var position = transform.position;
            return new Vector2(position.x - xDim / 2.0f + x, position.y + yDim / 2.0f - y);
        }
    }
}