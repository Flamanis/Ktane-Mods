using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System;
using System.Collections;
using System.Linq;

public class ChessBehaviour : MonoBehaviour
{


    #region ChessSim



    //Enums for different types of pieces, and spots on the board
    private enum ChessPieceType
    {
        Knight = 'N',
        Queen = 'Q',
        King = 'K',
        Bishop = 'B',
        Rook = 'R'
    }
    private enum BoardState
    {
        Empty,
        Covered,
        Occupied
    }

    //Class where the 'sim' happens. Contains the information about a piece, and static methods for each piece movement
    private class ChessPiece
    {
        public int xCoord, yCoord;
        public ChessPieceType piece;

        //Used to determine if the spot the piece is on is black or white
        public static bool GetWhite(int x, int y)
        {
            return x % 2 != y % 2;
        }

        //init, sets vars and occupies board space
        public ChessPiece(int x, int y, ChessPieceType p, ref BoardState[,] board)
        {
            xCoord = x;
            yCoord = y;
            piece = p;
            board[x, y] = BoardState.Occupied;
        }


        //Main check, checks if piece is in bounds, and attempts to set 'covered' state
        //Returns true if success, else false
        private static bool TrySetCover(int x, int y, ref BoardState[,] board)
        {
            bool inBounds = x >= 0 && x < board.GetLength(0) && y < board.GetLength(0) && y >= 0;
            if (inBounds && board[x, y] != BoardState.Occupied)
            {
                board[x, y] = BoardState.Covered;
                return true;
            }
            return false;

        }


        //Now to the fun bit.

        //Each set of 'touchable' and 'offsets' creates a search space. For rook and bishop it's a line to follow
        //For knight it's 8 specific spaces
        //King/Queen just use both rook and bishop, king having a bool for only checking a line one space deep


        private static int[,] rookOffsets = {
            {0, 1 },
            {1, 0 },
            {0, -1 },
            {-1, 0 }
        };

        //Use offsets, add to x/y, check space. If occupied or current piece is a king check next set
        private static void GetRookTouchable(int x, int y, ref BoardState[,] board, bool isKing = false)
        {
            for (int i = 0; i < rookOffsets.GetLength(0); i++)
                for (int j = x + rookOffsets[i, 0], k = y + rookOffsets[i, 1]; ; j += rookOffsets[i, 0], k += rookOffsets[i, 1])
                    if (!TrySetCover(j, k, ref board) || isKing)
                        break;
        }

        private static int[,] bishopOffsets = {
            {1, 1 },
            {1, -1 },
            {-1, 1 },
            {-1, -1 }
        };

        //Use offsets, add to x/y, check space. If occupied or current piece is a king check next set
        private static void GetBishopTouchable(int x, int y, ref BoardState[,] board, bool isKing = false)
        {
            for (int i = 0; i < bishopOffsets.GetLength(0); i++)
                for (int j = x + bishopOffsets[i, 0], k = y + bishopOffsets[i, 1]; ; j += bishopOffsets[i, 0], k += bishopOffsets[i, 1])
                    if (!TrySetCover(j, k, ref board) || isKing)
                        break;
        }

        private static int[,] knightOffsets = {
            {-2, 1},
            {-1, 2},
            {1, 2},
            {2, 1},
            {2, -1},
            {1, -2},
            {-1, -2},
            {-2, -1}
        };

        //Just check spaces around knight
        private static void GetKnightTouchable(int x, int y, ref BoardState[,] board)
        {
            for (int i = 0; i < knightOffsets.GetLength(0); i++)
                TrySetCover(x + knightOffsets[i, 0], y + knightOffsets[i, 1], ref board);

        }


        //Check rook and bishop

        private static void GetQueenTouchable(int x, int y, ref BoardState[,] board)
        {
            GetRookTouchable(x, y, ref board);
            GetBishopTouchable(x, y, ref board);
        }

        private static void GetKingTouchable(int x, int y, ref BoardState[,] board)
        {
            GetRookTouchable(x, y, ref board, true);
            GetBishopTouchable(x, y, ref board, true);
        }


        //Switch method to find which method to actually use.
        public void GetTouchable(ref BoardState[,] board)
        {
            switch (piece)
            {
                case ChessPieceType.Queen:
                    GetQueenTouchable(xCoord, yCoord, ref board);
                    break;
                case ChessPieceType.King:
                    GetKingTouchable(xCoord, yCoord, ref board);
                    break;
                case ChessPieceType.Bishop:
                    GetBishopTouchable(xCoord, yCoord, ref board);
                    break;
                case ChessPieceType.Knight:
                    GetKnightTouchable(xCoord, yCoord, ref board);
                    break;
                case ChessPieceType.Rook:
                    GetRookTouchable(xCoord, yCoord, ref board);
                    break;
            }
        }

    }

    #endregion

    #region imports
    //These are imports from the Unity Editor. Public variables that can be modified in the editor for ease of development

    //Buttons are the model buttons that can be clicked a-f 1-6
    public KMSelectable[] bottomButtons;
    public KMSelectable[] topButtons;

    //Bomb info is where info from the bomb is gotten
    public KMBombInfo mBombInfo;

    //Bomb module is the object itself, audio is a way to play audio
    public KMBombModule mBombModule;
    public KMAudio mAudio;

    //The meshes are just where text is displayed
    public TextMesh letterMesh;
    public TextMesh numberMesh;

    //The lights on the bottom
    public MeshRenderer[] lightsArray;

    public KMRuleSeedable ruleSeed;

    #endregion

    #region privateVars

    private const int BOARD_SIZE = 6;

    //This is for a separate mod made by somebody else. Just tracks when the module is solved. 
    [HideInInspector]
    public bool isSolved = false;

    //Tracks different stats
    private int previousNumberSelected = 1;
    private bool isLetterPressed = false;
    private int letterSelected;

    //The randomly generated coordinate pairs, 2 exclusive sets, then selected when the info becomes available
    private int[][] generatedPairs;
    private int[] indexSelected;
    private bool randInverted = false;
    private bool invertSerial = false;
    private int[] ruleSeedOrder;
    private bool ruleSeedDetected;

    //Module id stuff for logging
    private static int ModuleId = 1;
    private int MyModuleId;

    private string[] ModuleLoggingStrings;

    #endregion private



    //This is the randomizer and tester for the two exclusive sets
    //Sign is which set we're on and boardString is a logging string
    public static bool TestRand(ref int[][] generatedPairs, int sign, int MyModuleId, bool inverted, out string boardString)
    {
        boardString = "";

        //Try-catch just in case random randomness tomfoolery happens.
        try
        {
            if (inverted) sign += 2;
            //Randomly generate coordinate pairs in an int array. / 10 is x, % 10 is y
            int[] randPieces = new int[6];
            for (int i = 0; i < randPieces.Length; i++)
            {
                randPieces[i] = (BOARD_SIZE + 1) * 10;
            }
            int currentPieceNum = 0;
            while (currentPieceNum < 6)
            {
                int temp = UnityEngine.Random.Range(0, BOARD_SIZE) * 10 + UnityEngine.Random.Range(0, BOARD_SIZE);
                if (!randPieces.Contains(temp))
                {
                    randPieces[currentPieceNum] = temp;
                    currentPieceNum++;
                }
            }

            //Create board, default spaces to empty, and generate pieces based on the rules.
            BoardState[,] chessBoard = new BoardState[BOARD_SIZE, BOARD_SIZE];
            ChessPiece[] myPieces;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    chessBoard[i, j] = BoardState.Empty;
                }
            }

            myPieces = new ChessPiece[6];
            myPieces[1] = new ChessPiece(randPieces[1] / 10, randPieces[1] % 10, (sign % 2 == 0 ? ChessPieceType.Knight : ChessPieceType.Rook), ref chessBoard);
            myPieces[3] = new ChessPiece(randPieces[3] / 10, randPieces[3] % 10, ChessPieceType.Rook, ref chessBoard);
            if (ChessPiece.GetWhite(randPieces[4] / 10, randPieces[4] % 10) == !inverted)
            {
                myPieces[4] = new ChessPiece(randPieces[4] / 10, randPieces[4] % 10, ChessPieceType.Queen, ref chessBoard);
                myPieces[0] = new ChessPiece(randPieces[0] / 10, randPieces[0] % 10, ChessPieceType.King, ref chessBoard);
            }
            else
            {
                myPieces[4] = new ChessPiece(randPieces[4] / 10, randPieces[4] % 10, ChessPieceType.Rook, ref chessBoard);
                myPieces[0] = new ChessPiece(randPieces[0] / 10, randPieces[0] % 10, ChessPieceType.Bishop, ref chessBoard);
            }

            if (sign % 2 == 1 || myPieces[4].piece == ChessPieceType.Rook)
            {
                myPieces[2] = new ChessPiece(randPieces[2] / 10, randPieces[2] % 10, ChessPieceType.King, ref chessBoard);
            }
            else
            {
                myPieces[2] = new ChessPiece(randPieces[2] / 10, randPieces[2] % 10, ChessPieceType.Queen, ref chessBoard);
            }

            bool hasQueen = false;
            bool hasKnight = false;
            for (int i = 0; i < 5; i++)
            {
                if (myPieces[i].piece == ChessPieceType.Queen)
                {
                    hasQueen = true;
                }
                if (myPieces[i].piece == ChessPieceType.Knight)
                {
                    hasKnight = true;
                }
            }

            myPieces[5] = new ChessPiece(randPieces[5] / 10, randPieces[5] % 10,
                !hasQueen ? ChessPieceType.Queen : !hasKnight ? ChessPieceType.Knight : ChessPieceType.Bishop,
                ref chessBoard);

            //All pieces generated, find covered area
            foreach (ChessPiece i in myPieces)
            {
                i.GetTouchable(ref chessBoard);
            }


            //Check board for empty spaces
            int empty = 0;
            int emptyNum = 0;
            for (int i = 5; i >= 0 && empty < 2; i--)
            {

                for (int j = 0; j < 6 && empty < 2; j++)
                {
                    if (chessBoard[j, i] == BoardState.Empty)
                    {
                        empty++;
                        emptyNum = j * 10 + i;
                    }

                }
            }

            //If there's too many spaces
            if (empty != 1)
                return false;

            //We've made it this far, let's start some logging and set the piece locations. 

            for (int i = 0; i < 6; i++)
            {
                generatedPairs[sign][i] = randPieces[i];
            }
            generatedPairs[sign][6] = emptyNum;



            StringBuilder s = new StringBuilder("┌───┬───┬───┬───┬───┬───┐");

            //Starting piece at top
            int y = BOARD_SIZE - 1;

            //For each level, there's 11 levels alternating between space and wall, ending with space. 
            for (int level = 0; level < BOARD_SIZE * 2 - 1; level++)
            {

                //Newline, if we're at an odd place, we're at a wall line, just add to string
                s.Append("\n");
                if (level % 2 != 0)
                {
                    s.Append("├───┼───┼───┼───┼───┼───┤");
                    continue;
                }

                //Character blocks are 3 wide.

                //Loop for each x location, start with a wall and a space
                for (int x = 0; x < 6; x++)
                {
                    s.Append("│");
                    s.Append(" ");

                    //Set a dummy character as a space and check what character to put in it.
                    //If a piece is in that slot, convert Type to char, if it's the solution, put that x thingy
                    char spot = ' ';
                    for (int i = 0; i < 6; i++)
                    {
                        if (randPieces[i] != x * 10 + y)
                            continue;


                        spot = (char)myPieces[i].piece;
                    }
                    if (x * 10 + y == emptyNum)
                        spot = '×';

                    //Add character and an additional space
                    s.Append(spot);
                    s.Append(" ");
                }
                //End wall, lower down 'y' count
                s.Append("│");
                y--;

            }
            s.Append("\n");
            s.Append("└───┴───┴───┴───┴───┴───┘");


            boardString = s.ToString();

            return true;
        }
        catch (UnityException e)
        {
            Debug.Log("[Chess #" + MyModuleId + "] TestRand exception: " + e.Message);
        }
        return false;
    }

    //Get information from bomb (In this case last digit of serial number even/odd)
    //Set pairs accordingly, and then log
    void SetSolveIndex()
    {
        indexSelected = new int[7];
        string str = "abcde0";
        if (mBombInfo != null)
        {
            List<string> list = mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, null);
            if (list.Count > 0)
                str = JsonConvert.DeserializeObject<Dictionary<string, string>>(list[0])["serial"];
        }
        //Serial + invertSerial % 2 inverts the serial polarity. And randInverted will invert the board state which should invert color decisions.
        int ind = ((str[5] + (invertSerial ? 1 : 0)) % 2) + (randInverted ? 2 : 0);
        indexSelected = generatedPairs[ind];

        string[] logArr = new string[7];
        for (int i = 0; i < 7; i++)
        {
            logArr[i] = "" + (char)(indexSelected[i] / 10 + 'A') + (indexSelected[i] % 10 + 1);
        }
        string ruleSeedStr = "";
        if (ruleSeedDetected)
        {
            string[] ruleSeedArr = new string[6];
            for (int i = 0; i < 6; i++)
            {
                ruleSeedArr[i] = "" + (ruleSeedOrder[i] + 1);
            }
            ruleSeedStr = string.Format("\nRule Seed Detected. Rules are in order: {0} | The board is{1} inverted | The serial number is{2} inverted", string.Join(", ", ruleSeedArr), randInverted ? "" : " not", invertSerial ? "" : " not");
        }
        string MyDebugString = String.Join(", ", logArr);
        Debug.LogFormat("[Chess #{0}] Selected Solution: {1}\nBoard:\n{2}{3}", MyModuleId, MyDebugString.ToUpper(), ModuleLoggingStrings[ind], ruleSeedDetected ? ruleSeedStr : "");
    }

    //Set the display and the lights based on the piece selected. If index is -1 then wipe the display
    void DisplayCoords(int index)
    {

        //If we're on a different index then wipe the old index's indicator.
        //Only is false if the ROOM lights turn off then on.
        if (index != previousNumberSelected)
            lightsArray[previousNumberSelected].material.color = new Color(0, 0, 0);

        //Determine if wiping or not, if not modify display and set indicator.
        if (index != -1)
        {
            letterMesh.text = "" + (char)(indexSelected[index] / 10 + 'a');
            numberMesh.text = "" + (char)(indexSelected[index] % 10 + '1');
            lightsArray[index].material.color = new Color(0, 1, 0);
            previousNumberSelected = index;
        }
        else
        {
            letterMesh.text = "";
            numberMesh.text = "";
        }
    }

    //Handler for each letter button. Int specifying which button this specific one is
    bool HandleLetters(int letter)
    {

        //Play audio at button location
        mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, topButtons[letter].transform);

        //If we've not solved or pressed a button then set colors of indicators to red
        //Save which one we pressed
        if (!isSolved)
            if (!isLetterPressed)
            {
                for (int i = 0; i < lightsArray.Length; i++)
                {
                    lightsArray[i].material.color = new Color(1, 0, 0);
                }
                isLetterPressed = true;
                letterSelected = letter;
            }
        return false;
    }

    //Simple checker for checking the solve.
    //We have the 'letter' they selected saved, num is the number
    //So letter = 4, num = 3 is E4
    void CheckSolve(int num)
    {
        //Turn off all lights
        for (int i = 0; i < lightsArray.Length; i++)
        {
            lightsArray[i].material.color = new Color(0, 0, 0);
        }

        //If the solution is correct, pass and wipe the display, else have a strike.
        if (letterSelected * 10 + num == indexSelected[6])
        {
            mBombModule.HandlePass();
            isSolved = true;
            DisplayCoords(-1);
        }
        else
        {
            TwitchPlayStrike = true;
            mBombModule.HandleStrike();
            DisplayCoords(previousNumberSelected);
        }
        isLetterPressed = false;
    }


    //Handler for the number buttons, num is the button we're using.
    bool HandleNumbers(int num)
    {
        //Play audio
        mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, bottomButtons[num].transform);

        //If we're not solved check if a letter has been pressed, if not display new stuff, else check solve.
        if (!isSolved)
            if (!isLetterPressed)
            {
                DisplayCoords(num);
            }
            else
            {
                Debug.Log("[Chess #" + MyModuleId + "] Entered answer: " + (char)(letterSelected + 'a') + (num + 1));
                CheckSolve(num);
            }
        return false;
    }

    void RuleSeedHandler(MonoRandom rand)
    {
        ruleSeedDetected = true;
        //Determine order of rules.
        int[] order = { 0, 1, 2, 3, 4, 5 };
        order = rand.ShuffleFisherYates(order);

        ruleSeedOrder = order;
        //Randomize order
        for (int i = 0; i < 4; i++)
        {
            int[] pairs = new int[7];
            for (int j = 0; j < 6; j++)
            {
                pairs[j] = generatedPairs[i][order[j]];
            }
            pairs[6] = generatedPairs[i][6];
            generatedPairs[i] = pairs;
        }

        bool invertX = rand.Next() % 2 == 0;
        bool invertY = rand.Next() % 2 == 0;
        bool swap = rand.Next() % 2 == 0;
        bool invertField = rand.Next() % 2 == 1;

        int invert = 0;
        if (invertX) invert++;
        if (invertY) invert++;
        //Swapping letters and numbers do not cause an inversion of colors
        //if (swap) invert++;
        if (invertField) invert++;

        randInverted = invert % 2 == 1;

        invertSerial = rand.Next() % 2 == 0;



    }

    //On activate by room, happens when lights turn on for the first time.
    void OnActivate()
    {
        //Choose solution based on bomb, display default coordinates.
        SetSolveIndex();
        DisplayCoords(0);
    }

    //Init for module because Unity.
    void Start()
    {




        //Initialize stuff, save module id.
        MyModuleId = ModuleId++;
        ModuleLoggingStrings = new string[4];
        generatedPairs = new int[4][];

        for (int i = 0; i < 4; i++)
        {
            generatedPairs[i] = new int[7];
        }

        //Generate random coordinate pairs
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; !TestRand(ref generatedPairs, i, MyModuleId, false, out ModuleLoggingStrings[i]) && j < 2500; j++) ;

        }
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; !TestRand(ref generatedPairs, i, MyModuleId, true, out ModuleLoggingStrings[2 + i]) && j < 2500; j++) ;

        }
        var rand = ruleSeed.GetRNG();
        if (rand.Seed != 1)
        {
            RuleSeedHandler(rand);
        }


        //Set interact handlers for all buttons
        for (int i = 0; i < bottomButtons.Length; i++)
        {
            var temp = i;
            bottomButtons[i].OnInteract += () => { return HandleNumbers(temp); };
        }

        for (int i = 0; i < topButtons.Length; i++)
        {
            var temp = i;
            topButtons[i].OnInteract += () => { return HandleLetters(temp); };
        }

        //Wait for activate
        mBombModule.OnActivate += OnActivate;
    }



    //Twitch plays stuff, separate mod. Written by CaitSith2.
    private bool TwitchPlayStrike;
    IEnumerator ProcessTwitchCommand(string command)
    {
        TwitchPlayStrike = false;
        string[] split = command.ToLowerInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        if (split[0] == "cycle" && split.Length == 1)
        {
            for (var i = 0; i < 6; i++)
            {
                if (isSolved || TwitchPlayStrike)
                    yield break;
                yield return bottomButtons[i];
                yield return new WaitForSeconds(0.1f);
                yield return bottomButtons[i];
                yield return new WaitForSeconds(1.4f);
            }
            yield return bottomButtons[0];
            yield return new WaitForSeconds(0.1f);
            yield return bottomButtons[0];
        }

        else if (split[0] == "press")
        {
            foreach (string str in split.Skip(1))
            {
                foreach (char c in str)
                {
                    if (!"abcdef123456".Contains(c))
                        yield break;
                }
            }

            foreach (string str in split.Skip(1))
            {
                foreach (char c in str)
                {
                    int let = c - 'a';
                    int num = c - '1';
                    if (let >= 0 && let <= 5)
                    {
                        yield return topButtons[let];
                        yield return new WaitForSeconds(0.1f);
                        yield return topButtons[let];
                    }
                    else
                    {
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