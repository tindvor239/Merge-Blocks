using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.CustomComponents;

public class AnimationController : MonoBehaviour
{
    public void OnCompleteAnimation()
    {
        GetComponent<Animator>().SetBool("play", false);
        transform.rotation = new Quaternion();
        PoolParty.Instance.GetPool("Coins Pool").GetBackToPool(gameObject.transform.parent.gameObject);
        Debug.Log("In");
    }
}
