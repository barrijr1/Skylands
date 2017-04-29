using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIslandTest : MonoBehaviour {

    public Vector3 center;
    public float radiusX;
    public float radiusZ;
    public float density;
    public float spacingJitterX;
    public float spacingJitterZ;
    public float spacingJitterY;
    public float sizeJitter;
    public GameObject[] asteroids;

	// Use this for initialization
	void Start () {
        center = transform.position;

        //get circumference of ellipse
        float circum = 2 * Mathf.PI * Mathf.Sqrt((radiusX * radiusX + radiusZ * radiusZ) / 2.0f);
        //calculate the number of points
        float numPoints = circum / density;

        for (var pointNum = 0; pointNum < numPoints; pointNum++)
        {

            var i = (pointNum * 1.0) / numPoints + 0.0f;
            float angle = (float)i * Mathf.PI * 2;
            var x = Mathf.Cos(angle) * radiusX + Random.Range(-spacingJitterX, spacingJitterX);
            var z = Mathf.Sin(angle) * radiusZ + Random.Range(-spacingJitterZ, spacingJitterZ);
            var y = Random.Range(-spacingJitterY, spacingJitterY);
            var pos = new Vector3(x, y, z) + center;

            int r = Random.Range(0, asteroids.Length);
            GameObject asteroid = asteroids[r];
            asteroid.transform.localScale = Vector3.one * Random.Range(1, sizeJitter);

            Instantiate(asteroid, pos, Random.rotation);

            Mesh mesh = asteroid.GetComponent<MeshFilter>().mesh;

        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}