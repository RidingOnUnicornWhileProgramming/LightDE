using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
namespace LightDE.Core
{
    public class KeyManager
    {
        public static void AppCommand(AppComandCode commandCode)
        {
            int CommandID = (int)commandCode << 16;
            InteropHelper.SendMessageW(Process.GetCurrentProcess().MainWindowHandle, 0x319, Process.GetCurrentProcess().MainWindowHandle, (IntPtr)CommandID);
        }
        public enum AppComandCode : uint
        {
            BASS_BOOST = 20,
            BASS_DOWN = 19,
            BASS_UP = 21,
            BROWSER_BACKWARD = 1,
            BROWSER_FAVORITES = 6,
            BROWSER_FORWARD = 2,
            BROWSER_HOME = 7,
            BROWSER_REFRESH = 3,
            BROWSER_SEARCH = 5,
            BROWSER_STOP = 4,
            LAUNCH_APP1 = 17,
            LAUNCH_APP2 = 18,
            LAUNCH_MAIL = 15,
            LAUNCH_MEDIA_SELECT = 16,
            MEDIA_NEXTTRACK = 11,
            MEDIA_PLAY_PAUSE = 14,
            MEDIA_PREVIOUSTRACK = 12,
            MEDIA_STOP = 13,
            TREBLE_DOWN = 22,
            TREBLE_UP = 23,
            VOLUME_DOWN = 9,
            VOLUME_MUTE = 8,
            VOLUME_UP = 10,
            MICROPHONE_VOLUME_MUTE = 24,
            MICROPHONE_VOLUME_DOWN = 25,
            MICROPHONE_VOLUME_UP = 26,
            CLOSE = 31,
            COPY = 36,
            CORRECTION_LIST = 45,
            CUT = 37,
            DICTATE_OR_COMMAND_CONTROL_TOGGLE = 43,
            FIND = 28,
            FORWARD_MAIL = 40,
            HELP = 27,
            MEDIA_CHANNEL_DOWN = 52,
            MEDIA_CHANNEL_UP = 51,
            MEDIA_FASTFORWARD = 49,
            MEDIA_PAUSE = 47,
            MEDIA_PLAY = 46,
            MEDIA_RECORD = 48,
            MEDIA_REWIND = 50,
            MIC_ON_OFF_TOGGLE = 44,
            NEW = 29,
            OPEN = 30,
            PASTE = 38,
            PRINT = 33,
            REDO = 35,
            REPLY_TO_MAIL = 39,
            SAVE = 32,
            SEND_MAIL = 41,
            SPELL_CHECK = 42,
            UNDO = 34,
            DELETE = 53,
            DWM_FLIP3D = 54
        }
    }
        /// <summary>
        /// The KeyboardListener is a static class that allows registering a number
        /// of event handlers that you want to get called in case some keyboard key is pressed 
        /// or released. The nice thing is that this KeyboardListener is also active in case
        /// the parent application is running in the back.
        /// Taken from https://www.codeproject.com/Articles/9351/Background-applications-listening-for-keyboard-act
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "SkipVerification")]
        public class KeyboardListener
        {
            #region Private declarations

            /// <summary>
            /// The Window that intercepts Keyboard messages
            /// </summary>
            private static ListeningWindow s_Listener;

            #endregion

            #region Private methods
            /// <summary>
            /// The function that will handle all keyboard activity signaled by the ListeningWindow.
            /// In this context handling means calling all registered subscribers for every key pressed / released.
            /// </summary>
            /// <remarks>
            /// Inside this method the events could also be fired by calling
            /// s_KeyEventHandler(null,new KeyEventArgs(key,msg)) However, in case one of the registered
            /// subscribers throws an exception, execution of the non-executed subscribers is cancelled.
            /// </remarks>
            /// <param name="key"></param>
            /// <param name="msg"></param>
            private static void KeyHandler(ushort key, uint msg)
            {
                if (s_KeyEventHandler != null)
                {
                    Delegate[] delegates = s_KeyEventHandler.GetInvocationList();

                    foreach (Delegate del in delegates)
                    {
                        EventHandler sink = (EventHandler)del;

                        try
                        {
                            // This is a static class, therefore null is passed as the object reference
                            sink(null, new UniversalKeyEventArgs(key, msg));
                        }

                        // You can add some meaningful code to this catch block.
                        catch { };
                    }
                }
            }
            #endregion

            #region Public declarations

            /// <summary>
            /// An instance of this class is passed when Keyboard events are fired by the KeyboardListener.
            /// </summary>
            public class UniversalKeyEventArgs : KeyEventArgs
            {
                public readonly uint m_Msg;
                public readonly ushort m_Key;

                public UniversalKeyEventArgs(ushort aKey, uint aMsg) : base((Keys)aKey)
                {
                    m_Msg = aMsg;
                    m_Key = aKey;
                }
            }

            /// <summary>
            /// For every application thread that is interested in keyboard events
            /// an EventHandler can be added to this variable
            /// </summary>
            public static event EventHandler s_KeyEventHandler;

            #endregion

            #region Public methods

            static KeyboardListener()
            {
                ListeningWindow.KeyDelegate aKeyDelegate = new ListeningWindow.KeyDelegate(KeyHandler);
                s_Listener = new ListeningWindow(aKeyDelegate);
            }

            #endregion

            #region Definition ListeningWindow class
            /// <summary>
            /// A ListeningWindow object is a Window that intercepts Keyboard events.
            /// </summary>
            private class ListeningWindow : NativeWindow
            {
                #region Declarations
                public delegate void KeyDelegate(ushort key, uint msg);

                private const int
                    WS_CLIPCHILDREN = 0x02000000,
                    WM_INPUT = 0x00FF,
                    RIDEV_INPUTSINK = 0x00000100,
                    RID_INPUT = 0x10000003,
                    RIM_TYPEKEYBOARD = 1;

                private uint m_PrevMessage = 0;
                private ushort m_PrevControlKey = 0;
                private KeyDelegate m_KeyHandler = null;
                #endregion

                #region Unsafe types
                internal unsafe struct RAWINPUTDEV
                {
                    public ushort usUsagePage;
                    public ushort usUsage;
                    public uint dwFlags;
                    public void* hwndTarget;
                };

                internal unsafe struct RAWINPUTHEADER
                {
                    public uint dwType;
                    public uint dwSize;
                    public void* hDevice;
                    public void* wParam;
                };

                internal unsafe struct RAWINPUTHKEYBOARD
                {
                    public RAWINPUTHEADER header;
                    public ushort MakeCode;
                    public ushort Flags;
                    public ushort Reserved;
                    public ushort VKey;
                    public uint Message;
                    public uint ExtraInformation;

                };
                #endregion

                public ListeningWindow(KeyDelegate keyHandlerFunction)
                {
                    m_KeyHandler = keyHandlerFunction;

                    CreateParams cp = new CreateParams();

                    // Fill in the CreateParams details.
                    cp.Caption = "Hidden window";
                    cp.ClassName = null;
                    cp.X = 0x7FFFFFFF;
                    cp.Y = 0x7FFFFFFF;
                    cp.Height = 0;
                    cp.Width = 0;
                    //cp.Parent = parent.Handle;
                    cp.Style = WS_CLIPCHILDREN;

                    // Create the actual invisible window
                    this.CreateHandle(cp);

                    // Register for Keyboard notification
                    unsafe
                    {
                        try
                        {
                            RAWINPUTDEV myRawDevice = new RAWINPUTDEV();
                            myRawDevice.usUsagePage = 0x01;
                            myRawDevice.usUsage = 0x06;
                            myRawDevice.dwFlags = RIDEV_INPUTSINK;
                            myRawDevice.hwndTarget = this.Handle.ToPointer();

                            if (RegisterRawInputDevices(&myRawDevice, 1, (uint)sizeof(RAWINPUTDEV)) == false)
                            {
                                int err = Marshal.GetLastWin32Error();
                                throw new Win32Exception(err, "ListeningWindow::RegisterRawInputDevices");
                            }
                        }

                        catch { throw; }
                    }
                }


                #region Private methods
                protected override void WndProc(ref Message m)
                {
                    switch (m.Msg)
                    {
                        case WM_INPUT:
                            {
                                try
                                {
                                    unsafe
                                    {
                                        uint dwSize, receivedBytes;
                                        uint sizeof_RAWINPUTHEADER = (uint)(sizeof(RAWINPUTHEADER));

                                        // Find out the size of the buffer we have to provide
                                        int res = GetRawInputData(m.LParam.ToPointer(), RID_INPUT, null, &dwSize, sizeof_RAWINPUTHEADER);

                                        if (res == 0)
                                        {
                                            // Allocate a buffer and ...
                                            byte* lpb = stackalloc byte[(int)dwSize];

                                            // ... get the data
                                            receivedBytes = (uint)GetRawInputData((RAWINPUTHKEYBOARD*)(m.LParam.ToPointer()), RID_INPUT, lpb, &dwSize, sizeof_RAWINPUTHEADER);
                                            if (receivedBytes == dwSize)
                                            {
                                                RAWINPUTHKEYBOARD* keybData = (RAWINPUTHKEYBOARD*)lpb;

                                                // Finally, analyze the data
                                                if (keybData->header.dwType == RIM_TYPEKEYBOARD)
                                                {
                                                    if ((m_PrevControlKey != keybData->VKey) || (m_PrevMessage != keybData->Message))
                                                    {
                                                        m_PrevControlKey = keybData->VKey;
                                                        m_PrevMessage = keybData->Message;

                                                        // Call the delegate in case data satisfies
                                                        m_KeyHandler(keybData->VKey, keybData->Message);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                string errMsg = string.Format("WndProc::GetRawInputData (2) received {0} bytes while expected {1} bytes", receivedBytes, dwSize);
                                                throw new Exception(errMsg);
                                            }
                                        }
                                        else
                                        {
                                            string errMsg = string.Format("WndProc::GetRawInputData (1) returned non zero value ({0})", res);
                                            throw new Exception(errMsg);
                                        }
                                    }
                                }

                                catch { throw; }
                            }
                            break;
                    }

                    // In case you forget this you will run into problems
                    base.WndProc(ref m);
                }

                #endregion

                #region Private external methods

                // In case you want to have a comprehensive overview of calling conventions follow the next link:
                // http://www.codeproject.com/cpp/calling_conventions_demystified.asp

                [DllImport("User32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                internal static extern unsafe bool RegisterRawInputDevices(RAWINPUTDEV* rawInputDevices, uint numDevices, uint size);

                [DllImport("User32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
                [return: MarshalAs(UnmanagedType.I4)]
                internal static extern unsafe int GetRawInputData(void* hRawInput,
                    uint uiCommand,
                    byte* pData,
                    uint* pcbSize,
                    uint cbSizeHeader
                    );

                #endregion
            }
            #endregion
        }
    }

