﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoWeld : MonoBehaviour {
	public bool doAutoWeld;
	public MeshFilter meshFilter;
    public float threshold = 0.000001f;
	public float bucketStep = 65000f;
    public bool addFindEdges = false;
    
	// Use this for initialization
	void Start () {

        //if script is added to a gameobject, calls the static function from here
        //otherwise, function can be called from anywhere
        meshFilter = GetComponent<MeshFilter>();
        	
		meshFilter.mesh =  AutoWeldFunction(meshFilter.mesh,threshold,bucketStep);

        //start Find Edges Script

        

	}
	/*
	// Update is called once per frame
	void Update () {
	
		if (doAutoWeld)
		{
			AutoWeldFunction(meshFilter.mesh,threshold,bucketStep);
			doAutoWeld = false;
		}
	}
    */
  
	public static Mesh AutoWeldFunction (Mesh mesh, float threshold, float bucketStep) {

        Vector3[] oldVertices = mesh.vertices;
		Vector3[] newVertices = new Vector3[oldVertices.Length];
		int[] old2new = new int[oldVertices.Length];
		int newSize = 0;
		
		// Find AABB
		Vector3 min = new Vector3 (float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 max = new Vector3 (float.MinValue, float.MinValue, float.MinValue);
		for (int i = 0; i < oldVertices.Length; i++) {
			if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
			if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
			if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
			if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
			if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
			if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
		}
		
		// Make cubic buckets, each with dimensions "bucketStep"
		int bucketSizeX = Mathf.FloorToInt ((max.x - min.x) / bucketStep) + 1;
		int bucketSizeY = Mathf.FloorToInt ((max.y - min.y) / bucketStep) + 1;
		int bucketSizeZ = Mathf.FloorToInt ((max.z - min.z) / bucketStep) + 1;
		List<int>[,,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];
		
		// Make new vertices
		for (int i = 0; i < oldVertices.Length; i++) {
			// Determine which bucket it belongs to
			int x = Mathf.FloorToInt ((oldVertices[i].x - min.x) / bucketStep);
			int y = Mathf.FloorToInt ((oldVertices[i].y - min.y) / bucketStep);
			int z = Mathf.FloorToInt ((oldVertices[i].z - min.z) / bucketStep);
			
			// Check to see if it's already been added
			if (buckets[x, y, z] == null)
				buckets[x, y, z] = new List<int> (); // Make buckets lazily
			
			for (int j = 0; j < buckets[x, y, z].Count; j++) {
				Vector3 to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
				if (Vector3.SqrMagnitude (to) < threshold) {
					old2new[i] = buckets[x, y, z][j];
					goto skip; // Skip to next old vertex if this one is already there
				}
			}
			
			// Add new vertex
			newVertices[newSize] = oldVertices[i];
			buckets[x, y, z].Add (newSize);
			old2new[i] = newSize;
			newSize++;
			
		skip:;
		}
		
		// Make new triangles
		int[] oldTris = mesh.triangles;
		int[] newTris = new int[oldTris.Length];
		for (int i = 0; i < oldTris.Length; i++) {
			newTris[i] = old2new[oldTris[i]];
		}
		
		Vector3[] finalVertices = new Vector3[newSize];
		for (int i = 0; i < newSize; i++)
			finalVertices[i] = newVertices[i];

        Mesh newMesh = new Mesh();
        //newMesh.Clear();
        newMesh.vertices = oldVertices;// finalVertices;
		newMesh.triangles = newTris;
		newMesh.RecalculateNormals ();
		//newmesh.Optimize ();
		newMesh.name = "AutoWeldedMesh with Threshold " + threshold;
        //	meshFilter.sharedMesh = mesh;

        //    if (addFindEdges)
        //    {
        //        StartCoroutine("WaitAndFindEdges");

        //  }

        return newMesh;

    }

    public static Mesh AutoWeldFunctionIgnoringOuter(Mesh mesh, float threshold, float bucketStep,List<int> outerToIgnore)
    {

        Vector3[] oldVertices = mesh.vertices;
        Vector3[] newVertices = new Vector3[oldVertices.Length];
        int[] old2new = new int[oldVertices.Length];
        int newSize = 0;

        // Find AABB
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        for (int i = 0; i < oldVertices.Length; i++)
        {
            if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
            if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
            if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
            if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
            if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
            if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
        }

        // Make cubic buckets, each with dimensions "bucketStep"
        int bucketSizeX = Mathf.FloorToInt((max.x - min.x) / bucketStep) + 1;
        int bucketSizeY = Mathf.FloorToInt((max.y - min.y) / bucketStep) + 1;
        int bucketSizeZ = Mathf.FloorToInt((max.z - min.z) / bucketStep) + 1;
        List<int>[,,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];

        // Make new vertices
        for (int i = 0; i < oldVertices.Length; i++)
        {
            
            // Determine which bucket it belongs to
            int x = Mathf.FloorToInt((oldVertices[i].x - min.x) / bucketStep);
            int y = Mathf.FloorToInt((oldVertices[i].y - min.y) / bucketStep);
            int z = Mathf.FloorToInt((oldVertices[i].z - min.z) / bucketStep);

            // Check to see if it's already been added
            if (buckets[x, y, z] == null)
                buckets[x, y, z] = new List<int>(); // Make buckets lazily

            if (!outerToIgnore.Contains(i))
            {
                for (int j = 0; j < buckets[x, y, z].Count; j++)
                {
                    Vector3 to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
                    if (Vector3.SqrMagnitude(to) < threshold)
                    {
                        old2new[i] = buckets[x, y, z][j];
                        goto skip; // Skip to next old vertex if this one is already there
                    }
                }
            }
            // Add new vertex
            newVertices[newSize] = oldVertices[i];
            buckets[x, y, z].Add(newSize);
            old2new[i] = newSize;
            newSize++;

            skip:;
        }

        // Make new triangles
        int[] oldTris = mesh.triangles;
        int[] newTris = new int[oldTris.Length];
        for (int i = 0; i < oldTris.Length; i++)
        {
            newTris[i] = old2new[oldTris[i]];
        }

        Vector3[] finalVertices = new Vector3[newSize];
        for (int i = 0; i < newSize; i++)
            finalVertices[i] = newVertices[i];

        Mesh newMesh = new Mesh();
        //newMesh.Clear();
        newMesh.vertices = finalVertices;
        newMesh.triangles = newTris;
        newMesh.RecalculateNormals();
        //newmesh.Optimize ();
        newMesh.name = "AutoWeldedMesh with Threshold " + threshold;
        //	meshFilter.sharedMesh = mesh;

        //    if (addFindEdges)
        //    {
        //        StartCoroutine("WaitAndFindEdges");

        //  }

        return newMesh;

    }
}
