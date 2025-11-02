using UnityEngine;

namespace New
{
    public class Tile2D : MonoBehaviour
    {
        public int x, y;
        public SpriteRenderer highlight;
        public SpriteRenderer highlightCanEat;
        public GameObject stickySpot;
        
        public void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
            name = $"Tile_{x}_{y}";
            
            highlight.gameObject.SetActive(false);
            highlightCanEat.gameObject.SetActive(false);
            stickySpot.SetActive(false);
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
        
        public void ShowStickySpot(bool value)
        {
            stickySpot.SetActive(value);
        }
    }
}