using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{   
    public GameObject controller; 

    // Chessoiece that spawned the movePlate
    GameObject reference = null;

    //GameObject castling_rook = null;

    // Board positions, not world positions
    int matrixX;
    int matrixY;

    // fields which specify various types of movePlates when turned on or off
    public bool attack = false;
    public bool two = false;
    public bool enpassant = false;
    public bool k_castling = false;
    public bool q_castling = false;
    public bool promotion = false;
    public bool post_move = false;
    public bool check = false;
    public Sprite active_square, inactive_square;

    public void Start(){
        controller = GameObject.FindGameObjectWithTag("GameController");
        // Makes it transparent
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        if (controller.GetComponent<Game>().GetCurrentPlayer() == "black"){
            // Change green
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 0.4f);
        }
        if (attack || enpassant){
            // Change red
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        } else if (post_move){
            // Change the sprite and make it very transparent and change the z-transform
            gameObject.GetComponent<SpriteRenderer>().sprite = active_square;
            if (controller.GetComponent<Game>().GetCurrentPlayer() == "black"){
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            } else {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 0.2f);
            }
            
            gameObject.transform.position = new Vector3( gameObject.transform.position.x,
                                                         gameObject.transform.position.y,
                                                         -0.5f);
        } else if (check){
            // Change the sprite and make it red and make it very transparent and change the z-transform
            Debug.Log("Check!");
            gameObject.GetComponent<SpriteRenderer>().sprite = active_square;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.2f);
            gameObject.transform.position = new Vector3( gameObject.transform.position.x,
                                                         gameObject.transform.position.y,
                                                         -0.5f);
        }
    }
    public void Update(){
        if (controller.GetComponent<Game>().analysis){
            gameObject.layer = 2;
        } else {
            gameObject.layer = 0;
        }
    }
    public void OnMouseEnter(){
        gameObject.GetComponent<SpriteRenderer>().sprite = active_square;
    }

    public void OnMouseExit(){
        if (!post_move && !check){
            gameObject.GetComponent<SpriteRenderer>().sprite = inactive_square;
        }
    }

    public void OnMouseUp(){
        int y, init_X, init_Y;
        controller.GetComponent<Game>().enpassant_square = new int[]{-1,-1};
        controller.GetComponent<Game>().halfmove_clock++;
        // reset halfmove clock if a pawn is pushed
        if (reference.GetComponent<Chessman>().name == "white_pawn" ||
            reference.GetComponent<Chessman>().name == "black_pawn") {
                controller.GetComponent<Game>().halfmove_clock = 0;
            }
        if (controller.GetComponent<Game>().GetCurrentPlayer() == "white") {
            // for finding the appropriate rook in case of castling
            y = 0;
            } else {
                y = 7;
            }

        if (attack){
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);
            //if (cp.name == "white_king") controller.GetComponent<Game>().Winner("black");
            //if (cp.name == "black_king") controller.GetComponent<Game>().Winner("white");
            DestroyPiece(cp);
            controller.GetComponent<Game>().halfmove_clock = 0;
        }

        if (enpassant){
            // destroy captured pawn
            int yp = reference.GetComponent<Chessman>().GetYBoard();
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, yp);
            DestroyPiece(cp);
        }

        if (two){
            // if a pawn moved two squares, it is now enpassantable for one turn
            int ex = reference.GetComponent<Chessman>().GetXBoard();
            int ey = reference.GetComponent<Chessman>().GetYBoard();
            if (reference.GetComponent<Chessman>().GetPlayer() == "white"){
                controller.GetComponent<Game>().enpassant_square = new int[]{ex, ey+1};
            } else {
                controller.GetComponent<Game>().enpassant_square = new int[]{ex, ey-1};
            }
            //reference.GetComponent<Chessman>().enpassantable = true;
        }

        if (promotion){
            // Create a new piece (in the same positiona as the old one with the same ID), 
            // Set the new piece as reference and destroy the old one. 
            GameObject cp;
            GameObject[] team;
            int new_ID = reference.GetComponent<Chessman>().GetID();
            string new_piece = "black_queen";
            
            if (reference.name == "black_pawn"){
                team = controller.GetComponent<Game>().playerBlack;
                switch (controller.GetComponent<Game>().promotionPiece){
                    case "Q" : new_piece = "black_queen"; break;
                    case "K" : new_piece = "black_knight"; break;
                    case "R" : new_piece = "black_rook"; break;
                    case "B" : new_piece = "black_bishop"; break;
                }
                
            } else {
                team = controller.GetComponent<Game>().playerWhite;
                switch (controller.GetComponent<Game>().promotionPiece){
                    case "Q" : new_piece = "white_queen"; break;
                    case "K" : new_piece = "white_knight"; break;
                    case "R" : new_piece = "white_rook"; break;
                    case "B" : new_piece = "white_bishop"; break;
                }
            }
            cp = controller.GetComponent<Game>().Create(new_piece, 
                                                        reference.GetComponent<Chessman>().GetXBoard(),
                                                        reference.GetComponent<Chessman>().GetYBoard(), new_ID);
            team[new_ID] = cp;
            Destroy(reference);
            SetReference(cp);
        }

        if (k_castling){
            // Move the rook
            GameObject rook = controller.GetComponent<Game>().GetPosition(7,y);
            rook.GetComponent<Chessman>().Move(5,y);
        }

        if (q_castling){
            // Move the rook
            GameObject rook = controller.GetComponent<Game>().GetPosition(0,y);
            rook.GetComponent<Chessman>().Move(3,y);
        }

        if (post_move || check){return;}

        // Move the refernece piece, destroy ALL movePlates and generate new PostMovePlates
        init_X = reference.GetComponent<Chessman>().GetXBoard();
        init_Y = reference.GetComponent<Chessman>().GetYBoard();
        reference.GetComponent<Chessman>().Move(matrixX, matrixY);
        
        
        reference.GetComponent<Chessman>().Emsmallen(reference);
        controller.GetComponent<Game>().selecton = false;
        reference.GetComponent<Chessman>().DestroyMovePlates(false); 
        reference.GetComponent<Chessman>().mp_coords = new List<int[]>();
        reference.GetComponent<Chessman>().MovePlateSpawn(matrixX, matrixY, "PostMove");
        reference.GetComponent<Chessman>().MovePlateSpawn(init_X, init_Y, "PostMove");


        controller.GetComponent<Game>().NextTurn();
    }

    public void SetCoords (int x, int y){
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj){
        reference = obj;
    }

    public GameObject GetReference(){
        return reference;
    }

    public void DestroyPiece(GameObject obj){
        GameObject[] team;
        if (obj.GetComponent<Chessman>().GetPlayer() == "white"){
            team = controller.GetComponent<Game>().playerWhite;
        } else {
            team = controller.GetComponent<Game>().playerBlack;
        }
        for (int i = 0; i < team.Length; i++){
            if (team[i]){
                if (team[i].GetComponent<Chessman>().GetID() == obj.GetComponent<Chessman>().GetID()){
                    team[i] = null;
                }
            }
            
        }
        Destroy(obj);
    }


    
}
