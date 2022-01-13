using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    public GameObject chesspiece;
    // Locations and team for each piece
    private GameObject[,] locations = new GameObject[8,8];
    public GameObject[] playerBlack = new GameObject[16];
    public GameObject[] playerWhite = new GameObject[16];

    // Player whose turn it is to move
    private string currentPlayer = "white";

    // Is the game over?
    private bool gameOver = false;

    // Default promotion piece is Queen
    public string promotionPiece = "Q";

    // wqc - White Queenside Castling (true if White King and White Queen's Rook haven't moved)
    // wkc - White Kingside Castling
    // bqc - Black Queenside Castling
    // bkc - Black Kingside Castling
    public bool wqc = true;
    public bool wkc = true;
    public bool bqc = true;
    public bool bkc = true;

    // Has a piece been selected to move?
    public bool selecton = false;

    // Analysis mode: selecting pieces temporarily paused
    public bool analysis = true;

    // Selected piece
    public GameObject selected = null;

    // Start is called before the first frame update
    void Start()
    {
        
        playerWhite = new GameObject[]{
            Create("white_rook",0,0,0), Create("white_knight",1,0,1), Create("white_bishop",2,0,2), 
            Create("white_queen",3,0,3), Create("white_king",4,0,4), Create("white_bishop",5,0,5),
            Create("white_knight",6,0,6), Create("white_rook",7,0,7), Create("white_pawn",0,1,8),
            Create("white_pawn",1,1,9), Create("white_pawn",2,1,10), Create("white_pawn",3,1,11),
            Create("white_pawn",4,1,12), Create("white_pawn",5,1,13), Create("white_pawn",6,1,14),
            Create("white_pawn",7,1,15)
        };

        playerBlack = new GameObject[]{
            Create("black_rook",0,7,0), Create("black_knight",1,7,1), Create("black_bishop",2,7,2), 
            Create("black_queen",3,7,3), Create("black_king",4,7,4), Create("black_bishop",5,7,5),
            Create("black_knight",6,7,6), Create("black_rook",7,7,7), Create("black_pawn",0,6,8),
            Create("black_pawn",1,6,9), Create("black_pawn",2,6,10), Create("black_pawn",3,6,11),
            Create("black_pawn",4,6,12), Create("black_pawn",5,6,13), Create("black_pawn",6,6,14),
            Create("black_pawn",7,6,15)
        };

        //Set starting positions

        for (int i = 0; i < playerBlack.Length; i++){
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    // Instantiate a Chessman, set the required fields, and activate it
    // Params: name of the piece, board coords, id (0 - 15)
    public GameObject Create(string name, int x, int y, int id){
        GameObject obj = Instantiate(chesspiece, new Vector3(0,0,-1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.SetID(id);
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    // Updates ChessPiece position in locations grid according to the
    // value of the piece's xBoard and yBoard fields.
    public void SetPosition(GameObject obj){
        Chessman cm = obj.GetComponent<Chessman>();

        locations[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    // Sets that position in the locations grid to null.
    public void SetPositionEmpty(int x, int y){
        locations[x, y] = null;
    }

    // Returns Chesspiece at board position (x, y)
    public GameObject GetPosition(int x, int y){
        return locations[x, y];
    }

    // Returns false if (x, y) is not a valid board position
    public bool LocationOnBoard(int x, int y){
        if (x < 0 || y < 0 || x >= locations.GetLength(0) || y >= locations.GetLength(1)) return false;
        return true;
    }

    // Return current player
    public string GetCurrentPlayer(){
        return currentPlayer;
    }

    // Return true if game is over
    public bool IsGameOver(){
        return gameOver;
    }

    // IsCheck[2] == 1 or 0 depending on if the king of the cureent player is in check or not.
    // if IsCheck[2] == 1, (IsCheck[0], IsCheck[1]) are the board coords of the checked king.
    /*public int[] IsCheck(){
        GameObject[] enemies = new GameObject[16];
        if (currentPlayer == "white"){
            enemies = playerBlack;
        } else {
            enemies = playerWhite;
        }
        foreach (var v in enemies){
            if (v){
                // A king can't check another king
                if (v.GetComponent<Chessman>().GetID() != 4){
                    v.GetComponent<Chessman>().check_for_checks = false;
                    v.GetComponent<Chessman>().InstantiateMovePlates();
                    foreach (var c in v.GetComponent<Chessman>().mp_coords){
                        if (GetPosition(c[0], c[1]) == null){
                            continue;
                        } else if ((GetPosition(c[0], c[1]).GetComponent<Chessman>().name == "white_king" ||
                                GetPosition(c[0], c[1]).GetComponent<Chessman>().name == "black_king") &&
                                c[2] == 1){
                            v.GetComponent<Chessman>().check_for_checks = true;
                            v.GetComponent<Chessman>().DestroyMovePlates(true);    
                            // check!       
                            return new int[]{c[0], c[1], 1};            
                        }
                    }
                    v.GetComponent<Chessman>().check_for_checks = true;
                    v.GetComponent<Chessman>().DestroyMovePlates(true); 
                }
            } 
        }
        // no check
        return new int[]{0,0,0};
    }*/

    public int[] IsCheck(){
        GameObject[] enemies = new GameObject[16];
        string our_king;
        if (currentPlayer == "black"){
            enemies = playerWhite;
            our_king = "black_king";
        } else {
            enemies = playerBlack;
            our_king = "white_king";
        }
        foreach (var v in enemies){
            if (v){
                // A king can't check another king (or can he... ?)
                if (true){
                    v.GetComponent<Chessman>().check_for_checks = false;
                    v.GetComponent<Chessman>().mp_visible = false;
                    v.GetComponent<Chessman>().InstantiateMovePlates();
                    foreach (var c in v.GetComponent<Chessman>().mp_coords){
                        if (GetPosition(c[0], c[1]) == null){
                            continue;
                        } else if (GetPosition(c[0], c[1]).GetComponent<Chessman>().name == our_king &&
                                c[2] == 1){
                            v.GetComponent<Chessman>().mp_visible = true;        
                            v.GetComponent<Chessman>().check_for_checks = true;
                            //v.GetComponent<Chessman>().DestroyMovePlates(true);    
                            // check!       
                            return new int[]{c[0], c[1], 1};            
                        }
                    }
                    v.GetComponent<Chessman>().check_for_checks = true;
                    v.GetComponent<Chessman>().mp_visible = true;
                    //v.GetComponent<Chessman>().DestroyMovePlates(true); 
                }
            } 
        }
        // no check
        return new int[]{0,0,0};
    }

    // -1 : noramal, 0 : stalemate, 1 : checkmate
    public int IsCheckmate(){
        
        GameObject[] friends = new GameObject[16];
        if (currentPlayer == "white"){
            friends = playerWhite;
        } else {
            friends = playerBlack;
        }

        foreach (var v in friends){
            if (v){
                //v.GetComponent<Chessman>().check_for_checks = false;
                v.GetComponent<Chessman>().mp_visible = false;
                v.GetComponent<Chessman>().InstantiateMovePlates();
                if (v.GetComponent<Chessman>().mp_coords.Count > 0){
                    //v.GetComponent<Chessman>().check_for_checks = true;
                    //v.GetComponent<Chessman>().DestroyMovePlates(true);
                    v.GetComponent<Chessman>().mp_visible = true;
                    return -1;
                }
                v.GetComponent<Chessman>().mp_visible = true;
                //v.GetComponent<Chessman>().check_for_checks = true;
                //v.GetComponent<Chessman>().DestroyMovePlates(true); 
            }
        }
        if (IsCheck()[2] == 1){return 1;}
        else {return 0;}
    }

    // Performs checks for castling privileges,
    // Resets promotion piece to Queen,
    // Pawns that moved two squares on the last turn can no longer be captured enpassant.
    // (i.e they will no longer have an enpassant capture movePlate spawned on them by
    // an enemy pawn)
    // Change CurrentPlayer
    public void NextTurn(){
        
        promotionPiece = "Q";
        if (currentPlayer == "white"){
            foreach (var v in playerBlack){
                if (v){
                    v.GetComponent<Chessman>().enpassantable = false;
                }     
            }
            currentPlayer = "black";
        } else {
            foreach (var v in playerWhite){
                if (v){
                    v.GetComponent<Chessman>().enpassantable = false;
                }    
            }
            currentPlayer = "white";
        }
        if (wqc){
            if (!(locations[0,0] != null && locations[0,0].name == "white_rook" &&
                locations[4,0] != null && locations[4,0].name == "white_king")) wqc = false;
        }
        if (bqc){
            if (!(locations[0,7] != null && locations[0,7].name == "black_rook" &&
                locations[4,7] != null && locations[4,7].name == "black_king")) bqc = false;
        }
        if (wkc){
            if (!(locations[7,0] != null && locations[7,0].name == "white_rook" &&
                locations[4,0] != null && locations[4,0].name == "white_king")) wkc = false;
        }
        if (bkc){
            if (!(locations[7,7] != null && locations[7,7].name == "black_rook" &&
                locations[4,7] != null && locations[4,7].name == "black_king")) bkc = false;
        }
        if (IsCheck()[2] == 1){
            GameObject king = GetPosition(IsCheck()[0], IsCheck()[1]);
            king.GetComponent<Chessman>().mp_coords = new List<int[]>();
            //Debug.Log(king.GetComponent<Chessman>().mp_visible.ToString());
            king.GetComponent<Chessman>().MovePlateSpawn(IsCheck()[0], IsCheck()[1], "Check");
        }
        
        if (IsCheckmate() == 1){
            if (currentPlayer == "white"){Winner("black");}
            else {Winner("white");}
            return;
        }

        if (IsCheckmate() == 0){
            Draw();
        }    

    }

    // Called every frame
    // If the player hits "q" reset the board
    // If the player hits "k", "b", or "r", set the promotion piece for that turn accordingly
    // If the player hits "a", toggle analysis mode
    public void Update(){
        if ((gameOver == true && Input.GetMouseButtonDown(0)) ||
            Input.GetKeyDown("q")){
            gameOver = false;
            SceneManager.LoadScene("game");
        } else if (gameOver == false && Input.GetKeyDown("k")){
            promotionPiece = "K";
        } else if (gameOver == false && Input.GetKeyDown("b")){
            promotionPiece = "B";
        } else if (gameOver == false && Input.GetKeyDown("r")){
            promotionPiece = "R";
        } else if (gameOver == false && Input.GetKeyDown("a") && !analysis && !selecton){
            analysis = true;
        } else if (gameOver == false && Input.GetKeyDown("a") && !analysis && selecton){
            analysis = true;
            selecton = false;
            selected. GetComponent<Chessman>().Emsmallen(selected);/*<SpriteRenderer>().size -= new Vector2(0.05f, 0.05f);
            selected. GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Simple;*/
            selected. GetComponent<Chessman>().DestroyMovePlates(true);
            selected = null;
        } else if (gameOver == false && Input.GetKeyDown("a") && analysis){
            analysis = false;
        }
    }

    //Takes as input the name of the winning side, stops the game and shows a message.
    public void Winner(string playerwinner){
        gameOver = true;

        GameObject.FindGameObjectWithTag("right_text").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("right_text").GetComponent<Text>().text = playerwinner + " is victorious.";
        GameObject.FindGameObjectWithTag("left_text").GetComponent<Text>().enabled = true;
    }

    public void Draw(){
        gameOver = true;
        GameObject.FindGameObjectWithTag("right_text").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("right_text").GetComponent<Text>().text = "draw.";
        GameObject.FindGameObjectWithTag("left_text").GetComponent<Text>().enabled = true;

    }

    
}
