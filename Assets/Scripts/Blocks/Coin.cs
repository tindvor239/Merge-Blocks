using UnityEngine;
using UnityEngine.CustomComponents;

public class Coin : Block
{
    [SerializeField]
    private Animator animator;
    public override uint Point
    {
        get
        {
            try
            {
                string result = "";
                foreach(char character in Text)
                {
                    if(character != 'x')
                    {
                        result += character;
                    }
                }
                return uint.Parse(result);
            }
            catch
            {
                return 0;
            }
        }
        set
        {
            Text = $"x{value}";
        }
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        gravityMultiplier = fastGravityMultiplier;
        animator = GetComponentInChildren<Animator>();
        pool = PoolParty.Instance.GetPool("Coins Pool");
    }
    protected override void Update()
    {

    }
    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void HitAnimation()
    {
        animator.SetBool("play", true);
    }
}
