using System.Diagnostics;
using System.Media;
using System.Text.RegularExpressions;
using RegisterHotKey;
using KeyEventAPI;
using Microsoft.VisualBasic.ApplicationServices;
using System.Collections.Immutable;

namespace PasteTyper {
    public partial class Form1 : Form {
        private HotKeyHandler hkHandler = new HotKeyHandler();
        public Form1() {
            InitializeComponent();
        } //func
        void Form1_Load(object sender, EventArgs e) {
            textBox1.AcceptsTab = false;
            textBox1.Size = new Size(400, 300);
            textBox1.Text = "Put your desired text here, then go to any text editor field and press Ctrl+Shift+Alt+P\r\n" +
                "The text will be typed there automatically using virtual keyboard.\r\n" +
                "To stop typing, press Ctrl+Shift+Alt+O";
            BackColor = Color.DarkGray;
            //textBox1.Visible = false;

            hkHandler.addHotKey(Handle, 1, HotKeyHandler.controlModifier | HotKeyHandler.altModifier | HotKeyHandler.shiftModifier, 'P');
            hkHandler.addHotKey(Handle, 2, HotKeyHandler.controlModifier | HotKeyHandler.altModifier | HotKeyHandler.shiftModifier, 'O');
        } //func
        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            hkHandler.releaseHotKeys();
            stopPasting();
        } //func

        private void timer1_Tick(object sender, EventArgs e) {
            //Debug.WriteLine($"{crono.ElapsedMilliseconds % 1000}");
            if ( crono.ElapsedMilliseconds > currentInterval ) {
                //Debug.WriteLine($"trig {crono.ElapsedMilliseconds % 1000}");
                currentInterval = rnd.Next(minInterval, maxInterval);
                crono.Restart();
                nextEvent();
            } //if
        } //func

        protected override void WndProc(ref Message m) {
            if ((m.Msg == HotKeyHandler.WM_HOTKEY)
                && ((int)m.WParam == 1)) {
                startPasting();
            } //if
            if ((m.Msg == HotKeyHandler.WM_HOTKEY)
                && ((int)m.WParam == 2)) {
                stopPasting();
            } //if
            base.WndProc(ref m);
        } //func

        private SoundPlayer tickSound = new SoundPlayer(Properties.Resources.tick02_60);
        private int currentInterval = 0;
        private const int minInterval = 0;
        private const int maxInterval = 90;
        Stopwatch crono = new Stopwatch();
        private Random rnd = new Random();
        private bool isPasting = false;
        private List<KeyEventAPI.KeyEventHandler.KeyEvent> eventsToPaste = new List<KeyEventAPI.KeyEventHandler.KeyEvent>();
        private int pasteIndex = 0;

        private void startPasting() {
            if (isPasting) {
                return;
            } //gua
            Debug.WriteLine("pasting");
            isPasting = true;
            timer1.Stop();
            timer1.Interval = 5;
            crono.Reset();
            //Stopwatch.Frequency = 200;
            //Stopwatch.IsHighResolution = false;
            currentInterval = 400;
            eventsToPaste = KeyEventAPI.KeyEventHandler.KeyEvent.fromString(textBox1.Text);
            pasteIndex = 0;
            timer1.Start();
            crono.Start();
        } //func

        private void nextEvent() {
            if( pasteIndex < eventsToPaste.Count ) {
                //byte code = eventsToPaste[pasteIndex].keyCode;
                //int flags = eventsToPaste[pasteIndex].flags;
                //if (KeyEventAPI.KeyEventHandler.noShiftAndNoKeyUp(code, flags)) {
                    if (eventsToPaste[pasteIndex].noShiftAndNoKeyUp()) {
                        tickSound.Play();
                } //if
                //KeyEventAPI.KeyEventHandler.keybd_event(code, 0, flags, 0);
                eventsToPaste[pasteIndex].trigger();
                pasteIndex++;
            } //if
            else {
                SystemSounds.Beep.Play();
                stopPasting();
            } //else
        } //func

        private void stopPasting() {
            isPasting = false;
            timer1.Stop();
            crono.Reset();
        } //func
    } //class
} //ns