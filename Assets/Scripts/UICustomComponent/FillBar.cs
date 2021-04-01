using Coffee.UIEffects;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

namespace UnityEngine.CustomComponents
{
    [RequireComponent(typeof(Slider))]
    public class FillBar : MonoBehaviour
    {
        private Slider slider;
        [SerializeField]
        private UIShiny shinyEffect;
        [SerializeField]
        private float shinySpeed = 1f;
        [SerializeField]
        private float fadeSpeed = 0.5f;
        private float maxBrightness;
        private Sequence sequence;

        private delegate void OnSequenceStartCallBack();
        private event OnSequenceStartCallBack onSequenceStartCallBackEvent;
        private delegate void OnSequenceEndCallBack();
        private event OnSequenceEndCallBack onSequenceEndCallBackEvent;
        // Start is called before the first frame update
        private void Start()
        {
            maxBrightness = shinyEffect.brightness;
            if (slider == null)
            {
                slider = GetComponent<Slider>();
            }
            sequence = DOTween.Sequence();
        }
        private void Update()
        {
            if (onSequenceStartCallBackEvent != null)
            {
                onSequenceStartCallBackEvent.Invoke();
                if (shinyEffect.brightness >= maxBrightness)
                {
                    shinyEffect.brightness = maxBrightness;
                    onSequenceStartCallBackEvent = null;
                }
            }
            else if (onSequenceEndCallBackEvent != null)
            {
                onSequenceEndCallBackEvent.Invoke();
                if (shinyEffect.brightness <= 0)
                {
                    shinyEffect.brightness = 0;
                    onSequenceEndCallBackEvent = null;
                }

            }
        }
        private void ShiningEffect()
        {
            onSequenceStartCallBackEvent += Shine;
        }
        private void ShadingEffect()
        {
            onSequenceEndCallBackEvent += Fade;
        }
        private void Fade()
        {
            shinyEffect.brightness -= Time.deltaTime * fadeSpeed;
        }
        private void Shine()
        {
            shinyEffect.brightness += Time.deltaTime * shinySpeed;
        }

        public virtual void Move(float destination)
        {
            sequence.Append(slider.DOValue(destination, 0.3f).OnPlay(ShiningEffect).OnComplete(ShadingEffect));
        }
    }
}
