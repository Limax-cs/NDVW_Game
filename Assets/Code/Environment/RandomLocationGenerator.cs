using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLocationGenerator{
    private int width = 10;
    private int height = 10;

    private Vector3 location = Vector3.zero;

    public RandomLocationGenerator(Vector3 location)
    {
        this.location = location;
    }

    public RandomLocationGenerator(Vector3 location, int width, int height)
    {
        this.width = width;
        this.height = height;
        this.location = location;
    }

    public Vector3 getRandomLocation(){
        float half_w = this.width/2;
        float half_h = this.height/2;

        return new Vector3(Random.Range(-1*half_w, half_w) + location.x, 0, Random.Range(-1*half_h, half_h) + location.z);
    }
}