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
        private float fadeInDuration = 1f;
        [SerializeField]
        private float fadeOutDuration = 1f;
        [Header("Common Settings")]
        [SerializeField]
        private GameState runOnState;

        private byte count = 0;
        // Update is called once per frame
        private void Update()
        {
            if(GameManager.GameState == runOnState)
            {
                if(count == 0)
                {
                    if(isFading)
                    {
                        Sequence sequence = DOTween.Sequence();

                        sequence.Append(material.DOFade(0, fadeOutDuration)).SetEase(Ease.Linear)
                            .Append(material.DOFade(1, fadeInDuration)).SetEase(Ease.Linear)
                            .Append(material.DOFade(0, fadeOutDuration)).SetEase(Ease.Linear).SetLoops(-1);
                    }
                    count++;
                }
            }
            else
            {
                count = 0;
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
