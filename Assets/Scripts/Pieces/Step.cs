using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : Piece
{
    public override void SetProperties()
    {
        name = "Step";
        Vector2[] rotationsOne = new Vector2[]
        {
            V(1,1),
            V(0,1),
            V(0,0),
            V(-1,0)
        };
        Vector2[] rotationsTwo = new Vector2[]
        {
            V(-1,1),
            V(0,1),
            V(0,0),
            V(1,0)
        };
        relativeBlockPositions.Add(0, rotationsOne);
        relativeBlockPositions.Add(1, rotationsTwo);
        color = new Vector3(0, 1, 0);
    }
}
