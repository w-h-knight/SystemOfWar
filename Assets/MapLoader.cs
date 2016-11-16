using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MapLoader : MonoBehaviour {

    public Texture2D mapImage;
    //private static MapLoader _instance;
    private List<Color32> _mapkey = new List<Color32>
    { new Color32(0, 0, 0, 255),        //unknown or blank
      new Color32(255, 255, 255, 255),  //ice
      new Color32(0, 0, 96, 255),       //deep ocean
      new Color32(153, 217, 234, 255),  //shallow ocean
      new Color32(34, 177, 76, 255),    //grassland
      new Color32(73, 114, 71, 255),    //forest
      new Color32(127, 127, 127, 255),  //mountains
      new Color32(185, 122, 87, 255),   //hills
      new Color32(0, 162, 232, 255),    //fresh water
      new Color32(239, 228, 176, 255),  //plains
      new Color32(0, 160, 128, 255),    //swamp
      new Color32(255, 0, 0, 255)       //test
    };

    private int[,] _maptiles;

    public void LoadMap ()
    {
        Debug.Log("maploader started");
        if (mapImage == null)
        { throw new System.Exception("map image cannot be null"); }
        Color32[] _pixels = mapImage.GetPixels32(0);

        _maptiles = new int[mapImage.width, mapImage.height];
        for (int y = 0; y < mapImage.height; y++)
        {
            for (int x = 0; x < mapImage.width; x++)
            {
                Color32 pixel = _pixels[x + y * mapImage.width];
                int tilekey = _mapkey.FindIndex(obj => obj.Equals(pixel));
                if (tilekey == -1) { tilekey++; }  //make unfound tiles the same as unknown
                _maptiles[x, y] = tilekey;
            }
        }
    }

    public int[,] GetMapTiles()
    {
        return _maptiles;
    }


}
