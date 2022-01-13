using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject controller;
    //public float arrowheadSize;
    //private Vector3 startPosition, mouseWorld;
    //GameObject arrow;
    //private LineRenderer arrowLine = new LineRenderer();

    /*public void Start(){
        arrow = GameObject.FindGameObjectWithTag("Arrow");
        arrowLine = arrow.GetComponent<LineRenderer>();
        mouseWorld = new Vector3 ();
        arrowheadSize = 1.2f;
    }*/

   public void OnMouseDown(){
       // mostly not relevant I think. All of my current testing happens when "selecton" is off.
        controller = GameObject.FindGameObjectWithTag("GameController");
        if (controller.GetComponent<Game>().selecton){
            controller.GetComponent<Game>().selecton = false;
            controller.GetComponent<Game>().selected. GetComponent<Chessman>().Emsmallen(controller.GetComponent<Game>().selected);
            //controller.GetComponent<Game>().selected. GetComponent<SpriteRenderer>().size -= new Vector2(0.03f, 0.03f);
            //controller.GetComponent<Game>().selected. GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Simple;
            controller.GetComponent<Game>().selected. GetComponent<Chessman>().DestroyMovePlates(true);
        } /*if (true) /*eventually this will be an actual conditional. 
        For now though I want this behavior to occur everytime the mouse is clicked {
            // Here seems to be the problem... somewhere
            mouseWorld = Camera.main.ScreenToWorldPoint (
                new Vector3 (Input.mousePosition.x,
                            Input.mousePosition.y,
                            Camera.main.nearClipPlane
                ));
            startPosition = mouseWorld;
        }*/
    }

    /*public void OnMouseDrag(){
        //Turn on the arrow
        arrowLine.enabled = true;
        DrawArrow ();
    }

    public void DrawArrow(){
        mouseWorld = Camera.main.ScreenToWorldPoint (
                new Vector3 (Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.nearClipPlane
                ));
        //The longer the line gets, the smaller relative to the entire line the arrowhead should be
        float percentSize = (float) (arrowheadSize / Vector3.Distance (startPosition, mouseWorld));
        //h/t ShawnFeatherly (http://answers.unity.com/answers/1330338/view.html)
        arrowLine.SetPosition (0, startPosition);
        arrowLine.SetPosition (1, Vector3.Lerp(startPosition, mouseWorld, 0.999f - percentSize));
        arrowLine.SetPosition (2, Vector3.Lerp (startPosition, mouseWorld, 1 - percentSize));
        arrowLine.SetPosition (3, mouseWorld);
        arrowLine.widthCurve = new AnimationCurve (
                new Keyframe (0, 0.2f),
                new Keyframe (0.999f - percentSize, 0.2f),
                new Keyframe (1 - percentSize, 0.7f),
                new Keyframe (1 - percentSize, 0.7f),
                new Keyframe (1, 0f));
    }

    void OnMouseUp(){
        //Turn off the arrow
        arrowLine.enabled = false;
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100);
        transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
    }*/
}
