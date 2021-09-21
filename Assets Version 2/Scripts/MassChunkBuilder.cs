using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassChunkBuilder : MonoBehaviour
{
    public GameObject chunk;
    public int xScale;
    public int yScale;

    public static MassChunkBuilder instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Generate()
    {
        for(int x = 0; x < xScale * 15; x += 15) {
            for(int y = 0; y < yScale * 15; y += 15) {
                BuildAt(x, y);
            }
        }
    }

    void BuildAt(float x, float y) {
        GameObject currentChunk = Instantiate(chunk);
        currentChunk.transform.position = new Vector3(x, 0, y);
    }
}
