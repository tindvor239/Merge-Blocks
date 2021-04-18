using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState {mainMenu, play, pause, gameOver, onExit}
namespace UnityEngine.CustomComponents
{
    public class GameManager : Singleton<GameManager>
    {
        private static GameState gameState = GameState.mainMenu;
        private static float defaultScreenWidth = 720f, defaultScreenHeight = 1280f;
        private static float resolutionRatio;
        private static uint startExperience = 100;
        private static uint percentPerLevel = 44;
        private static uint currentExperience = 0;
        private static uint currentLevel = 1;
        [Header("Gameplay")]
        [SerializeField]
        private List<Sprite> catImages = new List<Sprite>();
        [SerializeField]
        private RectTransform screen;

        public delegate GameState OnStateChangeDelegate();
        public event OnStateChangeDelegate onStateChange;
        // Width Ratio = Screen Width / Object Width.
        // Height Ratio = Screen Height / Object Height.
        // Ratio = Width Ratio < Height Ratio ? Width Ratio : Height Ratio;
        #region Properties
        public static GameState GameState { get => gameState; set => gameState = value; }
        public static uint CurrentLevel
        {
            get
            {
                return currentLevel;
            }
            set
            {
                UIManager.Instance.CurrentLevel.text = value.ToString();
                currentLevel = value;
            }
        }
        private static uint NextLevel
        {
            get
            {
                UIManager.Instance.NextLevel.text = (currentLevel + 1).ToString();
                return currentLevel + 1;
            }
        }
        public static uint CurrentExperience
        {
            get => currentExperience;
            set
            {
                currentExperience = value;
                float process = (float)currentExperience / (float)NextLevelExperience;
                Debug.Log($"current exp: {currentExperience}, next exp: {NextLevelExperience}");
                UIManager.Instance.LevelBar.Move(process);
            }
        }
        public static uint NextLevelExperience
        {
            get => CalculateNextLevelExperience();
        }
        public List<Sprite> CatImages { get => catImages; }
        public int Money
        {
            get
            {
                try
                {
                    return int.Parse(UIManager.Instance.Money.text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    UIManager.Instance.Money.text = value.ToString();
                }
                catch
                {
                    UIManager.Instance.Money.text = "0";
                }
            }
        }
        #endregion
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            float widthRatio = defaultScreenWidth / screen.rect.width;
            float heightRatio = defaultScreenHeight / screen.rect.height;
            UIManager.Instance.Grid.GetComponent<GridLayout>();
            if(widthRatio < 0 || heightRatio < 0)
            {
                resolutionRatio = widthRatio < heightRatio ? widthRatio : heightRatio;
            }
            else
            {
                resolutionRatio = 1f;
            }
        }
        private void Start()
        {
            MainMenu();
            currentExperience = startExperience;
            //calculate next level experience.
        }
        // Update is called once per frame
        private void Update()
        {
            if(onStateChange != null)
            {
                onStateChange.Invoke();
                Debug.Log($"state: {gameState}");
                UIManager.Instance.ShowUI();
                onStateChange = null;
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                onStateChange += BackHandle;
            }
        }

        private GameState BackHandle()
        {
            switch(gameState)
            {
                case GameState.gameOver:
                    //TO DO: Game exit.
                    return GameState.gameOver;
                case GameState.play:
                    //TO DO: Pause.
                    return GameState.pause;
                case GameState.pause:
                    //TO DO: Play.
                    return GameState.play;
                case GameState.mainMenu:
                    return GameState.onExit;
                default:
                    return GameState.mainMenu;
            }
        }
        private static uint CalculateNextLevelExperience()
        {
            uint experienceNextLevel = startExperience + (percentPerLevel * NextLevel);
            return experienceNextLevel;
        }

        #region GameState
        public void Pause()
        {
            SetState(GameState.pause);
        }
        public void Play()
        {
            SetState(GameState.play);
        }
        public void MainMenu()
        {
            SetState(GameState.mainMenu);
        }
        public void GameOver()
        {
            SetState(GameState.gameOver);
        }
        public void SetState(GameState state)
        {
            gameState = state;
            onStateChange += OnSetState;
        }    
        private GameState OnSetState()
        {
            return gameState;
        }
        #endregion

        #region Particle
        public void GetAndPlayParticle(in Vector2 position, in ObjectPool pool)
        {
            GameObject particleObject = GetPooledObject(pool);
            particleObject.transform.position = position;
            if (particleObject.GetComponent<ParticleSystem>())
            {
                ParticleSystem particle = particleObject.GetComponent<ParticleSystem>();
                particle.Play();
            }
        }
        public void GetBackToPoolOnFinishParticle(in ObjectPool pool)
        {
            foreach (GameObject gameObject in pool.PooledObjects)
            {
                ParticleSystem particle = gameObject.GetComponent<ParticleSystem>();
                if (!particle.isPlaying && gameObject.activeInHierarchy)
                {
                    pool.GetBackToPool(particle.gameObject);
                }
            }
        }
        #endregion

        public static GameObject CreateObject(GameObject obj)
        {
            GameObject newGameObject = Instantiate(obj);
            if(newGameObject.GetComponent<Block>())
            {
                //newGameObject.transform.localScale = new Vector2(Grid.DefaultSize, Grid.DefaultSize);
            }
            return newGameObject;
        }
        public static GameObject CreateObject(GameObject obj, Transform parent)
        {
            GameObject newGameObject = Instantiate(obj, parent);
            if (newGameObject.GetComponent<Block>())
            {
                //newGameObject.transform.localScale = new Vector2(Grid.DefaultSize, Grid.DefaultSize);
            }
            return newGameObject;
        }
        public GameObject GetPooledObject(ObjectPool pool)
        {
            GameObject pooledObject = pool.GetPooledObject();
            if (pooledObject == null)
            {
                pooledObject = pool.CreatePooledObject();
            }
            return pooledObject;
        }
        public uint GetPointFromSprite(Sprite icon)
        {
            uint point = 1;
            for (byte index = 0; index < CatImages.Count; index++)
            {
                point *= 2;
                if (CatImages[index] == icon)
                {
                    return point;
                }
            }
            return 0;
        }
        public Sprite GetSpriteFromPoint(uint point)
        {
            uint newPoint = 1;
            for (byte index = 0; index < CatImages.Count; index++)
            {
                newPoint *= 2;
                if (newPoint == point)
                {
                    return CatImages[index];
                }
            }
            return null;
        }
        public static void LevelUp()
        {
            CurrentExperience = currentExperience - NextLevelExperience;
            CurrentLevel++;
        }
    }
}
