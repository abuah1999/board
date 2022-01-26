using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    private Vector3 start, end;
    private LineRenderer arrowLine = new LineRenderer();
    public void SetStart(Vector3 start_pos){ start = start_pos; }
    public void SetEnd(Vector3 end_pos){ end = end_pos; }
    public Vector3 getStart(){return start;}
    public Vector3 getEnd(){return end;}
    public float arrowheadSize = 0.5f;
    public void Draw(){
        arrowLine = gameObject.GetComponent<LineRenderer>();
        float percentSize = (float) (arrowheadSize / Vector3.Distance (start, end));
        arrowLine.SetPosition (0, start);
        arrowLine.SetPosition (1, Vector3.Lerp(start, end, 0.999f - percentSize));
        arrowLine.SetPosition (2, Vector3.Lerp (start, end, 1 - percentSize));
        arrowLine.SetPosition (3, end);
        arrowLine.widthCurve = new AnimationCurve (
                new Keyframe (0, 0.2f),
                new Keyframe (0.999f - percentSize, 0.2f),
                new Keyframe (1 - percentSize, 0.7f),
                new Keyframe (1 - percentSize, 0.7f),
                new Keyframe (1, 0f));
    }
    public void Activate(){
        
    }
}
