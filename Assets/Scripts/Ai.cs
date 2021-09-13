using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai : MonoBehaviour
{
    public static List<EndPosition> allEndingPositions = new List<EndPosition>();

    public class EvaluationReturn
    {
        public float Evaluation;
        public float numberOfHoles;
        public float maxHeight;
        public float numberOfLines;
        public float bumpiness;
    }

    public class EndPosition {
        public Vector2 position;
        public Vector2[] bodyPositions;
        public int rotation;
        public EvaluationReturn evaluation;
        public bool usingHeld = false;
    }

    public static EndPosition bestEndPosition;

    public static void Move(bool fromRenderer = false)
    {
        TetrisGame tetris = TetrisGame.instance;
        Piece mainPiece = tetris.mainPiece;

        allEndingPositions = new List<EndPosition>();

        Dictionary<EndPosition, float> endingEvaluations = new Dictionary<EndPosition, float>();

        // Get all ending positions
        allEndingPositions = GetAllEnding(mainPiece); 
        endingEvaluations = EvaluateEnding(allEndingPositions);

        Piece heldPiece = tetris.heldPiece != null ? tetris.heldPiece : tetris.nextPiece;
        List<EndPosition> allEndingPositionsForHeld = GetAllEnding(heldPiece);
        Dictionary<EndPosition, float> endingEvaluationsForHeld = EvaluateEnding(allEndingPositionsForHeld);


        EndPosition highestEnd = null;
        float highestEval = -3.1415926535897932f;

        foreach(KeyValuePair<EndPosition, float> entry in endingEvaluations) {
            if(highestEval == -3.1415926535897932f) {
                highestEval = entry.Value;
                highestEnd = entry.Key;
            }
            if(entry.Value > highestEval) {
                highestEval = entry.Value;
                highestEnd = entry.Key;
            }
        }

        EndPosition highestEndForHeld = null;
        float highestEvalForHeld = -3.1415926535897932f;

        foreach(KeyValuePair<EndPosition, float> entry in endingEvaluationsForHeld) {
            if(highestEvalForHeld == -3.1415926535897932f) {
                highestEvalForHeld = entry.Value;
                highestEndForHeld = entry.Key;
            }
            if(entry.Value > highestEvalForHeld) {
                highestEvalForHeld = entry.Value;
                highestEndForHeld = entry.Key;
            }
        }

        bestEndPosition = highestEnd;
        if(highestEvalForHeld > highestEval) {
            bestEndPosition = highestEndForHeld;
            tetris.ChooseHeld();
        }
    }

    public static void Push()
    {
        Move();
        TetrisGame.instance.mainPiece.position.x = bestEndPosition.position.x;
        TetrisGame.instance.mainPiece.rotationAmount = bestEndPosition.rotation;
        TetrisGame.instance.InstaDrop();
        // Debug.Log("Predicted: " + bestEndPosition.evaluation.Evaluation +
        // " Holes: " + bestEndPosition.evaluation.numberOfHoles +
        // " Max Height: " + bestEndPosition.evaluation.maxHeight +
        // " Number of lines: " + bestEndPosition.evaluation.numberOfLines +
        // " Bumpiness: " + bestEndPosition.evaluation.bumpiness
        // );
    }

    public static EvaluationReturn EvaluatePosition(Dictionary<Vector2, Vector3> staticTiles)
    {
        float holesAmount = 0;
        float maxHeight = TetrisGame.instance.BottomY;
        float bumpiness = 0;

        for(float x = TetrisGame.instance.bound.x; x <= TetrisGame.instance.bound.y; x ++) {
            bool setTopY = false;
            for(float y = TetrisGame.startingPosition.y; y >= TetrisGame.instance.BottomY; y--) {
                Vector2 position = new Vector2(x, y);
                bool hasBlock = staticTiles.ContainsKey(position);
                if(!setTopY && hasBlock) {
                    setTopY = true;
                }
                bool isHole = setTopY && !hasBlock;
                if(isHole) {
                    holesAmount ++;
                }
            }
        }

        for(float x = TetrisGame.instance.bound.x; x <= TetrisGame.instance.bound.y; x ++) {
            for(float y = TetrisGame.startingPosition.y; y >= TetrisGame.instance.BottomY; y--) {
                Vector2 position = new Vector2(x, y);
                bool hasBlock = staticTiles.ContainsKey(position);
                if(hasBlock) {
                    maxHeight = Mathf.Max(maxHeight, y);
                }
            }
        }

        List<float> yValues = new List<float>();

        for(float x = TetrisGame.instance.bound.x; x <= TetrisGame.instance.bound.y; x ++) {
            bool foundY = false;
            for(float y = TetrisGame.startingPosition.y; y >= TetrisGame.instance.BottomY; y--) {
                Vector2 position = new Vector2(x, y);
                bool hasBlock = staticTiles.ContainsKey(position);
                if(hasBlock) {
                    foundY = true;
                    yValues.Add(y);
                    break;
                }
            }
            if(!foundY) {
                yValues.Add(TetrisGame.instance.BottomY);
            }
        }

        for(int i = 1; i < yValues.Count; i++) {
            float bump = Mathf.Abs(yValues[i] - yValues[i-1]);
            bumpiness += bump;
        }

        float min = TetrisGame.instance.BottomY;
        float max = TetrisGame.startingPosition.y;

        float numberOfLines = 0;

        for(float y = min; y < max; y++) {
            int sameYCount = 0;
            foreach(KeyValuePair<Vector2, Vector3> entry in staticTiles) {
                if(entry.Key.y == y) {
                    sameYCount ++;
                }
            }
            if(sameYCount >= Mathf.Abs(TetrisGame.instance.bound.x) + Mathf.Abs(TetrisGame.instance.bound.y) + 1) {
                numberOfLines ++;
            }
        }

        // holesAmount *= holesAmount/2;
        // maxHeight *= maxHeight/2;
        // bumpiness *= bumpiness/2;
        // numberOfLines *= numberOfLines/2;

        maxHeight = (maxHeight - TetrisGame.instance.BottomY) / (TetrisGame.startingPosition.y - TetrisGame.instance.BottomY);
        maxHeight = maxHeight * (TetrisGame.startingPosition.y - TetrisGame.instance.BottomY);

        float eval = (holesAmount * TetrisGame.instance.holeMulti) + (maxHeight * TetrisGame.instance.largeHeightMulti) + (numberOfLines * TetrisGame.instance.linesValue) + (bumpiness * TetrisGame.instance.bumpMulti);
        EvaluationReturn returnValue = new EvaluationReturn();
        returnValue.Evaluation = -eval;
        returnValue.numberOfHoles = holesAmount;
        returnValue.maxHeight = maxHeight;
        returnValue.numberOfLines = numberOfLines;
        returnValue.bumpiness = bumpiness;

        return returnValue;
    }

    public static Dictionary<EndPosition, float> EvaluateEnding(List<EndPosition> endPosLst){
        Dictionary<EndPosition, float> endPosFloatLst = new Dictionary<EndPosition, float>();
        int index = 0;
        foreach(EndPosition endPosition in endPosLst) {
            Vector2 position = endPosition.position;
            Vector2[] bodyParts = endPosition.bodyPositions;

            Dictionary<Vector2, Vector3> staticTiles = new Dictionary<Vector2, Vector3>(TetrisGame.instance.staticTiles);
            foreach(Vector2 bodyPart in bodyParts) {
                staticTiles.Add(bodyPart, Vector2.one);
            }

            EvaluationReturn Eval = EvaluatePosition(staticTiles);
            endPosLst[index].evaluation = Eval;

            endPosFloatLst.Add(endPosLst[index], Eval.Evaluation);
            index++;
        }
        return endPosFloatLst;
    }

    public static List<EndPosition> GetAllEnding(Piece piece)
    {
        List<EndPosition> endPosLst = new List<EndPosition>();
        Piece mainPieceDupe = TetrisGame.GetPiece((int)TetrisGame.instance.GetPieceType(piece));
        for(int rotation = 0; rotation < 4; rotation ++){
            mainPieceDupe.rotationAmount = rotation;

            for(float x = TetrisGame.instance.bound.x; x < TetrisGame.instance.bound.y; x ++) {
                mainPieceDupe.position.x = x;
                Vector2 endingPosition = TetrisGame.instance.GetEndingPosition(mainPieceDupe);
                Vector2[] bodyParts = mainPieceDupe.GetBodyPositions(endingPosition);

                // Setting end position value
                EndPosition endPosition = new EndPosition();
                endPosition.position = endingPosition;
                endPosition.bodyPositions = bodyParts;
                endPosition.rotation = rotation;

                // if(endingPosition.x > tetris.bound.x && endingPosition.x < tetris.bound.y && endingPosition.y > tetris.BottomY && endingPosition.y < TetrisGame.startingPosition.y) {
                if(TetrisGame.instance.LegalPosition(mainPieceDupe, endingPosition)){
                    endPosLst.Add(endPosition);
                }    
                // }
            }
        }
        return endPosLst;
    }
}
