using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snaper : Singleton<Snaper>
{
    [SerializeField]
    private List<GameObject> snapPositions = new List<GameObject>();
    
    private Vector2 nearestPosition = new Vector2();
    private static Vector2 startPosition = new Vector2(0, 1.1f);
    private float nearestDistance = 0;
    #region Properties
    public Vector2 NearestPosition
    {
        get => nearestPosition;
    }
    public static Vector2 StartPosition
    {
        get => startPosition;
    }    
    #endregion
    // Awake is called when the script instance is being loaded.
    protected override void Awake()
    {
        #region Singleton
        base.Awake();
        #endregion
    }
    public Vector2 Snaping()
    {
        if (snapPositions.Count != 0)
        {
            for(int i = 0; i < snapPositions.Count; i++)
            {
                if(snapPositions[i] != null)
                {
                    if(i == 0)
                    {
                        nearestDistance = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, snapPositions[i].transform.position.y)), snapPositions[i].transform.position);
                        nearestPosition = new Vector2(snapPositions[i].transform.position.x, nearestPosition.y);
                    }
                    else
                    {
                        if (nearestDistance > Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, snapPositions[i].transform.position.y)), snapPositions[i].transform.position))
                        {
                            nearestDistance = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, snapPositions[i].transform.position.y)), snapPositions[i].transform.position);
                            nearestPosition = new Vector2(snapPositions[i].transform.position.x, nearestPosition.y);
                        }
                    }
                }
            }
        }
        return nearestPosition;
    }
}
