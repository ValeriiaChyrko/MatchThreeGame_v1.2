using System.Collections;
using UnityEngine;

namespace MatchThreeGame._Project.Scripts.GridPiece
{
    public class ClearablePiece : MonoBehaviour
    {
        [SerializeField] private AnimationClip clearAnimation;

        public bool IsBeingCleared { get; private set; }
        protected Piece Piece;

        private void Awake()
        {
            Piece = GetComponent<Piece>();
        }

        public virtual void Clear()
        {
            IsBeingCleared = true;
            StartCoroutine(ClearCoroutine());
        }

        private IEnumerator ClearCoroutine()
        {
            var animator = GetComponent<Animator>();
            if (!animator) yield break;
            
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length);
            
            Destroy(gameObject);
        }
    }
}
