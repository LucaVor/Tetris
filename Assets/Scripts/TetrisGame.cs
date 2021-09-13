using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrisGame : MonoBehaviour
{
    public float debugForLine;
    [Header("Ai")]
    public float holeMulti;
    public float largeHeightMulti;
    public float linesValue;
    public float bumpMulti;

    [Header("Other")]
    public Text scoring;

    public int editorDelay;
    public int applicationDelay;
    public Dictionary<Vector2, Vector3> staticTiles = new Dictionary<Vector2, Vector3>(); // Position (xyx), Colour (rgb)
    public Piece mainPiece = null;
    public Piece nextPiece = null;
    public Piece heldPiece = null;

    public TileType mainPieceType;
    public TileType nextPieceType;
    public TileType heldPieceType;

    public Vector2 bound;
    public float BottomY;
    public Vector2 nonstaticStartingPosition;
    public static Vector2 startingPosition;

    public int linesCleared = 0;

    public int updateEvery = 60;
    public int nonstaticUnitSize;
    public static int unitSize;

    public bool choseHeldPiece = false;

    public static TetrisGame instance;
    
    static int pieceChoice = 0;

    public enum TileType {
        L, Line, Square, Step, Tri, Null
    }

    public void Awake()
    {
        instance = this;
    }

    public void Init() {
        // GeneticAlgorithm.instance.AgentUpdate();

        TetrisGame.unitSize = nonstaticUnitSize;
        TetrisGame.startingPosition = nonstaticStartingPosition;

        staticTiles = new Dictionary<Vector2, Vector3>();
        linesCleared = 0;
        choseHeldPiece = false;

        mainPiece = GetRandomPiece();
        nextPiece = GetRandomPiece();
    }

    public void GameStep()
    {
        Vector2 currentPosition = mainPiece.position;
        Vector2 futurePosition = mainPiece.position + new Vector2(0, -1);
        Vector2[] futurePositions = mainPiece.GetBodyPositions(futurePosition);


        if(LegalPosition(mainPiece, futurePosition))
            mainPiece.Update();
        else {
            Vector2[] bodyParts = mainPiece.GetBodyPositions();
            foreach(Vector2 bodyPart in bodyParts) {
                try{ 
                    staticTiles.Add(bodyPart, mainPiece.color);
                } catch(System.Exception err) {
                    Init();
                    return;
                }
            }

            mainPiece = nextPiece;
            nextPiece = GetRandomPiece();

            float min = BottomY;
            float max = startingPosition.y;

            List<float> LinesOn = new List<float>();

            for(float y = min; y <= max; y++) {
                bool nonIncluded = false;
                for(float x = bound.x; x <= bound.y; x++) {
                    if(!staticTiles.ContainsKey(new Vector2(x, y))) {
                        nonIncluded = true;
                    }
                }
                if(!nonIncluded) {
                    LinesOn.Add(y);
                    linesCleared += 1;
                }
            }

            LinesOn.Sort();
            foreach(float y in LinesOn) {
                for(float x = bound.x; x <= bound.y; x++) {
                    staticTiles.Remove(new Vector2(x, y));
                }
            }
            
            // Apply Gravity.
            bool hasFloatingLines = LinesOn.Count > 0;
            while(hasFloatingLines) {
                // TODO: Sort one floating line
                for(float y = min; y <= max; y++) {
                    bool hasBlock = false;
                    for(float x = bound.x; x <= bound.y; x++) {
                        if(staticTiles.ContainsKey(new Vector2(x, y))) {
                            hasBlock = true;
                        }
                    }

                    if(!hasBlock) continue;
                    if(y == min) continue;

                    Debug.Log("Has block and not at bottom");

                    float belowY = y - 1;
                    bool bottomHasBlock = false;
                    for(float x = bound.x; x <= bound.y; x++) {
                        if(staticTiles.ContainsKey(new Vector2(x, belowY))) {
                            bottomHasBlock = true;
                        }
                    }

                    if(bottomHasBlock) continue;
                    Debug.Log("Floating line spotted");

                    // At this point, this is a floating line. Move it down...
                    for(float x = bound.x; x <= bound.y; x++) {
                        if(staticTiles.ContainsKey(new Vector2(x, y))) {
                            Vector3 col = staticTiles[new Vector2(x, y)];
                            staticTiles.Remove(new Vector2(x, y));
                            staticTiles.Add(new Vector2(x, y - 1), col);
                        }
                    }

                    break;
                }

                hasFloatingLines = false;

                // TODO: reset has floating lines variable
                for(float y = min; y <= max; y++) {
                    bool hasBlock = false;
                    for(float x = bound.x; x <= bound.y; x++) {
                        if(staticTiles.ContainsKey(new Vector2(x, y))) {
                            hasBlock = true;
                        }
                    }
                    if(!hasBlock) continue;
                    if(y == min) continue;
                    float belowY = y - 1;
                    bool bottomHasBlock = false;
                    for(float x = bound.x; x <= bound.y; x++) {
                        if(staticTiles.ContainsKey(new Vector2(x, belowY))) {
                            bottomHasBlock = true;
                        }
                    }

                    if(bottomHasBlock) continue;
                    hasFloatingLines = true;
                }

            }

            choseHeldPiece = false;
        }
        
    }

    public void Start()
    {
        Init();
    }

    public void Update()
    {
        scoring.text = "Score: " + linesCleared.ToString();

        if(Input.GetKeyDown(KeyCode.LeftControl)) {
        // if(Time.frameCount % 2 == 0) {
            Ai.Push();
            return;
        }
        
        HandleMovementInput();

        if(Input.GetKeyDown(KeyCode.H) && !choseHeldPiece) {
            ChooseHeld();
        }

        mainPieceType = GetPieceType(mainPiece);
        nextPieceType = GetPieceType(nextPiece);
        heldPieceType = GetPieceType(heldPiece);
        
    }

    public void FixedUpdate()
    {
        int fixedFrames = FixedFrameCount();

        if(fixedFrames % updateEvery == 0) {
            GameStep();
        }
    }

    public void ChooseHeld()
    {
        if(choseHeldPiece) {
            return;
        }
        bool hasHeld = heldPiece != null;
        Piece heldPieceChoice = GetPiece((int)GetPieceType(hasHeld ? heldPiece : nextPiece));
        heldPiece = mainPiece;
        mainPiece = heldPieceChoice;
        if(!hasHeld) nextPiece = GetRandomPiece();
        choseHeldPiece = true;
    }

    public bool LegalPosition(Piece piece, Vector2 pos) {
        bool canMove = true;
        foreach(Vector2 bodyPart in piece.GetBodyPositions(pos)) {
            if(bodyPart.x < bound.x || bodyPart.x > bound.y) {
                canMove = false;
                break;
            }
            if(bodyPart.y < BottomY) {
                canMove = false;
                break;
            }
            foreach(KeyValuePair<Vector2, Vector3> entry in staticTiles) {
                if(bodyPart == entry.Key) {
                    canMove = false;
                    break;
                }
            }
        }
        return canMove;
    }

    public void InstaDrop()
    {
        float endingY = GetEndingPosition(TetrisGame.instance.mainPiece).y;
        mainPiece.position.y = endingY;
    }

    public Vector2 GetEndingPosition(Piece piece) {
        float x = piece.position.x;
        float y = piece.position.y;

        while(true) {
            if(LegalPosition(piece, new Vector2(x, y-1))) {
                y -= 1;
            } else {
                break;
            }
        }

        return new Vector2(x, y);

    }

    public void HandleMovementInput()
    {
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            mainPiece.rotationAmount += 1;
            Vector2[] bodyParts = mainPiece.GetBodyPositions();
            bool canMove = true;
            foreach(Vector2 bodyPart in bodyParts) {
                if(bodyPart.y < BottomY) {
                    canMove = false;
                    break;
                }
                if(bodyPart.x < bound.x || bodyPart.x > bound.y) {
                    canMove = false;
                    break;
                }

                foreach(KeyValuePair<Vector2, Vector3> entry in staticTiles) {
                    if(bodyPart == entry.Key) {
                        canMove = false;
                        break;
                    }
                }
            }
            if(canMove == false) {
                mainPiece.rotationAmount -= 1;
            }
        }
        int delay = Application.isEditor ? editorDelay : applicationDelay;
        if(Input.GetKey(KeyCode.A) && Time.frameCount % delay == 0) {
            Vector2 newPosition = mainPiece.position + Vector2.left;
            if(LegalPosition(mainPiece, newPosition)) {
                mainPiece.position = newPosition;
            }
        }
        else if (Input.GetKey(KeyCode.D) && Time.frameCount % delay == 0) {
            Vector2 newPosition = mainPiece.position + Vector2.right;
            if(LegalPosition(mainPiece, newPosition)) {
                mainPiece.position = newPosition;
            }
        }
        else if (Input.GetKey(KeyCode.S) && Time.frameCount % delay == 0) {
            Vector2 newPosition = mainPiece.position + Vector2.down;
            if(LegalPosition(mainPiece, newPosition)) {
                mainPiece.position = newPosition;
            }
        }

        if(Input.GetMouseButtonDown(0)) {
            InstaDrop();
        }
    }

    public static int FixedFrameCount()
    {
        return Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
    }

    public TileType GetPieceType(Piece piece)
    {
        if(piece.name == "L") {
            return TileType.L;
        }
        if(piece.name == "Line") {
            return TileType.Line;
        }
        if(piece.name == "Square") {
            return TileType.Square;
        }
        if(piece.name == "Step") {
            return TileType.Step;
        }
        if(piece.name == "Tri") {
            return TileType.Tri;
        }
        return TileType.Null;
    }

    public static Piece GetRandomPiece() {
        // pieceChoice = Random.Range(0, 5);
        if(Random.Range(0, 100) < 80){
            pieceChoice = (pieceChoice + 1) % 5;
        }else {
            pieceChoice = Random.Range(0, 5);
        }  
        return GetPiece(pieceChoice);
    }

    public static Piece GetPiece(int pieceChoice)
    {
        switch(pieceChoice) {
            case 0:
                return new L();
                break;
            case 1:
                return new Line();
                break;
            case 2:
                return new Square();
                break;
            case 3:
                return new Step();
                break;
            case 4:
                return new Tri();
                break;
            default:
                return null;
        }
        return null;
    }

}
