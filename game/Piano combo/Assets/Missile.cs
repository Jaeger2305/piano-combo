using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Vector3 target;

    // Update is called once per frame
    void Update()
    {
        MoveToPoint(6f);
    }

    void MoveToPoint(float speed)
    {
        Vector3 nextPosition = Vector3.MoveTowards(this.gameObject.transform.position, this.target, speed * Time.deltaTime);
        this.gameObject.transform.position = nextPosition;
        if (nextPosition == this.target) Destroy(this.gameObject);
    }
}
