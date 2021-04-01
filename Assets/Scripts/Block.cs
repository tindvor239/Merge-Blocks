using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.CustomComponents;

public class Block : MonoBehaviour
{

    [SerializeField]
    private Image icon;
    [SerializeField]
    private Image effect;
    private Sequence sequence;
    [SerializeField]
    protected Component text;
    protected ObjectPool pool;
    protected Vector2 destination = new Vector2();

    public static float normalGravityMultiplier = 0.5f, slowGravityMultiplier = 0.05f, fastGravityMultiplier = 1.2f;
    public float gravityMultiplier = slowGravityMultiplier;
    public delegate void OnMergeDelegate();
    public event OnMergeDelegate onMerge;
    public delegate void OnMoveDelegate();
    public event OnMoveDelegate onMove;

    #region Properties
    public string Text
    {
        get
        {
            if(text is TextMesh)
            {
                TextMesh newText = (TextMesh)text;
                return newText.text;
            }
            else if(text is UnityEngine.UI.Text)
            {
                UnityEngine.UI.Text textUI = (UnityEngine.UI.Text)text;
                return textUI.text;
            }
            return "Error!!! component didn't have text";
        }
        set
        {
            if (text is TextMesh)
            {
                TextMesh newText = (TextMesh)text;
                newText.text = value;
            }
            else if (text is UnityEngine.UI.Text)
            {
                UnityEngine.UI.Text textUI = (UnityEngine.UI.Text)text;
                textUI.text = value;
            }
        }
    }
    public Image Icon { get => icon; }
    public virtual uint Point
    {
        get
        {
            try
            {
                return uint.Parse(Text);
            }
            catch
            {
                return 0;
            }
        }
        set
        {
            Text = value.ToString();
        }
    }
    #endregion
    // Start is called before the first frame update
    protected virtual void Start()
    {
        pool = PoolParty.Instance.GetPool("Blocks Pool");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
    protected virtual void FixedUpdate()
    {
        if(GameManager.GameState == GameState.play)
        {
            if(onMerge != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, destination, normalGravityMultiplier * 9.82f * Time.deltaTime);
                onMerge.Invoke();
            }
            if(onMove != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, destination, normalGravityMultiplier * 9.82f * Time.deltaTime);
                onMove.Invoke();
            }
        }
    }
    public void Move(Vector2 destination)
    {
        this.destination = destination;
        onMove += OnMove;
    }
    public void Merge(Vector2 destination)
    {
        this.destination = destination;
        onMerge += OnMerge;
    }
    public void MoveDown()
    {
        transform.position -= transform.up * gravityMultiplier * 9.82f * Time.deltaTime;
    }
    public void Push()
    {
        gravityMultiplier = fastGravityMultiplier;
    }
    private void OnMove()
    {
        if((Vector2)transform.position == destination)
        {
            GameController.Instance.ShiftBlocks.Remove(this);
            GameManager.Instance.GetAndPlayParticle(transform.position, PoolParty.Instance.GetPool("Hit Effects Pool"));
            onMove = null;
        }
    }
    private void OnMerge()
    {
        if((Vector2)transform.position == destination)
        {
            GameController.Instance.MergedBlock.Remove(this);
            pool.GetBackToPool(gameObject);
            onMerge = null;
        }
    }
    public void Fading(in float duration)
    {
        sequence = DOTween.Sequence();
        sequence.Append(Fade(effect, 1f, duration/2)).SetEase(Ease.Linear).Append(Fade(effect, 0f, duration/2)).SetEase(Ease.Linear);
    }
    public void ShowBlock(in float duration)
    {
        sequence = DOTween.Sequence();
        sequence.Append(Scale(new Vector2(0.1f, 0.1f), 0.01f)).SetEase(Ease.Linear)
            .Append(Scale(new Vector2(1f, 1f), duration - 0.2f)).SetEase(Ease.Linear)
            .Append(Scale(new Vector2(1.4f, 1.4f), duration + 0.1f)).SetEase(Ease.Linear)
            .Append(Scale(new Vector2(1f, 1f), duration)).SetEase(Ease.Linear);
    }
    private Tween Scale(in Vector2 scale, in float time)
    {
        return gameObject.transform.DOScale(scale, time);
    }
    private Tween Fade(in Image image, in float alpha, in float time)
    {
        return image.DOFade(alpha, time);
    }
}