using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemy;
    public List<GameObject> pool = new List<GameObject>();

    private System.Random rnd = new System.Random();

    public void SpawnEnemy() {
        var spawnedEnemy = Instantiate(enemy, new Vector3(0 ,0, 0), Quaternion.identity);
        var spawnedEnemyState = spawnedEnemy.GetComponent<Enemy>();
        spawnedEnemyState.pool = this;
        spawnedEnemyState.speed = 0.5f;
        spawnedEnemy.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        spawnedEnemy.transform.position = this.segmentPositionAroundCircle(5, 8, spawnedEnemyState.scalePosition, 5, 10, -2.5);

        this.pool.Add(spawnedEnemy);
    }

    Vector3 segmentPositionAroundCircle(int radius, int segmentCount, int segmentPosition, int angleRandomness, int distanceRandomness, double segmentOffset) {
        int randX = this.rnd.Next(0, distanceRandomness) / 10 * this.randomlyInvertMultiplier();
        int randY = this.rnd.Next(0, distanceRandomness) / 10 * this.randomlyInvertMultiplier();

        int segmentAngle = 360 / segmentCount;
        double segmentAngleOffset = segmentOffset * segmentAngle;
        double angleDegrees = segmentPosition * segmentAngle;
        double angleOffset = angleDegrees + segmentAngleOffset + (this.rnd.Next(0, angleRandomness) * this.randomlyInvertMultiplier());
        double angle = -(System.Math.PI / 180) * angleOffset;

        double x = System.Math.Cos(angle) * radius + randX;
        double y = System.Math.Sin(angle) * radius + randY;
        return new Vector3((float)x, (float)y, 0);
    }

    private int randomlyInvertMultiplier() {
        return this.rnd.Next(2) < 1 ? -1 : 1;
    }

    public GameObject GetEnemyWithNote(int scalePosition) {
        return this.pool.FirstOrDefault(x => x.GetComponent<Enemy>().scalePosition == scalePosition);
    }

    public void DestroyEnemy(GameObject enemy) {
            this.pool.Remove(enemy);
            Destroy(enemy);
    }

    public void DestroyAllEnemies() {
        foreach (var enemy in this.pool) {
            Destroy(enemy);
        }
        this.pool.Clear();
    }
}
