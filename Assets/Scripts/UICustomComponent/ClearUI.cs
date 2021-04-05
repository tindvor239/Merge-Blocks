using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.CustomComponents
{
    using UnityEngine.UI;
    using DG.Tweening;
    public class ClearUI : CustomUIComponent
    {
        private Sequence sequence;
        #region Properties
        public Image GoalCat { get => (Image)GetDisplay("catCongratulations"); }
        public Image ShinyEffect { get => (Image)GetDisplay("shinyEffect"); }
        #endregion
        // Start is called before the first frame update
        protected override void Start()
        {
        }

        // Update is called once per frame
        protected override void Update()
        {
        
        }
        public void Initialize()
        {
            GoalCat.sprite = GameManager.Instance.CatImages[GameManager.Instance.CatImages.IndexOf(UIManager.Instance.GoalCat.sprite)];
            CatScaleInEffect(new Vector3(), new Vector3(1, 1, 1), Ease.Linear);
            StartAnimate(new Vector3(0, 0, 180), 2f, 1f);
            PlayParticles();
        }
        private void PlayParticles()
        {
            foreach(ParticleSystem particle in UIManager.Instance.HorrayParticles)
            {
                particle.Play();
            }
        }
        private void CatScaleInEffect(in Vector3 startScale, in Vector3 endScale, in Ease ease)
        {
            Vector3 middleScale = endScale + new Vector3(0.5f, 0.5f, 0.5f);
            Sequence sequence = DOTween.Sequence();
            GoalCat.transform.localScale = startScale;
            sequence.Append(GoalCat.transform.DOScale(middleScale, 1f).SetEase(ease))
                .Append(GoalCat.transform.DOScale(endScale, 0.5f).SetEase(ease));
        }
        private void StartAnimate(in Vector3 rotateAngle, in float rotateTime, in float blinkTime)
        {
            Fading(in blinkTime);
            Rotate(in rotateAngle, in rotateTime);
        }
        private void Fading(in float duration)
        {
            if(sequence != null && sequence.IsPlaying())
            {
                sequence.Kill(true);
            }
            sequence = DOTween.Sequence();
            sequence.Append(Fade(0.2f, duration)).Append(Fade(1f, duration)).SetLoops(-1);
        }
        private void Rotate(in Vector3 destination, in float time)
        {
            ShinyEffect.transform.DORotate(destination, time).SetEase(Ease.Linear).SetLoops(-1);
        }
        private Tween Fade(float alpha, float time)
        {
            return ShinyEffect.DOFade(alpha, time);
        }
    }
}
