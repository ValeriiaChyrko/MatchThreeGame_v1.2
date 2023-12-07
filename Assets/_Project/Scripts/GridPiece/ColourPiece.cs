using System.Collections.Generic;
using MatchThreeGame._Project.Scripts.Enums;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.GridPiece
{
    public class ColourPiece : MonoBehaviour
    {
        [SerializeField] private ColourSprite[] colourSprite;
        private SpriteRenderer _spriteRenderer;
        
        private Dictionary<ColourType, Sprite> _colourSpriteDictionary;
        public int ColourAmount => colourSprite.Length;

        private ColourType _colourType;
        public ColourType ColourType
        {
            get => _colourType;
            set => SetColour(value);
        }
        private void SetColour(ColourType newColour)
        {
            _colourType = newColour;

            if (_colourSpriteDictionary.ContainsKey(newColour))
                _spriteRenderer.sprite = _colourSpriteDictionary[_colourType];
        }

        private void Awake()
        {
            _spriteRenderer = transform.Find("piece").GetComponent<SpriteRenderer>();
            _colourSpriteDictionary = new Dictionary<ColourType, Sprite>();

            foreach (var t in colourSprite)
                _colourSpriteDictionary.TryAdd(t.colour, t.sprite);
        }
    }
}
