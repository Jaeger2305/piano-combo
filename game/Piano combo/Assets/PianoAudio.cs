using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PianoAudio : MonoBehaviour
{
    private Scale scale;
    public float beatsPerSecond = 1f;
    public int beatDivisions = 4;
    private float timer = 0f;

    private TMPro.TextMeshProUGUI uiTimerTMP;
    private TMPro.TextMeshProUGUI uiRhythmTMP;
    private TMPro.TextMeshProUGUI uiScaleDescriptionTMP;
    private Game scoreManager;

    public AudioClip percussion;
    public AudioClip strings;

    [System.Serializable]
    public struct SerializableScaleMap
    {
        public KeyCode inputKey;
        public AudioSource audioSource;
    }
    public SerializableScaleMap[] serialisedScales;
    public string scaleDescription;

    public struct NoteHistory
    {
        public int scalePosition;
        public bool isGoodTiming;
        public bool isArpeggio;
        public bool isDyad;
        public bool isChromatic;
    }
    private List<NoteHistory> noteHistory = new List<NoteHistory>();
    private float nextUpdate = 0f;
    private float nextBar = 0f;

    void Start()
    {
        this.scoreManager = FindObjectOfType<Game>();
        this.scale = new Scale();
        for (int i = 0; i < serialisedScales.Length; i++)
        {
            scale.scaleMap[i] = serialisedScales[i].audioSource;
        }
        this.scale.description = this.scaleDescription;
        this.uiScaleDescriptionTMP.text = this.scale.description;
    }

    void Update()
    {
        this.timer += Time.deltaTime;
        var roundedTime = Mathf.Round(Time.time * 100) / 100;
        if(roundedTime >= nextUpdate){
             nextUpdate = roundedTime + (2f / this.beatsPerSecond);
             this.DisplayTimer();
         }

        if (Time.time >= nextBar && this.strings)
        {
            nextBar = roundedTime + (this.beatsPerSecond * this.beatDivisions);
            AudioSource.PlayClipAtPoint(this.strings, new Vector3(0, 0, 0), 0.1f);
        }
    }

    public void PlayPosition(int position)
    {
        scale.PlayPosition(position);
        this.AddNoteHistory(position);
        if (this.IsGoodTiming())
        {
            this.uiRhythmTMP.text = "neat!";
        }
        else
        {
            this.uiRhythmTMP.text = "bad :(";
        }
    }

    public void SetUI(TMPro.TextMeshProUGUI uiTimerTMP, TMPro.TextMeshProUGUI uiRhythmTMP, TMPro.TextMeshProUGUI uiScaleDescriptionTMP) {
        this.uiTimerTMP = uiTimerTMP;
        this.uiRhythmTMP = uiRhythmTMP;
        this.uiScaleDescriptionTMP = uiScaleDescriptionTMP;
    }

    public void SetTempo(float beatsPerSecond, int beatDivisions) {
        this.beatsPerSecond = beatsPerSecond;
        this.beatDivisions = beatDivisions;
    }

    public void SetInstruments(bool percussion, bool strings)
    {
        if (!percussion) this.percussion = null;
        if (!strings) this.strings = null;
    }

    void DisplayTimer()
    {
        this.uiTimerTMP.SetText("{0:0}", this.timer % 4 + 1);
        if (this.percussion) AudioSource.PlayClipAtPoint(this.percussion, new Vector3(0,0,0), 0.3f);
    }

    void AddNoteHistory(int scalePosition)
    {
        NoteHistory newNote;
        newNote.scalePosition = scalePosition;
        newNote.isGoodTiming = this.IsOnTime();
        newNote.isArpeggio = false;
        newNote.isDyad = false;
        newNote.isChromatic = false;
        this.noteHistory.Add(newNote);
        if (this.scale.IsArpeggio(this.noteHistory)) {
            this.scale.PlayArpeggio();
            this.scoreManager.AddScore(newNote.isGoodTiming ? 10 : 2, "arpeggio");
            newNote.isArpeggio = true;
        } else if (this.scale.IsDyad(this.noteHistory)) {
            this.scale.PlayDyad();
            this.scoreManager.AddScore(newNote.isGoodTiming ? 2 : 1, "dyad");
            newNote.isDyad = true;
        }
        if (this.scale.IsChromatic(this.noteHistory)) {
            this.scale.PlayUpperDyad();
            this.scoreManager.AddScore(newNote.isGoodTiming ? 4 : 2, "chromatic");
            newNote.isChromatic = true;
        }
    }

    bool IsOnTime()
    {
        float distanceFromBeat = (this.timer * this.beatDivisions * this.beatsPerSecond) % (1);
        return distanceFromBeat < 0.4 || distanceFromBeat > 0.6;
    }

    bool IsGoodTiming()
    {
        if (this.noteHistory.Count < 3) return false;

        return !this.noteHistory.GetRange(this.noteHistory.Count - 3, 3).Exists(x => !x.isGoodTiming);
    }
}
