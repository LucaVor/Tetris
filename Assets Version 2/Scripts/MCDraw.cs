using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCDraw : MonoBehaviour
{
    public MeshGenerator[] meshGenerators;
    public float lastPressed;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetMouseButton(0) || Input.GetMouseButton(2)) && Time.frameCount > lastPressed + 10){
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit)) {
                if(Input.GetMouseButton(0)) {
                    MeshGenerationUtility.add = true;
                }
                if(Input.GetMouseButton(2)) {
                    MeshGenerationUtility.add = false;
                }

                lastPressed = Time.frameCount;

                meshGenerators = FindObjectsOfType<MeshGenerator>();

                foreach(MeshGenerator meshGen in meshGenerators) {
                    if(meshGen.CalculateBounds(hit.point) && meshGen.gameObject.activeSelf) {
                        // MeshGenerationUtility.target = hit.point - meshGen.transform.position;
                        MeshGenerationUtility.target = hit.point;
                        meshGen.generate = true;
                        meshGen.isTargetRefresh = false;
                    }
                }

                return;
            }
        } else {
            MeshGenerationUtility.target = -Vector3.one;
        }
    }
}
