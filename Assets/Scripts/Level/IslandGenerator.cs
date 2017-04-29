using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour {

    public List<Vector3> block = new List<Vector3>();
    public List<Vector2> blockUV = new List<Vector2>();
    public List<int> blockTriangle = new List<int>();
    public Mesh mesh;

    // Perlin Seed
    [Range(1, 120)]
    public int seed;
    [Range(1, 60)]
    public float scale;

    // collider
    // collision
    public List<Vector3> colVertices = new List<Vector3>();
    public List<int> colTriangles = new List<int>();
    private int colCount;
    private MeshCollider col;

    public byte[,] blocks;
    [Range(1,160)]
    public int blocksX;
    [Range(1, 160)]
    public int blocksY;
    private int blockCount;

    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2(0, 0);
    private Vector2 tDirt = new Vector2(0, 1);
    private Vector2 tGold = new Vector2(2, 2);
    private Vector2 tGrass = new Vector2(0, 2);

    // Use this for initialization
    void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();

        GenerateBlocks();
        BuildMesh();
        UpdateMesh();

        //mesh.vertices = block.ToArray();
        //mesh.triangles = blockTriangle.ToArray();
        //mesh.uv = blockUV.ToArray();
        //mesh.RecalculateNormals();
    }

    void GeneratePoints(int x, int y, Vector2 texture)
    {
        block.Add(new Vector3(x, y, 0));
        block.Add(new Vector3(x + 1, y, 0));
        block.Add(new Vector3(x + 1, y - 1, 0));
        block.Add(new Vector3(x, y - 1, 0));

        blockUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y + tUnit));
        blockUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y + tUnit));
        blockUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y));
        blockUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y));

        blockTriangle.Add(blockCount * 4);
        blockTriangle.Add((blockCount * 4) + 1);
        blockTriangle.Add((blockCount * 4) + 3);
        blockTriangle.Add((blockCount * 4) + 1);
        blockTriangle.Add((blockCount * 4) + 2);
        blockTriangle.Add((blockCount * 4) + 3);

        blockCount++;
    }

    int returnPerlin(int x, int y, int exp)
    {
        return (int)(Mathf.Pow((Mathf.PerlinNoise(x / scale, y / scale) * seed), (exp)));
    }

    //int Noise(int x, int y, float scale)
    //{
    //    return (int)(Mathf.Pow((Mathf.PerlinNoise(seed / x, seed / y) * persistance), (0)));

       // return (int) (Mathf.Pow((Mathf.PerlinNoise(x / scale , y / scale) * mag), (exp)));
    //}

    void GenerateBlocks()
    {
        blocks = new byte[blocksX, blocksY];
        //Debug.Log(returnPerlin(1, 0, 1));
        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            int layerDirt = returnPerlin(x, 0, 1) + seed + 10;
            int layerGrass = returnPerlin(x, 0, 1) + seed + 12;
            int layerStone = returnPerlin(x, 0, 1) + seed;

            for (int y = 0; y < blocks.GetLength(1); y++)
            {      
                if (y < layerStone)
                {
                    // stone
                    blocks[x, y] = 1;

                    if (returnPerlin(x, y * 6, 1) > 19)
                    {
                        blocks[x, y] = 0;
                    }
                    if (returnPerlin(x, y * 4, 1) > 27)
                    {
                        // gold
                        blocks[x, y] = 4;
                    }
                }
                else if(y < layerDirt)
                {
                    // dirt
                    blocks[x, y] = 2;
                }
                else if(y < layerGrass)
                {
                    // grass
                    blocks[x, y] = 3;
                }
            }
        }
    }

    void BuildMesh()
    {
        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                GenerateCollider(x, y);

                // if block isn't air
                if (blocks[x, y] != 0)
                {
                    if (blocks[x, y] == 1)
                    {
                        GeneratePoints(x, y, tStone);
                    }
                    else if(blocks[x,y] == 2)
                    {
                        GeneratePoints(x, y, tDirt);
                    }
                    else if (blocks[x, y] == 3)
                    {
                        GeneratePoints(x, y, tGrass);
                    }
                    else if (blocks[x, y] == 4)
                    {
                        GeneratePoints(x, y, tGold);
                    }
                }
            }
        }
    }

    byte Block(int x, int y)
    {
        if (x == -1 || x == blocks.GetLength(0) || y == -1 || y == blocks.GetLength(1))
        {
            return (byte)1;
        }

        return blocks[x, y];
    }

    void ColliderTriangles()
    {
        colTriangles.Add(colCount * 4);
        colTriangles.Add((colCount * 4) + 1);
        colTriangles.Add((colCount * 4) + 3);
        colTriangles.Add((colCount * 4) + 1);
        colTriangles.Add((colCount * 4) + 2);
        colTriangles.Add((colCount * 4) + 3);
    }

    void GenerateCollider(int x, int y)
    {
        if (Block(x, y + 1) == 0)
        {
            colVertices.Add(new Vector3(x, y, 1));
            colVertices.Add(new Vector3(x + 1, y, 1));
            colVertices.Add(new Vector3(x + 1, y, 0));
            colVertices.Add(new Vector3(x, y, 0));

            ColliderTriangles();

            colCount++;
            Debug.Log("test");

        }

        //bot
        if (Block(x, y - 1) == 0)
        {
            colVertices.Add(new Vector3(x, y - 1, 0));
            colVertices.Add(new Vector3(x + 1, y - 1, 0));
            colVertices.Add(new Vector3(x + 1, y - 1, 1));
            colVertices.Add(new Vector3(x, y - 1, 1));

            ColliderTriangles();
            colCount++;
        }

        //left
        if (Block(x - 1, y) == 0)
        {
            colVertices.Add(new Vector3(x, y - 1, 1));
            colVertices.Add(new Vector3(x, y, 1));
            colVertices.Add(new Vector3(x, y, 0));
            colVertices.Add(new Vector3(x, y - 1, 0));

            ColliderTriangles();

            colCount++;
        }

        //right
        if (Block(x + 1, y) == 0)
        {
            colVertices.Add(new Vector3(x + 1, y, 1));
            colVertices.Add(new Vector3(x + 1, y - 1, 1));
            colVertices.Add(new Vector3(x + 1, y - 1, 0));
            colVertices.Add(new Vector3(x + 1, y, 0));

            ColliderTriangles();

            colCount++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = block.ToArray();
        mesh.triangles = blockTriangle.ToArray();
        mesh.uv = blockUV.ToArray();
        mesh.RecalculateNormals();

        blockCount = 0;
        block.Clear();
        blockTriangle.Clear();
        blockUV.Clear();

        Mesh newMesh = new Mesh();
        newMesh.vertices = colVertices.ToArray();
        newMesh.triangles = colTriangles.ToArray();
        col.sharedMesh = newMesh;

        colVertices.Clear();
        colTriangles.Clear();
        colCount = 0;
    }

    // Update is called once per frame
    void Update() {
        BuildMesh();
        UpdateMesh();
       // transform.position = new Vector3(0, Mathf.PerlinNoise(Time.time, 0) * 3 + 1, 0);
    }
}
