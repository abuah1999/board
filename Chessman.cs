using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Chessman : MonoBehaviour
{
    // References
    public GameObject controller;
    public GameObject movePlate;

    // Variable to keep track of enpassantable pawns
    public bool enpassantable = false;
    
    // List of the coordinates and attack value ([0,1]) of the moveplates the piece can generate
    public List<int[]> mp_coords;

    // When generating MovePlates, should the piece check for checks on its own king.
    public bool check_for_checks = true;

    //Should the piece instantiate movePlates(true) or only mp_coords(false) 
    public bool mp_visible = true;

    //public List<bool> is_mp_attack;

    // Locations
    private int xBoard = -1;
    private int yBoard = -1;

    // (0 - 15)
    private int id;

    // Variable to keep track of turn
    private string player;

    // References for all the sprites the chesspiece can be
    public Sprite black_queen, black_knight, black_king, black_pawn, black_bishop, black_rook;
    public Sprite white_queen, white_knight, white_king, white_pawn, white_bishop, white_rook;

    // Update is called once per frame
    // If the player hits "f", flip all the pieces 180 degrees
    void Update()
    {
        if (Input.GetKeyDown("f")){
            // flip the pieces
            Vector3 temp = gameObject.transform.eulerAngles;
            temp.z += 180.0f;
            gameObject.transform.eulerAngles = temp;
        }
    }

    // Set the transform and sprite of the piece
    public void Activate(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        
        //take the instantiated location and adjust the transform
        SetCoords();

        //set the appropriate sprite based on the piece's name
        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black";break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black";break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black";break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black";break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black";break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black";break;
            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white";break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white";break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white";break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white";break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white";break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white";break;
        }
    }

    //take the instantiated board location (set in Game.Create()) and adjust the transform
    public void SetCoords(){
        float x = xBoard;
        float y = yBoard;

        x *= 1.1f;
        y *= 1.1f;

        x += -3.85f;
        y += -3.85f;

        this.transform.position = new Vector3(x,y,-1);
    }

    public int GetXBoard(){ return xBoard; }
    public int GetYBoard(){ return yBoard; }
    public void SetXBoard(int x){ xBoard = x; }
    public void SetYBoard(int y){ yBoard = y; }


    // Increase slightly the size of the sprite of obj
    public void Embiggen(GameObject obj){
        obj.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
        obj.GetComponent<SpriteRenderer>().size += new Vector2(0.04f, 0.04f);
        obj.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Simple;
    }

    // Decrease slightly the size of the sprite of obj
    public void Emsmallen(GameObject obj){
        obj.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
        obj.GetComponent<SpriteRenderer>().size -= new Vector2(0.04f, 0.04f);
        obj.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Simple;
    }

    public void OnMouseEnter(){
        if (!controller.GetComponent<Game>().analysis){
        if (controller.GetComponent<Game>().GetCurrentPlayer() == player &&
            !controller.GetComponent<Game>().selecton && 
            !controller.GetComponent<Game>().IsGameOver()){
                Embiggen(gameObject);
            }
        }
    }

    public void OnMouseExit(){
        if (!controller.GetComponent<Game>().analysis){
        if (controller.GetComponent<Game>().GetCurrentPlayer() == player &&
            !controller.GetComponent<Game>().selecton && 
            !controller.GetComponent<Game>().IsGameOver()){
                Emsmallen(gameObject);
            }
        }
    }

    private void OnMouseOver(){
        if (Input.GetMouseButtonUp(0)){
            if (!controller.GetComponent<Game>().analysis){
                //clicked on an already selected piece
                if (controller.GetComponent<Game>().selecton &&
                    controller.GetComponent<Game>().selected == gameObject){
                    controller.GetComponent<Game>().selecton = false;
                    DestroyMovePlates(true);
                }
                //clicked on another piece other than the selected one.
                else if (controller.GetComponent<Game>().selecton &&
                        controller.GetComponent<Game>().selected != gameObject &&
                        controller.GetComponent<Game>().GetCurrentPlayer() == player){
                            Embiggen(gameObject);
                            Emsmallen(controller.GetComponent<Game>().selected);
                            controller.GetComponent<Game>().selected = gameObject;
                            DestroyMovePlates(true);
                            InstantiateMovePlates();
                    }
                // clicked on a piece fresh
                else if (!controller.GetComponent<Game>().IsGameOver() && 
                    controller.GetComponent<Game>().GetCurrentPlayer() == player){
                        controller.GetComponent<Game>().selecton = true;
                        controller.GetComponent<Game>().selected = gameObject;
                        DestroyMovePlates(true);
                        InstantiateMovePlates();
                }
            }
            //Debug.Log(string.Join(" ", gameObject.GetComponent<Chessman>().mp_coords));
            //Debug.Log(controller.GetComponent<Game>().GetCurrentPlayer());
        }
            
    }

    // Destroy the piece's movePlates
    // Param leave_post_move_plate : false to destroy ALL movePlates
    /*public void DestroyMovePlates(bool leave_post_move_plates){
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        if (leave_post_move_plates){
            for (int i = 0; i < movePlates.Length; i++){
                if (movePlates[i].GetComponent<MovePlate>().GetReference().GetComponent<Chessman>().GetPlayer() == player &&
                    !movePlates[i].GetComponent<MovePlate>().post_move &&
                    !movePlates[i].GetComponent<MovePlate>().check){
                    Destroy(movePlates[i]);
                }
            }
        } else {
            for (int i = 0; i < movePlates.Length; i++){
                Destroy(movePlates[i]);
            }
        }
        
    }*/

    public void DestroyMovePlates(bool leave_post_move_plates){
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        
        if (leave_post_move_plates){
            for (int i = 0; i < movePlates.Length; i++){
                if (!movePlates[i].GetComponent<MovePlate>().post_move &&
                    !movePlates[i].GetComponent<MovePlate>().check){
                    Destroy(movePlates[i]);
                }
            }
        } else {
            for (int i = 0; i < movePlates.Length; i++){
                Destroy(movePlates[i]);
            }
        }
        
        
    }

    //public Tuple<List<Tuple<int, int>>, List<bool>> InstantiateMovePlates()
    
    
    public void InstantiateMovePlates(){
        mp_coords = new List<int[]>();
        //is_mp_attack = new List<bool>();
        
        switch (this.name)
        {
            case "black_queen": 
            case "white_queen": 
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;
            case "black_king":
            case "white_king":
                SurroundMovePlate();
                if ((controller.GetComponent<Game>().wqc || 
                    controller.GetComponent<Game>().bqc ||
                    controller.GetComponent<Game>().wkc ||
                    controller.GetComponent<Game>().bkc ) &&
                    // Don't generate castle movePlates when checking for checks
                    mp_visible){
                        CastleMovePlate();
                    }
                
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "black_pawn":
                if (yBoard == 6)
                {
                    PawnMovePlate(xBoard, yBoard - 1);
                    DoublePawnMovePlate(xBoard, yBoard - 2);
                }
                else if (yBoard == 1){
                    PromotionMovePlate(xBoard, yBoard - 1); 
                }
                else
                {
                    PawnMovePlate(xBoard, yBoard - 1);
                }
                break;
            case "white_pawn":
                 if (yBoard == 1)
                {
                    PawnMovePlate(xBoard, yBoard + 1);
                    DoublePawnMovePlate(xBoard, yBoard + 2);
                }
                else if (yBoard == 6){
                    PromotionMovePlate(xBoard, yBoard + 1); 
                }
                else
                {
                    PawnMovePlate(xBoard, yBoard + 1);
                }
                break;
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement){
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.LocationOnBoard(x,y) && sc.GetPosition(x,y) == null){
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.LocationOnBoard(x,y) && sc.GetPosition(x,y).GetComponent<Chessman>().player != player){
            MovePlateSpawn(x,y, "Attack");
        }
    }

    public void LMovePlate(){
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate(){
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard + 1, yBoard);
    }

    public void PointMovePlate(int x, int y){
        Game sc = controller.GetComponent<Game>();
        if (sc.LocationOnBoard(x,y)){
            GameObject cp = sc.GetPosition(x,y);

            if (cp == null){
                MovePlateSpawn(x,y);
            }
            else if (cp.GetComponent<Chessman>().player != player){
                MovePlateSpawn(x,y, "Attack");
            }
        }
    }

    public void PawnMovePlate(int x, int y){
        Game sc = controller.GetComponent<Game>();
        if (sc.LocationOnBoard(x, y)){
            if (sc.GetPosition(x,y) == null){
                MovePlateSpawn(x,y);
            }
            if (sc.LocationOnBoard(x+1,y)&&sc.GetPosition(x+1,y)!=null&&
                sc.GetPosition(x+1,y).GetComponent<Chessman>().player!=player) {
                MovePlateSpawn(x+1, y, "Attack");
            }
            if (sc.LocationOnBoard(x+1,y)&&sc.GetPosition(x+1, yBoard) !=null&&
                sc.GetPosition(x+1, yBoard).GetComponent<Chessman>().player!=player &&
                sc.GetPosition(x+1, yBoard).GetComponent<Chessman>().enpassantable){
                    MovePlateSpawn(x+1, y, "Enpassant");
                }
            if (sc.LocationOnBoard(x-1,y)&&sc.GetPosition(x-1,y)!=null&&
                sc.GetPosition(x-1,y).GetComponent<Chessman>().player!=player){
                MovePlateSpawn(x-1, y, "Attack");
            }
            if (sc.LocationOnBoard(x-1,y)&&sc.GetPosition(x-1, yBoard) !=null&&
                sc.GetPosition(x-1, yBoard).GetComponent<Chessman>().player!=player &&
                sc.GetPosition(x-1, yBoard).GetComponent<Chessman>().enpassantable){
                    MovePlateSpawn(x-1, y, "Enpassant");
                }
        }
    }

    public void DoublePawnMovePlate(int x, int y){
        Game sc = controller.GetComponent<Game>();
        if (sc.LocationOnBoard(x, y)){
            if (sc.GetPosition(x,y) == null){
                MovePlateSpawn(x,y, "Double");

            }
        }
    }

    public void PromotionMovePlate(int x, int y){
        Game sc = controller.GetComponent<Game>();
        if (sc.GetPosition(x,y) == null){
            MovePlateSpawn(x, y, "Promotion");
        }
        if (sc.LocationOnBoard(x+1,y)&&sc.GetPosition(x+1,y)!=null&&
            sc.GetPosition(x+1,y).GetComponent<Chessman>().player!=player){
                MovePlateSpawn(x+1, y, "PromotionAttack");
        }
        if (sc.LocationOnBoard(x-1,y)&&sc.GetPosition(x-1,y)!=null&&
            sc.GetPosition(x-1,y).GetComponent<Chessman>().player!=player){
                MovePlateSpawn(x-1, y, "PromotionAttack");
        }
    }

    public void CastleMovePlate(){
        int y;
        bool qc;
        bool kc;
        if (controller.GetComponent<Game>().GetCurrentPlayer() == "white") {
            y = 0; 
            //name = "white_rook";
            qc = controller.GetComponent<Game>().wqc;
            kc = controller.GetComponent<Game>().wkc;
        } else {
            y = 7;
            //name = "black_rook";
            qc = controller.GetComponent<Game>().bqc;
            kc = controller.GetComponent<Game>().bkc;
        }
        
        if (qc &&
            controller.GetComponent<Game>().GetPosition(3, y) == null &&
            controller.GetComponent<Game>().GetPosition(2, y) == null &&
            controller.GetComponent<Game>().GetPosition(1, y) == null &&
            !IsSquareCheck(3, y) &&
            !IsSquareCheck(2, y) &&
            !IsSquareCheck(1, y)){
            MovePlateSpawn(2, y, "QCastling");
                }
        if (kc &&
            controller.GetComponent<Game>().GetPosition(6, y) == null &&
            controller.GetComponent<Game>().GetPosition(5, y) == null &&
            !IsSquareCheck(6, y) &&
            !IsSquareCheck(5, y)){
            MovePlateSpawn(6, y, "KCastling");
                }
    }

    // look to see if there will be check if the piece moves to this square
    public bool IsSquareCheck(int x, int y){
        // save the initial position of the piece
        int a = xBoard; int b = yBoard;

        // Get opposite team
        GameObject[] target_team;
        if (player == "white"){
           target_team = controller.GetComponent<Game>().playerBlack;
        } else{
           target_team = controller.GetComponent<Game>().playerWhite;
        }
        // if there is an enemy piece at the target square, set it to null so that
        // it won't participate in the check for check
        GameObject target = controller.GetComponent<Game>().GetPosition(x, y);
        if(target){
            target_team[target.GetComponent<Chessman>().GetID()] = null;
        }

        // move the piece
        Move(x, y);

        // if there is check there, move the piece back, reset the target and return true
        if (controller.GetComponent<Game>().IsCheck()[2] == 1){
            Move(a, b);
            if(target){
                target_team[target.GetComponent<Chessman>().GetID()] = target;
                controller.GetComponent<Game>().SetPosition(target);
            }
            return true;
        }
        // if there is no check, move the piece back, reset the target and return false
        Move(a, b);
        if(target){
            target_team[target.GetComponent<Chessman>().GetID()] = target;
            controller.GetComponent<Game>().SetPosition(target);
        }
        return false;
    }

    public void MovePlateSpawn(int matrixX, int matrixY, string type = "Normal"){

        // before spawning, check if moving to this plate would result in a check
        // and if so, return (i.e do not spawn)
        if (check_for_checks && type != "Check" && type != "PostMove"){
            if (IsSquareCheck(matrixX, matrixY)){return;}
        }
        
        // Add this coordinate to movePlate coords
        int[] mp_coord = new int[3]; 
       
        if (type == "Attack" || type == "PromotionAttack")
        /* we don't need to include the enpassant case 
        since this is only for checking checks and you can't enpassant a king*/
        {
            mp_coord = new int[]{matrixX, matrixY, 1}; 
        } else {
            mp_coord = new int[]{matrixX, matrixY, 0}; 
        }
        
        mp_coords.Add(mp_coord);

        // for invisible moveplates, we're done
        if (!mp_visible){return;}

        // Instantiate movePlate and set requisite fields

        float x = matrixX;
        float y = matrixY;

        x *= 1.1f;
        y *= 1.1f;

        x += -3.9f;
        y += -3.9f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate myScript = mp.GetComponent<MovePlate>();

        switch (type){
            case "Normal" : break;
            case "Double": myScript.two = true; break;
            case "Promotion": myScript.promotion = true; break;
            case "Attack": myScript.attack = true; break;
            case "PromotionAttack":
                myScript.attack = true;
                myScript.promotion = true;
                break;
            case "Enpassant": myScript.enpassant = true; break;
            case "PostMove": myScript.post_move = true; break;
            case "Check": myScript.check = true; break;
            case "QCastling": myScript.q_castling = true; break;
            case "KCastling": myScript.k_castling = true; break;
        }

        myScript.SetReference(gameObject);
        myScript.SetCoords(matrixX, matrixY);
    }

    

    /*public void MovePlateCastleSpawn(int yp, string side){
        int xp;
        float x;
        if (side == "K"){
            x = 6.0f;
            xp = 6;
        }
        else{
            x = 2.0f;
            xp = 2;
        }
        
        float y = yp + 0.0f;

        x *= 1.1f;
        y *= 1.1f;

        x += -3.9f;
        y += -3.9f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate myScript = mp.GetComponent<MovePlate>();
        if (side == "K"){
            myScript.k_castling = true;
        } else {
            myScript.q_castling = true;
        }
        myScript.SetReference(gameObject);
        myScript.SetCoords(xp, yp);
    }*/

    public string GetPlayer(){
        return player;
    }

    public int GetID(){
        return id;
    }

    public void SetID(int num){
        id = num;
    }

    public void Move(int x, int y){
        controller.GetComponent<Game>().SetPositionEmpty(xBoard, yBoard);
        SetXBoard(x);
        SetYBoard(y);
        SetCoords();
        controller.GetComponent<Game>().SetPosition(gameObject);
    }

}
