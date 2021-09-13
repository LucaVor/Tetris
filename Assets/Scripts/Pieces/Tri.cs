using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tri : Piece
{
    public override void SetProperties()
    {
        name = "Tri";
        Vector2[] rotationsOne = new Vector2[]
        {
            V(-1,0),
            V(0,0),
            V(1,0),
            V(0,1)
        };
        relativeBlockPositions.Add(0, rotationsOne);
        color = new Vector3(1, 0, 1);
    }
}
