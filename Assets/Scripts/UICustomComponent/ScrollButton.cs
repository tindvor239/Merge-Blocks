namespace UnityEngine.CustomComponents
{
    using UnityEngine.UI;
    using DG.Tweening;
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollButton : MonoBehaviour
    {
        private ScrollRect scroll;
        [SerializeField]
        private float duration = 0.5f;
        [SerializeField]
        private string currentDisplay;
        [SerializeField]
        private Text[] displays;
        private int index = 0;
        private void Start()
        {
            currentDisplay = displays[0].text;
            scroll = GetComponent<ScrollRect>();
        }
        public void ScrollHorizontal(int x)
        {
            if(index + x >= 0 && index + x < displays.Length)
            {
                index += x;
                currentDisplay = displays[index].text;
                scroll.DOHorizontalNormalizedPos(x, duration);
            }
        }
    }
}