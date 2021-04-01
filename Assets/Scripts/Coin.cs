using UnityEngine.CustomComponents;

public class Coin : Block
{
    public override uint Point
    {
        get
        {
            try
            {
                string result = "";
                foreach(char character in Text)
                {
                    if(character != '+')
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
            Text = $"+{value}";
        }
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        gravityMultiplier = fastGravityMultiplier;
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
}
