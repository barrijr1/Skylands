using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PolygonGenerator : MonoBehaviour
{
    // mesh
    public List<Vector3> newVertices = new List<Vector3>();
    public List<int> newTriangles = new List<int>();
    public List<Vector2> newUV = new List<Vector2>();
    public Mesh mesh;
    public byte[,] blocks;

    //textures
    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2(0, 0);
    private Vector2 tDirt = new Vector2(0, 1);
    private Vector2 tGold = new Vector2(2, 2);
    private Vector2 tGrass = new Vector2(0, 2);

    //block count
    private int squareCount;
    public int blocksX = 125;
    public int blocksY = 400;

    // terrain type cluster count
    private int tStoneClu;
    private int tDirtClu;
    private int tGoldClu;
    private int tAirClu;

    // world size
    public GameObject island;

    // collision
    public List<Vector3> colVertices = new List<Vector3>();
    public List<int> colTriangles = new List<int>();
    private int colCount;
    private MeshCollider col;

    // misc variables
    public int tileShape = 4;
    public bool update = false;

    int MAXDEBUG = 100;

    // Use this for initialization
    void Start ()
    {

        tStoneClu = Random.Range(2, 4);
        tDirtClu = Random.Range(2, 4);
        tGoldClu = Random.Range(1, 3);
        tAirClu = Random.Range(1, 2);

        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();
        GenerateTerrain();
        BuildMesh();
        UpdateMesh();

    }


    void GenerateSquare(int x, int y)
    {

        newVertices.Add(new Vector3(x, y, 0));
        newVertices.Add(new Vector3(x + 1, y, 0));
        newVertices.Add(new Vector3(x + 1, y - 1, 0));
        newVertices.Add(new Vector3(x, y - 1, 0));

        newTriangles.Add(squareCount * tileShape);
        newTriangles.Add((squareCount * tileShape) + 1);
        newTriangles.Add((squareCount * tileShape) + 3);
        newTriangles.Add((squareCount * tileShape) + 1);
        newTriangles.Add((squareCount * tileShape) + 2);
        newTriangles.Add((squareCount * tileShape) + 3);

        squareCount++;
    }

    void GenerateTexture(Vector2 texture)
    {
        newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y + tUnit));
        newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y + tUnit));
        newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y));
        newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y));
    }

    int Noise(int x, int y, float scale, float mag, float exp)
    {
        return (int)(Mathf.Pow((Mathf.PerlinNoise(x / scale , y / scale) * mag), (exp)));
    }

    void GenerateTerrain()
    {
        blocks = new byte[blocksX, blocksY];

        for (int px = 0; px < blocks.GetLength(0); px++)
        {
            int grass = Noise(px, 0, 15, 2, 1) + 100;
            int stone = Noise(px, 0, Random.Range(12, 18), Random.Range(12, 18), 1) + 90;


            for (int py = 0; py < blocks.GetLength(1); py++)
            {
                if (py < stone)
                {
                    blocks[px, py] = 1;

                    //if(py > blocks.GetLength(1) - Random.Range(10, 16) && (py > blocks.GetLength(1) - Random.Range(10, 16))

                    if (Noise(px, py * tDirtClu, 12, 16, 1) > 10)
                    { // random dirt in the stone
                        blocks[px, py] = 2;
                    }

                    if (Noise(px, py * tAirClu, 16, 14, 1) > 10)
                    { // random caves in the stone
                        blocks[px, py] = 0;
                    }

                    if (Noise(px, py * tGoldClu, 8, 13, 1) > 10)
                    { // random gold in the stone
                        blocks[px, py] = 3;
                    }

                    if (py < Random.Range(10, 60) && px < Random.Range(blocksX / 2, blocksX / 2 + 20))
                    {
                        blocks[px, py] = 0;
                    }

                    if (py < Random.Range(10, 60) && px < blocks.GetLength(1) - Random.Range(blocksX / 2, blocksX / 2 + 20))
                    {
                        blocks[px, py] = 0;
                    }

                    if (py < Random.Range(1, blocks.GetLength(0)) && px < 5)
                    {
                        blocks[px, py] = 0;
                    }

                    if (py < Random.Range(1, blocks.GetLength(0)) && px > (blocks.GetLength(1) - 20))
                    {
                        blocks[px, py] = 0;
                    }

                }
                else if (py < grass)
                {
                    blocks[px, py] = 4;

                    if (Noise(px, py * tStoneClu, 11, 14, 1) > 10)
                    { // random dirt in the stone
                        blocks[px, py] = 2;
                    }
                }
            }
        }
    }

    void BuildMesh()
    {
        for (int px = 0; px < blocks.GetLength(0); px++)
        {
            for (int py = 0; py < blocks.GetLength(1); py++)
            {
                // if block isn't air
                if (blocks[px, py] != 0)
                {
                    GenerateCollider(px, py);

                    if (blocks[px, py] == 1)
                    {
                        GenerateSquare(px, py);
                        GenerateTexture(tStone);
                    }
                    else if (blocks[px, py] == 2)
                    {
                        GenerateSquare(px, py);
                        GenerateTexture(tDirt);
                    }
                    else if (blocks[px, py] == 3)
                    {
                        GenerateSquare(px, py);
                        GenerateTexture(tGold);
                    }
                    else if (blocks[px, py] == 4)
                    {
                        GenerateSquare(px, py);
                        GenerateTexture(tGrass);
                    }
                }
            }
        }
    }

    byte Block(int x, int y)
    {
        if(x == -1 || x == blocks.GetLength(0) || y == - 1 || y == blocks.GetLength(1))
        {
            return (byte)1;
        }

        return blocks[x, y];
    }

    void ColliderTriangles()
    {
        colTriangles.Add(colCount * tileShape);
        colTriangles.Add((colCount * tileShape) + 1);
        colTriangles.Add((colCount * tileShape) + 3);
        colTriangles.Add((colCount * tileShape) + 1);
        colTriangles.Add((colCount * tileShape) + 2);
        colTriangles.Add((colCount * tileShape) + 3);
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
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.RecalculateNormals();

        squareCount = 0;
        newVertices.Clear();
        newTriangles.Clear();
        newUV.Clear();


        Mesh newMesh = new Mesh();
        newMesh.vertices = colVertices.ToArray();
        newMesh.triangles = colTriangles.ToArray();
        col.sharedMesh = newMesh;

        colVertices.Clear();
        colTriangles.Clear();
        colCount = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        BuildMesh();
        UpdateMesh();
    }
}
