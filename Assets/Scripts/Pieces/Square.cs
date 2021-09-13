using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : Piece
{
    public override void SetProperties()
    {
        name = "Square";
        Vector2[] rotationsOne = new Vector2[]
        {
            V(0,0),
            V(0,1),
            V(1,0),
            V(1,1)
        };
        relativeBlockPositions.Add(0, rotationsOne);
        color = new Vector3(1, 1, 0);
    }
}
