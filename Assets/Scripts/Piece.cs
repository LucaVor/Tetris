using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    public Vector2 position = TetrisGame.startingPosition;
    public Dictionary<int, Vector2[]> relativeBlockPositions = new Dictionary<int, Vector2[]>();

    public int chosenBlockConfig = 0;
    public string name;

    public Vector2 rotateOrigin = Vector2.zero;

    public int rotationAmount = 0;
    private bool choseBlockConfig = false;

    public Vector3 color = Vector3.one;


    public Piece()
    {
        GetNewBlockConfig();
        SetProperties();
    }

    public void Update()
    {
        position.y -= 1;
    }
    
    public Vector2[] GetBodyPositions(Vector2 basePosition)
    {
        Vector2[] bodyPositions = new Vector2[relativeBlockPositions[chosenBlockConfig].Length];
        
        int index = 0;
        foreach(Vector2 bodyPart in relativeBlockPositions[chosenBlockConfig]) {
            Vector2 rotatedPart = bodyPart;
            if(name != "Square"){
                for(int i = 0; i < Mathf.Abs(rotationAmount); i++) {
                    rotatedPart = rotatePoint(rotatedPart, rotateOrigin, rotationAmount < 0 ? -1 : 1);
                }
            }    
            Vector2 relativeToBody = basePosition + rotatedPart;
            Vector2 relativeToSpace = SetPositionRelative(relativeToBody);
            relativeToSpace.x = (int)Mathf.Round(relativeToSpace.x);
            relativeToSpace.y = (int)Mathf.Round(relativeToSpace.y);
            // Setting var

            bodyPositions[index] = relativeToSpace;

            index ++;
            // MOM IS SO MEAN TO ME PLEASE STOP BOTHERING MEEEEE
        }

        return bodyPositions;
    }

    public Vector2[] GetBodyPositions()
    {
        return GetBodyPositions(position);
    }

    public Vector2 SetPositionRelative(Vector2 _p) {
        return new Vector2(_p.x * TetrisGame.unitSize, _p.y * TetrisGame.unitSize);
    }

    public Vector2 GetInSpacePosition()
    {
        return SetPositionRelative(position) - TetrisGame.startingPosition;
    }

    public virtual void SetProperties() { // Unique for each override.

    }

    public Vector2 V(float x, float y) {
        return new Vector2(x, y);
    }

    public Vector2 rotatePoint(Vector2 point, Vector2 origin, float multiplier) {
        float s = Mathf.Sin(-1.5707963267948966f * multiplier);
        float c = Mathf.Cos(-1.5707963267948966f * multiplier);
        point.x = point.x - origin.x;
        point.y = point.y - origin.y;

        float xnew = point.x * c - point.y * s;
        float ynew = point.x * s + point.y * c;

        point.x = xnew + origin.x;
        point.y = ynew + origin.y;

        return point;
    }

    private void GetNewBlockConfig()
    {
        choseBlockConfig = true;
        chosenBlockConfig = Random.Range(0, relativeBlockPositions.Count);
    }
}
