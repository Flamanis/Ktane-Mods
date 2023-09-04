using System.Collections;
using System.Collections.Generic;
using UnityEngine;    

public class MorseWar : MonoBehaviour {
    

    //Imports
    public KMSelectable subButton;
    public KMSelectable supButton;
    public MeshRenderer[] topRow;
    public MeshRenderer[] midRow;
    public MeshRenderer[] botRow;
    public MeshRenderer morseLight;
    public KMBombModule module;

    private KMAudio audio;

    private static int ModuleCount = 1;

    private int ModuleNum = 0;


    //Behavior booleans. Can't press buttons if they aren't true-false
    private bool canSolve = false;
    
    private bool isSolved = false;

    //Color statics for the lights
    private static Color lightOn = new Color(.39f,1f,.3f);
    private static Color lightOff = new Color(0,0,0);
    private static Color morseOn = new Color(1f, 1f, .2f);


    //Length of the morse dot in seconds
    private float dotLength = .3f;


    //Solution values
    private string solution = "";

    private string workingSolution = "";

    private string lights = "";

    private int wordNum;

    private int tableNum;


    //Tables from the manual transcribed as strings
    private static string[] RowTable = { "1100", "1010", "1001", "0110", "0101", "0011" };

    private static string[] WordTable = {"abr","rbs","svr", "zux", "zaq", "moi", "opa", "vzq", "xrp", "oll", "air", "rhg", "mjn", "vtt", "xzs", "sun" };

    private static string[] MorseChart =
    {
        "01",
        "1000",
        "1010",
        "100",
        "0",
        "0010",
        "110",
        "0000",
        "00",
        "0111",
        "101",
        "0100",
        "11",
        "10",
        "111",
        "0110",
        "1101",
        "010",
        "000",
        "1",
        "001",
        "0001",
        "011",
        "1001",
        "1011",
        "1100"
    };

    private static string[] FireTable = {"ssss","usss", "suss", "uuss", "ssus", "usus", "suus", "uuus", "sssu", "ussu", "susu", "uusu", "ssuu", "usuu", "suuu", "uuuu", };

    private static byte[,,] AFTables = new byte[6,6,6]{
        {//A
            {5,4,3,2,1,4 },
            {6,7,6,5,8,3 },
            {7,8,1,4,7,2 },
            {8,1,2,3,6,1 },
            {1,2,3,4,5,8 },
            {2,3,4,5,6,7 },
        },
        {//B
            {6,5,4,3,2,5 },
            {7,8,7,6,1,4 },
            {8,1,2,5,8,3 },
            {1,2,3,4,7,2 },
            {2,3,4,5,6,1 },
            {3,4,5,6,7,8 },
        },
        {//C
            {7,6,5,4,3,6 },
            {8,1,8,7,2,5 },
            {1,2,3,6,1,4 },
            {2,3,4,5,8,3 },
            {3,4,5,6,7,2 },
            {4,5,6,7,8,1 },
        },
        {//D
            {8,7,6,5,4,7 },
            {1,2,1,8,3,6 },
            {2,3,4,7,2,5 },
            {3,4,5,6,1,4 },
            {4,5,6,7,8,3 },
            {5,6,7,8,1,2 },
        },
        {//E
            {1,8,7,6,5,8 },
            {2,3,2,1,4,7 },
            {3,4,5,8,3,6 },
            {4,5,6,7,2,5 },
            {5,6,7,8,1,4 },
            {6,7,8,1,2,3 },
        },
        {//F
            {2,1,8,7,6,1 },
            {3,4,3,2,5,8 },
            {4,5,6,1,4,7 },
            {5,6,7,8,3,6 },
            {6,7,8,1,2,5 },
            {7,8,1,2,3,4 },
        }
    };


    //Coroutine run when a strike is generated.
    //Waits time and then resets module.
    IEnumerator StrikeHandler()
    {
        yield return new WaitForSeconds(5);
        canSolve = true;
        GenerateSolution();
        yield return new WaitForSeconds(2);
        StartCoroutine("LightBlinker");
    }
    

    //Run for each button press, plays audio, adds to working solution, and then checks for pass/strike
    bool ButtonPressed(char but)
    {
        //Play audio and then check if we can do anything
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (isSolved || !canSolve) return false;

        //Else, add to the working solution and check if the solution is pass/strike worthy
        workingSolution += but;
        if(workingSolution.Length == solution.Length)
        {
            string finString = "";
            //Check if the solution is passed, otherwise if it's a strike start the strike handler coroutine
            if(workingSolution == solution)
            {
                module.HandlePass();
                isSolved = true;
                finString = "PASS, disabling module";
            }
            else
            {
                module.HandleStrike();
                canSolve = false;
                StartCoroutine("StrikeHandler");
                finString = "STRIKE, resetting module";
            }
            Debug.Log(string.Format("[Morse War #{0}] Answer submitted: {1}\nCorrect Answer: {2}\nResult: {3}",ModuleNum, workingSolution.ToUpper(), solution.ToUpper(), finString));
            //Stop the morse light, set all lights to off, reset variable.
            StopCoroutine("LightBlinker");
            morseLight.material.color = lightOff;
            for (int i = 0; i < topRow.Length; i++)
            {
                topRow[i].material.color = lightOff;
                midRow[i].material.color = lightOff;
                botRow[i].material.color = lightOff;
            }
            workingSolution = "";
        }
        return false;
    }

    //Generates the solutions for the module.
    void GenerateSolution()
    {
        //Generate lights string by random number
        lights = "";
        for(int j = 0; j < 3; j++)
        {
            lights += UnityEngine.Random.Range(0, 6) + 1;
        }

        //Temp array so I can just loop through the three rows and light them up correctly.
        MeshRenderer[][] rows = { botRow, midRow, topRow };
        for (int i = 0; i < rows.Length; i++)
        {
            string layer = RowTable[lights[i]-'1'];
            for(int j = 0; j < layer.Length; j++)
            {
                Color c;
                if (layer[j] == '1')
                    c = lightOn;
                else
                    c = lightOff;

                rows[i][j].material.color = c;
            }
        }
        

        tableNum = AFTables[lights[0] - '1', lights[2] - '1', lights[1] - '1'];
        wordNum = UnityEngine.Random.Range(0, 16);
        solution = FireTable[(wordNum + tableNum - 1) % 16];

        Debug.Log(string.Format("[Morse War #{0}] Generated random values: \nMorse word = {1}\nTable position generated: {2}\nResulting number from table: {3}\nFinal solution: {4}",ModuleNum , WordTable[wordNum],"" + (char)(lights[0] - '1' + 'A') + (char)(lights[1] - '1' + 'A') + lights[2], tableNum, solution.ToUpper()));
    }
    

    //Coroutine for the morse code light.
    IEnumerator LightBlinker()
    {
        //Initial delay just to make it not instantly start playing on bomb start
        yield return new WaitForSeconds(.1f);
        

        //Get the morse word from the chart as an array
        string word = WordTable[wordNum];
        string[] morse = new string[3];
        for (int i = 0; i < word.Length; i++)
        {
            morse[i] = MorseChart[word[i] - 'a'];
        }


        //Basically an infinite loop, let's be honest.
        while (!isSolved)
        {
            //For each character, get the set of morse characters, loop through that, then set the color and delay for dots and dashes
            //Dots are 1 length, dashes are 3, each element is separated by 1, each character separated by 3, each word separated by 7
            for (int morseChar = 0; morseChar < morse.Length; morseChar++)
            {
                string currentWord = morse[morseChar];
                for (int cha = 0; cha < currentWord.Length; cha++)
                {
                    morseLight.material.color = morseOn;
                    if (currentWord[cha] == '0')
                        yield return new WaitForSeconds(dotLength);
                    else
                        yield return new WaitForSeconds(dotLength * 3);
                    morseLight.material.color = lightOff;
                    yield return new WaitForSeconds(dotLength);
                }
                yield return new WaitForSeconds(dotLength * 2);
            }
            yield return new WaitForSeconds(dotLength * 4);
        }


        yield return new WaitForSeconds(20);
    }

    //Idk man
    void OnActivate()
    {
        canSolve = true;
        GenerateSolution();
        StartCoroutine("LightBlinker");

    }

	//Initial actions
	void Start () {
        ModuleNum = ModuleCount++;
        subButton.OnInteract += () => { return ButtonPressed('u'); } ;
        supButton.OnInteract += () => { return ButtonPressed('s'); } ;
        module.OnActivate += OnActivate;
        audio = GetComponent<KMAudio>();
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press USSU [press submarine, supply, supply, submarine]";
#pragma warning restore 414

    List<KMSelectable> ProcessTwitchCommand(string command)
    {
        command = command.ToUpperInvariant().Trim();

        if (!command.StartsWith("press ", StringComparison.OrdinalIgnoreCase))
            return null;

        var buttons = new List<KMSelectable>();

        for (int i = 6; i < command.Length; i++)
        {
            if (command[i] == ' ')
                continue;
            else if (command[i] == 'U')
                buttons.Add(subButton);
            else if (command[i] == 'S')
                buttons.Add(supButton);
            else
                return null;
        }

        return buttons;
    }
}
