using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class PairBlock
{
    public Block block;
    public Vector2 destination;

    public PairBlock(Block block, Vector2 destination)
    {
        this.block = block;
        this.destination = destination;
    }
}
