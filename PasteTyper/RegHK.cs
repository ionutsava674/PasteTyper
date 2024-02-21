using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RegisterHotKey {
    public class HotKeyHandler {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void SetWindowText(int hWnd, String text); 
        [DllImport("user32.dll")]
        public static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        public static extern int UnregisterHotKey(IntPtr hwnd, int id);

        public const int altModifier = 0x0001;
        public const int controlModifier = 0x0002;
        public const int shiftModifier = 0x004;
        public const int winModifier = 0x008;
        public const int noRepeatModifier = 0x400;
        public const int WM_HOTKEY = 0x312;

        private HashSet<HotKeyPair> hotKeyPairs = new HashSet<HotKeyPair>();

        public bool addHotKey(IntPtr hwnd, int id, int fsModifiers, int vk) {
            HotKeyPair h =new HotKeyPair(hwnd, id);
            if ( hotKeyPairs.Contains(h) ) {
                Debug.WriteLine("already");
                return false;
            } //if
            if ( RegisterHotKey(hwnd, id, fsModifiers, vk) != 0 ) {
                Debug.WriteLine("registered");
                hotKeyPairs.Add(h);
                return true;
            } //if
            return false;
        } //func
        public void releaseHotKeys() {
            foreach (HotKeyPair h in hotKeyPairs) {
                UnregisterHotKey(h.handle, h.id);
                Debug.WriteLine("unregistered");
            } //fe            
            hotKeyPairs.Clear();
            Debug.WriteLine($"cleared {hotKeyPairs.Count}");
        } //func
        struct HotKeyPair {
            public IntPtr handle;
            public int id;
            public HotKeyPair(IntPtr handle, int id) {
                this.handle = handle;
                this.id = id;
            } //init
        } //struct

        //private HotKeyHandler() {
        //} //private init
    } //class
} //ns
