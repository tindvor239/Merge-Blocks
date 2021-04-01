using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

namespace UnityEngine.CustomComponents
{
    public class ComplimentUI : CustomUIComponent
    {
        private Image image;
        public void ShowComplimentEffect(Image image)
        {
            Sequence sequence = DOTween.Sequence();
            if(!image.gameObject.activeInHierarchy)
            {
                image.gameObject.SetActive(true);
            }
            image.DOFade(1f, 0.4f).SetEase(Ease.Linear);
            sequence.Append(image.transform.DOScale(new Vector3(1, 1, 1), 0.3f).SetEase(Ease.Linear))
                .Append(image.transform.DOScale(new Vector3(1.4f, 1.4f, 1), 0.4f).SetEase(Ease.Linear))
                .Append(image.transform.DOScale(new Vector3(0f, 0f, 1), 0.3f).SetEase(Ease.Linear));
            this.image = image;
            sequence.OnComplete(Disable);
        }

        private void Disable()
        {
            image.gameObject.SetActive(false);
        }
        public Image GetCompliment(int combo)
        {
            string compliment = "";
            if (combo == 3)
            {
                compliment = "nice";
            }
            else if(combo == 4)
            {
                compliment = "good";
            }
            else if(combo == 5)
            {
                compliment = "excellent";
            }
            else if(combo == 6)
            {
                compliment = "perfect";
            }
            else if (combo == 7)
            {
                compliment = "amazing";
            }
            else if (combo == 8)
            {
                compliment = "unbelievable";
            }
            return (Image)GetDisplay(compliment);
        }
    }
}