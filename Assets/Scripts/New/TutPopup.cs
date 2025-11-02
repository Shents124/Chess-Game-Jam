using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace New
{
    public class TutPopup : MonoBehaviour
    {
        [SerializeField] private Image tutImage;
        [SerializeField] private GameObject container;
        [SerializeField] private GameObject tutAnim;
        [SerializeField] private Button closeButton;
        [SerializeField] private Sprite[] tutSprite;

        private void Awake()
        {
            container.SetActive(false);
            closeButton.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            container.SetActive(false);
        }

        public void ShowTut(int id)
        {
            closeButton.interactable = false;
            tutImage.sprite = tutSprite[id];
            container.SetActive(true);
            tutAnim.SetActive(true);
            tutAnim.transform.localScale = Vector3.one * 2;
            tutAnim.transform.DOScale(Vector3.one * 4, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                closeButton.interactable = true;
            });
        }
    }
}