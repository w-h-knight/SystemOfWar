using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraManager : MonoBehaviour {

    public Camera MapCamera;
    private int xSlowSensitivity = 50;
    private int xSlowSpeed = 1;
    private int xFastSensitivity = 10;
    private int xFastSpeed = 10;
    private int ySlowSensitivity = 50;
    private int ySlowSpeed = 1;
    private int yFastSensitivity = 10;
    private int yFastSpeed = 10;
    private Vector3 rightDirection = Vector3.right;
    private Vector3 leftDirection = Vector3.left;
    private Vector3 forwardDirection = Vector3.forward;
    private Vector3 backDirection = Vector3.back;
    private GameObject CameraView;
    private RectTransform CameraViewRect;
    // Use this for initialization
    void Start () {
        //create a hexagon so we can use it to help with the camera and minimap math
        //hexagon width (tilesize) is 2*radius (r) and the hexagon constructor takes a radius size not the width
        Hexagon hexagon = new Hexagon(GameController.instance.tileSize / 2);

        //Debug.Log("Hey I got a camera manager");
        Debug.Log("screen width is " + Screen.width);
        MapCamera = Camera.main;
        SetCameraLocation(MapCamera.orthographicSize * MapCamera.aspect, MapCamera.orthographicSize);
        CameraView = GameObject.Find("CameraView");
        CameraViewRect = CameraView.GetComponent<RectTransform>();
        //multiple the height by two because orthographicsize is half the total but divide by 2 because our minimap is half the height of map image
        //then we have to divide by the height of a hex to get size in tiles (which in our case should be size in pixels)
        float minimapHeight = (MapCamera.orthographicSize / hexagon.Height);
        //now the width which should be the same just multiplied by the camera(screen) aspect ratio  note even though hexes are offset each one still counts
        //as one tile horizontally and as one pixel on the minimap
        float minimapWidth = MapCamera.orthographicSize * MapCamera.aspect;
        CameraViewRect.anchoredPosition = new Vector3(10 * MapCamera.aspect, 10, 0);
        //because the transform is scaled by 0.25 (to make the outline smaller) we need to multiply by 4
        //TODO: use an outline image that doesnt require scaling
        CameraViewRect.sizeDelta = new Vector2(minimapWidth*4, minimapHeight*4);
    }

    // Update is called once per frame
    void Update () {
        if (Input.mouseScrollDelta.y != 0.0)
        { DollyCamera(Input.mouseScrollDelta.y); }

        // Check if on the left edge
        if (Input.mousePosition.x > 0 && Input.mousePosition.x <= xSlowSensitivity)
        {
            if (Input.mousePosition.x <= xFastSensitivity)
            { PanCamera(leftDirection * Time.deltaTime * xFastSpeed); }
            else
            { PanCamera(leftDirection * Time.deltaTime * xSlowSpeed); }
        }

        // Check if on the right edge
        if (Input.mousePosition.x < Screen.width && Input.mousePosition.x >= Screen.width - xSlowSensitivity)
        {
            if (Input.mousePosition.x >= Screen.width - xFastSensitivity)
            { PanCamera(rightDirection * Time.deltaTime * xFastSpeed); }
            else
            { PanCamera(rightDirection * Time.deltaTime * xSlowSpeed); }
        }

        // Check if on top edge
        if (Input.mousePosition.x < Screen.height && Input.mousePosition.y >= Screen.height - ySlowSensitivity)
        {
            if (Input.mousePosition.y >= Screen.height - yFastSensitivity)
            { PanCamera(forwardDirection * Time.deltaTime * yFastSpeed); }
            else
            { PanCamera(forwardDirection * Time.deltaTime * ySlowSpeed); }
        }

        // Check if on bottom edge
        if (Input.mousePosition.y > 0 && Input.mousePosition.y <= ySlowSensitivity)
        {
            if (Input.mousePosition.y <= yFastSensitivity)
            { PanCamera(backDirection * Time.deltaTime * yFastSpeed); }
            else
            { PanCamera(backDirection * Time.deltaTime * ySlowSpeed); }
        }
    }

    public void PanCamera(Vector3 panVector)
    {
        //move the camera
        MapCamera.transform.position += panVector;
        //update the minimap
                
    }
    public void DollyCamera(float yOffset)
    {
        MapCamera.orthographicSize = Mathf.Clamp(MapCamera.orthographicSize - yOffset, 1, 100);
    }

    public void SetCameraLocation(float x, float z)
    {
        MapCamera.transform.position = new Vector3(x, MapCamera.transform.position.y, z);
    }

    public void SetCameraElevation(float y)
    {
        MapCamera.transform.position = new Vector3(MapCamera.transform.position.x, y, MapCamera.transform.position.z);
    }
}
