﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : Chessman {
    public override bool[, ] PossibleMove () {
        bool[, ] r = new bool[8, 8];
        //UpLeft
        KnightMove (CurrentX - 1, CurrentY + 2, ref r);
        //UpRight
        KnightMove (CurrentX + 1, CurrentY + 2, ref r);
        //RightUp
        KnightMove (CurrentX + 2, CurrentY + 1, ref r);
        //RightDown
        KnightMove (CurrentX + 2, CurrentY - 1, ref r);
        //DownLeft
        KnightMove (CurrentX - 1, CurrentY - 2, ref r);
        //DownRight
        KnightMove (CurrentX + 1, CurrentY - 2, ref r);
        //LeftUp
        KnightMove (CurrentX - 2, CurrentY + 1, ref r);
        //LeftDown
        KnightMove (CurrentX - 2, CurrentY - 1, ref r);

        return r;
    }

    public void KnightMove (int x, int y, ref bool[, ] r) {
        Chessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8) {
            c = BoardManager.Instance.Chessmans[x, y];
            if (c == null) {
                r[x, y] = true;
            } else if (isWhite != c.isWhite) {
                r[x, y] = true;
            }
        }
    }
    
    //(-2, 2)|(-1, 2)|(0, 2)|(1, 2)|(2, 2)
    //(-2, 1)|(-1, 1)|(0, 1)|(1, 1)|(2, 1)
    //(-2, 0)|(-1, 0)|(0, 0)|(1, 0)|(2, 0)
    //(-2,-1)|(-1,-1)|(0,-1)|(1,-1)|(3,-1)
    //(-2,-2)|(-1,-2)|(0,-2)|(1,-2)|(2,-2)
    
}
