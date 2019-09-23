using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public GameObject[] asteroids;
    public int amount;
    public int range;

    public int scaleMin;
    public int scaleMax;

    List<GameObject> generatedAsteroids;

    public void GenerateAsteroids()
    {
        generatedAsteroids = new List<GameObject>();
         
        for (int i = 0; i < amount; i += 1)
        {
            GameObject asteroid = Instantiate(
                asteroids[Random.Range(1, asteroids.Length - 1)],
                new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)),
                Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f))
            );

            asteroid.transform.localScale *= Random.Range(scaleMin, scaleMax);
            asteroid.transform.SetParent(transform);
            generatedAsteroids.Add(asteroid);
        }
    }

    public void DeleteAsteroids()
    {
        foreach (GameObject asteroid in generatedAsteroids)
        {
            GameObject.DestroyImmediate(asteroid, true);
        }
    }
}
