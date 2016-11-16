using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class GameController : MonoBehaviour {

    public static GameController instance = null;
    public int TurnNumber = 1;
    //the size of the tile in world coordinates
    public float tileSize = 1.0f;
    public MouseManager mouseManager;
    public CameraManager cameraManager;
    public MapLoader mapLoader;
    public GameMap gameMap;
	// Use this for initialization
	void Awake () {

        if(instance==null)
        { instance = this; }
        else if(instance != this)
        { DestroyObject(gameObject); }
        mouseManager = GetComponent<MouseManager>();
        cameraManager = GetComponent<CameraManager>();
        mapLoader = GetComponent<MapLoader>();
        mapLoader.LoadMap();
        gameMap = Instantiate<GameMap>(gameMap);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
