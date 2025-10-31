using UnityEngine;

namespace New
{
    public class Tile2D : MonoBehaviour
    {
        public int x, y;
        public SpriteRenderer highlight;
        public SpriteRenderer highlightCanEat;

        public void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
            name = $"Tile_{x}_{y}";
        }

        public void SetHighlight(bool on, bool canEat = false)
        {
            if (highlight != null)
                highlight.gameObject.SetActive(on);

            if (highlightCanEat != null)
                highlightCanEat.gameObject.SetActive(canEat);
        }
    }
}