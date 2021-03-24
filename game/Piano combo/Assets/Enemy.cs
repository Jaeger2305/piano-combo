using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    static System.Random rnd = new System.Random();
    
    public struct Note
    {
        public int probability;
        public int scalePosition;
    }
    public List<Note> notes = new List<Note>{
        new Note(){ probability = 25, scalePosition = 1 },
        new Note(){ probability = 12, scalePosition = 2 },
        new Note(){ probability = 20, scalePosition = 3 },
        new Note(){ probability = 3, scalePosition = 4 },
        new Note(){ probability = 20, scalePosition = 5 },
        new Note(){ probability = 1, scalePosition = 6 },
        new Note(){ probability = 9, scalePosition = 7 },
        new Note(){ probability = 10, scalePosition = 8 },
    };
    public int scalePosition;
    public Vector3 destination = new Vector3(0,0,0);
    public float speed = 0.5f;
    
    public EnemyPool pool;
    private Game scoreManager;

    private float fade = 0f;
    private bool dying = false;
    private Material material;

    void Awake()
    {
        int r = rnd.Next(1,101);
        int noteIndex = 0;
        for(int i = 0; i < r; i += this.notes[noteIndex++].probability) {
            
        }
        this.scalePosition = this.notes[noteIndex - 1].scalePosition;
        this.gameObject.GetComponent<TMPro.TextMeshPro>().text = this.scalePosition.ToString();
        this.scoreManager = FindObjectOfType<Game>();
        this.material = GetComponentInChildren<SpriteRenderer>().material;
    }

    void Update() {
        if (this.dying)
        {
            this.DestroyAfterFade();
            return;
        }
        if (this.fade < 1f)
        {
            this.fade += 0.01f;
            this.material.SetFloat("Disappear", this.fade);
        }
        MoveToPoint(this.speed);
    }

    void MoveToPoint(float speed)
    {
        Vector3 nextPosition = Vector3.MoveTowards(this.gameObject.transform.position, this.destination, speed * Time.deltaTime);
        this.gameObject.transform.position = nextPosition;

        if (this.gameObject.transform.position == this.destination) {            
            this.dying = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "Missile(Clone)")
        {
            Destroy(col.gameObject);
            this.dying = true;
            scoreManager.AddScore(1);
        }
    }

    void DestroyAfterFade()
    {
        this.fade -= 0.04f;
        this.material.SetFloat("Disappear", this.fade);
        if (this.fade <= 0)
        {
            this.pool.DestroyEnemy(this.gameObject);
        }
    }
}
