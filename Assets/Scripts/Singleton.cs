using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Singleton
    public static T Instance { get; private set; }
    
    protected virtual void Awake()
    {
        if(Instance != null)
        {
            if(Instance == GetComponent<T>())
            {
                //Debug.Log("Already have this instance");
                return;
            }
        }
        Instance = GetComponent<T>();
        //Debug.Log($"set instance of \'{instance.gameObject.name}\'");
    }
    #endregion
}
