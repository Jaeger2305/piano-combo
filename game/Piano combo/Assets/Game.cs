using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public EnemyPool enemyPool;
    public GameObject[] pianos;
    public GameObject uiTimer;
    public GameObject uiRhythm;
    public GameObject uiScore;
    public GameObject uiScaleDescription;

    private GameObject activePiano;
    private System.Random rnd = new System.Random();

    private int score = 0;

    // Tempo stuff
    private float beatsPerSecond = 1f;
    private int beatDivisions = 4;

    private float nextNote = 0f;
    private float nextBar = 0f;

    private float startTime;

    public bool isEndless = false;
    public bool isGameOver = false;

    [System.Serializable]
    public struct Bar
    {
        public int chord;
        public int spawnChance;
        public int dyadChance;
        public bool percussion;
        public bool strings;
        // public bool dynamics;
    };
    public Bar[] bars = new Bar[] {
        new Bar() { chord = 1, spawnChance = 25, dyadChance = 50, percussion = false, strings = true },
        new Bar() { chord = 4, spawnChance = 25, dyadChance = 50, percussion = false, strings = true},
        new Bar() { chord = 1, spawnChance = 25, dyadChance = 50, percussion = false, strings = false },
        new Bar() { chord = 4, spawnChance = 25, dyadChance = 50, percussion = false, strings = false },
        new Bar() { chord = 5, spawnChance = 25, dyadChance = 50, percussion = false, strings = true },
        new Bar() { chord = 4, spawnChance = 25, dyadChance = 50, percussion = false, strings = false },
        new Bar() { chord = 1, spawnChance = 25, dyadChance = 50, percussion = false, strings = false },
        new Bar() { chord = 5, spawnChance = 25, dyadChance = 50, percussion = false, strings = false },
    };

    private Bar activeBar;

    // Phrases
    private int activeBarIndex = 0;

    private void Awake()
    {
        this.startTime = Time.time;
    }

    void Update() {
        if (this.CheckGameOver()) {
            return;
        }
        if(Time.time >= this.nextBar){
             this.nextBar = Mathf.Round(Time.time * 100) / 100 + this.beatDivisions / this.beatsPerSecond;
             this.GetNextBar();
         }
        if(Time.time >= this.nextNote){
             this.nextNote = Mathf.Round(Time.time * 100) / 100 + 1f / this.beatsPerSecond;
             this.SpawnEnemy();
         }
    }

    private bool CheckGameOver() {
        if (this.isGameOver) return true;
        var isGameEnding = Time.time - this.startTime >= this.bars.Length * this.beatDivisions / this.beatsPerSecond;

        if (isGameEnding && !this.isEndless) {
            this.enemyPool.DestroyAllEnemies();
            Destroy(this.activePiano);
            Scene scene = SceneManager.GetActiveScene();
            string highscoreKey = "score-" + scene.name;
            int currentHighscore = PlayerPrefs.GetInt(highscoreKey);
            if (this.score > currentHighscore)
            {
                PlayerPrefs.SetInt(highscoreKey, this.score);
            }
            this.isGameOver = true;
        }

        return isGameEnding;
    }

    void SpawnEnemy() {
        if (this.rnd.Next(0, 100) <= this.activeBar.spawnChance) {
            this.enemyPool.SpawnEnemy();
            if (this.rnd.Next(0, 100) <= this.activeBar.dyadChance) {
                this.enemyPool.SpawnEnemy();
            }
        }
    }

    void GetNextBar() {
        this.activeBar = this.bars[this.activeBarIndex];
        this.activeBarIndex = (this.activeBarIndex + 1) % this.bars.Length;

        int pianoIndex = this.activeBar.chord;
        
        if (this.activePiano != null) Destroy(this.activePiano);

        this.activePiano = Instantiate(this.pianos[pianoIndex], new Vector3(0, 0, 0), Quaternion.identity);
        PianoAudio piano = this.activePiano.GetComponent<PianoAudio>();
        piano.SetUI(
            this.uiTimer.GetComponent<TMPro.TextMeshProUGUI>(),
            this.uiRhythm.GetComponent<TMPro.TextMeshProUGUI>(),
            this.uiScaleDescription.GetComponent<TMPro.TextMeshProUGUI>()
        );
        piano.SetTempo(this.beatsPerSecond, this.beatDivisions);
        piano.SetInstruments(this.activeBar.percussion, this.activeBar.strings);
    }

    public PianoAudio GetPiano()
    {
        return this.activePiano.GetComponent<PianoAudio>();
    }

    public void AddScore(int score, string note) {
        this.score += score;
        this.uiScore.GetComponent<TMPro.TextMeshProUGUI>().SetText("{0} - " + note, this.score);
    }
    public void AddScore(int score) {
        this.score += score;
        this.uiScore.GetComponent<TMPro.TextMeshProUGUI>().SetText("{0}", this.score);
    }
}
