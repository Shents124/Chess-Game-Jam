using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Vfx
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class VfxScaleFlyUp : MonoBehaviour
    {
        [SerializeField] private float scaleUp = 1f;
        [SerializeField] private float scaleTime = 0.25f;

        [SerializeField] private float moveUpDistance = 1f;
        [SerializeField] private float moveDuration = 1f;

        [SerializeField] private float fadeDelay = 0.3f;

        private SpriteRenderer _spriteRenderer;
        private Sequence _seq;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            gameObject.SetActive(false);
        }
        
        private void OnDestroy()
        {
            _seq?.Kill();
        }

        public void PlayEffect(Action callBack = null)
        {
            gameObject.SetActive(true);
            // Reset trạng thái
            transform.localScale = Vector3.zero;
            Color startColor = _spriteRenderer.color;
            startColor.a = 1f;
            _spriteRenderer.color = startColor;
            gameObject.SetActive(true);
            Vector3 targetPosition = transform.position + Vector3.up * moveUpDistance;

            _seq?.Kill();
            _seq = DOTween.Sequence();

            // Phóng to
            _seq.Append(transform.DOScale(scaleUp, scaleTime).SetEase(Ease.OutBack));

            // Di chuyển lên
            _seq.Join(transform.DOMoveY(targetPosition.y, moveDuration).SetEase(Ease.OutCubic));

            // Fade out sau delay
            _seq.Insert(fadeDelay, _spriteRenderer.DOFade(0f, moveDuration - fadeDelay));

            // Huỷ hoặc pooling
            _seq.OnComplete(() => {
                callBack?.Invoke();
                gameObject.SetActive(false);
            });
        }
        
        public async UniTask PlayEffectAsync()
        {
            gameObject.SetActive(true);
            // Reset trạng thái
            transform.localScale = Vector3.zero;
            Color startColor = _spriteRenderer.color;
            startColor.a = 1f;
            _spriteRenderer.color = startColor;
            gameObject.SetActive(true);
            Vector3 targetPosition = transform.position + Vector3.up * moveUpDistance;

            _seq?.Kill();
            _seq = DOTween.Sequence();

            // Phóng to
            _seq.Append(transform.DOScale(scaleUp, scaleTime).SetEase(Ease.OutBack));

            // Di chuyển lên
            _seq.Join(transform.DOMoveY(targetPosition.y, moveDuration).SetEase(Ease.OutCubic));

            // Fade out sau delay
            _seq.Insert(fadeDelay, _spriteRenderer.DOFade(0f, moveDuration - fadeDelay));
            // Huỷ hoặc pooling
            _seq.OnComplete(() => {
                gameObject.SetActive(false);
            });
           
            await _seq.AsyncWaitForCompletion();
        }

        private async UniTask DelayDestroy()
        {
            await UniTask.Delay(100);
            Destroy(gameObject);
        }
    }
}