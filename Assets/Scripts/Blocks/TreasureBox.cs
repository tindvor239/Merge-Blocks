using UnityEngine;
using UnityEngine.CustomComponents;

public class TreasureBox : Block
{
    private static uint[] coins = { 20, 30, 50, 60, 80, 90, 110, 120, 140};
    [SerializeField]
    private uint point = 0;
    public override uint Point { get => point; set => point = value; }
    // Start is called before the first frame update
    protected override void Start()
    {
        gravityMultiplier = fastGravityMultiplier;
        pool = PoolParty.Instance.GetPool("Treasure Boxes Pool");
    }
    protected override void Update()
    {

    }
    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void GetRandomCoin()
    {
        int index = Random.Range(0, coins.Length);
        Point = coins[index];
    }
}
