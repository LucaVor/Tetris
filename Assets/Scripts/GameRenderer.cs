using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRenderer : MonoBehaviour
{
    public GameObject tile;
    public Vector3 renderNextAt = new Vector3(0, 0, 0);
    public Vector3 renderHeldAt = new Vector3(0, 0, 0);
    List<GameObject> tiles = new List<GameObject>();

    public int possibleEndingPositionIndex = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Render();
    }

    void Render()
    {
        Dictionary<Vector2, Vector3> staticTiles = TetrisGame.instance.staticTiles;
        Vector2[] bodyParts = TetrisGame.instance.mainPiece.GetBodyPositions();
        Vector3 color = TetrisGame.instance.mainPiece.color;

        int totalTiles = staticTiles.Count + bodyParts.Length * 4;
        totalTiles += 4;

        bool hasHeld = TetrisGame.instance.heldPiece != null;

        if(!hasHeld) {
            totalTiles -= 4;
        }

        while(tiles.Count < totalTiles) {
            GameObject newTile = Instantiate(tile);
            tiles.Add(newTile);
        }

        while(tiles.Count > totalTiles) {
            Destroy(tiles[tiles.Count - 1]);
            tiles.RemoveAt(tiles.Count - 1);
        }

        int index = 0;
        foreach(KeyValuePair<Vector2, Vector3> entry in staticTiles) {
            tiles[index].transform.position = new Vector3(entry.Key.x, entry.Key.y, 0);
            SpriteRenderer sr = tiles[index].GetComponent<SpriteRenderer>();
            sr.color = new Color(entry.Value.x, entry.Value.y, entry.Value.z);
            index++;
        }
        foreach(Vector2 bodyPart in bodyParts) {
            tiles[index].transform.position = new Vector3(bodyPart.x, bodyPart.y, 0);
            SpriteRenderer sr = tiles[index].GetComponent<SpriteRenderer>();
            sr.color = new Color(color.x, color.y, color.z);
            index++;
        }

        Vector2[] futurePositions = TetrisGame.instance.mainPiece.GetBodyPositions(TetrisGame.instance.GetEndingPosition(TetrisGame.instance.mainPiece));
        foreach(Vector2 bodyPart in futurePositions)
        {
            tiles[index].transform.position = new Vector3(bodyPart.x, bodyPart.y, 0);
            SpriteRenderer sr = tiles[index].GetComponent<SpriteRenderer>();
            sr.color = new Color(color.x, color.y, color.z, 0.3f);
            index++;
        }

        Vector2[] nextBodyPositions = TetrisGame.instance.nextPiece.GetBodyPositions(Vector2.zero);
        Vector3 nextColor = TetrisGame.instance.nextPiece.color;
        foreach(Vector2 bodyPart in nextBodyPositions) {
            tiles[index].transform.position = renderNextAt + new Vector3(bodyPart.x, bodyPart.y, 0);
            SpriteRenderer sr = tiles[index].GetComponent<SpriteRenderer>();
            sr.color = new Color(nextColor.x, nextColor.y, nextColor.z);
            index++;
        }

        if(hasHeld) {
            Vector2[] heldBodyPositions = TetrisGame.instance.heldPiece.GetBodyPositions(Vector2.zero);
            Vector3 heldColor = TetrisGame.instance.heldPiece.color;

            foreach(Vector2 bodyPart in heldBodyPositions) {
                tiles[index].transform.position = renderHeldAt + new Vector3(bodyPart.x, bodyPart.y, 0);
                SpriteRenderer sr = tiles[index].GetComponent<SpriteRenderer>();
                sr.color = new Color(heldColor.x, heldColor.y, heldColor.z);
                index++;
            }
        }

        if(index < tiles.Count - 1) {
            while(tiles.Count != index) {
                Destroy(tiles[tiles.Count - 1]);
                tiles.RemoveAt(tiles.Count - 1);
            }
        }

        // Ai.Move(true);
        // Ai.EndPosition endPos = Ai.bestEndPosition;
        // foreach(Vector2 bodyPart in endPos.bodyPositions) {
        //     tiles[index].transform.position = new Vector3(bodyPart.x, bodyPart.y, 0);
        //     SpriteRenderer sr = tiles[index].GetComponent<SpriteRenderer>();
        //     sr.color = new Color(0.5f, 0.5f, 0.5f);
        //     index++;
        // }
    }
}
