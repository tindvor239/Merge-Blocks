using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snaper : Singleton<Snaper>
{
    [SerializeField]
    private List<Transform> defaultSnapTransforms = new List<Transform>();
    //will be unserialize.
    private List<Transform> snapableTransforms = new List<Transform>();
    //will be unserialize.
    private List<Transform> trimedSnapableTransform = new List<Transform>();
    //will be unserialize.
    private int snapColumn;
    private Vector2 nearestPosition = new Vector2();
    private static Vector2 startPosition = new Vector2(0, 6f);
    private float nearestDistance = 0;
    #region Properties
    public Vector2 NearestPosition
    {
        get
        {
            for(byte index = 0; index < trimedSnapableTransform.Count; index++)
            {
                if(nearestPosition == (Vector2)trimedSnapableTransform[index].position)
                {
                    return nearestPosition;
                }
            }
            nearestPosition = trimedSnapableTransform[trimedSnapableTransform.Count - 1].position;
            return nearestPosition;
        }
    }
    public static Vector2 StartPosition
    {
        get => startPosition;
    }
    public int SnapColumn { get => snapColumn; }
    #endregion
    // Awake is called when the script instance is being loaded.
    protected override void Awake()
    {
        #region Singleton
        base.Awake();
        #endregion
    }
    private void Start()
    {
        snapColumn = defaultSnapTransforms.Count / 2;
    }
    public void SetBlockColumnOnSpawn()
    {
        int emptyRowIndex = Grid.Instance.GetEmptyRowIndex(snapColumn);
        if(emptyRowIndex == -1)
        {
            snapableTransforms = GetSnapablePositions();
            trimedSnapableTransform = TrimSnapableTransforms();
            for (byte column = 0; column < trimedSnapableTransform.Count; column++)
            {
                if (trimedSnapableTransform[column] != null)
                {
                    snapColumn = ConvertSnapTransformColumnToGridColumn(column);
                    nearestPosition = new Vector2(trimedSnapableTransform[column].transform.position.x, nearestPosition.y);
                    startPosition = new Vector2(nearestPosition.x, startPosition.y);
                    break;
                }
            }
            emptyRowIndex = Grid.Instance.GetEmptyRowIndex(snapColumn);
        }
        Gameplay.Instance.ControlledBlock.DestinateColumn = snapColumn;
        Gameplay.Instance.ControlledBlock.DestinateRow = emptyRowIndex;
        if (Gameplay.Instance.ControlledBlock != null && Gameplay.Instance.ControlledBlock.IsHit == false)
        {
            Gameplay.Instance.ControlledBlock.transform.position = startPosition;
        }
    }
    public void SwitchBlockPosition()
    {
        if (defaultSnapTransforms.Count != 0)
        {
            snapableTransforms = GetSnapablePositions();
            trimedSnapableTransform = TrimSnapableTransforms();
            for(byte column = 0; column < trimedSnapableTransform.Count; column++)
            {
                if (trimedSnapableTransform[column] != null)
                {
                    if(column == 0)
                    {
                        //Find nearest distance in array.
                        //Get 1st distance in array => then compare to other distances in array.
                        nearestDistance = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, trimedSnapableTransform[column].transform.position.y)), trimedSnapableTransform[column].transform.position);
                        nearestPosition = new Vector2(trimedSnapableTransform[column].transform.position.x, nearestPosition.y);
                        if (Gameplay.Instance.ControlledBlock != null && Gameplay.Instance.ControlledBlock.IsHit == false)
                        {
                            Gameplay.Instance.ControlledBlock.transform.position = new Vector2(nearestPosition.x, Gameplay.Instance.ControlledBlock.transform.position.y);
                        }
                        //Get index on snaping.
                        snapColumn = ConvertSnapTransformColumnToGridColumn(column);
                        Gameplay.Instance.ControlledBlock.DestinateColumn = snapColumn;
                        Gameplay.Instance.ControlledBlock.DestinateRow = Grid.Instance.GetEmptyRowIndex(snapColumn);
                        startPosition = new Vector2(nearestPosition.x, startPosition.y);
                    }
                    else
                    {
                        float distance = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, trimedSnapableTransform[column].transform.position.y)), trimedSnapableTransform[column].transform.position);
                        if (nearestDistance > distance)
                        {
                            nearestDistance = distance;
                            nearestPosition = new Vector2(trimedSnapableTransform[column].transform.position.x, nearestPosition.y);
                            if (Gameplay.Instance.ControlledBlock != null && Gameplay.Instance.ControlledBlock.IsHit == false)
                            {
                                Gameplay.Instance.ControlledBlock.transform.position = new Vector2(nearestPosition.x, Gameplay.Instance.ControlledBlock.transform.position.y);
                            }
                            //Get index on snaping.
                            snapColumn = ConvertSnapTransformColumnToGridColumn(column);
                            Gameplay.Instance.ControlledBlock.DestinateColumn = snapColumn;
                            Gameplay.Instance.ControlledBlock.DestinateRow = Grid.Instance.GetEmptyRowIndex(snapColumn);
                            startPosition = new Vector2(nearestPosition.x, startPosition.y);
                        }
                    }
                }
            }

        }
    }

    private int ConvertSnapTransformColumnToGridColumn(int column)
    {
        for (int index = 0; index < defaultSnapTransforms.Count; index++)
        {
            if (trimedSnapableTransform[column].gameObject == defaultSnapTransforms[index].gameObject)
            {
                return index;
            }
        }
        // Don't have any return -1.
        return -1;
    }
    private List<Transform> TrimSnapableTransforms()
    {
        List<Transform> result = new List<Transform>();
        for(byte column = 0; column < snapableTransforms.Count; column++)
        {
            for (byte defaultColumn = 0; defaultColumn < defaultSnapTransforms.Count; defaultColumn++)
            {
                if (snapableTransforms[column].gameObject == defaultSnapTransforms[defaultColumn].gameObject)
                {
                    result.Add(snapableTransforms[column]);

                    if (column + 1 < snapableTransforms.Count
                        && defaultColumn + 1 < defaultSnapTransforms.Count
                        && snapableTransforms[column + 1].gameObject != defaultSnapTransforms[defaultColumn + 1].gameObject)
                    {
                        return result;
                    }
                }
            }
        }
        return result;
    }
    private List<Transform> GetSnapablePositions()
    {
        List<Transform> snapablePositions = new List<Transform>();
        for (byte column = 0; column < defaultSnapTransforms.Count; column++)
        {
            byte row = (byte)(Grid.Instance.Row - 1);
            if (Grid.Instance.MasterBlocks.Rows[row].Columns[column] == null || Grid.Instance.MasterBlocks.Rows[row].Columns[column] != null
                && Gameplay.Instance.ControlledBlock != null && Grid.Instance.MasterBlocks.Rows[row].Columns[column] == Gameplay.Instance.ControlledBlock.gameObject)
            {
                snapablePositions.Add(defaultSnapTransforms[column]);
            }
        }
        return snapablePositions;
    }
}
