namespace UnityEngine.CustomComponents
{
    using DG.Tweening;
    public class Paralax : MonoBehaviour
    {
        [SerializeField]
        private Material material;
        [Header("Movement")]
        [SerializeField]
        private Vector2 offsetMovement = new Vector2();
        [SerializeField]
        private float speed = 0.2f;
        [Header("Fading")]
        [SerializeField]
        private bool isFading;
        [SerializeField]
        private float fadeInPercent;
        [SerializeField]
        private float fadeOutPercent;
        [SerializeField]
        private float fadeInDuration = 1f;
        [SerializeField]
        private float fadeOutDuration = 1f;
        [Header("Common Settings")]
        [SerializeField]
        private GameState runOnState;

        private Sequence sequence;
        private float time = 0;
        private byte count = 0;
        // Update is called once per frame
        private void Start()
        {
            time = 1/speed;
            Debug.Log($"time{time}");
            sequence = DOTween.Sequence();
            if (isFading)
            {
                sequence
                .Append(material.DOFade(1, time / 4)).SetEase(Ease.Linear)
                .Append(material.DOFade(0, time / 4)).SetEase(Ease.Linear)
                ;
                sequence.SetLoops(-1);
                sequence.PrependInterval(2 * time / 3);
            }
            //s2 = 1.                            t2 = ???
            //s1 = speed * Time.deltaTime.       t1 = Time.deltaTime.
        }
        private void Update()
        {
            if(GameManager.GameState == runOnState)
            {
                material.mainTextureOffset = new Vector2(Mathf.Clamp(material.mainTextureOffset.x, 0f, 1f), Mathf.Clamp(material.mainTextureOffset.y, 0f, 1f));
                if(material.mainTextureOffset.x == 1f)
                {
                    material.mainTextureOffset = new Vector2(0, material.mainTextureOffset.y);
                }
                if(material.mainTextureOffset.y == 1)
                {
                    material.mainTextureOffset = new Vector2(material.mainTextureOffset.x, 0);
                }
            }
        }
        private void FixedUpdate()
        {
            if(GameManager.GameState == runOnState)
            {
                float velocity = speed * Time.deltaTime;
                material.mainTextureOffset += new Vector2(offsetMovement.x * velocity, offsetMovement.y * velocity);
                
            }
        }
    }
}
