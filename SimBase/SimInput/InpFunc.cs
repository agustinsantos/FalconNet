using FalconNet.F4Common;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FalconNet.SimBase.SimInput
{
    public delegate void InputFunctionType(uint val, int state, object userdata);

    public struct joyButton
    {
        public InputFunctionType func;   //this is the pointer to the appropriate function
        public int cpButtonID; //this is the ID of the associated cockpit button
    }


    public class POVfunc
    {
        public const int MAX_POV_DIR = 8;

        public readonly InputFunctionType[] func = new InputFunctionType[MAX_POV_DIR];   //this is the pointer to the appropriate function
        public readonly int[] cpButtonID = new int[MAX_POV_DIR]; //this is the ID of the associated cockpit button

        public POVfunc()
        {
            for (int j = 0; j < MAX_POV_DIR; j++)
            {
                this.func[j] = null;
                this.cpButtonID[j] = -1;
            }
        }
    }

    public class FunctionPtrListEntry
    {
        public InputFunctionType theFunc;
        public int buttonId;
        public int mouseSide;
        public int flags;
        public long controlID;
        public FunctionPtrListEntry next;
    }

    public class InputFunctionHashTable
    {

        // Retro 31Dec2003 - these values are also in sinput.h should not be defined in duplicate
        // FIXME
        public const int SIMLIB_MAX_DIGITAL = 32;
        public const int SIM_NUMDEVICES = 16;
        public const int SIMLIB_MAX_POV = 4;

        public const int NumHashEntries = 256 /*DIK_APPS +1*/;
        public const int NumButtons = SIMLIB_MAX_DIGITAL * SIM_NUMDEVICES;
        public const int NumPOVs = SIMLIB_MAX_POV;


        private FunctionPtrListEntry[] functionTable = new FunctionPtrListEntry[NumHashEntries];
        private joyButton[] buttonTable = new joyButton[NumButtons];
        private POVfunc[] POVTable = new POVfunc[NumPOVs];


        public InputFunctionHashTable()
        {
            //for (int i = 0; i < NumHashEntries; i++)
            //{
            //    functionTable[i] = NULL;
            //}

            for (int i = 0; i < NumButtons; i++)
            {
                buttonTable[i].func = null;
                buttonTable[i].cpButtonID = -1;
            }

            for (int i = 0; i < NumPOVs; i++)
            {
                POVTable[i] = new POVfunc();
                //for (int j = 0; j < 8; j++)
                // {
                //     POVTable[i].func[j] = null;
                //     POVTable[i].cpButtonID[j] = -1;
                // }
            }
        }

        //~InputFunctionHashTable();

        public void AddFunction(int key, int flags, int buttonId, int mouseSide, InputFunctionType funcPtr)
        {
            if (key < 0 || key >= NumHashEntries)
                return;

            // Check for duplicate
            FunctionPtrListEntry tmpEntry = functionTable[key];

            while (tmpEntry != null)
            {
                if (tmpEntry.flags == flags)
                    break;

                tmpEntry = tmpEntry.next;
            }

            // F4Assert (tmpEntry == NULL);

#if USE_SH_POOLS
    tmpEntry = (FunctionPtrListEntry*)MemAllocFS(gInputMemPool);
#else
            tmpEntry = new FunctionPtrListEntry();
#endif
            tmpEntry.mouseSide = mouseSide;
            tmpEntry.buttonId = buttonId;
            tmpEntry.flags = flags;
            tmpEntry.theFunc = funcPtr;
            tmpEntry.controlID = 0;
            tmpEntry.next = functionTable[key];
            functionTable[key] = tmpEntry;
        }

        public void RemoveFunction(int key, int flags)
        {
            FunctionPtrListEntry lastEntry = null;

            if (key == -1)
                return;

            FunctionPtrListEntry tmpEntry = functionTable[key];

            while (tmpEntry != null)
            {
                if (tmpEntry.flags == flags)
                    break;

                lastEntry = tmpEntry;
                tmpEntry = tmpEntry.next;
            }

            if (tmpEntry != null && tmpEntry.flags == flags)
            {
                if (lastEntry != null)
                    lastEntry.next = tmpEntry.next;
                else
                    functionTable[key] = tmpEntry.next;

#if USE_SH_POOLS
                MemFreeFS(tmpEntry);
#else
                //delete tmpEntry;
#endif
            }
        }

        public void ClearTable() { throw new NotImplementedException(); }

        public InputFunctionType GetFunction(int key, int flags, out int pbuttonId, out int pmouseSide)
        {
            pbuttonId = -1;
            pmouseSide = -1;

            InputFunctionType retval = null;

            if (key == -1)
                return null;

            FunctionPtrListEntry tmpEntry = functionTable[key];

            while (tmpEntry != null)
            {
                if (tmpEntry.flags == flags)
                    break;

                tmpEntry = tmpEntry.next;
            }

            if (tmpEntry != null)
            {
                pbuttonId = tmpEntry.buttonId;
                pmouseSide = tmpEntry.mouseSide;
                retval = tmpEntry.theFunc;
            }

            return retval;
        }
        public int GetButtonId(InputFunctionType funcPtr)  //Wombat778 2-05-04
        {
            FunctionPtrListEntry tmpEntry;

            for (int i = 0; i < NumHashEntries; i++) //Wombat778 2-05-04 I will burn in hell for doing this to a hash table
            {
                tmpEntry = functionTable[i];

                while (tmpEntry != null)
                {
                    if (tmpEntry.theFunc == funcPtr)
                        return tmpEntry.buttonId;

                    tmpEntry = tmpEntry.next;
                }

            }

            return 0;
        }

        public long GetControl(int key, int flags)
        {
            FunctionPtrListEntry tmpEntry;
            long retval = 0;

            if (key == -1)
                return retval;

            tmpEntry = functionTable[key];

            while (tmpEntry != null)
            {
                if (tmpEntry.flags == flags)
                    break;

                tmpEntry = tmpEntry.next;
            }

            if (tmpEntry != null)
            {
                retval = tmpEntry.controlID;
            }

            return retval;
        }

        public bool SetControl(int key, int flags, long control)
        {
            FunctionPtrListEntry tmpEntry;
            bool retval = false;

            if (key == -1)
                return retval;

            tmpEntry = functionTable[key];

            while (tmpEntry != null)
            {
                if (tmpEntry.flags == flags)
                    break;

                tmpEntry = tmpEntry.next;
            }

            if (tmpEntry != null)
            {
                tmpEntry.controlID = control;
                retval = true;
            }

            return retval;
        }

        public bool SetButtonFunction(int buttonId, InputFunctionType theFunc, int cpButtonID)
        {
            if (buttonId < 0 || buttonId >= NumButtons)
                return false;

            buttonTable[buttonId].func = theFunc;
            buttonTable[buttonId].cpButtonID = cpButtonID;
            return true;
        }

        public InputFunctionType GetButtonFunction(int buttonID, ref int cpButtonID)
        {
            if (buttonID < 0 || buttonID >= NumButtons)
            {
                if (cpButtonID != 0)
                    cpButtonID = -1;

                return null;
            }

            if (cpButtonID != 0)
                cpButtonID = buttonTable[buttonID].cpButtonID;

            return buttonTable[buttonID].func;
        }

        public bool SetPOVFunction(int POV, int dir, InputFunctionType theFunc, int cpButtonID)
        {
            if (POV < 0 || POV >= NumPOVs)
                return false;

            if (dir < 0 || dir >= POVfunc.MAX_POV_DIR)
                return false;

            POVTable[POV].func[dir] = theFunc;
            POVTable[POV].cpButtonID[dir] = cpButtonID;
            return true;
        }

        public InputFunctionType GetPOVFunction(int POV, int dir, ref int cpButtonID)
        {
            if (POV < 0 || POV >= NumPOVs || dir < 0 || dir >= POVfunc.MAX_POV_DIR)
            {
                if (cpButtonID != 0)
                    cpButtonID = -1;

                return null;
            }

            if (cpButtonID != 0)
                cpButtonID = POVTable[POV].cpButtonID[dir];

            return POVTable[POV].func[dir];
        }

        private static InputFunctionHashTable UserFunctionTable = new InputFunctionHashTable();

#if TODO

        extern int CommandsKeyCombo;
        extern int CommandsKeyComboMod;
#endif
        public static void SetupInputFunctions()
        {
            throw new NotImplementedException();
        }

        public static void CleanupInputFunctions()
        {
            //UserFunctionTable.ClearTable();
        }

        public static void CallInputFunction(uint val, int state, object userdata)
        {
            throw new NotImplementedException();
        }

        private static readonly Regex pattern = new Regex(@"(\w+)\s+(-?\w+)\s+(-?\w+)\s+(-?\w+)\s+(-?\w+)\s+((-?\w+)\s+(-?\w+)\s+)?(-?\w+)\s+""([^""]*)""");
        private static int ParseInt(string val)
        {
            if (val.StartsWith("0x"))
                return int.Parse(val.Substring(2), System.Globalization.NumberStyles.HexNumber);
            else
                return int.Parse(val);
        }

        public static void LoadFunctionTables(string fname)// = PlayerOptionsClass.PlayerOptions.GetKeyfile())
        {
            SimlibFileClass funcFile;
            string tmpStr;
            string dir = Path.Combine(F4File.FalconDirectory, "Config");

            //char pilotName[_MAX_PATH] = {0};
            int key1 = -1, mod1 = -1;
            int key2 = -1, mod2 = -1;
            int flags, buttonId = -1, mouseSide = -1;
            InputFunctionType theFunc;

            string fileName = Path.Combine(dir, fname + ".key");

            funcFile = SimlibFileClass.Open(fileName, FileMode.Open, FileAccess.Read);

            if (funcFile == null)
            {
                //fileName = string.Format("{0}\\User\Config\\keystrokes.key", F4File.FalconDataDirectory);
                fileName = Path.Combine(dir, "keystrokes.key");
                funcFile = SimlibFileClass.Open(fileName, FileMode.Open, FileAccess.Read);

                if (funcFile == null)
                {
                    throw new Exception("No Function Table File\n");
                    //return;
                }
            }

            while ((tmpStr = funcFile.ReadLine()) != null)
            {
                // Skip Comments
                if (string.IsNullOrWhiteSpace(tmpStr) || tmpStr[0] == '#')
                    continue;

                string funcName = null;
                //sscanf(tmpStr, "%s %d %d %x %x %x %x %*[^\n]", funcName, &buttonId, &mouseSide, &key2, &mod2, &key1, &mod1);
                var match = pattern.Match(tmpStr);
                try
                {
                    Debug.Assert(match.Success);
                    int gn = 1;
                    funcName = match.Groups[gn++].Value;
                    buttonId = int.Parse(match.Groups[gn++].Value);
                    mouseSide = int.Parse(match.Groups[gn++].Value);
                    key2 = ParseInt(match.Groups[gn++].Value);
                    mod2 = ParseInt(match.Groups[gn++].Value);
                    if (!String.IsNullOrEmpty(match.Groups[gn++].Value))
                    {
                        key1 = ParseInt(match.Groups[gn++].Value);
                        mod1 = ParseInt(match.Groups[gn++].Value);
                    }
                    else gn += 2;
                    int p = int.Parse(match.Groups[gn++].Value);
                    string guitext = match.Groups[gn++].Value;
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }
                theFunc = UserFunction.FindFunctionFromString(funcName);

                if (theFunc != null)
                {
                    if (key1 == -1)
                    {
                        flags = mod1 + (key2 << SECOND_KEY_SHIFT) + (mod2 << SECOND_KEY_MOD_SHIFT);

                        //for (int i=0; i<UserFunctionTable.NumHashEntries; i++)
                        for (int i = DIK_1; i <= DIK_0; i++)

                        {
                            //Find this key combo
                            UserFunctionTable.AddFunction(i, flags, buttonId, mouseSide, theFunc);
                        }

                        UserFunctionTable.AddFunction(DIK_ESCAPE, flags, buttonId, mouseSide, theFunc);
                        UserFunctionTable.AddFunction(DIK_SYSRQ, flags, buttonId, mouseSide, theFunc); // screen shot
                    }
                    else
                    {
                        //this function has no key combo assigned
                        if (key2 == -1)
                            continue;

                        if (key2 == -2)
                        {
                            UserFunctionTable.SetButtonFunction(buttonId, theFunc, mouseSide);
                            //int this case mouseside contains the cockpit button ID while
                            //buttonID is the corresponding joystick button
                        }
                        else if (key2 == -3)
                        {
                            UserFunctionTable.SetPOVFunction(buttonId, mod2, theFunc, mouseSide);
                            //int this case mouseside contains the cockpit button ID while
                            //buttonID is the corresponding hat
                            //mod2 is the direction the hat is pressed
                        }
                        else
                        {
                            //Find this key combo
                            flags = mod2 + (key1 << SECOND_KEY_SHIFT) + (mod1 << SECOND_KEY_MOD_SHIFT);
                            UserFunctionTable.AddFunction(key2, flags, buttonId, mouseSide, theFunc);
                        }
                    }
                }
                else
                {
                    // MonoPrint ("ERROR !!!!! %s not found\n", funcName);
# if  DEBUG
                    //sprintf (tmpStr, "ERROR !!!!! %s not found\n", funcName);
                    //OutputDebugString (tmpStr);
#endif
                }
            }

            funcFile.Close();
            //delete funcFile;

#if TODO
            //Wombat778 10-07-2003 Load scroll wheel functions. Added these here because I need to be 100% sure that keys have been loaded.
            scrollupfunc = UserFunction.FindFunctionFromString(g_strScrollUpFunction);
            scrolldownfunc = UserFunction.FindFunctionFromString(g_strScrollDownFunction);
            middlebuttonfunc = UserFunction.FindFunctionFromString(g_strMiddleButtonFunction);
#endif
        }

        public const int SHIFT_KEY = 0x1;
        public const int CTRL_KEY = 0x2;
        public const int ALT_KEY = 0x4;
        public const int MODS_MASK = (CTRL_KEY | ALT_KEY | SHIFT_KEY);
        public const int KEY_DOWN = 0x8;
        public const int SECOND_KEY_SHIFT = 8;
        public const int SECOND_KEY_MOD_SHIFT = 16;


        public const int DIK_ESCAPE = 0x01;
        public const int DIK_1 = 0x02;
        public const int DIK_2 = 0x03;
        public const int DIK_3 = 0x04;
        public const int DIK_4 = 0x05;
        public const int DIK_5 = 0x06;
        public const int DIK_6 = 0x07;
        public const int DIK_7 = 0x08;
        public const int DIK_8 = 0x09;
        public const int DIK_9 = 0x0A;
        public const int DIK_0 = 0x0B;
        public const int DIK_SYSRQ = 0xB7;

    }
}
