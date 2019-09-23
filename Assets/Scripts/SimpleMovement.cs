using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public GameObject[] targets;
    public int speed;
    public int distance;

    private int currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targets.Length > 0)
        {
            transform.LookAt(targets[currentTarget].transform);
            if ((transform.position - targets[currentTarget].transform.position).magnitude > 50)
            {
                transform.position = Vector3.MoveTowards(transform.position, targets[currentTarget].transform.position, speed * Time.fixedDeltaTime);
            } else
            {
                if (currentTarget == targets.Length - 1) currentTarget = 0;
                else currentTarget += 1;
            }
        }
    }
}
