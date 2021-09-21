using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FN_USE_DOUBLES
using FN_DECIMAL = System.Double;
#else
using FN_DECIMAL = System.Single;
#endif

public class Cube
{
	/*
	
	Node names are in position format
	{
		1. U = Up
		1. D = Down
		2. F = Forward
		2. B = Backward
		3. R = Right
		3. L = Left

		UFR = UpForwardRight
	}

	*/

	public Vector3 position;
	public Node UFR, UFL, DFR, DFL, UBR, UBL, DBR, DBL; // Edge nodes
	public Node DB, DR, DF, DL, UB, UR, UF, UL, BL, BR, FR, FL; // Mid point nodes, used to connect
	public Node[] values;
	public Node[] triValues;

	public Cube(Vector3 _pos, 
		Node _UFR, Node _UFL, 
		Node _DFR, Node _DFL, 
		Node _UBR, Node _UBL, 
		Node _DBR, Node _DBL) {

		position = _pos;

		UFR = _UFR;
		UFL = _UFL;
		DFR = _DFR;
		DFL = _DFL;
		UBR = _UBR;
		UBL = _UBL;
		DBR = _DBR;
		DBL = _DBL;

		float unitSize = 0.5f;

		// UD = x
		// FB = y
		// RL = z

		DB = new Node(new Vector3(_pos.x - unitSize, _pos.y - unitSize, _pos.z));
		DR = new Node(new Vector3(_pos.x - unitSize, _pos.y, _pos.z + unitSize));
		DF = new Node(new Vector3(_pos.x - unitSize, _pos.y + unitSize, _pos.z));
		DL = new Node(new Vector3(_pos.x - unitSize, _pos.y, _pos.z - unitSize));
		UB = new Node(new Vector3(_pos.x + unitSize, _pos.y - unitSize, _pos.z));
		UR = new Node(new Vector3(_pos.x + unitSize, _pos.y, _pos.z + unitSize));
		UF = new Node(new Vector3(_pos.x + unitSize, _pos.y + unitSize, _pos.z));
		UL = new Node(new Vector3(_pos.x + unitSize, _pos.y, _pos.z - unitSize));
		BL = new Node(new Vector3(_pos.x, _pos.y - unitSize, _pos.z - unitSize));
		BR = new Node(new Vector3(_pos.x, _pos.y - unitSize, _pos.z + unitSize));
		FR = new Node(new Vector3(_pos.x, _pos.y + unitSize, _pos.z + unitSize));
		FL = new Node(new Vector3(_pos.x, _pos.y + unitSize, _pos.z - unitSize));

		values = new Node[] { DBL, DBR, DFR, DFL, UBL, UBR, UFR, UFL };
		triValues = new Node[] { DB, DR, DF, DL, UB, UR, UF, UL, BL, BR, FR, FL };
	}

	public void UpdateNodeParentPositions(Vector3 position) {
		for(int i = 0; i < 8; i++) {
			values[i].parentPosition = position;
		}
		// for(int i = 0; i < 12; i++) {
		// 	triValues[i].parentPosition = position;
		// }
	}

	public int CalculateCubeIndex()
	{

		int cubeIndex = 0;
		for(int i = 0; i < 8; i++) {
			values[i].CalculateIsActive();
			if (values[i].active) {
				cubeIndex |= 1 << i;
			}
		}

		return cubeIndex;
	}
}

public class Node
{
	public Vector3 position;
	public Vector3 parentPosition;
	public bool active;

	public int vertexIndex = -1;


	public Node(Vector3 _pos){
		position = _pos;
		CalculateIsActive();
	}

	public void CalculateIsActive()
	{
		if(MeshGenerationUtility.target != -Vector3.one) {
			if(Vector3.Distance(MeshGenerationUtility.target, parentPosition+position) < MeshGenerationUtility.threshold) {
				active = MeshGenerationUtility.add;
			}
			return;
		}
		active = false;
	}
}

public static class MeshGenerationUtility
{
	public static Vector3 target;
	public static float threshold;
	public static FastNoise noiseGen;
	public static bool add;
}