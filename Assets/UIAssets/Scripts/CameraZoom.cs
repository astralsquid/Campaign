﻿/*
 * CameraScript manages user input for the camera, 
 * and adjusts orthographic size and other variables
 * depending on resolution
 * */
using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {
	//level controller and script
	GameObject levelController;
	GameController levelControllerScript;

	//number of rows and columns in the level grid
	private int mapRows;
	private int mapCols;

	//camera dimensions
	private float camHeight;
	private float camWidth;
	//the actual camera
	public Camera cam;

	//cuts camera semsitivity, should be adjustable in the future
	public float originalCameraSpeedCutBy = 10f;
	public float cameraSpeedCutBy;

	//pixels per unit and scale, needed for camera 
	public float ppuScale = 1f;
	public int ppu = 32;

	//camera zoom boundaries
	private float cameraCurrentZoom;
	private float cameraZoomMax;
	public float cameraZoomMin = 0;
	//used to view currentCameraZoom without changing it
	public float czm;

	//last position of cursor, used for panning the map
	private Vector3 lastPosition;


	void Start ()
	{
		//connect with level controller and script
		levelController = GameObject.Find ("GameController");
		levelControllerScript = levelController.GetComponent<GameController> ();

		//find the rows and columns in of the level grid
		mapRows = levelControllerScript.GetMapRows();
		mapCols = levelControllerScript.GetMapCols();

		//calculate maximum camera zoom //Orthographic size = ((Vert Resolution)/(PPUScale * PPU)) * 0.5
		cameraCurrentZoom = ((Screen.height)/(ppuScale * 32))*0.5000f;
		cameraZoomMax = cameraCurrentZoom*4;
		Camera.main.orthographicSize = cameraCurrentZoom;

		//make the camera less sensitive as we zoom in
		cameraSpeedCutBy = originalCameraSpeedCutBy * (cameraCurrentZoom / cameraZoomMax);

		//czm is for viewing only
		czm = cameraCurrentZoom;

		//get the dimensions of the camera (screen resolution), save values in max variables
		camHeight = 2f * cam.orthographicSize;
		camWidth = camHeight * cam.aspect;
	}

	void LateUpdate()
	{
		//make sure camera height and with are always accurate
		camHeight = 2f * cam.orthographicSize;
		camWidth = camHeight * cam.aspect;

		czm = cameraCurrentZoom;

		//clamps camera position so we don't go off the board 
		Vector3 v3 = transform.position;
		//v3.x = Mathf.Clamp(v3.x, - (mapCols / 2) + camWidth / 2 , (mapCols / 2) - camWidth / 2 );
		//v3.y = Mathf.Clamp(v3.y, - (mapRows / 2) + camHeight / 2 , (mapRows / 2) - camHeight / 2 );
		v3.x = Mathf.Clamp(v3.x, - (mapCols / 2), (mapCols / 2));
		v3.y = Mathf.Clamp(v3.y, - (mapRows / 2), (mapRows / 2));
		transform.position = v3;

		//camera zooming
		if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
		{
			if (cameraCurrentZoom < cameraZoomMax)
			{
				cameraCurrentZoom += 1;
				Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize + 1);
			}
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
		{
				if (cameraCurrentZoom > 2)
			{
				cameraCurrentZoom -= 1;
				Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize - 1);
			}
		}
		//ensure camera sensitivity adjusts for zoom level
		cameraSpeedCutBy = (1f - ((cameraCurrentZoom) / cameraZoomMax));
	}

	void Update()
	{
		float xAxisValue = Input.GetAxis("Horizontal");
		float yAxisValue = Input.GetAxis("Vertical");
		if(Camera.current != null)
		{
			Camera.current.transform.Translate(new Vector3(xAxisValue, yAxisValue, 0.0f));
		}
	}
}

