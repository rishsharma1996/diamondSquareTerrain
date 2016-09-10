﻿using UnityEngine;
using System.Collections;

public class diamondSquare : MonoBehaviour {

	public int detail;
	public int snowHeight;
	public int grassHeight;
	public int dirtHeight;


	private const int GRASS = 0;
	private const int DIRT = 1;
	private const int SNOW = 2;


	Terrain currentTerrain;
	TerrainData tData;
	Vector3 terrainSize;
	Material material;
	int max;
	int size;

	// Use this for initialization
	void Start () {

		currentTerrain = Terrain.activeTerrain;
		tData = currentTerrain.terrainData;
		terrainSize = tData.size;
		material = Resources.Load ("landscape") as Material;

		InitialiseTerrain (detail);

		float[,] data = PopulateDataArray ();
		data = PopulateDataArray ();
		generateTerrain (data);
		addTextures ();



	}


	// Update is called once per frame
	void Update () {


	}

	private void InitialiseTerrain(int detail) {
		this.size = (int) Mathf.Pow(2,detail)+1;
		this.max = this.size - 1;
	}


	private void addTextures() {

		float[,,] alphaData = tData.GetAlphamaps (0, 0, tData.alphamapWidth, tData.alphamapHeight);
		Debug.Log (tData.alphamapHeight);
		Debug.Log (tData.alphamapWidth);
		for (int y = 0; y < tData.alphamapHeight; y++) {

			for (int x = 0; x < tData.alphamapWidth; x++) {

				float height = currentTerrain.SampleHeight (new Vector3 (y,0 , x));

				if (height > snowHeight) {
					alphaData [x, y, SNOW] = 1;
					alphaData [x, y, DIRT] = 0;
					alphaData [x, y, GRASS] = 0;

				} else if (height > grassHeight) {
					alphaData [x, y, GRASS] = 1;
					alphaData [x, y, DIRT] = 0;
					alphaData [x, y, SNOW] = 0;
				
				} else {
					alphaData [x, y, DIRT] = 1;
					alphaData [x, y, GRASS] = 0;
					alphaData [x, y, SNOW] = 0;
				}
			}
		}
		tData.SetAlphamaps (0, 0, alphaData);

	}

	private float [,] PopulateDataArray() {


		float[,] data = new float[size, size];
		float val, rnd;
		float h = 0.5f;

		int x, y, sideLength, halfSide = 0;


		// set the four corner points to inital values
		data [0,0] = 0;
		data [max,0] = 0;
		data [0,max] = 0;
		data [max,max] = 0;


		for (sideLength = max; sideLength >= 2; sideLength /= 2) {

			halfSide = sideLength / 2;


			for (x = 0; x < max; x += sideLength) {


				for (y = 0; y < max; y += sideLength) {


					val = data [x, y];
					val += data [x + sideLength, y];
					val += data [x, y + sideLength];
					val += data [x + sideLength, y + sideLength];

					val /= 4.0f;
					rnd = (Random.value * 2.0f * h) - h;
					val = Mathf.Clamp01 (val + rnd);

					data [x + halfSide, y + halfSide] = val;

				}

			}



			for (x = 0; x < max; x += halfSide) {


				for (y = (x + halfSide) % sideLength; y < max; y += sideLength) {

					val = data [(x - halfSide + max) % (max), y];
					val += data [(x + halfSide) % (max), y];
					val += data [x, (y + halfSide) % (max)];
					val += data [x, (y - halfSide + max) % (max)];

					val /= 4.0f;
					rnd = (Random.value * 2.0f * h) - h;
					val = Mathf.Clamp01 (val + rnd);

					data [x, y] = val;

					if (x == 0)
						data [max, y] = val;

					if (y == 0)
						data [x, max] = val;
				}

			}
			h /= 2.0f;
		}
		return data;

	}




	private void generateTerrain(float [,] data) {

		if (!currentTerrain) {
			return;
		}

		if (tData.heightmapResolution != size) {
			tData.heightmapResolution = size;
		}

		tData.SetHeights (0, 0, data);

	}




}
