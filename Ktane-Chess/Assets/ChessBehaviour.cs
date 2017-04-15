 using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System;
 using System.Collections;
 using System.Linq;

public class ChessBehaviour : MonoBehaviour {
    private enum ChessPieceType {
        Knight,
        Queen,
        King,
        Bishop,
        Rook
    }
    private enum BoardState {
        Empty,
        Covered,
        Filled
    }
    private class ChessPiece {
        public int xCoord, yCoord;
        public ChessPieceType piece;

        public static bool getWhite(int x, int y) {
            return x % 2 != y % 2;
        }

        public ChessPiece(int x, int y, ChessPieceType p, ref BoardState[,] board) {
            xCoord = x;
            yCoord = y;
            piece = p;
            board[x, y] = BoardState.Filled;
        }
        public static void getRookTouchable(int x, int y, ref BoardState[,] myList, bool tempKing = false) {


            for (int i = x + 1; i < 6; i++) {
                if (myList[i, y] == BoardState.Empty) {
                    myList[i, y] = BoardState.Covered;
                } else if (myList[i, y] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = x - 1; i >= 0; i--) {
                if (myList[i, y] == BoardState.Empty) {
                    myList[i, y] = BoardState.Covered;
                } else if (myList[i, y] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = y + 1; i < 6; i++) {
                if (myList[x, i] == BoardState.Empty) {
                    myList[x, i] = BoardState.Covered;
                } else if (myList[x, i] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = y - 1; i >= 0; i--) {
                if (myList[x, i] == BoardState.Empty) {
                    myList[x, i] = BoardState.Covered;
                } else if (myList[x, i] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }
        }

        public static void getBishopTouchable(int x, int y, ref BoardState[,] myList, bool tempKing = false) {


            for (int i = x + 1, j = y + 1; j < 6 && i < 6; i++, j++) {
                if (myList[i, j] == BoardState.Empty) {
                    myList[i, j] = BoardState.Covered;
                } else if (myList[i, j] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = x - 1, j = y + 1; j < 6 && i >= 0; i--, j++) {
                if (myList[i, j] == BoardState.Empty) {
                    myList[i, j] = BoardState.Covered;
                } else if (myList[i, j] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = x + 1, j = y - 1; j >= 0 && i < 6; i++, j--) {
                if (myList[i, j] == BoardState.Empty) {
                    myList[i, j] = BoardState.Covered;
                } else if (myList[i, j] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }

            for (int i = x - 1, j = y - 1; j >= 0 && i >= 0; i--, j--) {
                if (myList[i, j] == BoardState.Empty) {
                    myList[i, j] = BoardState.Covered;
                } else if (myList[i, j] == BoardState.Filled) {
                    break;
                }
                if (tempKing) break;

            }
        }


        public static void getQueenTouchable(int x, int y, ref BoardState[,] myList) {
            getRookTouchable(x, y, ref myList);
            getBishopTouchable(x, y, ref myList);
        }

        public static void getKnightTouchable(int x, int y, ref BoardState[,] myList) {
            int newX, newY;
            newX = x + 2;
            newY = y + 1;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x - 2;
            newY = y + 1;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x + 2;
            newY = y - 1;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x - 2;
            newY = y - 1;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x + 1;
            newY = y + 2;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x - 1;
            newY = y + 2;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x + 1;
            newY = y - 2;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }
            newX = x - 1;
            newY = y - 2;
            if (newX >= 0 && newX < 6 && newY < 6 && newY >= 0 && myList[newX, newY] == BoardState.Empty) {
                myList[newX, newY] = BoardState.Covered;
            }

        }

        public static void getKingTouchable(int x, int y, ref BoardState[,] myList) {
            getRookTouchable(x, y, ref myList, true);
            getBishopTouchable(x, y, ref myList, true);
        }


        public void getTouchable(ref BoardState[,] myList) {
            switch (piece) {
                case ChessPieceType.Queen:
                    getQueenTouchable(xCoord, yCoord, ref myList);
                    break;
                case ChessPieceType.King:
                    getKingTouchable(xCoord, yCoord, ref myList);
                    break;
                case ChessPieceType.Bishop:
                    getBishopTouchable(xCoord, yCoord, ref myList);
                    break;
                case ChessPieceType.Knight:
                    getKnightTouchable(xCoord, yCoord, ref myList);
                    break;
                case ChessPieceType.Rook:
                    getRookTouchable(xCoord, yCoord, ref myList);
                    break;
            }
        }

    }

    public KMSelectable[] bottomButtons;
    public KMSelectable[] topButtons;
    public KMBombInfo mBombInfo;
    public KMBombModule mBombModule;
    public KMAudio mAudio;
    public TextMesh letterMesh;
    public TextMesh numberMesh;
    public MeshRenderer[] lightsArray;


    [HideInInspector]
    public bool isSolved = false;
    private int previousNumberSelected;
    private bool isLetterPressed;
    private char letterSelected;
    private string[][] generatedPairs;
    private string[] indexSelected;

    private static int ModuleId = 1;
    private int MyModuleId;

    private string[] ModuleLoggingStrings;

    public static bool TestRand(ref string[][] generatedPairs, int Sign, int MyModuleId, out string BoardString) {
        BoardString = "";
        try {
            string[] RandPieces = new string[6];
            string TestString = "";
            int ji = 0;
            while (ji < 6) {
                string temp = (char)(UnityEngine.Random.Range(0, 6) + 'a') + "" + (UnityEngine.Random.Range(0, 6) + 1);
                if (!TestString.Contains(temp)) { 
                    RandPieces[ji] = temp;
                    TestString += temp;
                    ji++;
                }
            }
            BoardState[,] chessBoard = new BoardState[6, 6];
            ChessPiece[] myPieces;
            for (int i = 0; i < 6; i++) {
                for (int j = 0; j < 6; j++) {
                    chessBoard[i, j] = BoardState.Empty;
                }
            }

            myPieces = new ChessPiece[6];
            myPieces[1] = new ChessPiece((RandPieces[1][0]) - 'a', (RandPieces[1][1]) - '1', (Sign == 0 ? ChessPieceType.Knight : ChessPieceType.Rook), ref chessBoard);
            myPieces[3] = new ChessPiece(RandPieces[3][0] - 'a', RandPieces[3][1] - '1', ChessPieceType.Rook, ref chessBoard);
            if (ChessPiece.getWhite(RandPieces[4][0] - 'a', RandPieces[4][1] - '1')) {
                myPieces[4] = new ChessPiece(RandPieces[4][0] - 'a', RandPieces[4][1] - '1', ChessPieceType.Queen, ref chessBoard);
                myPieces[0] = new ChessPiece(RandPieces[0][0] - 'a', RandPieces[0][1] - '1', ChessPieceType.King, ref chessBoard);
            } else {
                myPieces[4] = new ChessPiece(RandPieces[4][0] - 'a', RandPieces[4][1] - '1', ChessPieceType.Rook, ref chessBoard);
                myPieces[0] = new ChessPiece(RandPieces[0][0] - 'a', RandPieces[0][1] - '1', ChessPieceType.Bishop, ref chessBoard);
            }

            if (Sign == 1 || myPieces[4].piece == ChessPieceType.Rook) {
                myPieces[2] = new ChessPiece(RandPieces[2][0] - 'a', RandPieces[2][1] - '1', ChessPieceType.King, ref chessBoard);
            } else {
                myPieces[2] = new ChessPiece(RandPieces[2][0] - 'a', RandPieces[2][1] - '1', ChessPieceType.Queen, ref chessBoard);
            }

            bool hasQueen = false;
            bool hasKnight = false;
            for (int i = 0; i < 5; i++) {
                if (myPieces[i].piece == ChessPieceType.Queen) {
                    hasQueen = true;
                }
                if (myPieces[i].piece == ChessPieceType.Knight) {
                    hasKnight = true;
                }
            }
            if (!hasQueen) {
                myPieces[5] = new ChessPiece(RandPieces[5][0] - 'a', RandPieces[5][1] - '1', ChessPieceType.Queen, ref chessBoard);
            } else if (!hasKnight) {
                myPieces[5] = new ChessPiece(RandPieces[5][0] - 'a', RandPieces[5][1] - '1', ChessPieceType.Knight, ref chessBoard);
            } else {
                myPieces[5] = new ChessPiece(RandPieces[5][0] - 'a', RandPieces[5][1] - '1', ChessPieceType.Bishop, ref chessBoard);
            }
            foreach (ChessPiece i in myPieces) {
                i.getTouchable(ref chessBoard);
            }

            int empty = 0;
            List<string> emptyArr = new List<string>();
            for (int i = 5; i >= 0 && empty < 2; i--) {

                for (int j = 0; j < 6 && empty < 2; j++) {
                    if (chessBoard[j, i] == BoardState.Empty) {
                        empty++;
                        emptyArr.Add((char)(j + 'a') + "" + (i + 1));
                    }

                }
            }
            if (empty == 1) {
                for (int i = 0; i < 6; i++) {
                    generatedPairs[Sign][i] = RandPieces[i];
                }
                generatedPairs[Sign][6] = emptyArr[0];
                int[] Numbers = new int[6];
                int SolutionNumber;
                {
                    string s1 = generatedPairs[Sign][6];
                    SolutionNumber = ((s1[0] - 'a')) + 6 * (s1[1] - '1');
                    for(int i = 0; i < 6; i++) {
                        s1 = generatedPairs[Sign][i];
                        Numbers[i] = ((s1[0] - 'a')) + 6 * (s1[1] - '1');
                    }
                }
                StringBuilder s = new StringBuilder("┌───┬───┬───┬───┬───┬───┐");
                
                for(int i = 0; i < 11; i++) {
                    s.Append("\n");
                    if( i % 2 == 0) {
                        int v =  5 - (i / 2);
                        for(int j = 0; j < 6; j++) {
                            s.Append("│");
                            int o = Array.FindIndex(Numbers, x => x == v * 6 + j);
                            s.Append(" ");
                            if (o != -1) {
                                ChessPieceType c = myPieces[o].piece;
                                char pieceChar = ' ';
                                switch (c) {
                                    case ChessPieceType.Bishop:
                                        pieceChar = 'B';
                                        break;
                                    case ChessPieceType.King:
                                        pieceChar = 'K';
                                        break;
                                    case ChessPieceType.Queen:
                                        pieceChar = 'Q';
                                        break;
                                    case ChessPieceType.Knight:
                                        pieceChar = 'N';
                                        break;
                                    case ChessPieceType.Rook:
                                        pieceChar = 'R';
                                        break;
                                }
                                s.Append(pieceChar);
                            } else {
                                
                                if (v * 6 + j == SolutionNumber)
                                    s.Append('×');
                                else
                                    s.Append(" ");
                                
                            }
                            s.Append(" ");
                        }
                        s.Append("│");
                    } else {
                        s.Append("├───┼───┼───┼───┼───┼───┤");
                    }
                }
                s.Append("\n");
                s.Append("└───┴───┴───┴───┴───┴───┘");


                BoardString = s.ToString();

                return true;

            }
        }catch(UnityException e) {
            Debug.Log("[Chess #" + MyModuleId + "] TestRand exception: " + e.Message);
        }
        return false;
    }
    void SetSolveIndex() {
        indexSelected = new string[7];
        string str = "abcde0";
        if (mBombInfo != null) {
            List<string> list = mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, null);
            if (list.Count > 0)
                str = JsonConvert.DeserializeObject<Dictionary<string, string>>(list[0])["serial"];
        }
        indexSelected = generatedPairs[str[5] % 2];
        string MyDebugString = String.Join(", ",indexSelected);
        Debug.LogFormat("[Chess #{0}] Selected Solution: {1}\nBoard:\n{2}",MyModuleId,MyDebugString.ToUpper(),ModuleLoggingStrings[str[5]%2]);     
    }

    void CheckSolve(int num) {
        for(int i = 0; i < lightsArray.Length; i++) {
            lightsArray[i].material.color = new Color(0, 0, 0);
        }
        if (letterSelected.ToString() + (num + 1).ToString() == indexSelected[6]) {
            mBombModule.HandlePass();
            isSolved = true;
            DisplayCoords(-1);
        } else {
            TwitchPlayStrike = true;
            mBombModule.HandleStrike();
            DisplayCoords(previousNumberSelected);
        }
        isLetterPressed = false;
    }

    void DisplayCoords(int index) {
        if(index != previousNumberSelected)
            lightsArray[previousNumberSelected].material.color = new Color(0, 0, 0);

        if (index != -1) {
            letterMesh.text = indexSelected[index][0].ToString();
            numberMesh.text = indexSelected[index][1].ToString();
            lightsArray[index].material.color = new Color(0, 1, 0);
            previousNumberSelected = index;
        }else {
            letterMesh.text = "";
            numberMesh.text = "";
        }
    }

    void HandleLetters(char letter) {
        if(!isSolved)
            if (!isLetterPressed) {
                for (int i = 0; i < lightsArray.Length; i++) {
                    lightsArray[i].material.color = new Color(1, 0, 0);
                }
                isLetterPressed = true;
                 letterSelected = letter;
            }
    }

    void HandleNumbers(int num) {
        if(!isSolved)
            if (!isLetterPressed) {
                DisplayCoords(num);
            } else {
                Debug.Log("[Chess #" + MyModuleId + "] Entered answer: " + letterSelected.ToString() + (num + 1).ToString());
                CheckSolve(num);
            }
    }


    void Start() {
        MyModuleId = ModuleId++;
        ModuleLoggingStrings = new string[2];
        generatedPairs = new string[2][];
        
        for(int i = 0; i < 2; i++) {
            generatedPairs[i] = new string[7];
        }
        for(int i = 0; i < 2; i++) {
            for (int j = 0; !TestRand(ref generatedPairs, i, MyModuleId, out ModuleLoggingStrings[i]) && j < 2500; j++) ;
        }
        isLetterPressed = false;
        for(int i = 0; i < bottomButtons.Length; i++) {
            var temp = i;
            bottomButtons[i].OnInteract += delegate () { mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, bottomButtons[temp].transform); HandleNumbers(temp); return false; };
        }

        for(int i = 0; i < topButtons.Length; i++) {
            var temp = i;
            topButtons[i].OnInteract += delegate () { mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, topButtons[temp].transform); HandleLetters((char)('a' + temp)); return false; };
        }
        mBombModule.OnActivate += OnActivate;
    }

    void OnActivate() {
        SetSolveIndex();
        DisplayCoords(0);
    }

    private bool TwitchPlayStrike;
    IEnumerator ProcessTwitchCommand(string command) {
        TwitchPlayStrike = false;
        string[] split = command.ToLowerInvariant().Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

        if (split[0] == "cycle" && split.Length == 1) {
            for (var i = 0; i < 6; i++) {
                if (isSolved || TwitchPlayStrike)
                    yield break;
                yield return bottomButtons[i];
                yield return new WaitForSeconds(0.1f);
                yield return bottomButtons[i];
                yield return new WaitForSeconds(0.9f);
            }
            yield return bottomButtons[0];
            yield return new WaitForSeconds(0.1f);
            yield return bottomButtons[0];
        }
       
        else if (split[0] == "press") {
            foreach (string str in split.Skip(1)) {
                foreach (char c in str) {
                    if (!"abcdef123456".Contains(c))
                        yield break;
                }
            }

            foreach (string str in split.Skip(1)) {
                foreach (char c in str) {
                    int let = c - 'a';
                    int num = c - '1';
                    if (let >= 0 && let <= 5) {
                        yield return topButtons[let];
                        yield return new WaitForSeconds(0.1f);
                        yield return topButtons[let];
                    } else {
                        yield return bottomButtons[num];
                        yield return new WaitForSeconds(0.1f);
                        yield return bottomButtons[num];
                    }
                    if (isSolved || TwitchPlayStrike)
                        yield break;
                }
            }
        }
    }
}