using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigitalRubyShared
{

    public class Player : MonoBehaviour
    {
        public EnemyPool enemyPool;
        public GameObject missile;
        private Dictionary<KeyCode, int> KeyBinding = new Dictionary<KeyCode, int>(){
            {KeyCode.Q, 1},
            {KeyCode.W, 2},
            {KeyCode.E, 3},
            {KeyCode.R, 4},
            {KeyCode.T, 5},
            {KeyCode.Y, 6},
            {KeyCode.U, 7},
            {KeyCode.I, 8},
        };
        private Game scoreManager;
        private SwipeGestureRecognizer swipeGesture;
        private readonly List<Vector3> swipeLines = new List<Vector3>();

        void Awake()
        {
            this.scoreManager = FindObjectOfType<Game>();
        }

        private void Start()
        {
            CreateSwipeGesture();
        }

        private void CreateSwipeGesture()
        {
            swipeGesture = new SwipeGestureRecognizer();
            swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
            swipeGesture.StateUpdated += SwipeGestureCallback;
            swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
            Debug.Log(FingersScript.Instance);
            FingersScript.Instance.AddGesture(swipeGesture);
        }

        private void SwipeGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                HandleSwipe();
            }
        }

        private void HandleSwipe()
        {
            Vector2 start = new Vector2(swipeGesture.StartFocusX, swipeGesture.StartFocusY);
            Vector2 end = new Vector2(swipeGesture.FocusX, swipeGesture.FocusY);
            float angle = Vector2.Angle(Vector2.up, end - start);
            int segmentsInCircle = 8;
            int segmentsFromUp = (int)System.Math.Ceiling(angle / (360 / segmentsInCircle));

            // Because Vector2.Angle returns the acute angle, we need to check which direction we're swiping, so we know if we should count backwards instead.
            int segmentId = swipeGesture.FocusX - swipeGesture.StartFocusX > 0 ? segmentsFromUp : segmentsInCircle + 1 - segmentsFromUp;

            this.HandleInput(segmentId);
        }

        // Update is called once per frame
        void Update()
        {
        
            foreach (KeyValuePair<KeyCode, int> kvp in KeyBinding)
            {
                if (Input.GetKeyDown(kvp.Key))
                {
                    this.HandleInput(kvp.Value);
                }

            }
        }

        private void HandleInput(int segmentId)
        {
            // Go back to main menu if the game is over and there's input.
            if (this.scoreManager.isGameOver)
            {
                SceneManager.LoadScene(0);
                return;
            }
            GameObject enemy = this.enemyPool.GetEnemyWithNote(segmentId);
            if (enemy) this.FireMissile(enemy);
            else this.scoreManager.AddScore(-2);

            PianoAudio pianoAudio = this.scoreManager.GetPiano();
            pianoAudio.PlayPosition(segmentId - 1);
        }

        private void FireMissile(GameObject enemy)
        {
            var spawnedMissile = Instantiate(this.missile, new Vector3(0, 0, 0), Quaternion.identity);
            spawnedMissile.GetComponent<Missile>().target = enemy.transform.position;
        }

    }

}