using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
//[ExecuteInEditMode]
public class MapChunk : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuildMesh(int chunk_x, int chunk_y, int chunksize_x, int chunksize_y, GameMap gamemap)
    {
        //the number of tiles is just the chunk width multiplied by the chunk height
        int numTiles = chunksize_x * chunksize_y;
        //for hexagons each tile is made up of 6 triangles so number of triangles
        // is six times the number of tiles
        //              _____ 
        //             /\ 2 /\
        //            / 1\ /3 \
        //           <----+---->
        //            \ 6/ \4 /
        //             \/_5_\/
        //              
        int numTris = numTiles * 6;
        //the number of vertices is dependent on whether we want to have the
        //mesh share the verts on the corners or whether we want to have
        //each hextile have its own verts for texturing..this is entirely
        //up to how the underlying game needs to be implemented. 
        //In this case I have decided to have each tile with it's own set of 
        //vertices...this will mean texturing will be applied per tile 
        //             0_____1 
        //             /\   /\
        //            /  \ /  \
        //          5<---6+---->2
        //            \  / \  /
        //             \/___\/
        //             4     3
        int numVerts = numTiles * 7;

        Vector3[] vertices = new Vector3[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];

        int[] triangles = new int[numTris * 3];

        //create a hexagon so we can use it to help with the vertex math we divide tile size by 2 because
        //hexagon width (tilesize) is 2*radius (r) and the hexagon constructor takes a radius size not the width
        Hexagon hexagon = new Hexagon(GameController.instance.tileSize / 2);

        for (int z = 0; z < chunksize_y; z++)
        {
            //certain values only change when we change z
            //so calculate and cache them here...that way we aren't
            //recalculating them over and over..remember tilesize is baked into hexagon
            float thisheight = z * hexagon.Height;
            float thiscenter = thisheight + hexagon.Center;
            float nextheight = (z + 1) * hexagon.Height;
            float chunkheight = (chunksize_y * hexagon.Height);
            float z_start = (chunk_y * chunkheight) + (((chunk_x * chunksize_x) % 2) * hexagon.Center);
            for (int x = 0; x < chunksize_x; x++)
            {
                //this adjusts the starting point for the hex based on odd or even row
                float rowoffset = (x % 2) * hexagon.Center;
                float chunkwidth = (chunksize_x * hexagon.Width) - (chunksize_x * hexagon.Offset);
                float x_start = chunk_x * chunkwidth;
                //y is always zero for now since we are just using a flat map
                //later we may want to introduce some bumpiness
                vertices[(((z * chunksize_x) + x) * 7) + 0] = new Vector3(x_start + (x * hexagon.Side) + hexagon.Offset, 0, z_start + nextheight + rowoffset);
                vertices[(((z * chunksize_x) + x) * 7) + 1] = new Vector3(x_start + (x * hexagon.Side) + hexagon.Side, 0, z_start + nextheight + rowoffset);
                vertices[(((z * chunksize_x) + x) * 7) + 2] = new Vector3(x_start + (x * hexagon.Side) + hexagon.Width, 0, z_start + thiscenter + rowoffset);
                vertices[(((z * chunksize_x) + x) * 7) + 3] = new Vector3(x_start + (x * hexagon.Side) + hexagon.Side, 0, z_start + thisheight + rowoffset);
                vertices[(((z * chunksize_x) + x) * 7) + 4] = new Vector3(x_start + (x * hexagon.Side) + hexagon.Offset, 0, z_start + thisheight + rowoffset);
                vertices[(((z * chunksize_x) + x) * 7) + 5] = new Vector3(x_start + x * hexagon.Side, 0, z_start + thiscenter + rowoffset);
                vertices[(((z * chunksize_x) + x) * 7) + 6] = new Vector3(x_start + x * hexagon.Side + hexagon.Radius, 0, z_start + thiscenter + rowoffset);

                normals[(((z * chunksize_x) + x) * 7) + 0] = Vector3.up;
                normals[(((z * chunksize_x) + x) * 7) + 1] = Vector3.up;
                normals[(((z * chunksize_x) + x) * 7) + 2] = Vector3.up;
                normals[(((z * chunksize_x) + x) * 7) + 3] = Vector3.up;
                normals[(((z * chunksize_x) + x) * 7) + 4] = Vector3.up;
                normals[(((z * chunksize_x) + x) * 7) + 5] = Vector3.up;
                normals[(((z * chunksize_x) + x) * 7) + 6] = Vector3.up;

                uv[(((z * chunksize_x) + x) * 7) + 0] = gamemap.GetTileUV(x + (chunksize_x * chunk_x), z + (chunksize_y * chunk_y), 0);
                uv[(((z * chunksize_x) + x) * 7) + 1] = gamemap.GetTileUV(x + (chunksize_x * chunk_x), z + (chunksize_y * chunk_y), 1);
                uv[(((z * chunksize_x) + x) * 7) + 2] = gamemap.GetTileUV(x + (chunksize_x * chunk_x), z + (chunksize_y * chunk_y), 2);
                uv[(((z * chunksize_x) + x) * 7) + 3] = gamemap.GetTileUV(x + (chunksize_x * chunk_x), z + (chunksize_y * chunk_y), 3);
                uv[(((z * chunksize_x) + x) * 7) + 4] = gamemap.GetTileUV(x + (chunksize_x * chunk_x), z + (chunksize_y * chunk_y), 4);
                uv[(((z * chunksize_x) + x) * 7) + 5] = gamemap.GetTileUV(x + (chunksize_x * chunk_x), z + (chunksize_y * chunk_y), 5);
                uv[(((z * chunksize_x) + x) * 7) + 6] = gamemap.GetTileUV(x + (chunksize_x * chunk_x), z + (chunksize_y * chunk_y), 6);
            }
        }

        for (int z = 0; z < chunksize_y; z++)
        {
            for (int x = 0; x < chunksize_x; x++)
            {
                int hexIndex = z * chunksize_x + x;
                int triOffset = hexIndex * 18;
                triangles[triOffset + 0] = (((z * chunksize_x) + x) * 7) + 0;
                triangles[triOffset + 1] = (((z * chunksize_x) + x) * 7) + 1;
                triangles[triOffset + 2] = (((z * chunksize_x) + x) * 7) + 6;

                triangles[triOffset + 3] = (((z * chunksize_x) + x) * 7) + 1;
                triangles[triOffset + 4] = (((z * chunksize_x) + x) * 7) + 2;
                triangles[triOffset + 5] = (((z * chunksize_x) + x) * 7) + 6;

                triangles[triOffset + 6] = (((z * chunksize_x) + x) * 7) + 2;
                triangles[triOffset + 7] = (((z * chunksize_x) + x) * 7) + 3;
                triangles[triOffset + 8] = (((z * chunksize_x) + x) * 7) + 6;

                triangles[triOffset + 9] = (((z * chunksize_x) + x) * 7) + 3;
                triangles[triOffset + 10] = (((z * chunksize_x) + x) * 7) + 4;
                triangles[triOffset + 11] = (((z * chunksize_x) + x) * 7) + 6;

                triangles[triOffset + 12] = (((z * chunksize_x) + x) * 7) + 4;
                triangles[triOffset + 13] = (((z * chunksize_x) + x) * 7) + 5;
                triangles[triOffset + 14] = (((z * chunksize_x) + x) * 7) + 6;

                triangles[triOffset + 15] = (((z * chunksize_x) + x) * 7) + 5;
                triangles[triOffset + 16] = (((z * chunksize_x) + x) * 7) + 0;
                triangles[triOffset + 17] = (((z * chunksize_x) + x) * 7) + 6;
            }
        }

        // Create a new Mesh and populate with the data
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        // Assign our mesh to our filter/renderer/collider
        MeshFilter mesh_filter = GetComponent<MeshFilter>();
        mesh_filter.mesh = mesh;
        MeshCollider mesh_collider = GetComponent<MeshCollider>();
        mesh_collider.sharedMesh = mesh;

    }
}
