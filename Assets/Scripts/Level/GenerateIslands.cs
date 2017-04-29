using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private CircleCollider2D currIslandCol;
    private CircleCollider2D prevIslandCol;

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

    IEnumerator MyMethod(GameObject currIsland)
    {
        yield return new WaitForSeconds(10);
        Destroy(currIsland.GetComponent<Rigidbody2D>());
        var col = currIsland.GetComponent<BoxCollider2D>();
        col.size = new Vector3(1f, 1f, 0);
    }


    void GenerateIsland(GameObject currIsland, Vector3 pos, float angle, float scale)
    {
        var newIsland = Instantiate(islandClone, pos, Quaternion.identity);
        newIsland.transform.SetParent(gameObject.transform, false);
        //newIsland.transform.SetPositionAndRotation(pos, Quaternion.identity);
        currIsland = newIsland;
        ScaleParent(currIsland.transform, gameObject.transform);



        List<Vector3> island = new List<Vector3>();
        List<Vector2> islandUV = new List<Vector2>();
        List<int> islandTriangle = new List<int>();

        var mesh = currIsland.GetComponent<MeshFilter>().mesh;
        var col = currIsland.GetComponent<BoxCollider2D>();

        col.offset = new Vector3(0.50f, -0.50f, 0);

        //currIsland.transform.position.z.Equals(z);

        var x = currIsland.transform.position.x;
        var y = currIsland.transform.position.y;

        //currIsland.transform.localScale = transform.parent.InverseTransformPoint(Vector3.one);
        currIsland.transform.position =  new Vector3(pos.x, pos.y, transform.localScale.y);
        currIsland.transform.localScale = new Vector3(scale, scale, transform.localScale.y);

        var rotationVector = currIsland.transform.rotation.eulerAngles;
        rotationVector.z = angle;
        currIsland.transform.rotation = Quaternion.Euler(rotationVector);

        island.Add(new Vector3(x, y, 0));
        island.Add(new Vector3(x + 1, y, 0));
        island.Add(new Vector3(x + 1, y - 1, 0));
        island.Add(new Vector3(x, y - 1, 0));

        islandTriangle.Add(0);
        islandTriangle.Add(1);
        islandTriangle.Add(3);
        islandTriangle.Add(1);
        islandTriangle.Add(2);
        islandTriangle.Add(3);

        islandUV.Add(new Vector2(0, 0));
        islandUV.Add(new Vector2(0, 1));
        islandUV.Add(new Vector2(1, 1));
        islandUV.Add(new Vector2(1, 0));

        mesh.Clear();
        mesh.vertices = island.ToArray();
        mesh.triangles = islandTriangle.ToArray();
        mesh.uv = islandUV.ToArray();
        mesh.RecalculateNormals();

        //if (Physics2D.OverlapCircle(currIsland.transform.position, scale))
        //{
        //    //Debug.Log(currIsland);
        //    //MoveIsland(currIsland);
        //}

        StartCoroutine(MyMethod(currIsland));
    }


    void GenerateCenterIsland(GameObject centerIsland, float size)
    {
        var mesh = centerIsland.GetComponent<MeshFilter>().mesh;
        var col = centerIsland.GetComponent<BoxCollider2D>();

        centerIsland.transform.localScale = new Vector3(size, size, transform.localScale.y);
        Debug.Log(radius);

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
    }
}