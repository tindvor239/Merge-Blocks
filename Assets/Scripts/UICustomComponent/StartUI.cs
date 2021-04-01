namespace UnityEngine.CustomComponents
{
    using UnityEngine.UI;
    public class StartUI : CustomUIComponent
    {
        #region Properties
        private Text highScoreDisplay { get => (Text)GetDisplay("highScore"); }
        public int highScore
        {
            get
            {
                if(highScoreDisplay != null)
                {
                    highScoreDisplay.text = PlayerPrefs.GetInt("highScore").ToString();
                    return PlayerPrefs.GetInt("highScore");
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                if(highScoreDisplay != null)
                {
                    PlayerPrefs.SetInt("highScore", value);
                    highScoreDisplay.text = value.ToString();
                }
            }
        }
        #endregion
        protected override void Start()
        {
            base.Start();
            highScore = highScore;
        }
    }
}