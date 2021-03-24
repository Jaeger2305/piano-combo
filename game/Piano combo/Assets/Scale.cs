using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale
{
    public string description;

    public Dictionary<int, AudioSource> scaleMap = new Dictionary<int, AudioSource>();

    public void PlayPosition(int scalePosition)
    {
        AudioSource.PlayClipAtPoint(this.scaleMap[scalePosition].clip, new Vector3(0,0,0), 0.8f);
    }

    public void PlayArpeggio()
    {
        AudioSource.PlayClipAtPoint(this.scaleMap[0].clip, new Vector3(0,0,0), 0.2f);
        AudioSource.PlayClipAtPoint(this.scaleMap[2].clip, new Vector3(0,0,0), 0.2f);
        AudioSource.PlayClipAtPoint(this.scaleMap[4].clip, new Vector3(0,0,0), 0.2f);
        AudioSource.PlayClipAtPoint(this.scaleMap[7].clip, new Vector3(0,0,0), 0.2f);
    }

    public bool IsArpeggio(List<PianoAudio.NoteHistory> noteHistory)
    {
        if (noteHistory.Count < 4) return false;

        var past4notes = noteHistory.GetRange(noteHistory.Count - 4, 4);
        return past4notes.Exists(x => x.scalePosition == 7) && 
          past4notes.Exists(x => x.scalePosition == 4) && 
          past4notes.Exists(x => x.scalePosition == 2) && 
          past4notes.Exists(x => x.scalePosition == 0);
    }

    public void PlayDyad()
    {
        AudioSource.PlayClipAtPoint(this.scaleMap[0].clip, new Vector3(0,0,0), 0.4f);
        AudioSource.PlayClipAtPoint(this.scaleMap[2].clip, new Vector3(0,0,0), 0.4f);
    }

    public void PlayUpperDyad()
    {
        AudioSource.PlayClipAtPoint(this.scaleMap[4].clip, new Vector3(0,0,0), 0.4f);
        AudioSource.PlayClipAtPoint(this.scaleMap[7].clip, new Vector3(0,0,0), 0.4f);
    }

    public bool IsDyad(List<PianoAudio.NoteHistory> noteHistory)
    {
        if (noteHistory.Count < 2) return false;

        var past2notes = noteHistory.GetRange(noteHistory.Count - 2, 2);
        return (past2notes.Exists(x => x.scalePosition == 0) && past2notes.Exists(x => x.scalePosition == 2)) || 
            (past2notes.Exists(x => x.scalePosition == 0) && past2notes.Exists(x => x.scalePosition == 4)) || 
            (past2notes.Exists(x => x.scalePosition == 2) && past2notes.Exists(x => x.scalePosition == 4)) || 
            (past2notes.Exists(x => x.scalePosition == 0) && past2notes.Exists(x => x.scalePosition == 7)) || 
            (past2notes.Exists(x => x.scalePosition == 2) && past2notes.Exists(x => x.scalePosition == 7)) || 
            (past2notes.Exists(x => x.scalePosition == 4) && past2notes.Exists(x => x.scalePosition == 7));
    }

    public bool IsChromatic(List<PianoAudio.NoteHistory> noteHistory)
    {
        if (noteHistory.Count < 3) return false;

        var distanceBetween1stAnd2nd = noteHistory[noteHistory.Count - 3].scalePosition - noteHistory[noteHistory.Count - 2].scalePosition;
        var distanceBetween2ndAnd3rd = noteHistory[noteHistory.Count - 2].scalePosition - noteHistory[noteHistory.Count - 1].scalePosition;

        return distanceBetween1stAnd2nd == distanceBetween2ndAnd3rd && System.Math.Abs(distanceBetween2ndAnd3rd) == 1;
    }
}
