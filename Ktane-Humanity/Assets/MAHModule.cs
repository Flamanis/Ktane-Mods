using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

public class MAHModule : MonoBehaviour {
    

    public KMSelectable[] LeftCardButtons;
    public KMSelectable[] RightCardButtons;
    public KMSelectable ResetButton;
    public KMSelectable AcceptButton;
    public TextMesh LeftCard;
    public TextMesh RightCard;
    public KMBombInfo mBombInfo;
    public KMBombModule mBombModule;
    public static Material LeftCardMat;
    public static Material RightCardMat;
    public MeshRenderer LeftCardMesh;
    public MeshRenderer RightCardMesh;
    public KMGameInfo mGameInfo;
    public TextMesh DummyMesh;
    public KMAudio Audio;

    private const int AMOUNT_OF_CARDS = 10;

    private static string[] WhiteModuleIDs = { "wire sequence", "simon says", "maze", "memory", "needy capacitor", "who's on first", "needy vent gas", "modules against humanity", "needy knob", "morse code", "two bits", "anagrams", "word scramble", "semaphore", "colour flash", "logic", "listening", "mystic square", "crazy talk", "silly slots", "probing", "forget me not", "morsematics", "simon states", "perspective pegs", "caesar cipher", "tic tac toe", "astrology", "adventure game", "skewed slots", "blind alley", "english test", "mouse in the maze", "turn the keys", "turn the key", "tetris", "sea shells", "murder","adjacent letters","colored squares" ,"hexamaze" ,"souvenir", "simon screams", "http response", "wire placement", "coordinates", "battleship", };
    private static string[] BlackModuleIDs = { "the button", "password", "wires", "keypad", "complicated wires", "chess", "switches", "emoji math", "letter keys", "orientation cube", "piano keys", "connection check", "cryptography", "number pad", "alphabet", "round keypad", "plumbing", "safety safe", "resistors", "microcontroller", "the gamepad", "laundry", "3d maze", "follow the leader", "friendship", "the bulb", "monsplode, fight!", "foreign exchange rates", "combination lock", "shape shift", "needy math", "lights out", "motion sense", "needy rotary phone", "needy quiz", "who's that monsplode?", "filibuster","third base","bitmaps","rock-paper-scissors-l.-sp.","square button", "broken buttons", "word search", "complicated buttons", "symbolic password", "light cycle", "text field", "double-oh"};

    private static Dictionary<string, string> ModuleTexts = new Dictionary<string, string>() {
        { "3d maze", "Want to see this in 3D? There are special glasses for it." },
        { "adjacent letters", "Bomb exploded. I was adjacent to it." },
        { "adventure game", "That's okay. There will be another adventure waiting for you soon." },
        { "alphabet", "Never try tongue twisters before you learn alphabet." },
        { "anagrams", "The instructions were clear and not in anagrams; you're an idiot." },
        { "astrology", "No success? I guess your zodiac was under the wrong planet's influence." },
        { "battleship", "This bomb is a true battle, just not on a ship." },
        { "bitmaps" ,  "Lost in this bomb? Here's a bit of a map."},
        { "blind alley", "You're done. No way out from this blind alley." },
        { "broken buttons", "Pressing these buttons is useless. They’re broken." },
        { "caesar cipher", "You dealt with it like a true Caesar." },
        { "chess", "I would play even Billy the Puppet if that game was chess." },
        { "colored squares" , "It's cool to be square but only if your colored." },
        { "colour flash", "That almost got you dead. Your whole life flashed before your eyes." },
        { "combination lock", "Are you sure this is the combination you need?" },
        { "complicated buttons", "Pressing buttons should be easy, not complicated." },
        { "complicated wires", "Cut all the wires! It's not that complicated after all." },
        { "connection check", "Bomb exploded due to bad wifi connection." },
        { "coordinates", "You're walking funny. Have you lost your coordinates?" },
        { "crazy talk", "You're talking crazy. Get out." },
        { "cryptography", "Your mind is so cryptic, I can't read it." },
        { "double-oh", "You may have double As, you may have double Ds, but double Os are the best." },
        { "emoji math" , "Did you know your texts can include words, and not just emojis?" },
        { "english test", "Do you know the difference between your crap and you're crap?" },
        { "filibuster", "Remember the title and just keep talking" },
        { "follow the leader", "If this bomb explodes, you're following the wrong political leader." },
        { "foreign exchange rates", "No currency exchange point? You should've taken your card." },
        { "forget me not", "Well done. You forgot it not." },
        { "friendship", "They say Monopoly ruins friendships. They have not tried KTANE." },
        { "hexamaze", "Someone must have put a-maze-ing hex on you." },
        { "http response", "404. The bomb not found." },
        { "keypad", "If you recognize any of these symbols, you're a smart fella." },
        { "laundry", "Friends are coming over. Quick, hide all your dirty laundry!" },
        { "letter keys", "Wait, now keys have letters on them? What is this sorcery?" },
        { "light cycle", "Dark in the room? Just cycle through the lights." },
        { "lights out", "Lights are out. Bomb's unavailable for 10 seconds." },
        { "listening", "Just got a strike. You need to listen to your experts better." },
        { "logic", "Do you have at least a pinch of logic in that head of yours?" },
        { "maze", "I know you feel lost. Sadly, this is not a maze so there's no way out." },
        { "memory", "Nope, that doesn't work. Try memorizing the manual next time." },
        { "microcontroller", "Trust your experts, don’t micro control them." },
        { "modules against humanity", "Yes, this bomb is very inhumane. Deal with it." },
        { "monsplode, fight!", "Why fight your family when you can fight Monsplodes? " },
        { "morse code" , "If you're bad with words, try Morse code instead." },
        { "morsematics", "If you're bad with words, try Morsematics code instead." },
        { "motion sense", "Moves like Jagger? Not with that motion sense of yours." },
        { "mouse in the maze", "Tsk. Poor little mouse, can't do anything right." },
        { "murder", "If this doesn't get you murdered, I don't know what will." },
        { "mystic square", "You need serious help. From a shaman or a mystic, nothing less." },
        { "needy capacitor", "You're on the dishonorable discharge, Take your capacitor and leave." },
        { "needy knob", "You can play with this knob even when you're done with the bomb." },
        { "needy math", "Math is hard." },
        { "needy quiz", "I know you hate answering questions, but we do have to test you." },
        { "needy rotary phone", "Here, phone a friend for help. Old school style." },
        { "needy vent gas", "Stop venting your emotions. Vent gas instead." },
        { "number pad", "Keypad here, keypad there. I want a keypad with numbers!" },
        { "orientation cube", "Bad luck with your crush? Maybe it’s just their sexual orientation." },
        { "password", "You need to know the secret password to defuse this bomb." },
        { "perspective pegs", "If you can't do it, try to look from a different perspective." },
        { "piano keys", "Musical instruments? I recommend piano, cruel one or not." },
        { "plumbing", "If you don't find a way out, try crawling through pipes." },
        { "probing", "Bad news. You're actually being probed by aliens." },
        { "resistors", "Don't resist the temptation. Just do it." },
        { "rock-paper-scissors-l.-sp.", "You're fighting a lizard. But don't use caber, use scissors." },
        { "round keypad", "You spin my head right round with those fancy symbols." },
        { "safety safe", "This bomb is so valuable it should be locked in a safe." },
        { "sea shells", "Who cares what she sells? She's not gonna sell it to you." },
        { "semaphore", "Just as I expected, you're waving a white flag already." },
        { "shape shift", "Why be a human when you can shapeshift into an owl?" },
        { "silly slots", "You failed. What a silly slot you are." },
        { "simon says", "Too late. simon says you should try again." },
        { "simon screams", "Simon screams at your incompetence." },
        { "simon states", "Too late. Simon states you should try again." },
        { "skewed slots", "Well, that's what happens when your world view is so skewed." },
        { "souvenir", "This explosion will leave a few souvenirs on your body." },
        { "square button", "Push the square button. Galvanize." },
        { "switches", "It’s useless. Switch to another bomb." },
        { "symbolic password", "Your password must be 6 symbols long." },
        { "tetris", "If you're tired of bombs, you could always play Tetris instead." },
        { "text field", "Who needs fields of gold when you have a field of text?" },
        { "the bulb", "You think you have a great idea? Nah, that's just a bulb above your head." },
        { "the button", "Push the button. Galvanize." },
        { "the gamepad", "Two kinds of people: those who use gamepad and the boring ones." },
        { "third base", "Don't tell me you're already in third base with this bomb?" },
        { "tic tac toe", "Lots of love, XOXO, The Bomb." },
        { "turn the key", "Just turn the key and leave the room." },
        { "turn the keys", "Just turn the keys and leave the room." },
        { "two bits", "Here's my two bits of information: you're gonna explode." },
        { "who's on first", "You're clearly the last, but who's first?" },
        { "who's that monsplode?", "Fighting Monsplodes? Identify them first." },
        { "wire placement", "Be careful where you place those wires!" },
        { "wire sequence", "Trickier than it sounds; all the wires are hidden behind panels." },
        { "wires", "Cut all the wires!" },
        { "word scramble", "You got scrambled by your own sword." },
        { "word search", "You’re searching for a word? It’s the bird." }
    };

    private List<string> WhiteCardText;
    private List<string> BlackCardText;

    private List<string> LeftCardText;
    private List<string> RightCardText;

    private Material LeftCardMaterial;
    private Material RightCardMaterial;

    private int LeftCardIndex;
    private int RightCardIndex;

    private int CorrectLeftCardIndex;
    private int CorrectRightCardIndex;

    private bool activated = false;


    private bool SwapWhiteBlack;
    private float limit;

    private static int ModuleId = 1;
    private int MyModuleId;

    private bool IsSolved = false;

    #region CardFormatting

    public static float GetWidth(TextMesh mesh) {
        float width = 0;
        foreach (char symbol in mesh.text) {
            CharacterInfo info;
            if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle)) {
                width += info.advance;
            }
        }
        return width * mesh.characterSize * 0.1f;
    }
    string FitTextInCard(string text) {
        StringBuilder txt = new StringBuilder();
        DummyMesh.text = "a";
        float spaceLength = GetWidth(DummyMesh);
        DummyMesh.text = "a a";
        spaceLength = GetWidth(DummyMesh) - spaceLength - spaceLength;
        float currAmount = 0;
        string[] split = text.Split(' ');
        txt.Append(split[0]);
        DummyMesh.text = split[0];
        currAmount = GetWidth(DummyMesh);
        for(int i = 1; i < split.Length; i++) {
            txt.Append(" ");
            DummyMesh.text = split[i];
            float currLength = GetWidth(DummyMesh);
            if((currAmount + spaceLength + currLength) > limit) {
                txt.Append("\n");
                currAmount = currLength;
            }else {
                currAmount += spaceLength + currLength;
            }
            
            txt.Append(DummyMesh.text);
        }
        return txt.ToString();
    }
    #endregion

    #region CardText
    void UpdateLeftCardText() {
        LeftCardIndex += BlackCardText.Count;
        LeftCardIndex %= BlackCardText.Count;
        LeftCard.text = LeftCardText[LeftCardIndex];
    }

    void UpdateRightCardText() {
        RightCardIndex += BlackCardText.Count;
        RightCardIndex %= BlackCardText.Count;
        RightCard.text = RightCardText[RightCardIndex];
    }

    private void OnLightsChange(bool on) {
        if (on) {
            UpdateLeftCardText();
            UpdateRightCardText();
        } else {
            LeftCard.text = "";
            RightCard.text = "";
        }
    }

    private void GenerateCardTexts() {
        LeftCardText = new List<string>();
        RightCardText = new List<string>();
        for (int i = 0; i < BlackCardText.Count; i++) {
            string black = BlackCardText[i];
            string white = WhiteCardText[i];
            if (!SwapWhiteBlack) {
                LeftCardText.Add(FitTextInCard(ModuleTexts[black]));
                RightCardText.Add(FitTextInCard(ModuleTexts[white]));
            } else {
                LeftCardText.Add(FitTextInCard(ModuleTexts[white]));
                RightCardText.Add(FitTextInCard(ModuleTexts[black]));
            }
        }
    }

    void ChooseRandomCardTexts() {
        BlackCardText = new List<string>();
        WhiteCardText = new List<string>();
        StringBuilder Blacks = new StringBuilder();
        StringBuilder Whites = new StringBuilder();

        for (int i = 0; i < AMOUNT_OF_CARDS; i++) {
            string blackChosen = "";
            do {
                blackChosen = BlackModuleIDs[UnityEngine.Random.Range(0, BlackModuleIDs.Length - 1)];
            } while (BlackCardText.Contains(blackChosen));
            BlackCardText.Add(blackChosen);
            string whiteChosen = "";
            do {
                whiteChosen = WhiteModuleIDs[UnityEngine.Random.Range(0, WhiteModuleIDs.Length - 1)];
            } while (WhiteCardText.Contains(whiteChosen));
            WhiteCardText.Add(whiteChosen);
            Blacks.Append(blackChosen + "|");
            Whites.Append(whiteChosen + "|");
        }
    }
    #endregion

    #region BombInfo
    bool SpellPoop(string text) {
        int pCount = 0, oCount = 0;
        text = text.ToLower();
        foreach(char s in text) {
            if (s == 'p')
                pCount++;
            else if (s == 'o')
                oCount++;
        }
        return (pCount > 1 && oCount > 1);
    }

    int GetPortCount(ref int uniques) {
        int i = 0;
        List<string> portTypes = new List<string>();
        foreach(string s in mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_PORTS,null)) {
            foreach(string c in JsonConvert.DeserializeObject<Dictionary<string, string[]>>(s)["presentPorts"]) {
                if (!portTypes.Contains(c)) {
                    portTypes.Add(c);
                }
                i++;
            }
        }
        uniques = portTypes.Count;
        return i;
    }

    void GetLitUnlitCount(out int lit, out int unlit) {
        lit = 0;
        unlit = 0;
        List<string> Response = mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_INDICATOR, null);
        foreach (string Value in Response) {
            Dictionary<string, string> IndicatorInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(Value);
            if (IndicatorInfo["on"] == "True") {
                lit++;
            } else if (IndicatorInfo["on"] == "False") {
                unlit++;
            }
        }
    }

    int GetNumberOfBatteries() {
        int i = 0;
        List<string> Response = mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, null);
        foreach (string Value in Response) {
            Dictionary<string, int> batteryInfo = JsonConvert.DeserializeObject<Dictionary<string, int>>(Value);
            i += batteryInfo["numbatteries"];
        }
        return i;
    }

    bool GetSerialMAH() {
        string s = JsonConvert.DeserializeObject<Dictionary<string, string>>(mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER,null)[0])["serial"].ToLower();
        return s.Contains("m") || s.Contains("a") || s.Contains("h");
    }
    #endregion

    void OnActivate() {
        activated = true;
        List<string> moduleNames = mBombInfo.GetModuleNames();

        for(int i = 0; i < moduleNames.Count; i++) {
            moduleNames[i] = moduleNames[i].ToLower();
        }

        StringBuilder outString = new StringBuilder();
        outString.AppendFormat("[Modules Against Humanity #{0}] Modules:\n", MyModuleId);

        if (!SwapWhiteBlack) {
            outString.AppendFormat("    {0,-30}|{1,30}\n","Black","White");
        } else {
            outString.AppendFormat("    {0,-30}|{1,30}\n","White","Black");
        }

        for (int i = 0; i < BlackCardText.Count; i++) {
            if (!SwapWhiteBlack) {
                outString.AppendFormat("{0,2}: {1,-30}|{2,30}\n", i+1, BlackCardText[i], WhiteCardText[i]);
            } else {
                outString.AppendFormat("{0,2}: {1,-30}|{2,30}\n", i+1, WhiteCardText[i], BlackCardText[i]);
            }
        }
        outString.Append("\nText for first two cards:\n");
        outString.AppendFormat("Black: {0}\nWhite: {1}\n\n", ModuleTexts[BlackCardText[0]], ModuleTexts[WhiteCardText[0]]);


        int tempWhiteIndex = 0;
        int tempBlackIndex = 0;
        bool containsMAH = GetSerialMAH();
        int uniquePorts = 0;
        int ports = GetPortCount(ref uniquePorts);

        int tempLit, tempUnlit;
        GetLitUnlitCount(out tempLit, out tempUnlit);
        if (SpellPoop(ModuleTexts[BlackCardText[tempBlackIndex]])) {
            outString.Append("Could spell poop with initial black card\n");
            tempBlackIndex = 1;
        } else {
            outString.Append("Could not spell poop with initial black card\n");
            tempBlackIndex = (tempUnlit + ports-1 + BlackCardText.Count) % BlackCardText.Count;
        }
        outString.AppendFormat("Secondary black card position is: {0}, text is: {1}\n\n",tempBlackIndex + 1, ModuleTexts[BlackCardText[tempBlackIndex]]);

        if (SpellPoop(ModuleTexts[WhiteCardText[tempWhiteIndex]])) {
            outString.Append("Could spell poop with initial white card\n");
            tempWhiteIndex = 1;
        } else {
            outString.Append("Could not spell poop with initial white card\n");
            int i = GetNumberOfBatteries();
            tempWhiteIndex = (tempLit + i - 1 + BlackCardText.Count) % BlackCardText.Count;
        }
        outString.AppendFormat("Secondary white card position is: {0}, text is: {1}\n\n", tempWhiteIndex + 1, ModuleTexts[WhiteCardText[tempWhiteIndex]]);


        if (moduleNames.Contains(BlackCardText[tempBlackIndex]) && moduleNames.Contains(WhiteCardText[tempWhiteIndex])) {
            outString.Append("Both modules were found on the bomb\n");
            tempBlackIndex += 4;
            tempBlackIndex %= BlackCardText.Count;
            tempWhiteIndex += 3;
            tempWhiteIndex %= BlackCardText.Count;
        }else if (moduleNames.Contains(BlackCardText[tempBlackIndex])) {
            outString.Append("Black card module was found on the bomb\n");
            tempWhiteIndex += 2;
            tempWhiteIndex %= BlackCardText.Count;
        }else if (moduleNames.Contains(WhiteCardText[tempWhiteIndex])) {
            outString.Append("White card module was found on the bomb\n");
            tempBlackIndex += 1;
            tempBlackIndex %= BlackCardText.Count;
        } else {
            outString.Append("Neither module was found on the bomb\n");
            if (containsMAH) {
                outString.Append("Serial contained M/A/H\n");
                tempWhiteIndex -= 2;
                tempBlackIndex -= 2;
            }else if (!SwapWhiteBlack) {
                outString.Append("Black card was on left\n");
                tempWhiteIndex = tempLit + tempUnlit - 1;
                tempBlackIndex = uniquePorts - 1;
            }else {
                outString.Append("Otherwise rule\n");
                tempBlackIndex = moduleNames.Count - 1;
            }

            tempWhiteIndex += BlackCardText.Count;
            tempBlackIndex += BlackCardText.Count;
            tempWhiteIndex %= BlackCardText.Count;
            tempBlackIndex %= BlackCardText.Count;
        }
        if (!SwapWhiteBlack) {
            CorrectLeftCardIndex = (tempBlackIndex + BlackCardText.Count) % BlackCardText.Count;
            CorrectRightCardIndex = (tempWhiteIndex + BlackCardText.Count) % BlackCardText.Count;
        } else {
            CorrectLeftCardIndex = (tempWhiteIndex + BlackCardText.Count) % BlackCardText.Count;
            CorrectRightCardIndex = (tempBlackIndex + BlackCardText.Count) % BlackCardText.Count;
        }
        outString.Append("Final cards: ");
        if (!SwapWhiteBlack) {
            outString.AppendFormat("Black: {0}, White: {1}", tempBlackIndex + 1, tempWhiteIndex + 1);
        } else {
            outString.AppendFormat("White: {0}, Black: {1}", tempWhiteIndex + 1, tempBlackIndex + 1);
        }


        Debug.Log(outString.ToString());      
    }

    bool ResetButtonInteract() {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ResetButton.transform);
        LeftCardIndex = 0;
        UpdateLeftCardText();
        RightCardIndex = 0;
        UpdateRightCardText();
        return false;
    }

    bool CheckSolve() {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, AcceptButton.transform);
        if (!activated || IsSolved)
            return false;
        StringBuilder s = new StringBuilder();
        s.AppendFormat("[Modules Against Humanity #{0}] Submitted: {1}: {2}, {3}: {4} : ", MyModuleId, (!SwapWhiteBlack ? "Black" : "White"), LeftCardIndex + 1, (!SwapWhiteBlack ? "White" : "Black"), RightCardIndex + 1);

        if (LeftCardIndex == CorrectLeftCardIndex && RightCardIndex == CorrectRightCardIndex) {
            IsSolved = true;
            s.Append("Pass");
            mBombModule.HandlePass();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, this.transform);
        } else {
            s.Append("Strike");
            mBombModule.HandleStrike();
            ResetButtonInteract();
        }
        Debug.Log(s.ToString());
        return false;
    }

    // Use this for initialization
    void Start () {
        MyModuleId = ModuleId++;
        limit = GetWidth(DummyMesh);

        if(LeftCardMat == null) {
            LeftCardMat = LeftCardMesh.material;
        }
        if(RightCardMat == null) {
            RightCardMat = RightCardMesh.material;
        }

        SwapWhiteBlack = UnityEngine.Random.Range(0, 1000) % 2 == 0;
        LeftCardMaterial = Instantiate(LeftCardMat);
        RightCardMaterial = Instantiate(RightCardMat);
        LeftCardMesh.material = LeftCardMaterial;
        RightCardMesh.material = RightCardMaterial;
        
        mGameInfo.OnLightsChange += OnLightsChange;

        if (!SwapWhiteBlack) {
            LeftCardMaterial.color = Color.black;
            RightCardMaterial.color = Color.white;
            LeftCard.color = Color.white;
            RightCard.color = Color.black;
        } else {
            LeftCardMaterial.color = Color.white;
            RightCardMaterial.color = Color.black;
            LeftCard.color = Color.black;
            RightCard.color = Color.white;
        }

        for (int i = 0; i < LeftCardButtons.Length; i++) {
            int j = i;
            LeftCardButtons[j].OnInteract += () => { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, LeftCardButtons[j].transform); LeftCardIndex += (2 * j) - 1; UpdateLeftCardText(); return false; };
        }

        for (int i = 0; i < RightCardButtons.Length; i++) {
            int j = i;
            RightCardButtons[j].OnInteract += () => { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, RightCardButtons[j].transform); RightCardIndex += (2 * j) - 1; UpdateRightCardText(); return false; };
        }



        ResetButton.OnInteract += ResetButtonInteract;

        ChooseRandomCardTexts();
        GenerateCardTexts();

        
        AcceptButton.OnInteract += CheckSolve;
        mBombModule.OnActivate += OnActivate;
        
    }

    KMSelectable[] ProcessTwitchCommand(string command) {
        List<KMSelectable> buttons = new List<KMSelectable>();
        string[] split = command.ToLowerInvariant().Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 2) {
            if (split[0] != "press" || (split[1] != "submit" && split[1] != "reset"))
                return null;
            buttons.Add(split[1] == "reset" ? ResetButton : AcceptButton);
        }
        else if (split.Length == 3) {
            int pos;
            if (split[0] != "move" || (split[1] != "white" && split[1] != "black") || !int.TryParse(split[2], out pos))
                return null;
            pos %= AMOUNT_OF_CARDS;
            KMSelectable[] WhiteButtons = SwapWhiteBlack ? LeftCardButtons : RightCardButtons;
            KMSelectable[] BlackButtons = SwapWhiteBlack ? RightCardButtons : LeftCardButtons;
            KMSelectable[] SelectedButtons = split[1] == "white" ? WhiteButtons : BlackButtons;
            int index = pos < 0 ? 0 : 1;
            pos = Math.Abs(pos);
            for (int i = 0; i < pos; i++)
                buttons.Add(SelectedButtons[index]);
        } else {
            return null;
        }
        return buttons.ToArray();
    }

}
