using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

public class MAHModule : MonoBehaviour {

    #region Imports
    public KMSelectable[] leftCardButtons;
    public KMSelectable[] rightCardButtons;
    public KMSelectable resetButton;
    public KMSelectable acceptButton;
    public TextMesh leftCard;
    public TextMesh rightCard;
    public KMBombInfo mBombInfo;
    public KMBombModule mBombModule;
    public static Material leftCardMat;
    public static Material rightCardMat;
    public MeshRenderer leftCardMesh;
    public MeshRenderer rightCardMesh;
    public KMGameInfo mGameInfo;
    public TextMesh dummyMesh;
    public new KMAudio audio;

    #endregion

    //Amount of cards
    private const int AMOUNT_OF_CARDS = 10;

    //Ids arrays for each individual module separated by what type of card it is
    private static string[] whiteModuleIds = { "wire sequence", "simon says", "maze", "memory", "needy capacitor", "who's on first", "needy vent gas", "modules against humanity", "needy knob", "morse code", "two bits", "anagrams", "word scramble", "semaphore", "colour flash", "logic", "listening", "mystic square", "crazy talk", "silly slots", "probing", "forget me not", "morsematics", "simon states", "perspective pegs", "caesar cipher", "tic tac toe", "astrology", "adventure game", "skewed slots", "blind alley", "english test", "mouse in the maze", "turn the keys", "turn the key", "tetris", "sea shells", "murder","adjacent letters","colored squares" ,"hexamaze" ,"souvenir", "simon screams", "http response", "wire placement", "coordinates", "battleship", "game of life simple", "colored switches", "the clock", "button sequences", "burglar alarm",  "backgrounds", "the stopwatch",  "the iphone", "ice cream", "the swan", "monsplode trading cards", "neutralization", "the sun", "european travel", "blind maze", "cheap checkout", "radiator", "discolored squares", "tasha squeals", "bases", "playfair cipher",};
    private static string[] blackModuleIds = { "the button", "password", "wires", "keypad", "complicated wires", "chess", "switches", "emoji math", "letter keys", "orientation cube", "piano keys", "connection check", "cryptography", "number pad", "alphabet", "round keypad", "plumbing", "safety safe", "resistors", "microcontroller", "the gamepad", "laundry", "3d maze", "follow the leader", "friendship", "the bulb", "monsplode, fight!", "foreign exchange rates", "combination lock", "shape shift", "needy math", "lights out", "motion sense", "needy rotary phone", "needy quiz", "who's that monsplode?", "filibuster", "third base", "bitmaps", "rock-paper-scissors-l.-sp.", "square button", "broken buttons", "word search", "complicated buttons", "symbolic password", "light cycle", "text field", "double-oh", "game of life cruel", "chord qualities", "big circle", "braille", "creation", "cooking", "zoo", "hunting", "symbolic coordinates", "the moon", "mortal kombat", "faulty backgrounds", "unfair cipher", "forget everything", "tangrams", "purgatory", "scripting", "subscribe to pewdiepie", "3d tunnels", "101 dalmatians"};

    //Dictionary of ids to texts
    private static Dictionary<string, string> ModuleTexts = new Dictionary<string, string>() {
        { "3d maze", "Want to see this in 3D? There are special glasses for it." },
        { "3d tunnels", "If you can't solve this bomb, just tunnel your way out."},
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
        { "double-oh", "Who cares about Double As or Double Ds when you can have Double Os?" },
        { "emoji math" , "Did you know your texts can include words, and not just emojis?" },
        { "english test", "Do you know the difference between your crap and you're crap?" },
        { "filibuster", "Remember the title and just keep talking." },
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
        { "piano keys", "Musical instruments? I recommend trying piano." },
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
        { "word search", "You’re searching for a word? It’s the bird." },
        { "ice cream", "I scream, you scream, but only I love it." },
        { "the swan", "Your serial number is 4815162342." },
        { "monsplode trading cards", "Who cares about Monsplodes? Trade them away!" },
        { "neutralization", "Your goal is to neutralize this bomb." },
        { "the sun", "It's so bright! Who turned the sun on?" },
        { "european travel", "Europe is a great destination for travels." },
        { "blind maze", "You'll feel like a blind man in this maze." },
        { "cheap checkout", "Check out this bomb sale! It's so cheap!" },
        { "the iphone", "Put down your iPhone and solve this bomb!" },
        { "the stopwatch", "Stop the watch! Hammer time!" },
        { "backgrounds", "Are you sure you want that background for your photo?" },
        { "burglar alarm", "This burglar alarm is distracting me from defusing." },
        { "button sequences", "You can't just press all the buttons, it has to be the correct sequence." },
        { "the clock", "Tick tock, start the bomb clock." },
        { "colored switches", "Let's switch on these colours." },
        { "game of life simple", "I want to play a game. A game of life and death." },
        { "faulty backgrounds", "You bombed that selfie cause of faulty background." },
        { "mortal kombat", "The kombat with this bomb is a mortal one." },
        { "the moon", "Darkness all around, not even the moon is shining." },
        { "symbolic coordinates", "Coordination does not exist, it's only symbolic." },
        { "hunting", "If you explode, I'm gonna hunt you down." },
        { "zoo", "You're an animal. You should be in a zoo." },
        { "cooking", "You blew me up even though I cooked for you!" },
        { "creation", "God created the world and you blew it up. Shame on you." },
        { "braille", "The bomb blew your eyes out. Start learning Braille." },
        { "big circle", "You blow up, you die. The big circle of life." },
        { "chord qualities", "You struck the wrong chords. Kaboom." },
        { "game of life cruel", "Life is cruel. So are games." },
        { "unfair cipher", "Bombs are unfair. So are ciphers." },
        { "the hexabutton", "Push the hexabutton. Galvanize." },
        { "forget everything", "Well done. You forgot everything." },
        { "tangrams", "You blew out the circuit. Module solved." },
        { "radiator", "If it's hot in here, it's just you." },
        { "discolored squares", "Well done idiot. You discolored all your squares." },
        { "the triangle", "The Illuminati sent this bomb, and this triangle." },
        { "tasha squeals", "Move over Simon. Tasha is here, and is better than you." },
        { "purgatory", "When you blow up, you're going to purgatory." },
        { "scripting", "Don't like these modules? Script a new one." },
        { "bases", "What, do you not know the bases?" },
        { "playfair cipher", "This bomb doesn't play fair." },
        { "subscribe to pewdiepie", "Subscribe to pewdiepie. This urination match has gone on long enough." },
        { "101 dalmatians", "Spot the 101 differences!" }
    };

    //Texts to display on white/black
    private List<string> whiteCardText;
    private List<string> blackCardText;


    //Texts to display on left/right
    private List<string> leftCardText;
    private List<string> rightCardText;


    //Materials to change color for left/right
    private Material leftCardMaterial;
    private Material rightCardMaterial;

    //Indexes for text on cards
    private int leftCardIndex;
    private int rightCardIndex;

    //Indexes for correct answers
    private int correctLeftCardIndex;
    private int correctRightCardIndex;

    //Bool determining which side white and black should be on.
    private bool swapWhiteBlack;

    //Float for the max size text can be in one of the card text meshes
    //Generated from a premade 'dummy' text mesh that is hidden inside the module
    //This text mesh has certain characters that together span the length of the card
    private float limit;

    private static int moduleId = 1;
    private int myModuleId;

    private bool isSolved = false;

    private bool isActive = false;

    #region CardFormatting
    //Get the width of text in a text mesh
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

    //Attempt to fix all the Text into a card with no overlapping the edge.
    string FitTextInCard(string text) {
        StringBuilder txt = new StringBuilder();
        //Get length of a space
        dummyMesh.text = "a";
        float spaceLength = GetWidth(dummyMesh);
        dummyMesh.text = "a a";
        spaceLength = GetWidth(dummyMesh) - spaceLength - spaceLength;

        //Split the text by spaces, and then add the first word to a working stringbuilder.
        float currAmount = 0;
        string[] split = text.Split(' ');
        txt.Append(split[0]);
        dummyMesh.text = split[0];
        currAmount = GetWidth(dummyMesh);

        //Loop through the rest of the words and attempt to fit
        for(int i = 1; i < split.Length; i++) {
            //Add a space, then get the width of the next word
            txt.Append(" ");
            dummyMesh.text = split[i];
            float currLength = GetWidth(dummyMesh);

            //If width of line is greater than card width, then add newline and reset
            if((currAmount + spaceLength + currLength) > limit) {
                txt.Append("\n");
                currAmount = currLength;
            }else {
                currAmount += spaceLength + currLength;
            }
            
            txt.Append(dummyMesh.text);
        }
        return txt.ToString();
    }
    #endregion

    #region CardText
    //Loop around card text
    void UpdateLeftCardText() {
        leftCardIndex += AMOUNT_OF_CARDS;
        leftCardIndex %= AMOUNT_OF_CARDS;
        leftCard.text = leftCardText[leftCardIndex];
    }

    void UpdateRightCardText() {
        rightCardIndex += AMOUNT_OF_CARDS;
        rightCardIndex %= AMOUNT_OF_CARDS;
        rightCard.text = rightCardText[rightCardIndex];
    }

    //When lights turn on, show text, when lights turn off, void text
    private void OnLightsChange(bool on) {
        if (!isActive) return;
        if (on) {
            UpdateLeftCardText();
            UpdateRightCardText();
        } else {
            leftCard.text = "";
            rightCard.text = "";
        }
    }

    //Generate the leftcard/rightcard text based off the black/white texts already randomly picked
    //Run it through FixTextInCard to insert the newlines in the right spaces.
    private void GenerateCardTexts() {
        leftCardText = new List<string>();
        rightCardText = new List<string>();
        for (int i = 0; i < AMOUNT_OF_CARDS; i++) {
            string black = blackCardText[i];
            string white = whiteCardText[i];
            if (!swapWhiteBlack) {
                leftCardText.Add(FitTextInCard(ModuleTexts[black]));
                rightCardText.Add(FitTextInCard(ModuleTexts[white]));
            } else {
                leftCardText.Add(FitTextInCard(ModuleTexts[white]));
                rightCardText.Add(FitTextInCard(ModuleTexts[black]));
            }
        }
    }

    //Randomly select indexes from the card texts lists based on the module ids.
    //Repeat until full.
    //It does have a 50% ish chance to choose a module on the bomb as opposed to a random module
    void ChooseRandomCardTexts(List<string> modules) {
        blackCardText = new List<string>();
        whiteCardText = new List<string>();
        

        //Count and categorise modules on the bomb
        List<string> blackModules = new List<string>();
        List<string> whiteModules = new List<string>();

        foreach (string s in modules)
        {
            if (whiteModuleIds.Contains(s))
            {
                whiteModules.Add(s);
            }
            else if (blackModuleIds.Contains(s))
            {
                blackModules.Add(s);
            }
        }

        
        //Generate all cards for both black and white
        for (int i = 0; i < AMOUNT_OF_CARDS; i++) {
            //Randomly generate a number to choose for a chance of module ids on the bomb
            string blackChosen = "";
            int a = UnityEngine.Random.Range(1, 101);
            
            //If we want a random module OR we have no more modules to pick from choose random modules until we get one we haven't gotten
            if (a > 50 || blackModules.Count == 0)
                do {
                    blackChosen = blackModuleIds[UnityEngine.Random.Range(0, blackModuleIds.Length)];
                } while (blackCardText.Contains(blackChosen));
            else
            {
                //Else, choose from modules on bomb
                do
                {
                    blackChosen = blackModules[UnityEngine.Random.Range(0, blackModules.Count)];
                } while (blackCardText.Contains(blackChosen));
            }
            //If we picked a module that's on the bomb, through the 50% or through luck, delete it
            if (blackModules.Contains(blackChosen)) blackModules.Remove(blackChosen);
            //Add to list
            blackCardText.Add(blackChosen);

            //Exact same process for white
            string whiteChosen = "";
            a = UnityEngine.Random.Range(1, 101);
            if (a > 50 || whiteModules.Count == 0) { 
                do {
                    whiteChosen = whiteModuleIds[UnityEngine.Random.Range(0, whiteModuleIds.Length)];
                } while (whiteCardText.Contains(whiteChosen));
            }
            else
            {
                do
                {
                    whiteChosen = whiteModules[UnityEngine.Random.Range(0, whiteModules.Count)];
                } while (whiteCardText.Contains(whiteChosen));
            }
            if (whiteModules.Contains(whiteChosen)) whiteModules.Remove(whiteChosen);
            whiteCardText.Add(whiteChosen);
        }
    }
    

   
    //If we can spell the word poop from the text on a card
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
    #endregion

    #region BombInfo
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
    //Indicators
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

    //On bomb activation
    void OnActivate() {

        //Get all modules, then lowercase them and get the Distinct entries. 
        //So duplicate modules on the bomb aren't counted. So we don't have duplicate card entries.
        List<string> moduleNames = mBombInfo.GetModuleNames();

        for (int i = 0; i < moduleNames.Count; i++) {
            moduleNames[i] = moduleNames[i].ToLower();
        }
        

        ChooseRandomCardTexts(moduleNames.Distinct().ToList());
        GenerateCardTexts();


        //Get the length of the longest word, for log formatting purposes.
        int longestWord = 0;
        for(int i = 0; i < AMOUNT_OF_CARDS;i++)
        {
            longestWord = blackCardText[i].Length > longestWord ? blackCardText[i].Length : longestWord;
            longestWord = whiteCardText[i].Length > longestWord ? whiteCardText[i].Length : longestWord;
        }

        //Logging stuff, so that the state of the module can be decided.
        StringBuilder outString = new StringBuilder();
        outString.AppendFormat("[Modules Against Humanity #{0}] Modules:\n", myModuleId);
        //Log which side white and black are on.
        var formatString1 = "    {0," + -longestWord + "}|{1," + longestWord + "}\n";
        if (!swapWhiteBlack) {
            outString.Append(String.Format(formatString1,"Black","White"));
        } else {
            outString.Append(String.Format(formatString1, "White","Black"));
        }
        //Log the module ids in proper columns
        string formatString2 = "{0,2}: {1," + -longestWord + "}|{2," + longestWord + "}\n";
        for (int i = 0; i < AMOUNT_OF_CARDS; i++) {
            if (!swapWhiteBlack) {
                outString.Append(String.Format(formatString2, i+1, blackCardText[i], whiteCardText[i]));
            } else {
                outString.Append(String.Format(formatString2, i+1, whiteCardText[i], blackCardText[i]));
            }
        }

        //Log the starting texts
        outString.Append("\nText for first two cards:\n");
        outString.AppendFormat("Black: {0}\nWhite: {1}\n\n", ModuleTexts[blackCardText[0]], ModuleTexts[whiteCardText[0]]);

        //Solution calculation
        //Start off with dummy solution indexes
        int tempWhiteIndex = 0;
        int tempBlackIndex = 0;

        //Get information from the bomb
        bool containsMAH = GetSerialMAH();
        int uniquePorts = 0;
        int ports = GetPortCount(ref uniquePorts);

        int tempLit, tempUnlit;
        GetLitUnlitCount(out tempLit, out tempUnlit);

        //If the text on the first card can spell 'poop', yes, I'm sad too.
        //Determine black/white secondary position based on the case
        if (SpellPoop(ModuleTexts[blackCardText[tempBlackIndex]])) {
            outString.Append("Could spell poop with initial black card\n");
            tempBlackIndex = 1;
        } else {
            outString.Append("Could not spell poop with initial black card\n");
            tempBlackIndex = (tempUnlit + ports-1 + AMOUNT_OF_CARDS) % AMOUNT_OF_CARDS;
        }
        outString.AppendFormat("Secondary black card position is: {0}, text is: {1}\n\n",tempBlackIndex + 1, ModuleTexts[blackCardText[tempBlackIndex]]);

        if (SpellPoop(ModuleTexts[whiteCardText[tempWhiteIndex]])) {
            outString.Append("Could spell poop with initial white card\n");
            tempWhiteIndex = 1;
        } else {
            outString.Append("Could not spell poop with initial white card\n");
            int i = GetNumberOfBatteries();
            tempWhiteIndex = (tempLit + i - 1 + AMOUNT_OF_CARDS) % AMOUNT_OF_CARDS;
        }
        outString.AppendFormat("Secondary white card position is: {0}, text is: {1}\n\n", tempWhiteIndex + 1, ModuleTexts[whiteCardText[tempWhiteIndex]]);


        //If the cards reference a module on the bomb.
        if (moduleNames.Contains(blackCardText[tempBlackIndex]) && moduleNames.Contains(whiteCardText[tempWhiteIndex])) {
            outString.Append("Both modules were found on the bomb\n");
            tempBlackIndex += 4;
            tempBlackIndex %= AMOUNT_OF_CARDS;
            tempWhiteIndex += 3;
            tempWhiteIndex %= AMOUNT_OF_CARDS;
        }else if (moduleNames.Contains(blackCardText[tempBlackIndex])) {
            outString.Append("Black card module was found on the bomb\n");
            tempWhiteIndex += 2;
            tempWhiteIndex %= AMOUNT_OF_CARDS;
        }else if (moduleNames.Contains(whiteCardText[tempWhiteIndex])) {
            outString.Append("White card module was found on the bomb\n");
            tempBlackIndex += 1;
            tempBlackIndex %= AMOUNT_OF_CARDS;
        } else {
            //If neither, then test rules in otherwise section.
            outString.Append("Neither module was found on the bomb\n");
            if (containsMAH) {
                outString.Append("Serial contained M/A/H\n");
                tempWhiteIndex -= 2;
                tempBlackIndex -= 2;
            }else if (!swapWhiteBlack) {
                outString.Append("Black card was on left\n");
                tempWhiteIndex = tempLit + tempUnlit - 1;
                tempBlackIndex = uniquePorts - 1;
            }else {
                outString.Append("Otherwise rule\n");
                tempBlackIndex = moduleNames.Count - 1;
            }

            tempWhiteIndex += AMOUNT_OF_CARDS;
            tempBlackIndex += AMOUNT_OF_CARDS;
            tempWhiteIndex %= AMOUNT_OF_CARDS;
            tempBlackIndex %= AMOUNT_OF_CARDS;
        }
        //Determine which ones are the right indexes to set, then set the solution indexes
        if (!swapWhiteBlack) {
            correctLeftCardIndex = (tempBlackIndex + AMOUNT_OF_CARDS) % AMOUNT_OF_CARDS;
            correctRightCardIndex = (tempWhiteIndex + AMOUNT_OF_CARDS) % AMOUNT_OF_CARDS;
        } else {
            correctLeftCardIndex = (tempWhiteIndex + AMOUNT_OF_CARDS) % AMOUNT_OF_CARDS;
            correctRightCardIndex = (tempBlackIndex + AMOUNT_OF_CARDS) % AMOUNT_OF_CARDS;
        }

        //Final logging stuffs.
        outString.Append("Final cards: ");
        if (!swapWhiteBlack) {
            outString.AppendFormat("Black: {0}, White: {1}", tempBlackIndex + 1, tempWhiteIndex + 1);
        } else {
            outString.AppendFormat("White: {0}, Black: {1}", tempWhiteIndex + 1, tempBlackIndex + 1);
        }

        Debug.Log(String.Format("[Modules Against Humanity#{0}]: Total number of modules on the bomb: {1}",myModuleId,moduleNames.Count));
        Debug.Log(outString.ToString());
        isActive = true;
    }

    //Button Interaction handlers
    
    //Reset button is hit, reset cards to 0 index
    bool ResetButtonInteract() {
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, resetButton.transform);
        leftCardIndex = 0;
        UpdateLeftCardText();
        rightCardIndex = 0;
        UpdateRightCardText();
        return false;
    }

    //Solve button it hit, check if the submitted indexes are the right indexes.
    bool CheckSolve() {
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, acceptButton.transform);
        if (!isActive || isSolved)
            return false;
        StringBuilder s = new StringBuilder();
        s.AppendFormat("[Modules Against Humanity #{0}] Submitted: {1}: {2}, {3}: {4} : ", myModuleId, (!swapWhiteBlack ? "Black" : "White"), leftCardIndex + 1, (!swapWhiteBlack ? "White" : "Black"), rightCardIndex + 1);

        if (leftCardIndex == correctLeftCardIndex && rightCardIndex == correctRightCardIndex) {
            isSolved = true;
            s.Append("Pass");
            Debug.Log(s.ToString());
            mBombModule.HandlePass();
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, this.transform);
        } else {
            s.Append("Strike");
            Debug.Log(s.ToString());
            mBombModule.HandleStrike();
            ResetButtonInteract();
        }
        return false;
    }

    //Init
    void Start () {
        myModuleId = moduleId++;
        //Set limit width of text to the predefined length generated in the editor
        limit = GetWidth(dummyMesh);

        //Set the materials to the right materials for the meshes
        if(leftCardMat == null) {
            leftCardMat = leftCardMesh.material;
        }
        if(rightCardMat == null) {
            rightCardMat = rightCardMesh.material;
        }

        //Randomly decide if the cards should swap colors
        swapWhiteBlack = UnityEngine.Random.Range(0, 1000) % 2 == 0;

        //Instantiate a NEW version of the material.
        //Stops the effect of changing one modules color to change them all.
        leftCardMaterial = Instantiate(leftCardMat);
        rightCardMaterial = Instantiate(rightCardMat);
        leftCardMesh.material = leftCardMaterial;
        rightCardMesh.material = rightCardMaterial;
        
        //Set the lights change handler
        mGameInfo.OnLightsChange += OnLightsChange;

        //Determine the swap sides, then set the card and the text colors accordingly
        if (!swapWhiteBlack) {
            leftCardMaterial.color = Color.black;
            rightCardMaterial.color = Color.white;
            leftCard.color = Color.white;
            rightCard.color = Color.black;
        } else {
            leftCardMaterial.color = Color.white;
            rightCardMaterial.color = Color.black;
            leftCard.color = Color.black;
            rightCard.color = Color.white;
        }

        //Set handlers
        for (int i = 0; i < leftCardButtons.Length; i++) {
            int j = i;
            leftCardButtons[j].OnInteract += () => { audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, leftCardButtons[j].transform); leftCardIndex += (2 * j) - 1; UpdateLeftCardText(); return false; };
        }

        for (int i = 0; i < rightCardButtons.Length; i++) {
            int j = i;
            rightCardButtons[j].OnInteract += () => { audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, rightCardButtons[j].transform); rightCardIndex += (2 * j) - 1; UpdateRightCardText(); return false; };
        }



        resetButton.OnInteract += ResetButtonInteract;



        
        acceptButton.OnInteract += CheckSolve;
        mBombModule.OnActivate += OnActivate;
        
    }

    //Made by Caitsith2, twitch plays support.
    KMSelectable[] ProcessTwitchCommand(string command) {
        List<KMSelectable> buttons = new List<KMSelectable>();
        string[] split = command.ToLowerInvariant().Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 2) {
            if (split[0] != "press" || (split[1] != "submit" && split[1] != "reset"))
                return null;
            buttons.Add(split[1] == "reset" ? resetButton : acceptButton);
        }
        else if (split.Length == 3) {
            int pos;
            if (split[0] != "move" || (split[1] != "white" && split[1] != "black") || !int.TryParse(split[2], out pos))
                return null;
            pos %= AMOUNT_OF_CARDS;
            KMSelectable[] WhiteButtons = swapWhiteBlack ? leftCardButtons : rightCardButtons;
            KMSelectable[] BlackButtons = swapWhiteBlack ? rightCardButtons : leftCardButtons;
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
