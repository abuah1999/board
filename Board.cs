using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject controller;
    public GameObject arrow;
    //public float arrowheadSize;
    private Vector3 startPosition, mouseWorld;
    public GameObject currentArrow;
    public List<GameObject> arrowList;
    //private LineRenderer arrowLine = new LineRenderer();

    public Vector3[,] grid = new Vector3[8,8];

    public void Start(){
        //arrow = GameObject.FindGameObjectWithTag("Arrow");
        //arrowLine = arrow.GetComponent<LineRenderer>();
        mouseWorld = new Vector3 ();
        //arrowheadSize = 0.4f;
        float xpoint, ypoint;
        //float[,] result = new float[8,8];
        for (int i = 0; i < 8; i++){
            for (int j = 0; j < 8; j++){
                xpoint = i;
                ypoint = j;

                xpoint *= 1.1f;
                ypoint *= 1.1f;

                xpoint += -3.85f;
                ypoint += -3.85f;

                grid[i,j] = new Vector3(xpoint, ypoint, -2);
            }
        }

    }

    public void Update(){
        /*startPosition = new Vector3(0,0,-2);
        //mouseWorld =  new Vector3(4,+4,-2);
        DrawArrow();*/
        controller = GameObject.FindGameObjectWithTag("GameController");
        if (!controller.GetComponent<Game>().analysis || Input.GetKeyDown("c")){
            foreach (GameObject a in arrowList){
                if (a){
                    Destroy(a);
                }
                
            }
        }
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null) {
                if (controller.GetComponent<Game>().analysis){
                //eventually this will be an actual conditional. For now though I want this behavior to occur everytime the mouse is clicked {
                    // Here seems to be the problem... somewhere
                    mouseWorld = Camera.main.ScreenToWorldPoint (
                        new Vector3 (Input.mousePosition.x,
                                    Input.mousePosition.y,
                                    -2
                        ));
                    mouseWorld[2] = -2;
                    //Debug.Log(mouseWorld[0]);
                    //Debug.Log(mouseWorld[1]);
                    //Debug.Log(mouseWorld[2]);

                    //startPosition = new Vector3(0,0,-2);
                    startPosition = mouseWorld;
                    currentArrow = Instantiate(arrow, startPosition, Quaternion.identity);
                    arrowList.Add(currentArrow);
                }
            }
        }
    
    }

   public void OnMouseDown(){
       // mostly not relevant I think. All of my current testing happens when "selecton" is off.
        controller = GameObject.FindGameObjectWithTag("GameController");
        if (controller.GetComponent<Game>().selecton){
            controller.GetComponent<Game>().selecton = false;
            controller.GetComponent<Game>().selected. GetComponent<Chessman>().Emsmallen(controller.GetComponent<Game>().selected);
            //controller.GetComponent<Game>().selected. GetComponent<SpriteRenderer>().size -= new Vector2(0.03f, 0.03f);
            //controller.GetComponent<Game>().selected. GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Simple;
            controller.GetComponent<Game>().selected. GetComponent<Chessman>().DestroyMovePlates(true);
        } /*if (controller.GetComponent<Game>().analysis){
         //eventually this will be an actual conditional. For now though I want this behavior to occur everytime the mouse is clicked {
            // Here seems to be the problem... somewhere
            mouseWorld = Camera.main.ScreenToWorldPoint (
                new Vector3 (Input.mousePosition.x,
                            Input.mousePosition.y,
                            -2
                ));
            mouseWorld[2] = -2;
            //Debug.Log(mouseWorld[0]);
            //Debug.Log(mouseWorld[1]);
            //Debug.Log(mouseWorld[2]);

            //startPosition = new Vector3(0,0,-2);
            startPosition = mouseWorld;
            currentArrow = Instantiate(arrow, startPosition, Quaternion.identity);
            arrowList.Add(currentArrow);
        }*/
    }

    public void OnMouseDrag(){
        //Turn on the arrow
        //arrowLine.enabled = true;
        if (controller.GetComponent<Game>().analysis && currentArrow){
        mouseWorld = Camera.main.ScreenToWorldPoint (
                new Vector3 (Input.mousePosition.x,
                Input.mousePosition.y,
                -2
                ));

        mouseWorld[2] = -2;
        startPosition = snapToGrid(startPosition); 
        mouseWorld = snapToGrid(mouseWorld);
        currentArrow.GetComponent<Arrow>().SetStart(startPosition);
        currentArrow.GetComponent<Arrow>().SetEnd(mouseWorld);
        currentArrow.GetComponent<Arrow>().Draw();
    }}

    /*public void DrawArrow(){
        mouseWorld = Camera.main.ScreenToWorldPoint (
                new Vector3 (Input.mousePosition.x,
                Input.mousePosition.y,
                -2
                ));

        mouseWorld[2] = -2;
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
    }*/

    void OnMouseUp(){
        //Turn off the arrow
        //arrowLine.enabled = false;
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100);
        transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
    }

    
    public Vector3 snapToGrid(Vector3 input){
        float min_distance, current_distance;
        Vector3 closest_square = grid[0,0];
        min_distance = Vector3.Distance(grid[0,0], input);
        for (int i = 0; i < 8; i++){
            for (int j = 0; j < 8; j++){
                current_distance = Vector3.Distance(grid[i,j], input);
                if (current_distance < min_distance){
                    min_distance = current_distance;
                    closest_square = grid[i,j];
                }
                if (current_distance < 0.1){
                    return grid[i,j];
                }
            }
        }
        return closest_square;
    }
}
