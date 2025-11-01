using UnityEngine;

namespace New
{
    public class Tile2D : MonoBehaviour
    {
        public int x, y;
        public SpriteRenderer highlight;
        public SpriteRenderer highlightCanEat;

        private void Start()
        {
            highlight.gameObject.SetActive(false);
            highlightCanEat.gameObject.SetActive(false);
        }

        public void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
            name = $"Tile_{x}_{y}";
        }

        public void SetHighlight(bool on, bool canEat = false)
        {
            if (canEat)
            {
                highlight.gameObject.SetActive(false);
                highlightCanEat.gameObject.SetActive(true);
            }
            else
            {
                highlight.gameObject.SetActive(on);
                highlightCanEat.gameObject.SetActive(false);
            }
        }
    }
}