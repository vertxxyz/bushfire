// Mesh Merger Script
// Copyright 2009, Russ Menapace
// http://humanpoweredgames.com

// Summary:
//  This script allows you to draw a large number of meshes with a single 
//  draw call.  This is particularly useful for iPhone games.

// License:
//  Free to use as you see fit, but I would appreciate one of the following:
//  * A credit for Human Powered Games, or even a link to humanpoweredgames.com
//    in whatever you make with this
//  * Hire me to make games or simulations
//  * A donation to the PayPal account at russ@databar.com.  I'm very poor, so 
//    even a small donation would be greatly appreciated!
//  * A thank you note to russ@databar.com
//  * Suggestions on how the script could be improved mailed to russ@databar.com

// Warranty:
//  This software carries no warranty, and I don't guarantee anything about it.
//  If it burns down your house or gets your cat pregnant, don't look at me. 

// Acnowledgements: 
//  This was pieced together out of code I found onthe UnifyCommunity wiki, and 
//  the Unity forum.  I did not keep track of names, but I do recall gaining
//  a lot of insight from the posts of PirateNinjaAlliance.  
//  Thanks to anybody that may have been involved.

// Requirements:  
//  All the meshes you want to use must use the same material.  
//  This material may be a texture atlas and the meshes UV to portions of the atlas.
//  The texture atlas technique works particularly well for GUI stuff.

// Usage:
//  There are two ways to use this script:

//  Implicit:  
//    Simply drop the script into a GameObject that has a number of
//    child objects containing mesh filters.

//  Explicit:
//    Populate the meshFilter array with the meshes you want merged
//    Optionally, set the material to be used.  If no material is selected,
//    The script will apply the first material it encounters to all subsequent
//    meshes

// To see if it's working:
//  Move the camera so you can see several of your objects in the Game pane
//  Note the number of draw calls
//  Hit play. You should see the number of draw calls for those meshes reduced to one

using UnityEngine;
using System.Collections.Generic;



//==============================================================================
public class MeshMerger : MonoBehaviour 
{
	class MergeInstance {
		public List<MeshFilter> filters = new List<MeshFilter>();
		public int vertCount;
	}
	public MeshFilter[] meshFilters = new MeshFilter[0];
	//----------------------------------------------------------------------------
	void Start ()
	{
		// if not specified, go find meshes
		if(meshFilters == null || meshFilters.Length == 0)
		{
			// find all the mesh filters
			meshFilters = GetComponentsInChildren<MeshFilter>();
		}
		// Now make sure we are not biting off more than we can chew!
		Dictionary<Material, MergeInstance> instances = new Dictionary<Material, MergeInstance>();
		foreach(MeshFilter mf in meshFilters)
		{
			Material curMat = mf.GetComponent<Renderer>().sharedMaterial;
			if(!instances.ContainsKey(curMat)) {
				instances.Add(curMat, new MergeInstance());
			}
			instances[curMat].vertCount += mf.mesh.vertices.Length;
			if(instances[curMat].vertCount < 65536) {
				instances[curMat].filters.Add (mf);
			} else {
				MergeMeshes(instances[curMat].filters.ToArray());
				instances[curMat].filters.Clear();
				instances[curMat].filters.Add(mf);
				instances[curMat].vertCount = mf.mesh.vertices.Length;
			}
		}
		foreach(MergeInstance instance in instances.Values) {
			MergeMeshes(instance.filters.ToArray());
		}
	}

	public void MergeMeshes(MeshFilter[] filters) {
		// figure out array sizes
		int vertCount = 0;
		int normCount = 0;
		int triCount = 0;
		int uvCount = 0;
		int uv2Count = 0;
		Material material = null;
		foreach(MeshFilter mf in filters)
		{
			vertCount += mf.mesh.vertices.Length; 
			normCount += mf.mesh.normals.Length;
			triCount += mf.mesh.triangles.Length; 
			uvCount += mf.mesh.uv.Length;
			uv2Count += mf.mesh.uv2.Length;
			if(material == null)
				material = mf.gameObject.GetComponent<Renderer>().sharedMaterial;       
		}
		
		// allocate arrays
		Vector3[] verts = new Vector3[vertCount];
		Vector3[] norms = new Vector3[normCount];
		Transform[] aBones = new Transform[filters.Length];
		Matrix4x4[] bindPoses = new Matrix4x4[filters.Length];
		BoneWeight[] weights = new BoneWeight[vertCount];
		int[] tris  = new int[triCount];
		Vector2[] uvs = new Vector2[uvCount];
		Vector2[] uv2s = new Vector2[uv2Count];
		
		int vertOffset = 0;
		int normOffset = 0;
		int triOffset = 0;
		int uvOffset = 0;
		int uv2Offset = 0;
		int meshOffset = 0;
		
		// merge the meshes and set up bones
		foreach(MeshFilter mf in filters)
		{     
			foreach(int i in mf.mesh.triangles)
				tris[triOffset++] = i + vertOffset;
			
			aBones[meshOffset] = mf.transform;
			bindPoses[meshOffset] = Matrix4x4.identity;
			
			foreach(Vector3 v in mf.mesh.vertices)
			{
				weights[vertOffset].weight0 = 1.0f;
				weights[vertOffset].boneIndex0 = meshOffset;
				verts[vertOffset++] = v;
			}
			
			foreach(Vector3 n in mf.mesh.normals)
				norms[normOffset++] = n;
			
			foreach(Vector2 uv in mf.mesh.uv)
				uvs[uvOffset++] = uv;
			foreach(Vector2 uv2 in mf.mesh.uv2)
				uv2s[uv2Offset++] = uv2;
			
			meshOffset++;
			
			MeshRenderer mr = mf.gameObject.GetComponent<MeshRenderer>();
			
			if(mr)
				mr.enabled = false;
		}
		
		// hook up the mesh
		Mesh me = new Mesh();       
		me.name = gameObject.name;
		me.vertices = verts;
		me.normals = norms;
		me.boneWeights = weights;
		me.uv = uvs;
		me.uv2 = uv2s;
		me.triangles = tris;
		me.bindposes = bindPoses;

		GameObject newRenderer = new GameObject(gameObject.name + " (Combined)");
		newRenderer.transform.parent = transform;
		newRenderer.layer = gameObject.layer;

		// hook up the mesh renderer        
		SkinnedMeshRenderer smr = 
			newRenderer.AddComponent<SkinnedMeshRenderer>();
		
		smr.sharedMesh = me;
		smr.bones = aBones;
		smr.material = material;
		smr.updateWhenOffscreen = true;
	}
}