using UnityEngine;
using System.Collections.Generic;
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
        int ItemClothing = Mod6(BombInfo.GetSolvableModuleNames().Count - solved + TotalIndicators);
        int ItemMaterial = Mod6(TotalPorts + solved - BatteryHolders);
        int ItemColor = Mod6(LastDigitSerial + TotalBatteries);
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
    
    int Mod6(int val)
    {
        return (val %= 6 < 0) ? val + 6 : val;
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

   
}
