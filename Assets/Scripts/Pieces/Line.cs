using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : Piece
{
    public override void SetProperties()
    {
        name = "Line";
        rotateOrigin = V(0.5f,0.5f);
        Vector2[] rotationsOne = new Vector2[]
        {
            V(-1, 1),
            V(0, 1),
            V(1, 1),
            V(2, 1)
        };
        relativeBlockPositions.Add(0, rotationsOne);
        color = new Vector3(0, 1, 1);
    }
}
