using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;

public struct Triangle
{
	public int value;
}

public struct Vertex
{
	public Vector3 coords;
}

public class MeshGenerator : MonoBehaviour
{
	public Vector3 NumberOfSquares = new Vector3(10, 10, 10);
	public List<Vector3> vertices;
	public List<int> triangles;
	public Grid grid;
	public bool generate;

	public string ReadonlyId;

	public float groundThreshold;
	public Vector3 MinCorner;
	public Vector3 MaxCorner;

	public bool isTargetRefresh = false;

	public class Grid
	{
		public Cube[,,] cubes;
		public Dictionary<Vector3, Node> nodes;

		public Grid(Vector3 SquaresLength, Vector3 offset) {
			int gridLength = (int)(SquaresLength.x * SquaresLength.y * SquaresLength.z);

			cubes = new Cube[(int)SquaresLength.x,(int)SquaresLength.y,(int)SquaresLength.z];
			nodes = new Dictionary<Vector3, Node>();

			for(float x = 0; x < cubes.GetLength(0); x++) {
				for(float y = 0; y < cubes.GetLength(1); y++) {
					for(float z = 0; z < cubes.GetLength(2); z++) {

						/*
						Node _UFR, Node _UFL, 
			    		Node _DFR, Node _DFL, 
			    		Node _UBR, Node _UBL, 
			    		Node _DBR, Node _DBL
			    		*/

			    		Vector3 position = new Vector3(x, y, z); // Center
			    		float unitSize = 0.5f;

			    		// Debug.Log(position.x + 0.5f + " : " + position.x);

			    		Node _UFR = new Node(
			    			new Vector3(position.x + unitSize, position.y + unitSize, position.z + unitSize)
			    		); try{ nodes.Add(
			    			new Vector3(position.x + unitSize, position.y + unitSize, position.z + unitSize),
			    			_UFR
			    		); } catch(Exception err) { }
			    		Node _UFL = new Node(
			    			new Vector3(position.x + unitSize, position.y + unitSize, position.z - unitSize)
			    		); try{ nodes.Add(
			    			new Vector3(position.x + unitSize, position.y + unitSize, position.z - unitSize),
			    			_UFL
			    		); } catch(Exception err) { }
			    		Node _DFR = new Node(
			    			new Vector3(position.x - unitSize, position.y + unitSize, position.z + unitSize)
			    		); try{ nodes.Add(
			    			new Vector3(position.x - unitSize, position.y + unitSize, position.z + unitSize),
			    			_DFR
			    		); } catch(Exception err) { }
			    		Node _DFL = new Node(
			    			new Vector3(position.x - unitSize, position.y + unitSize, position.z - unitSize)
			    		); try{ nodes.Add(
			    			new Vector3(position.x - unitSize, position.y + unitSize, position.z - unitSize),
			    			_DFL
			    		); } catch(Exception err) { }
			    		Node _UBR = new Node(
			    			new Vector3(position.x + unitSize, position.y - unitSize, position.z + unitSize)
			    		); try{ nodes.Add(
			    			new Vector3(position.x + unitSize, position.y - unitSize, position.z + unitSize),
			    			_UBR
			    		); } catch(Exception err) { }
			    		Node _UBL = new Node(
			    			new Vector3(position.x + unitSize, position.y - unitSize, position.z - unitSize)
			    		); try{ nodes.Add(
			    			new Vector3(position.x + unitSize, position.y - unitSize, position.z - unitSize),
			    			_UBL
			    		); } catch(Exception err) { }
			    		Node _DBR = new Node(
			    			new Vector3(position.x - unitSize, position.y - unitSize, position.z + unitSize)
			    		); try{ nodes.Add(
			    			new Vector3(position.x - unitSize, position.y - unitSize, position.z + unitSize),
			    			_DBR
			    		); } catch(Exception err) { }
			    		Node _DBL = new Node(
			    			new Vector3(position.x - unitSize, position.y - unitSize, position.z - unitSize)
			    		); try{ nodes.Add(
			    			new Vector3(position.x - unitSize, position.y - unitSize, position.z - unitSize),
			    			_DBL
			    		); } catch(Exception err) { }

						Cube cube = new Cube(position,
							nodes[_UFR.position], nodes[_UFL.position], 
				    		nodes[_DFR.position], nodes[_DFL.position], 
				    		nodes[_UBR.position], nodes[_UBL.position], 
				    		nodes[_DBR.position], nodes[_DBL.position]
			    		);

			    		cubes[(int) x, (int) y, (int) z] = cube;
					}
				}
			}
			
		}
	}

    public void GenerateMeshCPU(bool targetCall = false) {
    	Mesh mesh = new Mesh();
    	MeshFilter meshFilter = GetComponent<MeshFilter>();
		if(grid == null) {
			grid = new Grid(NumberOfSquares, Vector3.zero);
		}
    	Cube[,,] cubes = grid.cubes;

    	vertices = new List<Vector3>();
    	triangles = new List<int>();

		int[] startingIndexes = new int[] {
			0, 3, 6, 9, 12
		};

    	for(int x = 0; x < NumberOfSquares.x; x++){
    		for(int y = 0; y < NumberOfSquares.y; y++){
	    		for(int z = 0; z < NumberOfSquares.z; z++){
	    			try {
	    				Cube cube = cubes[x, y, z];
						cube.UpdateNodeParentPositions(transform.position);

	    				Vector3 squarePosition = cube.position;

	    				int cubeIndex = cube.CalculateCubeIndex();

	    				foreach(int i in startingIndexes) {
							int firstVal = TriangleTable.triTable[cubeIndex, i + 0];
							int secondVal = TriangleTable.triTable[cubeIndex, i + 1];
							int thirdVal = TriangleTable.triTable[cubeIndex, i + 2];

							if(firstVal == -1) {
								continue;
							}

							Node[] triangle = new Node[] { cube.triValues[firstVal], 
								cube.triValues[secondVal], cube.triValues[thirdVal] };


							for(int triIndex = 0; triIndex < triangle.Length; triIndex++) {
								triangle[triIndex].vertexIndex = vertices.Count;
								vertices.Add(triangle[triIndex].position);
							}

							triangles.Add(triangle[0].vertexIndex);
							triangles.Add(triangle[1].vertexIndex);
							triangles.Add(triangle[2].vertexIndex);
	    				}


	    			} catch(Exception err){
	    				Debug.Log(err);
	    			}
	    		}
    		}
    	}

    	mesh.vertices = vertices.ToArray();
    	mesh.triangles = triangles.ToArray();
    	mesh.RecalculateNormals();

    	meshFilter.mesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
    	MeshGenerationUtility.noiseGen = new FastNoise(UnityEngine.Random.Range(1000,9999));
    	MeshGenerationUtility.threshold = groundThreshold;


    	grid = new Grid(NumberOfSquares, Vector3.zero);
        vertices = new List<Vector3>();
        triangles = new List<int>();

    }

    // Update is called once per frame
    void Update()
    {
		if(grid == null) grid = new Grid(NumberOfSquares, Vector3.zero);

    	if(( generate )) {

			MeshGenerationUtility.threshold = groundThreshold;
    		GenerateMeshCPU(isTargetRefresh);
    		generate = false;

			isTargetRefresh = false;
    	}

		ReadonlyId = gameObject.GetInstanceID().ToString();

    }

    public bool CalculateBounds(Vector3 point) {
    	// Vector3 Center = transform.position + (NumberOfSquares / 2);
    	// float xDiff = Mathf.Sqrt(Mathf.Pow(Center.x - point.x, 2));
    	// float yDiff = Mathf.Sqrt(Mathf.Pow(Center.y - point.y, 2));
    	// float zDiff = Mathf.Sqrt(Mathf.Pow(Center.z - point.z, 2));

    	// return xDiff < NumberOfSquares.x + 1 && yDiff < NumberOfSquares.y + 1 && zDiff < NumberOfSquares.z + 1;
    	Vector3 cornerOne = transform.position - (Vector3.one*0.5f);
    	Vector3 cornerTwo = (transform.position + NumberOfSquares) - (Vector3.one*0.5f);

    	float X = point.x;
    	float Y = point.y;
    	float Z = point.z;

		float slack = 3;

    	float xMin = Mathf.Min(cornerOne.x, cornerTwo.x) - slack;
    	float yMin = Mathf.Min(cornerOne.y, cornerTwo.y) - slack;
    	float zMin = Mathf.Min(cornerOne.z, cornerTwo.z) - slack;
    	float xMax = Mathf.Max(cornerOne.x, cornerTwo.x) + slack;
    	float yMax = Mathf.Max(cornerOne.y, cornerTwo.y) + slack;
    	float zMax = Mathf.Max(cornerOne.z, cornerTwo.z) + slack;

		MinCorner = new Vector3(xMin + slack, yMin + slack, zMin + slack);
		MaxCorner = new Vector3(xMax - slack, yMax - slack, zMax - slack);

    	bool inMin = X >= xMin && Y >= yMin && Z >= zMin;
		bool inMax = X <= xMax && X <= yMax && Z <= zMax;

		return inMin && inMax;
    }

    Vector3 sub(Vector3 v, float s){
    	return new Vector3(v.x - s, v.y - s, v.z - s);
    }

    // void OnDrawGizmos()
    // {
    // 	Cube[,,] cubes = grid.cubes;
    // 	int n = 0;
    // 	for(int x = 0; x < NumberOfSquares.x; x++){
    // 		for(int y = 0; y < NumberOfSquares.y; y++){
	//     		for(int z = 0; z < NumberOfSquares.z; z++){
	//     			// if(x > 0 || y > 0 || z > 0){
	//     			// 	continue;
	//     			// }
	//     			try {
	//     				n++;
	//     				Color squareColor = Color.green;
	//     				squareColor.a = 0.5f;

	//     				Cube cube = cubes[x, y, z];

	//     				Vector3 squarePosition = cube.position;
	//     				Gizmos.color = squareColor;
	//     				Gizmos.DrawCube(transform.position+squarePosition, Vector3.one);

	//     				// Gizmos.color = cube.UFR.active ? Color.red : Color.blue;
	//     				// Gizmos.DrawCube(cube.UFR.position, Vector3.one * 0.2f);
	//     				// Gizmos.color = cube.UFL.active ? Color.red : Color.blue;
	//     				// Gizmos.DrawCube(cube.UFL.position, Vector3.one * 0.2f);
	//     				// Gizmos.color = cube.DFR.active ? Color.red : Color.blue;
	//     				// Gizmos.DrawCube(cube.DFR.position, Vector3.one * 0.2f);
	//     				// Gizmos.color = cube.DFL.active ? Color.red : Color.blue;
	//     				// Gizmos.DrawCube(cube.DFL.position, Vector3.one * 0.2f);
	//     				// Gizmos.color = cube.UBR.active ? Color.red : Color.blue;
	//     				// Gizmos.DrawCube(cube.UBR.position, Vector3.one * 0.2f);
	//     				// Gizmos.color = cube.UBL.active ? Color.red : Color.blue;
	//     				// Gizmos.DrawCube(cube.UBL.position, Vector3.one * 0.2f);
	//     				// Gizmos.color = cube.DBR.active ? Color.red : Color.blue;
	//     				// Gizmos.DrawCube(cube.DBR.position, Vector3.one * 0.2f);
	//     				// Gizmos.color = cube.DBL.active ? Color.red : Color.blue;
	//     				// Gizmos.DrawCube(cube.DBL.position, Vector3.one * 0.2f);

	//     			} catch(Exception err){

	//     			}
	//     		}
    // 		}
    // 	}
	// 	Gizmos.color = Color.magenta;
	// 	Gizmos.DrawCube(MinCorner, Vector3.one/2);
	// 	Gizmos.DrawCube(MaxCorner, Vector3.one/2);
    // }
}
