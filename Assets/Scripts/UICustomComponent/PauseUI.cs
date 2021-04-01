using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.CustomComponents
{
    using UnityEngine.UI;
    public class PauseUI : MonoBehaviour
    {
        [Header("Sound")]
        [SerializeField]
        private Image currentSound;
        [SerializeField]
        private Text soundText;
        [SerializeField]
        private Sprite soundOnIcon, soundOffIcon;
        [Header("Music")]
        [SerializeField]
        private Image currentMusic;
        [SerializeField]
        private Text musicText;
        [SerializeField]
        private Sprite musicOnIcon, musicOffIcon;
        [SerializeField]
        private Button close;
        private static bool isSound = true;
        private static bool isMusic = true;

        #region Properties
        public static bool IsSound { get => isSound; }
        #endregion
        public void Close()
        {
            GameManager.Instance.Play();
        }
        public void Sound()
        {
            isSound = !isSound;
            OnClickFlipFlopButton(!isSound, "sound", currentSound, soundOnIcon, soundOffIcon, soundText);
        }
        public void Music()
        {
            isMusic = !isMusic;
            OnClickFlipFlopButton(!isMusic, "music", currentMusic, musicOnIcon, musicOffIcon, musicText);
        }

        private void OnClickFlipFlopButton(bool condition, in string content, Image display, in Sprite onDisplay, in Sprite offDisplay, Text output)
        {
            condition = !condition;
            Debug.Log("In");
            if(condition)
            {
                output.text = content + " On";
                display.sprite = onDisplay;
            }
            else
            {
                output.text = content + " Off";
                display.sprite = offDisplay;
            }
        }
    }
}