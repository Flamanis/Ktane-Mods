using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;

public class Laundry : MonoBehaviour
{

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
    




    public KMBombInfo BombInfo;
    public KMSelectable Coinslot;
    public KMSelectable[] Knobs;
    public KMSelectable[] SlidersTop;
    public KMSelectable[] SlidersBottom;
    public TextMesh IroningTextDisplay;
    public TextMesh SpecialTextDisplay;

    public Material[] WashingDisplay;
    public Material[] DryingDisplay;

    public KMAudio Sound;
    public MeshRenderer LeftKnobDisplay;
    public MeshRenderer RightKnobDisplay;
    public KMBombModule BombModule;

    private static int[,] ClothingType = { { 7, 7, 3, 0 }, { 5, 3, 5, 5 }, { 4, 5, 0, 10 }, { 1, 0, 4, 10 }, { 10, 8, 3, 1 }, { 9, 11, 2, 8 } };
    private static int[,] MaterialType = { { 6, 4, 3, 6 }, { 9, 2, 0, 8 }, { 2, 8, 4, 10 }, { 4, 6, 5, 11 }, { 5, 6, 2, 7 }, { 3, 9, 1, 5 } };
    private static int[,] ColorType = { { 7, 3, 1, 4 }, { 9, 7, 0, 11 }, { 4, 0, 4, 9 }, { 4, 10, 3, 12 }, { 0, 1, 5, 11 }, { 7, 2, 4, 2, } };

    private static string[] WashingText = { "Machine Wash Permanent Press", "Machine Wash Gentle or Delicate", "Hand Wash", "Do Not Wash", "30°C", "40°C", "50°C", "60°C", "70°C", "95°C", "Do Not Wring" };
    private static string[] DryingText = { "Tumble Dry", "Low Heat", "Medium Heat", "High Heat", "No Heat", "Hang to Dry", "Drip Dry", "Dry Flat", "Dry in the Shade", "Do Not Dry", "Do Not Tumble Dry", "Dry" };
    private static string[] IroningText = { "Iron", "Don't Iron", "110°C", "300°F", "200°C", "No Steam" };
    private static string[] SpecialText = { "Bleach", "Don't Bleach", "No Chlorine", "Dryclean", "Any Solvent", "No Tetrachlore", "Petroleum Only", "Wet Cleaning", "Don't Dryclean", "Short Cycle", "Reduced Moist", "Low Heat", "No Steamfinish" };
    private static string[] ClothingNames = { "Corset", "Shirt", "Skirt", "Skort", "Shorts", "Scarf" };
    private static string[] MaterialNames = { "Polyester", "Cotton", "Wool", "Nylon", "Corduroy", "Leather" };
    private static string[] ColorNames = { "Ruby Fountain", "Star Lemon Quartz", "Sapphire Springs", "Jade Cluster", "Clouded Pearl", "Malinite" };

    private int IroningTextPos = 0;
    private int SpecialTextPos = 0;
    private int LeftKnobPos = 0;
    private int RightKnobPos = 0;
    private int TotalIndicators = 0;
    private int BatteryHolders = 0;
    private int TotalPorts = 0;
    private int TotalBatteries = 0;
    private int LastDigitSerial = 0;
    private bool HasBOB = false;
    private string SerialNum = "";
    private float WashingRotate;
    private float DryingRotate;
    private int[] Solution;
    private bool isSolved = false;

    private static int ModuleID = 1;

    private int MyModuleId;


    void IroningSlide (int sign)
    {
        IroningTextPos += sign;
        IroningTextPos = (IroningTextPos + IroningText.Length) % IroningText.Length;
        IroningTextDisplay.text = IroningText[IroningTextPos]; 
    }

    void SpecialSlide (int sign)
    {
        SpecialTextPos += sign;
        SpecialTextPos = (SpecialTextPos + SpecialText.Length) % SpecialText.Length;
        SpecialTextDisplay.text = SpecialText[SpecialTextPos];
    }

    void LeftKnob()
    {
        LeftKnobPos ++ ;
        LeftKnobPos = (LeftKnobPos + WashingDisplay.Length) % WashingDisplay.Length;
        Knobs[0].transform.Rotate(new Vector3(0, WashingRotate, 0));
        LeftKnobDisplay.material =  WashingDisplay[LeftKnobPos];
        Sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Knobs[0].transform);
    }

    int[] GetSolutionValues(out StringBuilder LogString) {
        LogString = new StringBuilder();
        LogString.Append("[Laundry #" + MyModuleId + "] Solution Values: ");
        if (HasBOB && TotalBatteries == 4 && BatteryHolders == 2) {
            LogString.Append("We got a Lit Bob and 4 batteries in two holders.\n");
            return new int[1];
        }

        int[] SolutionStates = new int[4];
        int solved = BombInfo.GetSolvedModuleNames().Count;
        int ItemClothing = (BombInfo.GetSolvableModuleNames().Count - solved + TotalIndicators + 6) % 6;
        int ItemMaterial = (TotalPorts + solved - BatteryHolders + 6) % 6;
        int ItemColor = (LastDigitSerial + TotalBatteries + 6) % 6;
        LogString.AppendFormat("Clothing: {0} ({1}), Material: {2} ({3}), Color: {4} ({5})\n", ClothingNames[ItemClothing], ItemClothing, MaterialNames[ItemMaterial], ItemMaterial, ColorNames[ItemColor], ItemColor);

        bool CloudedPearl = ItemColor == 4;
        bool LeatherJadeCluster = ItemMaterial == 5 || ItemColor == 3;
        bool CorsetCorduroy = ItemClothing == 0 || ItemMaterial == 4;
        bool WoolStarLemon = ItemMaterial == 2 || ItemColor == 1;
        bool MaterialSerial = false;

        foreach (char c in MaterialNames[ItemMaterial].ToLower()) {
            if (SerialNum.Contains(c + "")) {
                MaterialSerial = true;
            }
        }

        if (CloudedPearl) {
            LogString.Append("Special is Non-Chlorine Bleach, ");
            SolutionStates[3] = 2;
        } else if (CorsetCorduroy) {
            LogString.Append("Special on Material, ");
            SolutionStates[3] = MaterialType[ItemMaterial, 3];
        } else if (MaterialSerial) {
            LogString.Append("Special on Color, ");
            SolutionStates[3] = ColorType[ItemColor, 3];
        } else {
            LogString.Append("Special on Clothing, ");
            SolutionStates[3] = ClothingType[ItemClothing, 3];
        }


        if (LeatherJadeCluster) {
            LogString.Append("Washing is 80°F, ");
            SolutionStates[0] = 4;
        } else {
            LogString.Append("Washing on material, ");
            SolutionStates[0] = MaterialType[ItemMaterial, 0];
        }


        if (WoolStarLemon) {
            LogString.Append("Drying is High Heat, ");
            SolutionStates[1] = 3;
        } else {
            LogString.Append("Drying on Color, ");
            SolutionStates[1] = ColorType[ItemColor, 1];
        }
        LogString.Append("Ironing on Clothing\n");
        SolutionStates[2] = ClothingType[ItemClothing, 2];

        LogString.Append("End result: ");
        LogString.Append(WashingText[SolutionStates[0]] + ", ");
        LogString.Append(DryingText[SolutionStates[1]] + ", ");
        LogString.Append(IroningText[SolutionStates[2]] + ", ");
        LogString.Append(SpecialText[SolutionStates[3]]);
        LogString.Append("\n");
        return SolutionStates;
    }

    void RightKnob()
    {
        RightKnobPos ++ ;
        RightKnobPos = (RightKnobPos + DryingDisplay.Length) % DryingDisplay.Length;
        Knobs[1].transform.Rotate(new Vector3(0, DryingRotate, 0));
        RightKnobDisplay.material = DryingDisplay[RightKnobPos];
        Sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Knobs[1].transform);
    }

    void GetBombValues()
    {

        List<string> PortList = BombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_PORTS, null);
        foreach (string Portals in PortList)
        {
            Ports i = JsonConvert.DeserializeObject<Ports>(Portals);

            TotalPorts += i.PresentPorts.Length;
        
        }
          Serial SerialNumber = JsonConvert.DeserializeObject<Serial>(BombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, null)[0]);
          LastDigitSerial = SerialNumber.serial[5] - '0';
          SerialNum = SerialNumber.serial.ToLower();

        List<string> BatteryList = BombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, null);
        BatteryHolders = BatteryList.Count;


        foreach (string Batteries in BatteryList)
        {
            Battery i = JsonConvert.DeserializeObject<Battery>(Batteries);
            TotalBatteries += i.numbatteries;
        }


        List<string> IndicatorList = BombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_INDICATOR, null);
        TotalIndicators = IndicatorList.Count;

        foreach (string Indicators in IndicatorList)
        {
            Indicator i = JsonConvert.DeserializeObject<Indicator>(Indicators);
            if (i.on == "True" && i.label == "BOB") 
            {
                HasBOB = true;
            }
        }
    } 

    


    void HandlePass()
    {
        BombModule.HandlePass();
    }

    void HandleStrike()
    {
        BombModule.HandleStrike();
    }


    void UseCoin()
    {
        if (!isSolved) {
            StringBuilder s;
            Solution = GetSolutionValues(out s);
            s.AppendFormat("Entered Values: {0}, {1}, {2}, {3}\n", WashingText[LeftKnobPos], DryingText[RightKnobPos], IroningText[IroningTextPos], SpecialText[SpecialTextPos]);



            if (Solution.Length == 1) {
                isSolved = true;
                s.Append("Passed");
                Debug.Log(s.ToString());
                HandlePass();
                return;
            }

            bool WashingCorrect = Solution[0] == LeftKnobPos;
            bool DryingCorrect = Solution[1] == RightKnobPos;
            bool IroningCorrect = Solution[2] == IroningTextPos;
            bool SpecialCorrect = Solution[3] == SpecialTextPos;

            if (WashingCorrect && DryingCorrect && IroningCorrect && SpecialCorrect) {
                isSolved = true;
                s.Append("Passed");
                HandlePass();
            } else {
                s.Append("Striked");
                HandleStrike();
            }
            Debug.Log(s.ToString());
        }
    }








    void Start()
    {
        MyModuleId = ModuleID++;
        WashingRotate = 360.0f / WashingDisplay.Length;
        DryingRotate = 360.0f / DryingDisplay.Length;
        BombModule.OnActivate += GetBombValues;
      
        for (int i = 0; i < SlidersTop.Length; i++)
        {
            var j = i;
            SlidersTop[i].OnInteract += delegate () { IroningSlide(j * 2 - 1); Sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, SlidersTop[j + 1 - 1 ].transform); return false; };

        }
        for (int i = 0; i < SlidersBottom.Length; i++)
        {
            var j = i;
            SlidersBottom[i].OnInteract += delegate () { SpecialSlide(j * 2 - 1); Sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, SlidersBottom[j + 1 - 1 ].transform); return false; };

        }

        Coinslot.OnInteract += delegate () { Sound.PlaySoundAtTransform("Insert Coin", Coinslot.transform);  UseCoin(); return false; }; 

        Knobs[0].OnInteract += delegate ()
        { LeftKnob(); return false; };

        Knobs[1].OnInteract += delegate ()
        { RightKnob(); return false; };

        IroningTextDisplay.text = IroningText[IroningTextPos];
        SpecialTextDisplay.text = SpecialText[SpecialTextPos];

        //LeftKnobDisplay.material.SetTexture("_MainTex", WashingDisplay[LeftKnobPos]);
        //RightKnobDisplay.material.SetTexture("_MainTex", DryingDisplay[RightKnobPos]);
        
    }

    //The only thing on this module that can cause a strike is inserting a coin.  By using IEnumerable, instructions are guaranteed to be
    //set as desired, even if a strike happens elsewhere on the bomb.  If we just returned KMSelectable[], the instruction sequence would
    //be interrupted by strikes caused by other modules.
    IEnumerator ProcessTwitchCommand(string command)
    {
        Dictionary<string, int> washIndex = new Dictionary<string, int>()
        {
            {"machinewashpermanentpress", 0},{"permanentpress", 0},{"stillusewater", 0},

            {"machinewashgentleordelicate", 1},{"machinewashgentle", 1},{"what", 1},
            {"machinewashdelicate", 1},{"gentle", 1},{"delicate", 1},

            {"handwash", 2},{"nodontputyourhandsin", 2},

            {"donotwash", 3},{"dontwash", 3},{"dontwashever", 3},

            {"30", 4},{"30c", 4},{"30°c", 4},{"80", 4},{"80f", 4},{"80°f", 4},{"1dot", 4},
            {"30hot", 4},{"dontwashtennisballs", 4},

            {"40",5 },{"40c",5 },{"40°c",5 },{"105",5 },{"105f",5 },{"105°f",5 },{"2 dot",5 },
            {"justuse30hot",5 },{"stopit",5 },

            {"50",6 },{"50c",6},{"50°c",6 },{"120",6 },{"120f",6 },{"120°f",6 },{"3dot",6 },
            {"stop",6 },{"notennisballs",6 },

            {"60",7 },{"60c",7},{"60°c",7 },{"140",7 },{"140f",7 },{"140°f",7 },{"4dot",7 },
            {"enough",7 },{"fuckssake",7 },

            {"70",8 },{"70c",8},{"70°c",8 },{"160",8 },{"160f",8 },{"160°f",8 },{"5dot",8 },
            {"stupid",8 },{"justuse30",8 },

            {"95",9 },{"95c",9},{"95°c",9 },{"200",9 },{"200f",9 },{"200°f",9 },{"6dot",9 },
            {"why",9 },{"toomanytennisballs",9 },

            {"donotwring",10 }, {"dontwring",10 }, {"nochristmascrackers",10 },
        };
        Dictionary<string, int> dryIndex = new Dictionary<string, int>()
        {
            {"tumbledry",0 }, {"noidea",0 },
            {"lowheat",1 }, {"1dot",1 }, {"cyclops",1 },
            {"mediumheat",2 }, {"2dot",2 }, {"what",2 },
            {"highheat",3 }, {"3dot",3 }, {"laundryisstupid",3 },
            {"no heat",4 }, //{"what",4 },
            {"hangtodry",5 }, {"hangdry",5 }, {"whatthefuckisthis",5 },
            {"dripdry",6 }, {"no",6 },
            {"dryflat",7 }, {"noneofthisshitmakessense",7 },
            {"dryintheshade",8 }, {"dryinshade",8 }, {"thefuck",8 },
            {"donotdry",9 }, {"dontdry",9 }, //{"what",9 },
            {"donottumbledry",10 }, {"donttumbledry",10 }, {"the",10 },
            {"dry",11 }, {"fuck",11 },
        };
        Dictionary<string, int> ironIndex = new Dictionary<string, int>()
        {
            {"iron",0 }, {"is",0 },
            {"donotiron",1 }, {"dontiron",1 }, {"happening",1 },
            {"110",2 }, {"110c",2 }, {"110°c",2 }, {"230",2 }, {"230f",2 }, {"230°f",2 }, {"bumpercar",2 },
            {"150",3 }, {"150c",3 }, {"150°c",3 }, {"300",3 }, {"300f",3 }, {"300°f",3 }, {"twoseatedbumpercar",3 },
            {"200",4 }, {"200c",4 }, {"200°c",4 }, {"390",4 }, {"390f",4 }, {"390°f",4 }, {"deluxbumpercar",4 }, {"deluxebumpercar",4 },
            {"nosteam",5 }, {"bumpercarwithnolegs",5 },
        };
        Dictionary<string, int> specialIndex = new Dictionary<string, int>()
        {
            {"bleach",0 }, {"triangle",0 },
            {"donotbleach",1 }, {"dontbleach",1 }, {"notriangle",1 },
            {"nochlorine",2 }, {"nochlorinebleach",2 }, {"nonchlorinebleach",2 }, {"thisthing",2 },
            {"dryclean",3 }, {"zero",3 },
            {"anysolvent",4 }, {"apathy",4 },
            {"anysolventexcepttetrachlorethylene",5 }, {"notetrachlorethylene",5 }, {"notetrachlore",5 }, {"pinks",5 },
            {"petroleumsolventonly",6 }, {"petroleumonly",6 }, {"frenchclothes",6 },
            {"wetcleaning",7 }, {"igiveup",7 },
            {"donotdryclean",8 }, {"dontdryclean", 8 },
            {"shortcycle",9 },
            {"reducedmoisture",10 }, {"reducedmoist",10 },
            {"lowheat",11 },
            {"nosteamfinishing",12 }, { "nosteamfinish",12},
        };

        string[] commands = new string[] { "set wash ", "set dry ", "set iron ", "set special " };
        string[] debuglog = new[] { "Wash", "Dry", "Ironing", "Special" };
        int[] targetValues = new[] { LeftKnobPos, RightKnobPos, IroningTextPos, SpecialTextPos };
        int[] currentValues = new[] {LeftKnobPos, RightKnobPos, IroningTextPos, SpecialTextPos};
        string[][] texts = new[] { WashingText, DryingText, IroningText, SpecialText };
        Dictionary<string, int>[] Indexes = new[] { washIndex, dryIndex, ironIndex, specialIndex };

        command = command.ToLowerInvariant();
        if (command == "insert coin")
        {
            yield return "Inserting the coin";
            Debug.LogFormat("[Laundry TwitchPlays #{0}] - Inserting the coin - {1}", MyModuleId, command);
            yield return Coinslot;
            yield return new WaitForSeconds(0.1f);
            yield return Coinslot;
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
                    MyModuleId, command, split.Length);
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
                    MyModuleId, command,
                    setAllValid[0] ? "Passed" : "Failed", setAllValid[1] ? "Passed" : "Failed",
                    setAllValid[2] ? "Passed" : "Failed", setAllValid[3] ? "Passed" : "Failed");
                yield break;
            }

            Debug.LogFormat("[Laundry TwitchPlays #{0}] - Setting Everything - {1}", MyModuleId, command);
            for (var i = 0; i < commands.Length; i++)
            {
                Debug.LogFormat("[Laundry TwitchPlays #{0}] - Setting {2} to {3} - {1}", MyModuleId, command, debuglog[i], texts[i][targetValues[i]]);
            }
        }
        else
        {
            for (var i = 0; i <= commands.Length; i++)
            {
                if (i == commands.Length)
                {
                    Debug.LogFormat("[Laundry TwitchPlays #{0}] - IRC command not parsed: {1}", MyModuleId, command);
                    yield break;
                }

                if (command.StartsWith(commands[i]))
                {
                    command = command.Substring(commands[i].Length).Replace(" ", "").Replace("'", "");
                    if (!Indexes[i].TryGetValue(command.Replace("-", ""), out targetValues[i]) && !int.TryParse(command, out targetValues[i]))
                    {
                        Debug.LogFormat("[Laundry TwitchPlays #{0}] - IRC command \"{2}\" parameter not valid. - {1}", MyModuleId, command, commands[i]);
                        yield break;
                    }

                    targetValues[i] %= texts[i].Length;
                    if (targetValues[i] < 0) targetValues[i] += texts[i].Length;
                    Debug.LogFormat("[Laundry TwitchPlays #{0}] - Setting {2} to {3} - {1}", MyModuleId, command, debuglog[i], texts[i][targetValues[i]]);
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
            Debug.LogFormat("[Laundry TwitchPlays #{0}] - Laundry already set to desired values", MyModuleId);
            yield break;
        }

        yield return "Doing the Laundry";
        while (targetValues[0] != LeftKnobPos)
        {
            yield return Knobs[0];
            yield return new WaitForSeconds(0.1f);
            yield return Knobs[0];
        }
        while (targetValues[1] != RightKnobPos)
        {
            yield return Knobs[1];
            yield return new WaitForSeconds(0.1f);
            yield return Knobs[1];
        }
        while (targetValues[2] != IroningTextPos)
        {
            int direction = (targetValues[2] < IroningTextPos) ? 0 : 1;
            yield return SlidersTop[direction];
            yield return new WaitForSeconds(0.1f);
            yield return SlidersTop[direction];
        }
        while (targetValues[3] != SpecialTextPos)
        {
            int direction = (targetValues[3] < SpecialTextPos) ? 0 : 1;
            yield return SlidersBottom[direction];
            yield return new WaitForSeconds(0.1f);
            yield return SlidersBottom[direction];
        }
    }

   
}
