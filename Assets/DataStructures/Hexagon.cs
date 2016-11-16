using System;
public class Hexagon {

    const float SQRT3 = 1.732050807568877f;

    public float Radius;    //R
    public float Height;    //H
    public float Width;     //W
    public float Side;      //S
    public float Offset;    //O
    public float Center;    //C

    //for hexagons here is some math concerning certain sizes
    //           |-----S----|-O-|  
    //    _          _______    
    //    |         /\  R  /\   
    //    |        /  \   /  \  
    //    |       /  R \ /  R \ 
    //   H|      <------+------> -+-
    //    |       \    / \    /   |
    //    |        \  /   \  /    C
    //    |_        \/_____\/    _|_
    //           |------W------|   


    //I am going to comment this out for now...this represents a true hexagon where the distances
    //would mathematically be exact...the problem comes when you try and use this to do texturing
    //if you define your textures as say 128x128 then the hexagon laid out below wont't work as
    //the Width which is 2*r can never match the Height which is Sqrt(3)*r... you can still
    //build your texture as 128x128 but when you stretch it to fit the UV coordinates 
    public Hexagon(float pRadius)
    {
        Radius = pRadius;
        Width = 2.0f * Radius;
        Side = 1.5f * Radius;
        Height = (float)Math.Sqrt(3.0d) * Radius;
        Center = Height / 2.0f;
        Offset = 0.5f * Radius;
    }
}
