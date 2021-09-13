using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L : Piece
{
    public override void SetProperties()
    {
        name = "L";
        Vector2[] rotationsOne = new Vector2[]
        {
            V(-1,1),
            V(-1,0),
            V(0,0),
            V(1,0)
        };

        Vector2[] rotationsTwo = new Vector2[]
        {
            V(-1,0),
            V(0,0),
            V(1,0),
            V(1,1)
        };
        relativeBlockPositions.Add(0, rotationsOne);
        relativeBlockPositions.Add(1, rotationsTwo);

        color = new Vector3(0, 0, 1);
    }
    
}
