
namespace UnityEngine.CustomComponents
{
    using DoozyUI;
    using UnityEngine.UI;
    using System.Collections.Generic;
    public class UIManager : Singleton<UIManager>
    {
        [Header("Game Play")]
        [SerializeField]
        private FillBar levelBar;
        [SerializeField]
        private CustomUIComponent goal;
        [SerializeField]
        private CustomUIComponent level;
        [SerializeField]
        private CustomUIComponent money;
        [SerializeField]
        private LevelUpUI levelUpUI;
        [SerializeField]
        private ComplimentUI compliment;
        [SerializeField]
        private ParticleSystem[] horrayParticles;
        [SerializeField]
        private ClearUI clearUI;
        [SerializeField]
        private RectTransform grid;
        [SerializeField]
        private GridLayout gridLayout;
        private float durationTime;

        [Header("Setting")]
        private PauseUI pauseUI;
        protected delegate void OnShowTemporaryUI(float duration, bool isVisible, string UIname);
        protected OnShowTemporaryUI onShowTemporaryUI;
        #region Properties
        public FillBar LevelBar { get => levelBar; }
        public Text CurrentLevel { get => (Text)level.GetDisplay("currentLevel"); }
        public Text NextLevel { get => (Text)level.GetDisplay("nextLevel"); }
        public Image GoalCat { get => (Image)goal.GetDisplay("catIcon"); }
        public Text GoalPoint { get => (Text)goal.GetDisplay("catPoint"); }
        public Text Money { get => (Text)money.GetDisplay("money"); }
        public RectTransform Grid { get => grid; }
        public GridLayout Layout { get => gridLayout; }
        public ParticleSystem[] HorrayParticles { get => horrayParticles; }
        #endregion
        protected override void Awake()
        {
            #region Singleton
            base.Awake();
            #endregion
        }
        private void Start()
        {
            ChangeGoal(128 * 2);
        }
        private void Update()
        {
            if(onShowTemporaryUI != null)
            {
                onShowTemporaryUI.Invoke(2f, false, "CLEAR_UI");
            }
        }
        public void ShowClearUI(uint point)
        {
            VisibleUI(true, "CLEAR_UI");
            ChangeGoal(point);
            StartShowTemporary();
            clearUI.Initialize();
        }
        public void ShowLevelUpUI()
        {
            VisibleUI(true, "LEVELUP_UI");
            GameManager.GameState = GameState.pause;
            levelUpUI.Initialized(GameManager.Instance.GetSpriteFromPoint(128));
        }
        public void ClaimReward()
        {
            GameManager.GameState = GameState.play;
            VisibleUI(false, "LEVELUP_UI");
        }
        public void ChangeGoal(in uint point)
        {
            Sprite icon = GameManager.Instance.GetSpriteFromPoint(point);
            Debug.Log($"icon: {icon}");
            int index = GameManager.Instance.CatImages.IndexOf(icon);
            Debug.Log($"index: {index}");
            GoalCat.sprite = GameManager.Instance.CatImages[index];
            GoalPoint.text = (point).ToString();
        }
        public void ShowUI()
        {
            switch (GameManager.GameState)
            {
                case GameState.mainMenu:
                    UIElement.ShowUIElement("START_UI");
                    break;
                case GameState.play:
                    UIElement.HideUIElement("START_UI");
                    UIElement.ShowUIElement("GAMEPLAY_UI");
                    UIElement.HideUIElement("PAUSE_UI");
                    break;
                case GameState.pause:
                    UIElement.ShowUIElement("PAUSE_UI");
                    break;
                case GameState.gameOver:
                    UIElement.ShowUIElement("GAMEOVER_UI");
                    break;
            }
        }
        private void StartShowTemporary()
        {
            GameManager.GameState = GameState.pause;
            durationTime = 0;
            onShowTemporaryUI += VisibleUIByTime;
        }
        public void ShowCompliment(int combo)
        {
            if(combo - GameController.Instance.DropCoinableComboPoint >= 0)
            {
                compliment.ShowComplimentEffect(compliment.GetCompliment(combo));
            }
        }
        private void VisibleUIByTime(float duration, bool isVisible, string UIname)
        {
            durationTime += Time.deltaTime;
            if(durationTime >= duration)
            {
                VisibleUI(isVisible, UIname);
                durationTime = 0;
                GameManager.GameState = GameState.play;
                onShowTemporaryUI = null;
            }
        }
        private void VisibleUI(bool isVisible, string UIname)
        {
            if(isVisible)
            {
                UIElement.ShowUIElement(UIname);
            }
            else
            {
                UIElement.HideUIElement(UIname);
            }
        }
    }
}
