using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Linq;

public class GenerateIslands : MonoBehaviour {


    // CENTRE ISLAND
    private List<Vector3> centerIslandV = new List<Vector3>();
    private List<Vector2> centerIslandUV = new List<Vector2>();
    private List<int> centerIslandTriangle = new List<int>();

    // ISLANDS
    public GameObject[] islands;
    public GameObject islandClone;
    [Range(20,140)]
    public float radius;
    [Range(0,1)]
    public float offsetIslandsX;
    [Range(0, 1)]
    public float offsetIslandsY;
    private Vector3 center;
    private float minRadius;
    private float strength = 0.5f;
    private GameObject currentIsland;

    // TERRAIN
    private List<Vector3> currIslandV = new List<Vector3>();
    private List<Vector2> currIslandUV = new List<Vector2>();
    private List<int> currIslandTriangle = new List<int>();
    public Mesh currIslandMesh;

    // TERRAIN COLLIDERS
    private List<Vector2> currIslandColVertices = new List<Vector2>();
    private List<int> currIslandColTriangles = new List<int>();
    private int currIslandColCount;
    private PolygonCollider2D currIslandCol;
    private int polyPathCount;
    private PolygonCollider2D polyPoint;
    private List<Vector2> polyPaths = new List<Vector2>();

    private byte[,] blocks;
    private int blocksX;
    private int blocksY;
    private int blockCount;
    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2(0, 0);
    private Vector2 tDirt = new Vector2(0, 1);
    private Vector2 tGold = new Vector2(2, 2);
    private Vector2 tGrass = new Vector2(0, 2);

    void GeneratePoints(GameObject currIsland, int x, int y, Vector2 texture)
    {


        currIslandV.Add(new Vector3(x, y, 0));
        currIslandV.Add(new Vector3(x + 1, y, 0));
        currIslandV.Add(new Vector3(x + 1, y - 1, 0));
        currIslandV.Add(new Vector3(x, y - 1, 0));

        //polyPathCount++;

        currIslandUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y + tUnit));
        currIslandUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y + tUnit));
        currIslandUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y));
        currIslandUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y));

        currIslandTriangle.Add(blockCount * 4);
        currIslandTriangle.Add((blockCount * 4) + 1);
        currIslandTriangle.Add((blockCount * 4) + 3);
        currIslandTriangle.Add((blockCount * 4) + 1);
        currIslandTriangle.Add((blockCount * 4) + 2);
        currIslandTriangle.Add((blockCount * 4) + 3);

        blockCount++;
    }

    int returnPerlin(int x, int y, int exp)
    {
        // fix this
        return (int)(Mathf.Pow((Mathf.PerlinNoise(x / 2, y / 2) * 6), (exp)));
    }


    void GenerateBlocks(GameObject currIsland, float scale)
    {
        blocks = new byte[(int)scale, (int)scale];

        //Debug.Log((int)currIsland.transform.localScale.x);

        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            int layerDirt = returnPerlin(x, 0, 1)  + 10;
            int layerGrass = returnPerlin(x, 0, 1)  + 12;
            int layerStone = returnPerlin(x, 0, 1) ;

            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                if (y < layerStone)
                {
                    // stone
                    blocks[x, y] = 1;

                    if (returnPerlin(x, y * 6, 1) > 2)
                    {
                        blocks[x, y] = 0;
                    }
                    if (returnPerlin(x, y * 4, 1) > 3)
                    {
                        // gold
                        blocks[x, y] = 4;
                    }
                }
                else if (y < layerDirt)
                {
                    // dirt
                    blocks[x, y] = 2;
                }
                else if (y < layerGrass)
                {
                    // grass
                    blocks[x, y] = 3;
                }
            }
        }
    }

    void BuildMesh(GameObject currIsland)
    {

        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                //GenerateCollider(currIsland, x, y);
                //Vector2[] eachPolyPoint = polyPaths.ToArray();
                //polyPoint.SetPath(polyPathCount, eachPolyPoint);
                //polyPathCount++;

                // if block isn't air
                if (blocks[x, y] != 0)
                {


                    //polyPaths.Clear();

                    if (blocks[x, y] == 1)
                    {
                        GeneratePoints(currIsland, x, y, tStone);
                    }
                    else if (blocks[x, y] == 2)
                    {
                        GeneratePoints(currIsland, x, y, tDirt);
                    }
                    else if (blocks[x, y] == 3)
                    {
                        GeneratePoints(currIsland, x, y, tGrass);
                    }
                    else if (blocks[x, y] == 4)
                    {
                        GeneratePoints(currIsland, x, y, tGold);
                    }
                }
            }
        }
    }

    byte Block(GameObject currIsland, int x, int y)
    {
        if (x == -1 || x == blocks.GetLength(0) || y == -1 || y == blocks.GetLength(1))
        {
            return (byte)1;
        }

        return blocks[x, y];
    }

    //void ColliderTriangles(GameObject currIsland)
    //{
    //    currIslandColTriangles.Add(currIslandColCount * 4);
    //    currIslandColTriangles.Add((currIslandColCount * 4) + 1);
    //    currIslandColTriangles.Add((currIslandColCount * 4) + 3);
    //    currIslandColTriangles.Add((currIslandColCount * 4) + 1);
    //    currIslandColTriangles.Add((currIslandColCount * 4) + 2);
    //    currIslandColTriangles.Add((currIslandColCount * 4) + 3);
    //}

    void GenerateCollider(GameObject currIsland, int x, int y)
    {
        Debug.Log("test4124");
        polyPaths.Add(new Vector2(x, y));
        polyPaths.Add(new Vector2(x + 1, y));
        polyPaths.Add(new Vector2(x + 1, y));
        polyPaths.Add(new Vector2(x, y));

        if (Block(currIsland, x, y + 1) == 0)
        {
            //polyPaths[0] = new Vector2(x, y);
            //polyPaths[1] = new Vector2(x + 1, y);
            //polyPaths[2] = new Vector2(x + 1, y);
            //polyPaths[3] = new Vector2(x, y);

            //polyPoint.SetPath(currIslandColCount, polyPaths);
            Debug.Log("test11");
            polyPaths.Add(new Vector2(x, y));
            polyPaths.Add(new Vector2(x + 1, y));
            polyPaths.Add(new Vector2(x + 1, y));
            polyPaths.Add(new Vector2(x, y));


            //ColliderTriangles(currIsland);

            currIslandColCount++;
            Debug.Log("test");
        }

        //bot
        if (Block(currIsland, x, y - 1) == 0)
        {

            //polyPaths[0] = new Vector2(x, y - 1);
            // polyPaths[1] = new Vector2(x + 1, y - 1);
            //polyPaths[2] = new Vector2(x + 1, y - 1);
            // polyPaths[3] = new Vector2(x, y - 1);

            //polyPoint.SetPath(currIslandColCount, polyPaths);

            polyPaths.Add(new Vector2(x, y - 1));
            polyPaths.Add(new Vector2(x + 1, y - 1));
            polyPaths.Add(new Vector2(x + 1, y - 1));
            polyPaths.Add(new Vector2(x, y - 1));

            //ColliderTriangles(currIsland);

            currIslandColCount++;
            Debug.Log("test2");
        }

        //left
        if (Block(currIsland, x - 1, y) == 0)
        {
            //polyPaths[0] = new Vector2(x, y - 1);
            //polyPaths[1] = new Vector2(x, y);
            //polyPaths[2] = new Vector2(x, y);
            //polyPaths[3] = new Vector2(x, y - 1);

            //polyPoint.SetPath(currIslandColCount, polyPaths);

            polyPaths.Add(new Vector2(x, y - 1));
            polyPaths.Add(new Vector2(x, y));
            polyPaths.Add(new Vector2(x, y));
            polyPaths.Add(new Vector2(x, y - 1));

            // ColliderTriangles(currIsland);

            currIslandColCount++;
            Debug.Log("test3");
        }

        //right
        if (Block(currIsland, x + 1, y) == 0)
        {
            //polyPaths[0] = new Vector2(x + 1, y);
            //polyPaths[1] = new Vector2(x + 1, y - 1);
            //polyPaths[2] = new Vector2(x + 1, y - 1);
            //polyPaths[3] = new Vector2(x + 1, y);

            //polyPoint.SetPath(currIslandColCount, polyPaths);

            polyPaths.Add(new Vector2(x + 1, y));
            polyPaths.Add(new Vector2(x + 1, y - 1));
            polyPaths.Add(new Vector2(x + 1, y - 1));
            polyPaths.Add(new Vector2(x + 1, y));

            //ColliderTriangles(currIsland);

            currIslandColCount++;
            Debug.Log("test4");
        }
    }

    public Vector2 Vector3To2(Vector3 v)
    {
        return new Vector2(v.y, v.z);
    }

    void UpdateMesh(GameObject currIsland)
    {
        currIslandMesh.Clear();
        currIslandMesh.vertices = currIslandV.ToArray();

        Vector2[] test = new Vector2[currIslandMesh.vertices.Length];

        Vector3[] points = currIslandMesh.vertices;
        for (int i = 0; i < test.Length; i++)
        {
            test[i].x = points[i].x;
            test[i].y = points[i].y;
        }

        polyPoint.points = test;


        currIslandMesh.triangles = currIslandTriangle.ToArray();
        currIslandMesh.uv = currIslandUV.ToArray();
        currIslandMesh.RecalculateNormals();

        //currIsland.AddComponent<PolygonCollider2D>();

        //polyPoint.SetPath(polyPathCount, polyPaths);
        //Debug.Log(polyPaths.ToArray());

        //polyPathCount = 0;



        //polyPaths = null;
        //polyPathCount = 0;
        //blockCount = 0;
        //currIslandV.Clear();
        //currIslandTriangle.Clear();
        //currIslandUV.Clear();

        //currIslandColVertices.Clear();
        //currIslandColTriangles.Clear();
        //currIslandColCount = 0;
    }

    void ScaleParent(Transform child, Transform parent)
    {
        child.parent = parent;
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one;
    }

    void MoveIsland(GameObject currIsland)
    {
        var x = currIsland.transform.position.x;
        var y = currIsland.transform.position.y;

        currIsland.transform.position = new Vector3(x+2, y+2, 0);
    }

    IEnumerator TimeInterval(GameObject currIsland)
    {
        //Destroy(currIsland.GetComponent<PolygonCollider2D>());
        yield return new WaitForSeconds(10);
        Destroy(currIsland.GetComponent<Rigidbody2D>());
        //var col = currIsland.GetComponent<PolygonCollider2D>();
        //col.offset = new Vector3(1f, 1f, 0);
        //currIsland.AddComponent<PolygonCollider2D>();
    }


    void GenerateIsland(GameObject currIsland, Vector3 pos, float angle, float scale)
    {
        var newIsland = Instantiate(islandClone, pos, Quaternion.identity);
        newIsland.transform.SetParent(gameObject.transform, false);
        //newIsland.transform.SetPositionAndRotation(pos, Quaternion.identity);
        currIsland = newIsland;
        ScaleParent(currIsland.transform, gameObject.transform);
        currentIsland = currIsland;



        List<Vector3> island = new List<Vector3>();
        List<Vector2> islandUV = new List<Vector2>();
        List<int> islandTriangle = new List<int>();

        var mesh = currIsland.GetComponent<MeshFilter>().mesh;
       // var col = currIsland.GetComponent<PolygonCollider2D>();

        currIslandMesh = mesh;
        //currIslandCol = col;

        polyPoint = currIsland.GetComponent<PolygonCollider2D>();
        //polyPoint.pathCount = (int)scale;

        GenerateBlocks(currIsland, scale);
        BuildMesh(currIsland);
        UpdateMesh(currIsland);

        //col.offset = new Vector3(0.50f, -0.50f, 0);

        //currIsland.transform.position.z.Equals(z);

        var x = currIsland.transform.position.x;
        var y = currIsland.transform.position.y;

        //currIsland.transform.localScale = transform.parent.InverseTransformPoint(Vector3.one);
        currIsland.transform.position =  new Vector3(pos.x, pos.y, transform.localScale.y);
        //currIsland.transform.localScale = new Vector3(scale, scale, transform.localScale.y);

        var rotationVector = currIsland.transform.rotation.eulerAngles;
        rotationVector.z = angle;
        currIsland.transform.rotation = Quaternion.Euler(rotationVector);

        //island.Add(new Vector3(x, y, 0));
        //island.Add(new Vector3(x + 1, y, 0));
        //island.Add(new Vector3(x + 1, y - 1, 0));
        //island.Add(new Vector3(x, y - 1, 0));

        //islandTriangle.Add(0);
        //islandTriangle.Add(1);
        //islandTriangle.Add(3);
        //islandTriangle.Add(1);
        //islandTriangle.Add(2);
        //islandTriangle.Add(3);

        //islandUV.Add(new Vector2(0, 0));
        //islandUV.Add(new Vector2(0, 1));
        //islandUV.Add(new Vector2(1, 1));
        //islandUV.Add(new Vector2(1, 0));

        //mesh.Clear();
        //mesh.vertices = island.ToArray();
        //mesh.triangles = islandTriangle.ToArray();
        //mesh.uv = islandUV.ToArray();
        //mesh.RecalculateNormals();

        //if (Physics2D.OverlapCircle(currIsland.transform.position, scale))
        //{
        //    //Debug.Log(currIsland);
        //    //MoveIsland(currIsland);
        //}

        StartCoroutine(TimeInterval(currIsland));
        currentIsland = null;
    }


    void GenerateCenterIsland(GameObject centerIsland, float size)
    {
        var mesh = centerIsland.GetComponent<MeshFilter>().mesh;
        var col = centerIsland.GetComponent<BoxCollider2D>();

        centerIsland.transform.localScale = new Vector3(size, size, transform.localScale.y);
        //Debug.Log(radius);

        var centerPosX = centerIsland.transform.position.x - 0.50f;
        var centerPosY = centerIsland.transform.position.y + 0.50f;
        centerIsland.transform.position = new Vector3(centerPosX, centerPosY, transform.localScale.y);


        var x = centerIsland.transform.position.x;
        var y = centerIsland.transform.position.y;

        centerIslandV.Add(new Vector3(x, y, 0));
        centerIslandV.Add(new Vector3(x + 1, y, 0));
        centerIslandV.Add(new Vector3(x + 1, y - 1, 0));
        centerIslandV.Add(new Vector3(x, y - 1, 0));

        centerIslandTriangle.Add(0);
        centerIslandTriangle.Add(1);
        centerIslandTriangle.Add(3);
        centerIslandTriangle.Add(1);
        centerIslandTriangle.Add(2);
        centerIslandTriangle.Add(3);

        centerIslandUV.Add(new Vector2(0, 0));
        centerIslandUV.Add(new Vector2(0, 1));
        centerIslandUV.Add(new Vector2(1, 1));
        centerIslandUV.Add(new Vector2(1, 0));

        mesh.Clear();
        mesh.vertices = centerIslandV.ToArray();
        mesh.triangles = centerIslandTriangle.ToArray();
        mesh.uv = centerIslandUV.ToArray();
        mesh.RecalculateNormals();
    }

    // Use this for initialization
    void Start()
    {

        var centerIsland = GameObject.FindGameObjectWithTag("CenterIsland");
        radius = islands.Length / 6 + radius;

        var centerIslandSize = islands.Length / 5 + (radius / 2);

        if (centerIsland != null)
        {
            GenerateCenterIsland(centerIsland, centerIslandSize);
        }

       

        if (islands.Length != 0)
        { 


            for (int i = 0; i < islands.Length; i++)
            {

                //var pointNum = (i * 1.0f) / numPoints;
                //var angle = i * Mathf.PI * 2;
                //var x = Mathf.Cos(angle) * radiusX + Random.Range(-offsetIslandsX, offsetIslandsX);
                //var y = Random.Range(-offsetIslandsY, offsetIslandsY);
                //var z = Mathf.Sin(angle) * radiusZ + Random.Range(-jitterZ, jitterZ);
                //var pos = new Vector3(x, y, z) + center;


                var angle = i * Mathf.PI * 10 / islands.Length;
                var posX = Mathf.Cos(angle) + Random.Range(-offsetIslandsX, offsetIslandsX);
                var posY = Mathf.Sin(angle) + Random.Range(-offsetIslandsY, offsetIslandsY);
                var pos = new Vector3(posX, posY, 0) * radius * 2;
                var scale = Random.Range(1f, 20f);
                //var scaleY = Random.Range(1f, 20f);

                GenerateIsland(islands[i], pos, angle, scale);
            }
        }

        var islandGroup = GameObject.FindGameObjectsWithTag("Island");

        foreach(var island in islandGroup)
        {
            var targetRotation = Quaternion.LookRotation(centerIsland.transform.position - island.transform.position);
            var str = Mathf.Min(strength * Time.deltaTime, 1);
            island.transform.rotation = Quaternion.Lerp(island.transform.rotation, targetRotation, str);
        }
    }

    // Update is called once per frame
    void Update () {
        //BuildMesh(currentIsland);
        //UpdateMesh(currentIsland);
    }
}

