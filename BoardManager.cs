using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public static BoardManager Instance { set; get; }
    private bool[, ] allowedMoves { set; get; }
    public Chessman[, ] Chessmans { set; get; }

    [SerializeField]
    private Chessman selectedChessman;
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    public ChessAgent agent;
    private int[] cells = new int[64];

    static int enemy = 6;
    static int king = 0;
    static int queen = 1;
    static int rook = 2;
    static int bishop = 3;
    static int horse = 4;
    static int pawn = 5;

    private int selectionX = -1;
    private int selectionY = -1;

    public bool isWhiteTurn = true;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activechessman = new List<GameObject> ();

    private Quaternion orientation = Quaternion.Euler (0, 180, 0);

    private void Start () {
        Instance = this;
        SpawnAllChessman ();
    }

    // Update is called once per frame
    void FixedUpdate () {
        UpdateSelection ();
        DrawChessboard ();

        /* 
        if (Input.GetMouseButtonDown (0)) {
            if (selectionX >= 0 && selectionY >= 0) {
                if (selectedChessman == null) {
                    //select chessman
                    SelectChessman (selectionX, selectionY);
                } else {
                    //move chessman
                    MoveChessman (selectionX, selectionY);
                }
            }
        }
        */

    }

    private void SelectChessman (int x, int y) {
        if (Chessmans[x, y] == null) {
            return;
        }
        if (Chessmans[x, y].isWhite != isWhiteTurn) {
            return;
        }

        bool hasAtleastOneMove = false;
        allowedMoves = Chessmans[x, y].PossibleMove ();
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (allowedMoves[i, j]) {
                    hasAtleastOneMove = true;
                }
            }
        }
        if (!hasAtleastOneMove) {
            return;
        }
        //Check (allowedMoves);
        selectedChessman = Chessmans[x, y];
        //Debug.Log ("SelectChessman , " + x + "  " + y + " , " + Chessmans[x, y] + "  " + selectedChessman);
        BoardHighlights.Instance.HighlightAllowedMoves (allowedMoves);
    }

    private void MoveChessman (int x, int y) {
        //Debug.Log("MoveChessman , " + x + "  " + y+" , "+Chessmans[x, y]+ "  "+ selectedChessman);
        //Debug.Log("allowedMove"+allowedMoves[x,y]);
        if (allowedMoves[x, y]) {
            Chessman c = Chessmans[x, y];
            if (c != null && c.isWhite != isWhiteTurn) {
                //Capture a piece
                //If it is the king
                if (c.GetType () == typeof (King)) {
                    EndGame ();
                    return;
                }

                activechessman.Remove (c.gameObject);
                Destroy (c.gameObject);
            }
            //Debug.Log("MoveChessman , " + x + "  " + y+" , "+Chessmans[x, y]+ "  "+ selectedChessman);
            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
            selectedChessman.transform.position = GetTileCenter (x, y);
            selectedChessman.SetPosition (x, y);
            Chessmans[x, y] = selectedChessman;
            isWhiteTurn = !isWhiteTurn;
        }
        BoardHighlights.Instance.HideHighlights ();
        selectedChessman = null;
    }

    private void UpdateSelection () {
        if (!Camera.main) {
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 25.0f, LayerMask.GetMask ("ChessPlane"))) {
            selectionX = (int) hit.point.x;
            selectionY = (int) hit.point.z;
            //Debug.Log (selectionX + "   " + selectionY);
        } else {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void SpawnChessman (int index, int x, int y) {
        GameObject go = Instantiate (chessmanPrefabs[index], GetTileCenter (x, y), orientation) as GameObject;
        go.transform.SetParent (transform);
        Chessmans[x, y] = go.GetComponent<Chessman> ();
        Chessmans[x, y].SetPosition (x, y);
        activechessman.Add (go);
    }

    private void SpawnAllChessman () {
        activechessman = new List<GameObject> ();
        Chessmans = new Chessman[8, 8];
        // Spawn white team
        SpawnChessman (king, 3, 0); //King
        SpawnChessman (queen, 4, 0); //Queen
        SpawnChessman (rook, 0, 0); //Rook
        SpawnChessman (rook, 7, 0); //Rook
        SpawnChessman (bishop, 2, 0); //Bishop
        SpawnChessman (bishop, 5, 0); //bishop
        SpawnChessman (horse, 1, 0); //horse
        SpawnChessman (horse, 6, 0); //horse
        for (int i = 0; i < 8; i++) {
            SpawnChessman (pawn, i, 1);
        }

        // Spawn black team
        SpawnChessman (enemy + king, 4, 7); //King
        SpawnChessman (enemy + queen, 3, 7); //Queen
        SpawnChessman (enemy + rook, 0, 7); //Rook
        SpawnChessman (enemy + rook, 7, 7); //Rook
        SpawnChessman (enemy + bishop, 2, 7); //Bishop
        SpawnChessman (enemy + bishop, 5, 7); //bishop
        SpawnChessman (enemy + horse, 1, 7); //horse
        SpawnChessman (enemy + horse, 6, 7); //horse
        for (int i = 0; i < 8; i++) {
            SpawnChessman (enemy + pawn, i, 6);
        }

    }

    private Vector3 GetTileCenter (int x, int y) {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    //Drawline
    private void DrawChessboard () {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++) {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine (start, start + widthLine);
            for (int j = 0; j <= 8; j++) {
                start = Vector3.right * j;
                Debug.DrawLine (start, start + heightLine);
            }
        }

        //Draw the selection
        if (selectionX >= 0 && selectionY >= 0) {
            Debug.DrawLine (
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));

            Debug.DrawLine (
                Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }

    //arraycheck

    public void Check (bool[, ] ch) {
        Debug.Log (ch[0, 0] + "," + ch[1, 0] + "," + ch[2, 0] + "," + ch[3, 0] + "," + ch[4, 0] + "," + ch[5, 0] + "," + ch[6, 0] + "," + ch[7, 0] + "\n" +
            ch[0, 1] + "," + ch[1, 1] + "," + ch[2, 1] + "," + ch[3, 1] + "," + ch[4, 1] + "," + ch[5, 1] + "," + ch[6, 1] + "," + ch[7, 1] + "\n" +
            ch[0, 0] + "," + ch[1, 2] + "," + ch[2, 2] + "," + ch[3, 2] + "," + ch[4, 2] + "," + ch[5, 2] + "," + ch[6, 2] + "," + ch[7, 2] + "\n" +
            ch[0, 0] + "," + ch[1, 3] + "," + ch[2, 3] + "," + ch[3, 3] + "," + ch[4, 3] + "," + ch[5, 3] + "," + ch[6, 3] + "," + ch[7, 3] + "\n" +
            ch[0, 0] + "," + ch[1, 4] + "," + ch[2, 4] + "," + ch[3, 4] + "," + ch[4, 4] + "," + ch[5, 4] + "," + ch[6, 4] + "," + ch[7, 4] + "\n" +
            ch[0, 0] + "," + ch[1, 5] + "," + ch[2, 5] + "," + ch[3, 5] + "," + ch[4, 5] + "," + ch[5, 5] + "," + ch[6, 5] + "," + ch[7, 5] + "\n" +
            ch[0, 0] + "," + ch[1, 6] + "," + ch[2, 6] + "," + ch[3, 6] + "," + ch[4, 6] + "," + ch[5, 6] + "," + ch[6, 6] + "," + ch[7, 6] + "\n" +
            ch[0, 0] + "," + ch[1, 7] + "," + ch[2, 7] + "," + ch[3, 7] + "," + ch[4, 7] + "," + ch[5, 7] + "," + ch[6, 7] + "," + ch[7, 7]
        );
    }

    private void EndGame () {
        if (isWhiteTurn) {
            Debug.Log ("White team Wins");
        } else {
            Debug.Log ("Black team Wins");
        }

        foreach (GameObject go in activechessman) {
            Destroy (go);
        }

        isWhiteTurn = true;
        BoardHighlights.Instance.HideHighlights ();
        SpawnAllChessman ();
    }

    public void AreaAction (int move) {
        //白のターンのとき処理しない
        if (!isWhiteTurn) {
            return;
        }
        //移動させたいマスに何も駒がないor白であれば動かせる
        if(move != -1 && (cells[move] == -1 || cells[move] >=enemy)){
            
        }
    }

    //(-2, 2)|(-1, 2)|(0, 2)|(1, 2)|(2, 2)
    //(-2, 1)|(-1, 1)|(0, 1)|(1, 1)|(2, 1)
    //(-2, 0)|(-1, 0)|(0, 0)|(1, 0)|(2, 0)
    //(-2,-1)|(-1,-1)|(0,-1)|(1,-1)|(3,-1)
    //(-2,-2)|(-1,-2)|(0,-2)|(1,-2)|(2,-2)

}