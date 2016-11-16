using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider))]
//[ExecuteInEditMode]
public class GameMap : MonoBehaviour {

    public Material textureAtlasMaterial;
    public MapChunk chunkprefab;

    private MapChunk[,] gamemapchunks;
    private HexMap gamemap;
    //these two int indicate size of the map in hexes
    private int mapsize_x = 512;
    private int mapsize_y = 288;
    //since unity can't handle more than 65535 verts in a single mesh we will need to chunk up the map
    //so...lets set chunk size...Be careful with the values you select here!!!
    //TDO: there are certain things I am not dealing with correctly at the moment...
    //      certain odd int values create visual artifacts
    //      if you create chunks that dont divide evenly into the map size
    //      if you care chunks larger than the map size
    private int chunksize_x = 128;
    private int chunksize_y = 72;
    //assuming square tiles here..if not true then we need x and y resolution
    private int tileResolution = 128;
    //data structure to hold UVs for tile atlas
    private Vector2[,] tileuvs;

	// Use this for initialization
	void Start () {
        //TODO: eventually we probably want to not pull in entire map...or maybe we do if its only ints...
        //depends a lot on whether we end up making each tile a full object...if so than loading could be
        //costly in terms of time and/or memory
        //tiles go from left to right horizontally and from the bottom up vertically (0,0) is lower left corner
        gamemap = new HexMap(mapsize_x, mapsize_y);
        ////just setting some tiles for testing
        //gamemap.SetTile(0, 0, 1);
        //gamemap.SetTile(1, 0, 1);
        //gamemap.SetTile(0, 1, 1);
        //gamemap.SetTile(1, 1, 1);
        //gamemap.SetTile(1, 2, 1);
        //gamemap.SetTile(2, 0, 1);
        ////gamemap.SetTile(0, 1, 2);
        ////gamemap.SetTile(1, 1, 3);
        //gamemap.SetTile(5, 5, 3);
        //gamemap.SetTile(5, 10, 2);
        //gamemap.SetTile(16, 8, 2);
        int[,] maptiles = GameController.instance.mapLoader.GetMapTiles();
        gamemap.SetTiles(maptiles);

        //build and fill the tileUV data structure for our texture atlas
        tileuvs = buildTileUVs();

        //meshes can be a max size of 65535 verts
        //so doing a little math 65535/7 ~ 9362 which means chunksize_x * chunksize_y cant be greater than 9362
        //check and throw an error if so
        if(chunksize_x* chunksize_y > 9362)
        {
            throw new System.Exception("chunksize too big");
        }
        //TODO: eventually we probably will want to build out chunks just around where player is viewing
        //dynamically adding and removing meshes as we scroll around...but for now this is sufficient
        //next we need to build out meshes based on the chunk size...so iterate through
        //chunks building meshes for each as we go first get number of chunks
        int numchunks_x = mapsize_x / chunksize_x;
        int numchunks_y = mapsize_y / chunksize_y;
        //allocate array of mapchunks
        gamemapchunks = new MapChunk[numchunks_x, numchunks_y];
        for (int y = 0; y < numchunks_y; y++)
        {
            for (int x = 0; x < numchunks_x; x++)
            {
                MapChunk chunk = Instantiate(chunkprefab);
                chunk.name = "GameMapChunk[" + x + ", " + y + "]";
                chunk.BuildMesh(x, y, chunksize_x, chunksize_y, this);
                gamemapchunks[x, y] = chunk;
                chunk.transform.SetParent(this.transform);
            }
        }
	
	}

    public Vector2 GetTileUV(int x, int z, int vertex)
    {
        return tileuvs[gamemap.GetTile(x, z), vertex];
    }

    public Vector2[,] buildTileUVs()
    {
        if(textureAtlasMaterial==null)
        { throw new System.Exception("TextureAtlasMaterial can't be null"); }
        //pull all the properties of the tile_atlas texture from the material
        int atlas_width = textureAtlasMaterial.mainTexture.width;
        int num_tiles_wide = atlas_width / tileResolution;

        int atlas_height = textureAtlasMaterial.mainTexture.height;
        int num_tiles_high = atlas_height / tileResolution;
        //next we can build our data structure of UV values to cache each tiles UV coordinates
        //we have 7 because there are 7 UV coordinates for each hex
        Vector2[,] uv = new Vector2[num_tiles_wide * num_tiles_high, 7];
        //build a set of uv values for each tile ID ...example of a 2x2 atlas
        //      uv0,1                 0_____1  uv 1/num_tiles_wide,1  uv 1/num_tiles_wide,1  0_____1  uv 1,1     
        //                            /\   /\                                                /\   /\
        //                           /  \ /  \                                              /  \ /  \
        //                         5<---6+---->2    .................................     5<---6+---->2
        //                           \  / \  /                                              \  / \  /
        //                            \/___\/                                                \/___\/
        //                            4     3                                                4     3
        //                               :                                                      :
        //                               :                                                      :
        //      uv0,1/num_tiles_high     :    uv 1/num_tiles_wide,    uv 1/num_tiles_wide,      :    uv 1,1/num_tiles_high
        //                               :       1/num_tiles_high        1/num_tiles_high       :
        //      uv0,1/num_tiles_high  0_____1 uv 1/num_tiles_wide,    uv 1/num_tiles_wide,   0_____1 uv 1,1/num_tiles_high
        //                            /\   /\    1/num_tiles_high        1/num_tiles_high    /\   /\   
        //                           /  \ /  \                                              /  \ /  \
        //                         5<---6+---->2   .................................      5<---6+---->2
        //                           \  / \  /                                              \  / \  /
        //                            \/___\/                                                \/___\/
        //                            4     3                                                4     3
        //      uv0,0                         uv 1/num_tiles_wide,0   uv 1/num_tiles_wide,0          uv 1,0

        //create a hexagon for uv mapping
        //the UV mapping for a texture runs from 0 to 1 so let's build a hexagon that is 1.0 wide and 1.0 high and then scale the 
        //values to match the width and height of the texture atlas
        //to get a width of 1 we need to pass in a radius of 0.5 because W = 2R (see hexagon class)
        Hexagon uvhex = new Hexagon(0.5f);
        //now set scale factor for x and y (this is the size of each tile in UV coordinates)
        float tileXscale = (1.0f / num_tiles_wide);
        float tileYscale = (1.0f / num_tiles_high);
        //now scale all the hex values both in the x and y direction based on tiles in atlas
        uvhex.Offset *= tileXscale;
        uvhex.Side *= tileXscale;
        uvhex.Width *= tileXscale;
        uvhex.Radius *= tileXscale;
        uvhex.Height *= tileYscale;
        uvhex.Center *= tileYscale;
        //now we have the uv coordinates for a single hex tile we can iterate through the texture properties setting UV
        //coordinates for each tile
        for (int y = 0; y < num_tiles_high; y++)
        {
            //precompute this offset for Y iterator
            float tileYoffset = y * tileYscale;
            for (int x = 0; x < num_tiles_wide; x++)
            {
                //precomput this offset for X iterator
                float tileXoffset = x * tileXscale;
                uv[(y * num_tiles_wide) + x, 0] = new Vector2(tileXoffset + uvhex.Offset, tileYoffset + uvhex.Height);
                uv[(y * num_tiles_wide) + x, 1] = new Vector2(tileXoffset + uvhex.Side, tileYoffset + uvhex.Height);
                uv[(y * num_tiles_wide) + x, 2] = new Vector2(tileXoffset + uvhex.Width, tileYoffset + uvhex.Center);
                uv[(y * num_tiles_wide) + x, 3] = new Vector2(tileXoffset + uvhex.Side, tileYoffset);
                uv[(y * num_tiles_wide) + x, 4] = new Vector2(tileXoffset + uvhex.Offset, tileYoffset);
                uv[(y * num_tiles_wide) + x, 5] = new Vector2(tileXoffset, tileYoffset + uvhex.Center);
                uv[(y * num_tiles_wide) + x, 6] = new Vector2(tileXoffset + uvhex.Radius, tileYoffset + uvhex.Center);
            }
        }
        //when done we can use this data structure to set UV coordinates for any tile just by indexing into this datastructure by tile ID
        return uv;
    }

}
