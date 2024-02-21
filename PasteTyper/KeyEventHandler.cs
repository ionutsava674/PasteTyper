using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KeyEventAPI {
    internal class KeyEventHandler {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int leftShiftKey = 0xA0;
        public const int extendedKeyFlag = 0x0001;
        public const int keyUpFlag = 0x0002;

        public static bool noShiftAndNoKeyUp(byte code, int flags) {
            return (code != leftShiftKey)
                && ((flags & keyUpFlag) == 0);
        } //func


        public struct KeyEvent {
            public byte keyCode;
            public int flags;

            public static KeyEvent shiftDownEvent = new KeyEvent(leftShiftKey, extendedKeyFlag);
            public static KeyEvent shiftUpEvent = new KeyEvent(leftShiftKey, extendedKeyFlag | keyUpFlag);

            public KeyEvent(byte code, int flags) {
                this.keyCode = code;
                this.flags = flags;
            }//init 

            private static char[] keyChars = new char[] { '`', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=', 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[', ']', '\\', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';', '\'', 'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', '{', '}', '|', 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', ':', '"', 'Z', 'X', 'C', 'V', 'B', 'N', 'M', '<', '>', '?', '	', ' ', '\n' };
            private static string keyCharsString = new string(keyChars);
            private static int[] keyCodes = new int[] { 192, 49, 50, 51, 52, 53, 54, 55, 56, 57, 48, 189, 187, 81, 87, 69, 82, 84, 89, 85, 73, 79, 80, 219, 221, 220, 65, 83, 68, 70, 71, 72, 74, 75, 76, 186, 222, 90, 88, 67, 86, 66, 78, 77, 188, 190, 191, 192, 49, 50, 51, 52, 53, 54, 55, 56, 57, 48, 189, 187, 81, 87, 69, 82, 84, 89, 85, 73, 79, 80, 219, 221, 220, 65, 83, 68, 70, 71, 72, 74, 75, 76, 186, 222, 90, 88, 67, 86, 66, 78, 77, 188, 190, 191, 9, 32, 13 };
            private static bool[] keyShifts = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false };

            public void trigger() {
                keybd_event(keyCode, 0, flags, 0);
            } //func
            public bool noShiftAndNoKeyUp() {
                return KeyEventHandler.noShiftAndNoKeyUp(keyCode, flags);
            } //func
                public static List<KeyEvent> fromString(string input) {
                List<KeyEvent> result = new List<KeyEvent>();
                if ((keyCodes.Length != keyCharsString.Length)
                    || (keyCodes.Length != keyShifts.Length)) {
                    return result;
                } //gua
                foreach (char c in input) {
                    int i = keyCharsString.IndexOf(c);
                    if (i < 0) {
                        continue;
                    } //gua
                    if (keyShifts[i]) {
                        result.Add(shiftDownEvent);
                    } //if
                    result.Add(new KeyEvent((byte)keyCodes[i], 0));
                    result.Add(new KeyEvent((byte)keyCodes[i], keyUpFlag));
                    if (keyShifts[i]) {
                        result.Add(shiftUpEvent);
                    } //if
                } //fe
                Debug.WriteLine($"generated {result.Count}");
                return result;
            }//func
        }//struct 
    } //class
} //ns
