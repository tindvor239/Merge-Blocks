using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.CustomComponents;
using UnityEngine.EventSystems;

namespace UnityEngine.CustomComponents
{
    public class GameController : Singleton<GameController>
    {
        [Header("Controlling Block")]
        [SerializeField]
        private Block dropingBlock;
        [SerializeField]
        private Transform cursor;
        [SerializeField]
        private List<int> moveableColumns = new List<int>();
        private List<int> availableColumns = new List<int>();
        private List<int> firstAvailableColumns = new List<int>(), secondAvailableColumns = new List<int>();
        private static List<uint> spawnPoints = new List<uint>();
        [SerializeField]
        private int combo;
        private int dropCoinableComboPoint = 3;

        public static bool controlable = false;
        
        [Header("Next Cats")]
        [SerializeField]
        private Cat[] nextList = new Cat[4];
        [SerializeField]
        private Cat dropingCat = null;
        [SerializeField]
        [Range(0, 1)]
        private float spawnRate;
        [Header("Treasure Box")]
        //Each odd level will drop Treasure box at in that level spawn rate will be 50%.
        //Each even level will Reward number in that level
        //Drop maximum will be 1.
        private static byte maxRewardDrop = 1;
        [SerializeField]
        private byte currentRewardDrop = 0;
        [SerializeField]
        [Range(0, 1)]
        private float spawnRewardRate;
        private bool isOddLevel = false;
        private List<Block> movedBlocks = new List<Block>();
        private List<Block> mergedBlocks = new List<Block>();
        private Dictionary<Block, Vector2> shiftBlocks = new Dictionary<Block, Vector2>();
        private float lastX = 0;
        private float limitY = 0;
        private bool isHit = false;
        private bool doneShift = false;
        private bool doneMove = false;
        private bool doneChangeBlock = true;
        private bool doneMoveToReady = false;
        [SerializeField]
        private bool isRestart = false;
        private List<ParticleSystem> conflationParticles = new List<ParticleSystem>();

        private static uint goalPoint;
        private static List<Block> remainBlocks = new List<Block>();
        [SerializeField]
        private List<Vector2> lastPositions = new List<Vector2>();

        public delegate void OnChangeBlock();
        public event OnChangeBlock onChangeBlock;

        #region Properties
        public Block DropingBlock { get => dropingBlock; }
        public Dictionary<Block, Vector2> ShiftBlocks { get => shiftBlocks; }
        public List<Block> MergedBlock { get => mergedBlocks; }
        public List<uint> SpawnPoints { get => spawnPoints; }
        public int DropCoinableComboPoint { get => dropCoinableComboPoint; }
        #endregion
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            goalPoint = 128 * 2;
            //NEED TO DEVELOP!!
            SetPointsOnAwake();
            //TO DO:
            // - MaxPoint => goalPoint.
            // - Random sort block.
            // => MENU.
        }

        private void Start()
        {
            SpawnCatsOnStart();
            lastX = Grid.GetLimitX(Mathf.RoundToInt(Grid.Column / 2));
            Invoke("SetNextListPos", 0.1f);
        }
        // Update is called once per frame
        private void Update()
        {
            if (GameManager.GameState == GameState.play)
            {
                GameManager.Instance.GetBackToPoolOnFinishParticle(PoolParty.Instance.GetPool("Conflation Effects Pool"));
                GameManager.Instance.GetBackToPoolOnFinishParticle(PoolParty.Instance.GetPool("Hit Effects Pool"));
                if (dropingBlock != null)
                {
                    if (doneMoveToReady)
                    {
                        if(EventSystem.current.currentSelectedGameObject == null)
                        {
                            ControllingBlock();
                        }
                        bool isAtLimit = dropingBlock.transform.position.y > limitY ? false : true;
                        if (isAtLimit)
                        {
                            dropingBlock.transform.position = new Vector2(dropingBlock.transform.position.x, limitY);
                            lastX = dropingBlock.transform.position.x;
                            PlaceBlock(dropingBlock);
                            movedBlocks.Add(dropingBlock);
                            dropingBlock.playConflationEffect = true;
                            GameManager.Instance.GetAndPlayParticle(dropingBlock.transform.position, PoolParty.Instance.GetPool("Hit Effects Pool"));
                            dropingBlock = null;
                            isHit = true;
                        }
                        else
                        {
                            dropingBlock.MoveDown();
                        }
                    }
                    bool catIsOnBlock = dropingCat != null && (Mathf.Approximately(dropingCat.transform.position.y, dropingBlock.Icon.transform.position.y) || dropingCat.transform.position.y <= dropingBlock.Icon.transform.position.y);
                    if (catIsOnBlock)
                    {
                        SetCatOnBlock();
                        ReloadNextList();
                        if (dropingBlock != null)
                        {
                            SetControllingBlockPosition();
                        }
                    }
                    if (doneChangeBlock && dropingCat == null)
                    {
                        MoveToLastX(in dropingBlock);
                    }
                }
                else
                {
                    if (isHit && doneShift == false)
                    {
                        if(isRestart == false)
                        {
                            MereAll();
                        }
                        else
                        {
                            //Sort();
                            isRestart = false;
                        }
                    }
                    if (mergedBlocks.Count == 0 && doneMove == false && doneShift)
                    {
                        foreach (KeyValuePair<Block, Vector2> pair in shiftBlocks)
                        {
                            pair.Key.Move(pair.Value);
                        }
                        doneMove = true;
                    }
                    if (shiftBlocks.Count == 0 && doneMove && doneShift)
                    {
                        if (movedBlocks.Count == 0)
                        {
                            if (Grid.IsGridFull())
                            {
                                GameManager.Instance.GameOver();
                            }
                            else
                            {
                                //Drop Coin
                                if (combo >= dropCoinableComboPoint)
                                {
                                    UIManager.Instance.ShowCompliment(combo);
                                    SpawnCoin();
                                    controlable = false;
                                }
                                else
                                {
                                    SpawnBlockWait();
                                    DropCat();
                                }
                                Reset();
                            }
                        }
                        else
                        {
                            doneMove = false;
                            doneShift = false;
                        }
                    }
                }
            }
        }

        #region Gameplay
        private void MoveToLastX(in Block block)
        {
            if (block != null)
            {
                Vector2 target = new Vector2(lastX, block.transform.position.y);
                block.transform.position = Vector2.MoveTowards(block.transform.position, target, Block.normalGravityMultiplier * 9.82f * Time.deltaTime);
                bool isReachTarget = (Vector2)block.transform.position == target;
                if (isReachTarget)
                {
                    doneMoveToReady = true;
                    doneChangeBlock = false;
                }
            }
        }
        private void ControllingBlock()
        {
            if (Input.GetMouseButton(0) && controlable)
            {
                limitX();
                cursor.position = new Vector3(lastX, cursor.transform.position.y, cursor.transform.position.z);
                dropingBlock.transform.position = new Vector2(lastX, dropingBlock.transform.position.y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                dropingBlock.Push();
                controlable = false;
            }
        }
        private void SpawnCatsOnStart()
        {
            for (byte j = 0; j < nextList.Length; j++)
            {
                nextList[j] = GameManager.Instance.GetPooledObject(PoolParty.Instance.GetPool("Cats Pool")).GetComponent<Cat>();
                nextList[j].Icon = GetRandomSprite();
                nextList[j].transform.position = NextListPosition(j);
            }
        }
        private static void SetPointsOnAwake()
        {
            uint startPoint = 2;
            for (byte i = 0; i <= 5; i++)
            {
                spawnPoints.Add(startPoint);
                startPoint *= 2;
            }
        }
        private void DropCat()
        {
            dropingCat = nextList[nextList.Length - 1];
            nextList[nextList.Length - 1] = null;
            dropingCat.Move(dropingBlock.Icon.gameObject.transform.position);
        }
        private void SetCatOnBlock()
        {
            dropingBlock.Icon.sprite = dropingCat.Icon;
            dropingBlock.Point = GameManager.Instance.GetPointFromSprite(dropingBlock.Icon.sprite);
            dropingBlock.Icon.gameObject.SetActive(true);
            PoolParty.Instance.GetPool("Cats Pool").GetBackToPool(dropingCat.gameObject);

            if(!isOddLevel && currentRewardDrop <= 0)
            {
                float rate = Random.Range(0f, 1f);
                if (rate <= spawnRewardRate)
                {
                    //Show Reward Block...
                    dropingBlock.Point = 128;
                    dropingBlock.Icon.sprite = GameManager.Instance.GetSpriteFromPoint(dropingBlock.Point);
                    UIManager.Instance.ShowLevelUpUI();
                    currentRewardDrop++;
                }
            }

            controlable = true;
            dropingCat = null;
            doneChangeBlock = true;
        }
        private void SpawnBlockWait()
        {
            doneMoveToReady = false;
            GameObject pooledObject = GameManager.Instance.GetPooledObject(PoolParty.Instance.GetPool("Blocks Pool"));
            limitY = Grid.GetLimitYIgnoreCoin(Grid.Column / 2);
            pooledObject.transform.position = new Vector2(Grid.GetLimitX(Grid.Column / 2), StartPositionY(Grid.Row - 1));
            dropingBlock = pooledObject.GetComponent<Block>();
            dropingBlock.ShowBlock(0.1f);
            dropingBlock.gravityMultiplier = Block.slowGravityMultiplier;
            dropingBlock.Icon.gameObject.SetActive(false);
        }
        private void SetControllingBlockPosition()
        {
            moveableColumns = GetLimitedColumns();
            int columnIndex = Grid.GetColumnIndex(lastX);
            if (!moveableColumns.Contains(columnIndex))
            {
                lastX = Grid.GetLimitX(moveableColumns[0]);
                columnIndex = Grid.GetColumnIndex(lastX);
            }
            limitY = Grid.GetLimitYIgnoreCoin(columnIndex);
            cursor.position = new Vector3(lastX, cursor.transform.position.y, cursor.transform.position.z);
        }
        private void SetNextListPos()
        {
            for (byte n = 0; n < nextList.Length; n++)
            {
                // Get X - 1 Position.
                if (nextList[n] != null)
                {
                    nextList[n].Move(NextListPosition(n));
                }
            }
        }
        private static Vector2 NextListPosition(int column)
        {
            float spaceX = Grid.GetLimitX(1) - Grid.GetLimitX(0);
            return new Vector2(Grid.GetLimitX(column) - spaceX, 7f);
        }
        private static float StartPositionY(int row)
        {
            float spaceY = Grid.GetLimitY(1, 0) - Grid.GetLimitY(0, 0);
            return Grid.GetLimitY(row, 0) + spaceY;
        }
        private Sprite GetRandomSprite()
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            return GameManager.Instance.CatImages[randomIndex];
        }
        private void SortNextList()
        {
            //Sort Next List.
            for (int index = nextList.Length - 1; index >= 0; index--)
            {
                if (nextList[index] != null)
                {
                    for (int j = nextList.Length - 1; j > index; j--)
                    {
                        if (nextList[j] == null)
                        {
                            nextList[j] = nextList[index];
                            nextList[index] = null;
                            break;
                        }
                    }
                }
            }
        }
        private void ReloadNextList()
        {
            SortNextList();
            SpawnCat();
            SetNextListPos();
        }
        private void SpawnCoin()
        {
            if(isOddLevel && currentRewardDrop <= 0)
            {
                float rate = Random.Range(0f, 1f);
                if (rate <= spawnRewardRate)
                {
                    //Show Reward Block...
                    SpawnTreasureBox();
                }
            }
            else
            {
                if (combo >= 0)
            {
                UIManager.Instance.HorrayParticles[0].Play();
                //Drop coint.point == combo.
                GameObject pooledObject = GameManager.Instance.GetPooledObject(PoolParty.Instance.GetPool("Coins Pool"));
                Coin coin = pooledObject.GetComponent<Coin>();
                moveableColumns = GetLimitedColumns();
                //Get Random Column.
                int column = moveableColumns[Random.Range(0, availableColumns.Count)];
                //Get Row.
                limitY = Grid.GetLimitY(column);
                coin.Point = (uint)(combo - 1);
                coin.transform.position = new Vector2(Grid.GetLimitX(column), 8f);
                dropingBlock = coin;
                combo = 0;
            }
            }
        }
        private void SpawnCat()
        {
            GameObject pooledObject = GameManager.Instance.GetPooledObject(PoolParty.Instance.GetPool("Cats Pool"));
            bool rate = SpawnCatChance();
            if(rate)
            {
                pooledObject.GetComponent<Cat>().Icon = GetRandomSprite();
            }
            else
            {
                pooledObject.GetComponent<Cat>().Icon = GetRandomAvailableCat();
            }
            if (nextList[0] == null)
            {
                nextList[0] = pooledObject.GetComponent<Cat>();
                nextList[0].transform.position = NextListPosition(0);
            }
        }
        private bool SpawnCatChance()
        {
            bool canSpawnInSpawnPoint = AvailableBlockInSpawnPoints();
            if(canSpawnInSpawnPoint)
            {
                float rate = Random.Range(0f, 1f);
                return rate <= spawnRate ? true : false;
            }
            else
            {
                return true;
            }
        }
        private bool AvailableBlockInSpawnPoints()
        {
            foreach (Block block in Grid.MasterBlocks)
            {
                if (block != null)
                {
                    foreach (uint spawnPoint in spawnPoints)
                    {
                        if (block.Point == spawnPoint)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private List<Sprite> GetAvailableSpawnCat()
        {
            List<Sprite> availableCats = new List<Sprite>();
            foreach(Block block in Grid.MasterBlocks)
            {
                if (block != null && block.Icon != null && block.Point < 128 && !(block is Coin))
                {
                    if(!availableCats.Contains(block.Icon.sprite))
                    {
                        availableCats.Add(block.Icon.sprite);
                    }
                }       
            }
            return availableCats;
        }
        private Sprite GetRandomAvailableCat()
        {
            List<Sprite> availableCats = GetAvailableSpawnCat();
            if(availableCats.Count > 0)
            {
                int randomIndex = Random.Range(0, availableCats.Count);
                return availableCats[randomIndex];
            }
            return null;
        }
        private void limitX()
        {
            float x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            int roundedX = GetColumnIndexFromMousePosition(x);
            if (roundedX <= moveableColumns[0])
            {
                //Need to be fix
                roundedX = moveableColumns[0];
            }
            else if (roundedX >= moveableColumns[moveableColumns.Count - 1])
            {
                roundedX = moveableColumns[moveableColumns.Count - 1];
            }
            if (moveableColumns.Contains(roundedX) && dropingBlock != null && dropingBlock.transform.position.y > Grid.GetLimitYIgnoreCoin(roundedX))
            {
                lastX = Grid.GetLimitX(roundedX);
                limitY = Grid.GetLimitYIgnoreCoin(roundedX);
            }
        }
        private void Reset()
        {
            isHit = false;
            combo = 0;
            doneShift = false;
            doneMove = false;
        }
        private int GetColumnIndexFromMousePosition(float positionX)
        {
            int result = Mathf.RoundToInt((positionX / Grid.DefaultSpace) - Grid.Offset.x);
            if (result < 0)
            {
                result = 0;
            }
            else if (result >= Grid.Column)
            {
                result = Grid.Column - 1;
            }
            return result;
        }
        private static int GetRowIndexFromMousePosition(float positionY)
        {
            int result = Mathf.RoundToInt((positionY / Grid.DefaultSpace) - Grid.Offset.y);
            if (result < 0)
            {
                result = 0;
            }
            else if (result >= Grid.Row)
            {
                result = Grid.Row - 1;
            }
            return result;
        }

        public static void PlaceBlock(Block block)
        {
            int column = Grid.GetColumnIndex(block.transform.position.x);
            int row = Grid.GetRowIndex(block.transform.position.y);
            //Add coin when hit coin
            if (Grid.MasterBlocks[row, column] != null && Grid.MasterBlocks[row, column].GetComponent<Coin>())
            {
                Coin coin = Grid.MasterBlocks[row, column].GetComponent<Coin>();
                coin.HitAnimation();
                GameManager.Instance.Money += (int)coin.Point;
            }
            Grid.MasterBlocks[row, column] = block;
        }
        public void OnStart()
        {
            SpawnBlockWait();
            DropCat();
        }
        #endregion
        
        #region Arrange Blocks
        private void MereAll()
        {
            List<Block> mergedBlocks = new List<Block>();
            if (movedBlocks.Count != 0)
            {
                for (int i = movedBlocks.Count - 1; i >= 0; i--)
                {
                    List<Block> neighbours = GetNeighbours(movedBlocks[i]);
                    if (neighbours != null && neighbours.Count != 0)
                    {
                        combo++;
                        Merge(movedBlocks[i], neighbours);
                        movedBlocks[i].Fading(0.6f);
                        GameManager.Instance.GetAndPlayParticle(movedBlocks[i].transform.position, PoolParty.Instance.GetPool("Conflation Effects Pool"));
                    }
                    else
                    {
                        if (!mergedBlocks.Contains(movedBlocks[i]))
                            mergedBlocks.Add(movedBlocks[i]);
                    }
                }
                foreach (Block block in mergedBlocks)
                {
                    movedBlocks.Remove(block);
                }
                ShiftDown();
            }
            if(!isRestart)
            {
                doneShift = true;
            }
        }
        private void ShiftDown()
        {
            for (int column = 0; column < Grid.Column; column++)
            {
                for (int row = 0; row < Grid.Row; row++)
                {
                    if (Grid.MasterBlocks[row, column] != null)
                    {
                        if (row > 0)
                        {
                            for (int i = 0; i < row; i++)
                            {
                                if (Grid.MasterBlocks[i, column] == null)
                                {
                                    Grid.MasterBlocks[i, column] = Grid.MasterBlocks[row, column];
                                    Grid.MasterBlocks[row, column] = null;
                                    float y = (i * Grid.DefaultSpace) + Grid.Offset.y;
                                    //movedown.
                                    if (shiftBlocks.ContainsKey(Grid.MasterBlocks[i, column]))
                                    {
                                        shiftBlocks.Remove(Grid.MasterBlocks[i, column]);
                                    }
                                    shiftBlocks.Add(Grid.MasterBlocks[i, column], new Vector2(Grid.GetLimitX(column), Grid.GetLimitY(i, column)));
                                    if (!movedBlocks.Contains(Grid.MasterBlocks[i, column]))
                                    {
                                        movedBlocks.Add(Grid.MasterBlocks[i, column]);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void Merge(Block block, List<Block> neighbours)
        {
            bool isCoin = block is Coin;
            if (isCoin)
            {
                foreach (Block neighbour in neighbours)
                {
                    block.Point += neighbour.Point;
                }
            }
            else
            {
                foreach (Block neighbour in neighbours)
                {
                    block.Point *= 2;
                }
                block.Icon.sprite = GameManager.Instance.GetSpriteFromPoint(block.Point);
            }
            int score = CounExperience(block.Point);
            GameManager.CurrentExperience += (uint)score;
            if (GameManager.CurrentExperience >= GameManager.NextLevelExperience)
            {
                //Levelup...
                LevelUp();
                GameManager.LevelUp();
            }
        }
        private void LevelUp()
        {
            currentRewardDrop = 0;
            isOddLevel = SpawnBlockOrSpawnTreasure();
        }
        private bool SpawnBlockOrSpawnTreasure()
        {
            return GameManager.CurrentLevel % 2 == 0 ? false : true;
        }
        private void SpawnTreasureBox()
        {
            GameObject box = GameManager.Instance.GetPooledObject(PoolParty.Instance.GetPool("Treasure Boxes Pool"));
            TreasureBox treasureBox = box.GetComponent<TreasureBox>();
            treasureBox.GetRandomCoin();
            //Column
            int column = moveableColumns[Random.Range(0, availableColumns.Count)];
            //Get Row.
            limitY = Grid.GetLimitY(column);
            treasureBox.transform.position = new Vector2(Grid.GetLimitX(column), 8f);
            dropingBlock = treasureBox;
            controlable = false;
            currentRewardDrop++;
        }
        private void Sort()
        {
            foreach(Block block in Grid.MasterBlocks)
            {
                bool isCoin = block is Coin;
                if (block != null && !isCoin && !mergedBlocks.Contains(block))
                {
                    remainBlocks.Add(block);
                    lastPositions.Add(block.transform.position);
                }
            }

            for(int row = 0; row < Grid.Row; row++)
            {
                for(int column = 0; column < Grid.Column; column++)
                {
                    bool isCoin = Grid.MasterBlocks[row, column] is Coin;
                    if (Grid.MasterBlocks[row, column] != null && !isCoin)
                    {
                        Grid.MasterBlocks[row, column] = null;
                    }
                }
            }
            
            foreach(Block block in remainBlocks)
            {
                int randomIndex = Random.Range(0, lastPositions.Count);
                Debug.Log($"index: {randomIndex}");

                int rowIndex = Grid.GetRowIndex(lastPositions[randomIndex].y);
                int columnIndex = Grid.GetColumnIndex(lastPositions[randomIndex].x);
                
                Debug.Log($"row: {rowIndex}, column: {columnIndex}");
                Grid.MasterBlocks[rowIndex, columnIndex] = block;
                if (shiftBlocks.ContainsKey(block))
                {
                    shiftBlocks.Remove(block);
                }
                shiftBlocks.Add(block, lastPositions[randomIndex]);
                lastPositions.RemoveAt(randomIndex);
            }
            remainBlocks.Clear();
        }
        private int CounExperience(uint mergedPoint)
        {
            for (int index = 0; index < spawnPoints.Count; index++)
            {
                if (mergedPoint == spawnPoints[index])
                {
                    return index + 1;
                }
            }

            //Clear Level
            if(mergedPoint == goalPoint)
            {
                UIManager.Instance.ShowClearUI(goalPoint);
                goalPoint *= 2;
                UIManager.Instance.ChangeGoal(goalPoint);
                isRestart = true;
            }
            return spawnPoints.Count;
        }
        #endregion

        private int[] GetIndex(Block block)
        {
            if (block != null)
            {
                for (int row = 0; row < Grid.Row; row++)
                {
                    for (int column = 0; column < Grid.Column; column++)
                    {
                        if (Grid.MasterBlocks[row, column] != null && Grid.MasterBlocks[row, column].gameObject == block.gameObject)
                        {
                            return new int[] { row, column };
                        }
                    }
                }
            }
            return new int[] { -1, -1 };
        }
        private List<Block> GetNeighbours(Block block)
        {
            List<Block> result = new List<Block>();
            int[] indexes = GetIndex(block);
            bool indexesAvailable = indexes[0] != -1 ? true : false;
            if (indexesAvailable)
            {
                int row = indexes[0], column = indexes[1];
                Block leftBlock = GetLeft(block, row, column);
                if (leftBlock != null)
                {
                    result.Add(leftBlock);
                }

                Block rightBlock = GetRight(block, row, column);
                if (rightBlock != null)
                {
                    result.Add(rightBlock);
                }

                Block belowBlock = GetBelow(block, row, column);
                if (belowBlock != null)
                {
                    result.Add(belowBlock);
                }
                return result;
            }
            return null;
        }
        private Block GetLeft(Block block, int row, int column)
        {
            //To do: Get Left is Block Coin and left is coin
            bool currentBlockIsCoin = block is Coin;
            if (currentBlockIsCoin)
            {
                if (column - 1 >= 0 && Grid.MasterBlocks[row, column - 1] != null)
                {
                    Block leftBlock = Grid.MasterBlocks[row, column - 1];
                    bool isCoin = leftBlock is Coin;
                    if (isCoin)
                    {
                        mergedBlocks.Add(leftBlock);
                        leftBlock.Merge(block.transform.position);
                        Grid.MasterBlocks[row, column - 1] = null;
                        return leftBlock;
                    }
                }
            }
            else
            {
                if (column - 1 >= 0 && Grid.MasterBlocks[row, column - 1] != null && Grid.MasterBlocks[row, column - 1].Point == block.Point)
                {
                    Block leftBlock = Grid.MasterBlocks[row, column - 1];
                    bool isCoin = leftBlock is Coin;
                    if (!isCoin)
                    {
                        mergedBlocks.Add(leftBlock);
                        leftBlock.Merge(block.transform.position);
                        Grid.MasterBlocks[row, column - 1] = null;
                        return leftBlock;
                    }
                }
            }
            return null;
        }
        private Block GetRight(Block block, int row, int column)
        {
            bool currentBlockIsCoin = block is Coin;
            if (currentBlockIsCoin)
            {
                if (column + 1 < Grid.Column && Grid.MasterBlocks[row, column + 1] != null)
                {
                    Block rightBlock = Grid.MasterBlocks[row, column + 1];
                    bool isCoin = rightBlock is Coin;
                    if (isCoin)
                    {
                        mergedBlocks.Add(rightBlock);
                        rightBlock.Merge(block.transform.position);
                        Grid.MasterBlocks[row, column + 1] = null;
                        return rightBlock;
                    }
                }
            }
            else
            {
                if (column + 1 < Grid.Column && Grid.MasterBlocks[row, column + 1] != null && Grid.MasterBlocks[row, column + 1].Point == block.Point)
                {
                    Block rightBlock = Grid.MasterBlocks[row, column + 1];
                    bool isCoin = rightBlock is Coin;
                    if (!isCoin)
                    {
                        mergedBlocks.Add(rightBlock);
                        rightBlock.Merge(block.transform.position);
                        Grid.MasterBlocks[row, column + 1] = null;
                        return rightBlock;
                    }
                }
            }
            return null;
        }
        private Block GetBelow(Block block, int row, int column)
        {
            bool currentBlockIsCoin = block is Coin;
            if (currentBlockIsCoin)
            {
                if (row - 1 >= 0 && Grid.MasterBlocks[row - 1, column] != null)
                {
                    Block belowBlock = Grid.MasterBlocks[row - 1, column];
                    bool isCoin = belowBlock is Coin;
                    if (isCoin)
                    {
                        mergedBlocks.Add(belowBlock);
                        belowBlock.Merge(block.transform.position);
                        Grid.MasterBlocks[row - 1, column] = null;
                        return belowBlock;
                    }
                }
            }
            else
            {
                if (row - 1 >= 0 && Grid.MasterBlocks[row - 1, column] != null && Grid.MasterBlocks[row - 1, column].Point == block.Point)
                {
                    Block belowBlock = Grid.MasterBlocks[row - 1, column];
                    bool isCoin = belowBlock is Coin;
                    if (!isCoin)
                    {
                        mergedBlocks.Add(belowBlock);
                        belowBlock.Merge(block.transform.position);
                        Grid.MasterBlocks[row - 1, column] = null;
                        return belowBlock;
                    }
                }
            }
            return null;
        }
        private List<int> GetLimitedColumns()
        {
            availableColumns = GetAvailableColumns();
            firstAvailableColumns = new List<int>();
            foreach (int column in availableColumns)
            {
                firstAvailableColumns.Add(column);
            }
            secondAvailableColumns = new List<int>();

            for (int i = 0; i < firstAvailableColumns.Count; i++)
            {
                secondAvailableColumns.Add(firstAvailableColumns[i]);
                if (i + 1 < firstAvailableColumns.Count)
                {
                    bool isContinue = firstAvailableColumns[i + 1] == firstAvailableColumns[i] + 1 ? true : false;
                    if (isContinue == false)
                    {
                        break;
                    }
                }
            }
            foreach (int column in secondAvailableColumns)
            {
                if (firstAvailableColumns.Contains(column))
                {
                    firstAvailableColumns.Remove(column);
                }
            }
            List<List<int>> limitedColumnsGroup = new List<List<int>>();
            if (firstAvailableColumns.Count != 0)
            {
                limitedColumnsGroup.Add(firstAvailableColumns);
            }
            if (secondAvailableColumns.Count != 0)
            {
                limitedColumnsGroup.Add(secondAvailableColumns);
            }
            if (moveableColumns.Count < Grid.Column)
            {
                foreach (List<int> group in limitedColumnsGroup)
                {
                    foreach (int column in moveableColumns)
                    {
                        if (group.Contains(column))
                        {
                            return group;
                        }
                    }
                }
            }
            return limitedColumnsGroup[Random.Range(0, limitedColumnsGroup.Count)];
        }
        private List<int> GetAvailableColumns()
        {
            List<int> availableColumns = new List<int>();
            for (int index = 0; index < Grid.Column; index++)
            {
                if (Grid.MasterBlocks[Grid.Row - 1, index] == null)
                {
                    availableColumns.Add(index);
                }
            }
            return availableColumns;
        }
    }
}