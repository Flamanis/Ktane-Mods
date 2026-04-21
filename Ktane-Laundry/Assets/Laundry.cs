using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

public class Laundry : MonoBehaviour
{
    #region BombStuff
    private class Battery
    {
        public int numbatteries;
    }
    private class Serial
    {
        public string serial;
    }
    private class Indicator
    {
        public string label;
        public string on;
    }
    private class Ports
    {
        public string[] PresentPorts;
    }

    #endregion

    #region Imports
    //Bomb info
    public KMBombInfo bombInfo;

    //Different selectables
    public KMSelectable coinSlot;
    public KMSelectable[] knobs;
    public KMSelectable[] slidersTop;
    public KMSelectable[] slidersBottom;

    //Displays
    public TextMesh ironingTextDisplay;
    public TextMesh specialTextDisplay;

    public Material[] washingDisplay;
    public Material[] dryingDisplay;

    public MeshRenderer leftKnobDisplay;
    public MeshRenderer rightKnobDisplay;

    //Audio
    public KMAudio sound;

    //Module itself   
    public KMBombModule bombModule;
    #endregion

    #region Tables

    //Enums for instructions.
    private enum LWash
    {
        Perm = 0,
        Gentle,
        Hand,
        DoNot,
        OneDot,
        TwoDot,
        ThreeDot,
        FourDot,
        FiveDot,
        SixDot,
        Wring
    }

    private enum LDry
    {
        Tumble = 0,
        OneDot,
        TwoDot,
        ThreeDot,
        NoHeat,
        Hang,
        Drip,
        Flat,
        Shade,
        DoNotDry,
        DoNotTumble,
        Dry
    }

    private enum LIron
    {
        Iron,
        DoNot,
        OneDot,
        TwoDot,
        ThreeDot,
        NoSteam,
    }

    private enum LSpec
    {
        Bleach,
        DoNotBleach,
        ChlorineBleach,
        Dryclean,
        AnySolvent,
        Tetrachlore,
        Petroleum,
        WetCleaning,
        DoNotDryclean,
        ShortCycle,
        ReducedMoisture,
        LowHeat,
        NoSteam
    }


    private enum LClothing
    {
        Corset = 0,
        Shirt,
        Skirt,
        Skort,
        Shorts,
        Scarf
    }
    private enum LMaterial
    {
        Polyester = 0,
        Cotton,
        Wool,
        Nylon,
        Corduroy,
        Leather
    }
    private enum LColor
    {
        Ruby = 0,
        Star,
        Sapphire,
        Jade,
        Clouded,
        Malinite
    }

    //Manual hard coding
    private static Enum[,] ClothingType = 
    {
        { LWash.FourDot, LDry.Flat, LIron.TwoDot, LSpec.Bleach }, //Corset
        { LWash.TwoDot, LDry.ThreeDot, LIron.NoSteam, LSpec.Tetrachlore }, //Shirt
        { LWash.OneDot, LDry.Hang, LIron.Iron, LSpec.ReducedMoisture }, //Skirt
        { LWash.Gentle, LDry.Tumble, LIron.ThreeDot, LSpec.ReducedMoisture }, //Skort
        { LWash.Wring, LDry.Shade, LIron.TwoDot, LSpec.DoNotBleach }, //Shorts
        { LWash.SixDot, LDry.Dry, LIron.OneDot, LSpec.DoNotDryclean } //Scarf
    };

    private static Enum[,] MaterialType = 
    { 
        { LWash.ThreeDot, LDry.NoHeat, LIron.TwoDot, LSpec.Petroleum }, //Polyester
        { LWash.SixDot, LDry.TwoDot, LIron.Iron, LSpec.DoNotDryclean }, //Cotton
        { LWash.Hand, LDry.Shade, LIron.ThreeDot, LSpec.ReducedMoisture }, //Wool
        { LWash.OneDot, LDry.Drip, LIron.NoSteam, LSpec.LowHeat }, //Nylon
        { LWash.TwoDot, LDry.Drip, LIron.OneDot, LSpec.WetCleaning }, //Corduroy
        { LWash.DoNot, LDry.DoNotDry, LIron.DoNot, LSpec.Tetrachlore } //Leather
    };

    private static Enum[,] ColorType = 
    { 
        { LWash.FourDot, LDry.ThreeDot, LIron.DoNot, LSpec.AnySolvent }, //Ruby Fountain
        { LWash.SixDot, LDry.Flat, LIron.Iron, LSpec.LowHeat }, //Star Lemon
        { LWash.OneDot, LDry.Tumble, LIron.ThreeDot, LSpec.ShortCycle }, //Sapphire Springs
        { LWash.OneDot, LDry.DoNotTumble, LIron.TwoDot, LSpec.NoSteam }, //Jade Cluster
        { LWash.Perm, LDry.OneDot, LIron.NoSteam, LSpec.LowHeat }, //Clourded Pearl
        { LWash.FourDot, LDry.TwoDot, LIron.ThreeDot, LSpec.ChlorineBleach } //Malinite
    };


    //Different texts for showing on the module and logging
    private static string[] washingText = { "Machine Wash Permanent Press", "Machine Wash Gentle or Delicate", "Hand Wash", "Do Not Wash", "30°C", "40°C", "50°C", "60°C", "70°C", "95°C", "Do Not Wring" };
    private static string[] dryingText = { "Tumble Dry", "1 Dot", "2 Dot", "3 Dot", "No Heat", "Hang to Dry", "Drip Dry", "Dry Flat", "Dry in the Shade", "Do Not Dry", "Do Not Tumble Dry", "Dry" };
    private static string[] ironingText = { "Iron", "Do Not Iron", "110°C", "300°F", "200°C", "No Steam" };
    private static string[] specialText = { "Bleach", "Don't Bleach", "No Chlorine", "Dryclean", "Any Solvent", "No Tetrachlore", "Petroleum Only", "Wet Cleaning", "Don't Dryclean", "Short Cycle", "Reduced Moist", "Low Heat", "No Steam Finish" };
    private static string[] clothingNames = { "Corset", "Shirt", "Skirt", "Skort", "Shorts", "Scarf" };
    private static string[] materialNames = { "Polyester", "Cotton", "Wool", "Nylon", "Corduroy", "Leather" };
    private static string[] colorNames = { "Ruby Fountain", "Star Lemon Quartz", "Sapphire Springs", "Jade Cluster", "Clouded Pearl", "Malinite" };

    #endregion

    #region Internals

    //Index positions for changes
    private int ironingTextPos = 0;
    private int specialTextPos = 0;
    private int leftKnobPos = 0;
    private int rightKnobPos = 0;

    //Info from bomb
    private int totalIndicators = 0;
    private int batteryHolders = 0;
    private int totalPorts = 0;
    private int totalBatteries = 0;
    private int lastDigitSerial = 0;
    private bool hasBOB = false;
    private string serialNum = "";

    //Angle to rotate knobs
    private float washingRotate;
    private float dryingRotate;

    //Solution and if solved
    private Enum[][] solutions;
    private bool isSolved = false;

    #endregion


    private static int ModuleID = 1;

    private int myModuleId;

    //Get all of the bomb info
    void GetBombValues()
    {

        //Get ports stuff
        List<string> portList = bombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_PORTS, null);
        foreach (string port in portList)
        {
            Ports i = JsonConvert.DeserializeObject<Ports>(port);

            totalPorts += i.PresentPorts.Length;

        }

        //Get Serial number information.
        Serial serialNumber = JsonConvert.DeserializeObject<Serial>(bombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, null)[0]);
        lastDigitSerial = serialNumber.serial[5] - '0';
        serialNum = serialNumber.serial.ToLower();


        //Get batteries information
        List<string> batteryList = bombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, null);
        batteryHolders = batteryList.Count;


        foreach (string batteries in batteryList)
        {
            Battery i = JsonConvert.DeserializeObject<Battery>(batteries);
            totalBatteries += i.numbatteries;
        }

        //Get indicators information and check for lit bob
        List<string> indicatorList = bombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_INDICATOR, null);
        totalIndicators = indicatorList.Count;

        foreach (string indicators in indicatorList)
        {
            Indicator i = JsonConvert.DeserializeObject<Indicator>(indicators);
            if (i.on == "True" && i.label == "BOB")
            {
                hasBOB = true;
            }
        }

        solutions = new Enum[6][];
        
        for (int i = 0; i < 6; i++)
        {
            StringBuilder s = new StringBuilder();
            solutions[i] = GetSolutionValues(ref s, bombInfo.GetSolvableModuleNames().Count, i);
            Debug.Log(s);
        }
    }

    //Gets the solution for a specific set of 'solved' modules. Pregenerated at 1-5.
    Enum[] GetSolutionValues(ref StringBuilder logString, int totalCount, int solvedCount)
    {
        //Start the logging string. 
        logString.AppendFormat("[Laundry #{0}]: Solution values for {1} solved modules: \n", myModuleId, solvedCount);

        //Check for lit bob case, which is our only 'always win' condition
        if (hasBOB && totalBatteries == 4 && batteryHolders == 2)
        {
            logString.Append("We got a Lit Bob and four batteries in two holders.\n");
            return new Enum[1];
        }

        //Generate solution array, and indexes of the different pieces of the items and log that
        Enum[] solutionStates = new Enum[4];
        int itemClothing = (totalCount - solvedCount + totalIndicators + 6) % 6;
        int itemMaterial = ((totalPorts + solvedCount - batteryHolders) % 6 + 6) % 6;
        int itemColor = (lastDigitSerial + totalBatteries + 6) % 6;
        logString.AppendFormat("Item: {0} ({1}), Material: {2} ({3}), Color: {4} ({5})\n", clothingNames[itemClothing], itemClothing, materialNames[itemMaterial], itemMaterial, colorNames[itemColor], itemColor);


        //Check each conditional in the 'Special rules' area of the manual
        bool cloudedPearl = itemColor == (int)LColor.Clouded;
        bool leatherJadeCluster = itemMaterial == (int)LMaterial.Leather || itemColor == (int)LColor.Jade;
        bool corsetCorduroy = itemClothing == (int)LClothing.Corset || itemMaterial == (int)LMaterial.Corduroy;
        bool woolStarLemon = itemMaterial == (int)LMaterial.Wool || itemColor == (int)LColor.Star;
        bool materialSerial = false;

        foreach (char c in materialNames[itemMaterial].ToLower())
        {
            if (serialNum.Contains(c + ""))
            {
                materialSerial = true;
            }
        }


        //Set values based on 'Special rules' and log accordingly

        if (leatherJadeCluster)
        {
            logString.Append("Washing is 80°F, ");
            solutionStates[0] = LWash.OneDot;
        }
        else
        {
            logString.Append("Washing on Material, ");
            solutionStates[0] = MaterialType[itemMaterial, 0];
        }

        if (woolStarLemon)
        {
            logString.Append("Drying is 3 Dot, ");
            solutionStates[1] = LDry.ThreeDot;
        }
        else
        {
            logString.Append("Drying on Color, ");
            solutionStates[1] = ColorType[itemColor, 1];
        }

        logString.Append("Ironing on Item, ");
        solutionStates[2] = ClothingType[itemClothing, 2];

        if (cloudedPearl)
        {
            logString.Append("Special is Non-Chlorine Bleach\n");
            solutionStates[3] = LSpec.ChlorineBleach;
        }
        else if (corsetCorduroy)
        {
            logString.Append("Special on Material\n");
            solutionStates[3] = MaterialType[itemMaterial, 3];
        }
        else if (materialSerial)
        {
            logString.Append("Special on Color\n");
            solutionStates[3] = ColorType[itemColor, 3];
        }
        else
        {
            logString.Append("Special on Item\n");
            solutionStates[3] = ClothingType[itemClothing, 3];
        }





        //Finish the logging with the end results
        logString.Append("End result: ");
        logString.AppendFormat("{0}, ", washingText[(int)(LWash)solutionStates[0]]);
        logString.AppendFormat("{0}, ", dryingText[(int)(LDry)solutionStates[1]]);
        logString.AppendFormat("{0}, ", ironingText[(int)(LIron)solutionStates[2]]);
        logString.AppendFormat("{0}", specialText[(int)(LSpec)solutionStates[3]]);
        logString.Append("\n");
        return solutionStates;
    }


    //Function for the left/right buttons on the ironing text display, just increments/decrements index and updates accordingly
    void IroningSlide (int sign)
    {
        ironingTextPos += sign;
        ironingTextPos = (ironingTextPos + ironingText.Length) % ironingText.Length;
        ironingTextDisplay.text = ironingText[ironingTextPos];
    }

    //Function for the left/right buttons on the special text display, just increments/decrements index and updates accordingly
    void SpecialSlide (int sign)
    {
        specialTextPos += sign;
        specialTextPos = (specialTextPos + specialText.Length) % specialText.Length;
        specialTextDisplay.text = specialText[specialTextPos];
    }

    //Functions for the knob buttons. Rotate them around based on how many choices there are, update the picture.
    void LeftKnob()
    {
        leftKnobPos ++ ;
        leftKnobPos = (leftKnobPos + washingDisplay.Length) % washingDisplay.Length;
        knobs[0].transform.Rotate(new Vector3(0, washingRotate, 0));
        leftKnobDisplay.material =  washingDisplay[leftKnobPos];
        sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, knobs[0].transform);
    }

    void RightKnob()
    {
        rightKnobPos++;
        rightKnobPos = (rightKnobPos + dryingDisplay.Length) % dryingDisplay.Length;
        knobs[1].transform.Rotate(new Vector3(0, dryingRotate, 0));
        rightKnobDisplay.material = dryingDisplay[rightKnobPos];
        sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, knobs[1].transform);
    }
    

     

    

    //Functions for handling pass and strike, really useless, but nice to have.
    void HandlePass()
    {
        bombModule.HandlePass();
    }

    void HandleStrike()
    {
        bombModule.HandleStrike();
    }


    //Called when the coin slot is activated
    void UseCoin()
    {
        //Play the sound, it's a laundry machine after all
        sound.PlaySoundAtTransform("Insert Coin", coinSlot.transform);

        if (!isSolved) {
            //Start 'submitted' logging string.
            StringBuilder s = new StringBuilder();
            int solvedCount = bombInfo.GetSolvedModuleNames().Count % 6;
            s.AppendFormat("[Laundry #{1}]: Coin pressed at {0} modules solved\n", solvedCount, myModuleId);
            Enum[] solution = solutions[solvedCount];
            s.AppendFormat("Entered Values: {0}, {1}, {2}, {3}\n", washingText[leftKnobPos], dryingText[rightKnobPos], ironingText[ironingTextPos], specialText[specialTextPos]);

            if(solution.Length == 1)
            {
                isSolved = true;
                s.Append("Pass");
                HandlePass();
                return;
            }

            //Bools for checking if entered index are the solution indexes
            bool WashingCorrect = (int)(LWash)solution[0] == leftKnobPos;
            bool DryingCorrect = (int)(LDry)solution[1] == rightKnobPos;
            bool IroningCorrect = (int)(LIron)solution[2] == ironingTextPos;
            bool SpecialCorrect = (int)(LSpec)solution[3] == specialTextPos;

            //If so, then we pass, else we fail.
            if (WashingCorrect && DryingCorrect && IroningCorrect && SpecialCorrect) {
                isSolved = true;
                s.Append("Pass");
                HandlePass();
            } else {
                s.Append("Strike");
                HandleStrike();
            }
            Debug.Log(s.ToString());
        }
    }







    //Init function
    void Start()
    {
        //Grab id and rotation increments of the knobs based on their picture list
        myModuleId = ModuleID++;
        washingRotate = 360.0f / washingDisplay.Length;
        dryingRotate = 360.0f / dryingDisplay.Length;

        //Add handler for bomb activation
        bombModule.OnActivate += GetBombValues;

        

        
        //Add handler for each button
        for (int i = 0; i < slidersTop.Length; i++)
        {
            var j = i;
            //*2-1 gets -1 or 1.
            slidersTop[i].OnInteract += () => { IroningSlide(j * 2 - 1); sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, slidersTop[j].transform); return false; };

        }
        for (int i = 0; i < slidersBottom.Length; i++)
        {
            var j = i;
            slidersBottom[i].OnInteract +=  () => { SpecialSlide(j * 2 - 1); sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, slidersBottom[j].transform); return false; };

        }

        coinSlot.OnInteract += () => { UseCoin(); return false; }; 

        knobs[0].OnInteract += () => { LeftKnob(); return false; };

        knobs[1].OnInteract += () => { RightKnob(); return false; };

        //Set the text to display what it should
        ironingTextDisplay.text = ironingText[ironingTextPos];
        specialTextDisplay.text = specialText[specialTextPos];

    }


    //All below is CaitSith2 for Twitch Plays.
    static Dictionary<string, int> washIndex = new Dictionary<string, int>()
        {
            {"machinewashpermanentpress", 0},{"permanentpress", 0},

            {"machinewashgentleordelicate", 1},{"machinewashgentle", 1},
            {"machinewashdelicate", 1},{"gentle", 1},{"delicate", 1},

            {"handwash", 2},

            {"donotwash", 3},{"dontwash", 3},

            {"30", 4},{"30c", 4},{"30°c", 4},{"80", 4},{"80f", 4},{"80°f", 4},{"1dot", 4},

            {"40",5 },{"40c",5 },{"40°c",5 },{"105",5 },{"105f",5 },{"105°f",5 },{"2 dot",5 },

            {"50",6 },{"50c",6},{"50°c",6 },{"120",6 },{"120f",6 },{"120°f",6 },{"3dot",6 },

            {"60",7 },{"60c",7},{"60°c",7 },{"140",7 },{"140f",7 },{"140°f",7 },{"4dot",7 },

            {"70",8 },{"70c",8},{"70°c",8 },{"160",8 },{"160f",8 },{"160°f",8 },{"5dot",8 },

            {"95",9 },{"95c",9},{"95°c",9 },{"200",9 },{"200f",9 },{"200°f",9 },{"6dot",9 },
            {"why",9 },

            {"donotwring",10 }, {"dontwring",10 },
        };
    static Dictionary<string, int> dryIndex = new Dictionary<string, int>()
        {
            {"tumbledry",0 }, {"0dot",0}, {"0dots",0},
            {"lowheat",1 }, {"1dot",1 },
            {"mediumheat",2 }, {"2dot",2 },{"2dots",2 },
            {"highheat",3 }, {"3dot",3 },{"3dots",3 },
            {"no heat",4 },
            {"hangtodry",5 }, {"hangdry",5 },
            {"dripdry",6 },
            {"dryflat",7 },
            {"dryintheshade",8 }, {"dryinshade",8 },
            {"donotdry",9 }, {"dontdry",9 },
            {"donottumbledry",10 }, {"donttumbledry",10 },
            {"dry",11 },
        };
    static Dictionary<string, int> ironIndex = new Dictionary<string, int>()
        {
            {"iron",0 }, {"is",0 },
            {"donotiron",1 }, {"dontiron",1 },
            {"110",2 }, {"110c",2 }, {"110°c",2 }, {"230",2 }, {"230f",2 }, {"230°f",2 },
            {"150",3 }, {"150c",3 }, {"150°c",3 }, {"300",3 }, {"300f",3 }, {"300°f",3 },
            {"200",4 }, {"200c",4 }, {"200°c",4 }, {"390",4 }, {"390f",4 }, {"390°f",4 },
            {"nosteam",5 },
        };
    static Dictionary<string, int> specialIndex = new Dictionary<string, int>()
        {
            {"bleach",0 },
            {"donotbleach",1 }, {"dontbleach",1 },
            {"nochlorine",2 }, {"nochlorinebleach",2 }, {"nonchlorinebleach",2 },
            {"dryclean",3 },
            {"anysolvent",4 },
            {"anysolventexcepttetrachlorethylene",5 }, {"notetrachlorethylene",5 }, {"notetrachlore",5 },
            {"petroleumsolventonly",6 }, {"petroleumonly",6 },
            {"wetcleaning",7 },
            {"donotdryclean",8 }, {"dontdryclean", 8 },
            {"shortcycle",9 },
            {"reducedmoisture",10 }, {"reducedmoist",10 },
            {"lowheat",11 },
            {"nosteamfinishing",12 }, { "nosteamfinish",12},
        };

    static string[] commands = new string[] { "set wash ", "set dry ", "set iron ", "set special " };
    static string[] debuglog = new[] { "Wash", "Dry", "Ironing", "Special" };

    //The only thing on this module that can cause a strike is inserting a coin.  By using IEnumerable, instructions are guaranteed to be
    //set as desired, even if a strike happens elsewhere on the bomb.  If we just returned KMSelectable[], the instruction sequence would
    //be interrupted by strikes caused by other modules.
    IEnumerator ProcessTwitchCommand(string command)
    {
        string rawCommand = command;
        
        int[] targetValues = new[] { leftKnobPos, rightKnobPos, ironingTextPos, specialTextPos };
        int[] currentValues = new[] {leftKnobPos, rightKnobPos, ironingTextPos, specialTextPos};
        string[][] texts = new[] { washingText, dryingText, ironingText, specialText };
        Dictionary<string, int>[] Indexes = new[] { washIndex, dryIndex, ironIndex, specialIndex };

        command = command.ToLowerInvariant();
        if (command == "insert coin")
        {
            yield return "Inserting the coin";
            Debug.LogFormat("[Laundry TwitchPlays #{0}] - Inserting the coin - {1}", myModuleId, rawCommand);
            yield return coinSlot;
            yield return new WaitForSeconds(0.1f);
            yield return coinSlot;
            yield break;
        }

        if (command.StartsWith("set all "))
        {
            command = command.Substring(8).Replace(" ", "").Replace("'", "");
            string[] split = command.Split(new[] {","}, StringSplitOptions.None);
            bool[] setAllValid = new bool[] {true, true, true, true};
            if (split.Length != 4)
            {
                Debug.LogFormat(
                    "[Laundry TwitchPlays #{0}] - IRC command \"set all\" failed validation. Expected 4 parameters, got {2} parameters. \"{1}\"",
                    myModuleId, rawCommand, split.Length);
                yield break;
            }

            for (var i = 0; i < commands.Length; i++)
            {
                if (split[i] != "" && !Indexes[i].TryGetValue(split[i].Replace("-", ""), out targetValues[i]) && !int.TryParse(split[i],out targetValues[i]))
                    setAllValid[i] = false;
                else
                {
                    targetValues[i] %= texts[i].Length;
                    if (targetValues[i] < 0) targetValues[i] += texts[i].Length;
                }
            }

            if (!setAllValid[0] || !setAllValid[1] || !setAllValid[2] || !setAllValid[3])
            {
                Debug.LogFormat(
                    "[Laundry TwitchPlays #{0}] - IRC command \"set all\" parameters not valid. Validation results: Washing = {2}, Drying = {3}, Ironing = {4}, Special = {5}.  Command Sent: \"{1}\"",
                    myModuleId, rawCommand,
                    setAllValid[0] ? "Passed" : "Failed", setAllValid[1] ? "Passed" : "Failed",
                    setAllValid[2] ? "Passed" : "Failed", setAllValid[3] ? "Passed" : "Failed");
                yield break;
            }

            Debug.LogFormat("[Laundry TwitchPlays #{0}] - Setting Everything - {1}", myModuleId, rawCommand);
            for (var i = 0; i < commands.Length; i++)
            {
                Debug.LogFormat("Setting {1} to {2}", myModuleId, debuglog[i], texts[i][targetValues[i]]);
            }
        }
        else
        {
            for (var i = 0; i <= commands.Length; i++)
            {
                if (i == commands.Length)
                {
                    Debug.LogFormat("[Laundry TwitchPlays #{0}] - IRC command not parsed: {1}", myModuleId, rawCommand);
                    yield break;
                }

                if (command.StartsWith(commands[i]))
                {
                    command = command.Substring(commands[i].Length).Replace(" ", "").Replace("'", "");
                    if (!Indexes[i].TryGetValue(command.Replace("-", ""), out targetValues[i]) && !int.TryParse(rawCommand, out targetValues[i]))
                    {
                        Debug.LogFormat("[Laundry TwitchPlays #{0}] - IRC command \"{2}\" parameter not valid. - {1}", myModuleId, rawCommand, commands[i]);
                        yield break;
                    }

                    targetValues[i] %= texts[i].Length;
                    if (targetValues[i] < 0) targetValues[i] += texts[i].Length;
                    Debug.LogFormat("[Laundry TwitchPlays #{0}] - Setting {2} to {3} - {1}", myModuleId, rawCommand, debuglog[i], texts[i][targetValues[i]]);
                    break;
                }
            }
        }

        bool allSame = true;
        for (int i = 0; i < commands.Length && allSame; i++)
        {
            allSame &= targetValues[i] == currentValues[i];
        }

        if(allSame)
        {
            Debug.LogFormat("Laundry already set to desired values", myModuleId);
            yield break;
        }

        yield return "Doing the Laundry";
        while (targetValues[0] != leftKnobPos)
        {
            yield return knobs[0];
            yield return new WaitForSeconds(0.1f);
            yield return knobs[0];
        }
        while (targetValues[1] != rightKnobPos)
        {
            yield return knobs[1];
            yield return new WaitForSeconds(0.1f);
            yield return knobs[1];
        }
        while (targetValues[2] != ironingTextPos)
        {
            int direction = (targetValues[2] < ironingTextPos) ? 0 : 1;
            yield return slidersTop[direction];
            yield return new WaitForSeconds(0.1f);
            yield return slidersTop[direction];
        }
        while (targetValues[3] != specialTextPos)
        {
            int direction = (targetValues[3] < specialTextPos) ? 0 : 1;
            yield return slidersBottom[direction];
            yield return new WaitForSeconds(0.1f);
            yield return slidersBottom[direction];
        }
    }

   
}
