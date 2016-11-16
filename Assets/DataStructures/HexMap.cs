using UnityEngine;
using System;

public class HexMap {

    public int Width;
    public int Height;
    int[,] _tiles;

    public HexMap(int pWidth, int pHeight)
    {
        Width = pWidth;
        Height = pHeight;
        _tiles = new int[pWidth, pHeight];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _tiles[x,y] = 0;
            }
        }
    }

    public void SetTiles(int[,] tiles)
    {
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);
        //Debug.Log("Texwidth=" + width + ", Texheight=" + height);
        //Debug.Log("MapWidth=" + Width + ", Mapheight=" + Height);
        ////for (int y = 0; y < Height; y++)
        //for (int y = 0; y < 1; y++)
        //{
        //    //for (int x = 0; x < Width; x++)
        //    //{
        //    //    _tiles[x, y] = tiles[x, y];
        //    //}
        //    //Debug.Log("y*width=" + y*width + ", y=" + y);
        //    Array.Copy(tiles, 0, _tiles, 0, 72);
        //}
        int fillWidth = Width;
        if(width < Width)
        { fillWidth = width; }
        int fillHeight = Height;
        if(height<Height)
        { fillHeight = height; }
        for (int x = 0; x < fillWidth; x++)
        {
            Array.Copy(tiles, x * height, _tiles, x * Height, fillHeight);
        }
        //_tiles[0, 0] = 0;
        //_tiles[127, 0] = 0;
    }
    public void SetTile(int pX, int pY, int pTileID)
    {
        _tiles[pX, pY] = pTileID;
    }

    public int GetTile(int pX, int pY)
    {
        return _tiles[pX, pY];
    }

}
