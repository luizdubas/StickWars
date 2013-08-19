using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Noesis;

public class NoesisKeyCodes
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private static Dictionary<KeyCode, int> sNoesisKeyCode;
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private static void Init()
    {
        sNoesisKeyCode = new Dictionary<KeyCode, int>();
            
        sNoesisKeyCode.Add(KeyCode.Backspace, (int)Key.Back);
        sNoesisKeyCode.Add(KeyCode.Tab, (int)Key.Tab);
        sNoesisKeyCode.Add(KeyCode.Clear, (int)Key.Clear);
        sNoesisKeyCode.Add(KeyCode.Return, (int)Key.Return);
        sNoesisKeyCode.Add(KeyCode.Pause, (int)Key.Pause);
        sNoesisKeyCode.Add(KeyCode.Break, (int)Key.Pause);          // same as Pause
        
        sNoesisKeyCode.Add(KeyCode.RightShift, (int)Key.Shift);     // only one shift
        sNoesisKeyCode.Add(KeyCode.LeftShift, (int)Key.Shift);      // only one shift
        sNoesisKeyCode.Add(KeyCode.RightControl, (int)Key.Control); // only one control
        sNoesisKeyCode.Add(KeyCode.LeftControl, (int)Key.Control);  // only one control
        sNoesisKeyCode.Add(KeyCode.RightAlt, (int)Key.Alt);         // only one alt
        sNoesisKeyCode.Add(KeyCode.LeftAlt, (int)Key.Alt);          // only one alt
        
        sNoesisKeyCode.Add(KeyCode.Escape, (int)Key.Escape);
        
        sNoesisKeyCode.Add(KeyCode.Space, (int)Key.Space);
        sNoesisKeyCode.Add(KeyCode.PageUp, (int)Key.Prior);         // prior?
        sNoesisKeyCode.Add(KeyCode.PageDown, (int)Key.Next);        // next?
        sNoesisKeyCode.Add(KeyCode.End, (int)Key.End);
        sNoesisKeyCode.Add(KeyCode.Home, (int)Key.Home);
        sNoesisKeyCode.Add(KeyCode.LeftArrow, (int)Key.Left);
        sNoesisKeyCode.Add(KeyCode.UpArrow, (int)Key.Up);
        sNoesisKeyCode.Add(KeyCode.RightArrow, (int)Key.Right);
        sNoesisKeyCode.Add(KeyCode.DownArrow, (int)Key.Down);
        // SELECT KEY not defined
        sNoesisKeyCode.Add(KeyCode.Print, (int)Key.Print);
        // EXECUTE KEY not defined
        // PRINTSCR KEY not defined
        sNoesisKeyCode.Add(KeyCode.Insert, (int)Key.Insert);
        sNoesisKeyCode.Add(KeyCode.Delete, (int)Key.Delete);
        sNoesisKeyCode.Add(KeyCode.Help, (int)Key.Help);
        
        sNoesisKeyCode.Add(KeyCode.Alpha0, (int)Key.Alpha0);
        sNoesisKeyCode.Add(KeyCode.Alpha1, (int)Key.Alpha1);
        sNoesisKeyCode.Add(KeyCode.Alpha2, (int)Key.Alpha2);
        sNoesisKeyCode.Add(KeyCode.Alpha3, (int)Key.Alpha3);
        sNoesisKeyCode.Add(KeyCode.Alpha4, (int)Key.Alpha4);
        sNoesisKeyCode.Add(KeyCode.Alpha5, (int)Key.Alpha5);
        sNoesisKeyCode.Add(KeyCode.Alpha6, (int)Key.Alpha6);
        sNoesisKeyCode.Add(KeyCode.Alpha7, (int)Key.Alpha7);
        sNoesisKeyCode.Add(KeyCode.Alpha8, (int)Key.Alpha8);
        sNoesisKeyCode.Add(KeyCode.Alpha9, (int)Key.Alpha9);
        
        sNoesisKeyCode.Add(KeyCode.Keypad0, (int)Key.Pad0);
        sNoesisKeyCode.Add(KeyCode.Keypad1, (int)Key.Pad1);
        sNoesisKeyCode.Add(KeyCode.Keypad2, (int)Key.Pad2);
        sNoesisKeyCode.Add(KeyCode.Keypad3, (int)Key.Pad3);
        sNoesisKeyCode.Add(KeyCode.Keypad4, (int)Key.Pad4);
        sNoesisKeyCode.Add(KeyCode.Keypad5, (int)Key.Pad5);
        sNoesisKeyCode.Add(KeyCode.Keypad6, (int)Key.Pad6);
        sNoesisKeyCode.Add(KeyCode.Keypad7, (int)Key.Pad7);
        sNoesisKeyCode.Add(KeyCode.Keypad8, (int)Key.Pad8);
        sNoesisKeyCode.Add(KeyCode.Keypad9, (int)Key.Pad9);
        sNoesisKeyCode.Add(KeyCode.KeypadMultiply, (int)Key.Multiply);
        sNoesisKeyCode.Add(KeyCode.KeypadPlus, (int)Key.Add);
        // SEPARATOR KEY not defined
        sNoesisKeyCode.Add(KeyCode.KeypadMinus, (int)Key.Subtract);
        sNoesisKeyCode.Add(KeyCode.KeypadPeriod, (int)Key.Decimal);
        sNoesisKeyCode.Add(KeyCode.KeypadDivide, (int)Key.Divide);
        sNoesisKeyCode.Add(KeyCode.KeypadEnter, (int)Key.Return);      // same as Return
        sNoesisKeyCode.Add(KeyCode.KeypadEquals, 0);                    // no match
        
        sNoesisKeyCode.Add(KeyCode.A, (int)Key.A);
        sNoesisKeyCode.Add(KeyCode.B, (int)Key.B);
        sNoesisKeyCode.Add(KeyCode.C, (int)Key.C);
        sNoesisKeyCode.Add(KeyCode.D, (int)Key.D);
        sNoesisKeyCode.Add(KeyCode.E, (int)Key.E);
        sNoesisKeyCode.Add(KeyCode.F, (int)Key.F);
        sNoesisKeyCode.Add(KeyCode.G, (int)Key.G);
        sNoesisKeyCode.Add(KeyCode.H, (int)Key.H);
        sNoesisKeyCode.Add(KeyCode.I, (int)Key.I);
        sNoesisKeyCode.Add(KeyCode.J, (int)Key.J);
        sNoesisKeyCode.Add(KeyCode.K, (int)Key.K);
        sNoesisKeyCode.Add(KeyCode.L, (int)Key.L);
        sNoesisKeyCode.Add(KeyCode.M, (int)Key.M);
        sNoesisKeyCode.Add(KeyCode.N, (int)Key.N);
        sNoesisKeyCode.Add(KeyCode.O, (int)Key.O);
        sNoesisKeyCode.Add(KeyCode.P, (int)Key.P);
        sNoesisKeyCode.Add(KeyCode.Q, (int)Key.Q);
        sNoesisKeyCode.Add(KeyCode.R, (int)Key.R);
        sNoesisKeyCode.Add(KeyCode.S, (int)Key.S);
        sNoesisKeyCode.Add(KeyCode.T, (int)Key.T);
        sNoesisKeyCode.Add(KeyCode.U, (int)Key.U);
        sNoesisKeyCode.Add(KeyCode.V, (int)Key.V);
        sNoesisKeyCode.Add(KeyCode.W, (int)Key.W);
        sNoesisKeyCode.Add(KeyCode.X, (int)Key.X);
        sNoesisKeyCode.Add(KeyCode.Y, (int)Key.Y);
        sNoesisKeyCode.Add(KeyCode.Z, (int)Key.Z);
        
        sNoesisKeyCode.Add(KeyCode.F1, (int)Key.F1);
        sNoesisKeyCode.Add(KeyCode.F2, (int)Key.F2);
        sNoesisKeyCode.Add(KeyCode.F3, (int)Key.F3);
        sNoesisKeyCode.Add(KeyCode.F4, (int)Key.F4);
        sNoesisKeyCode.Add(KeyCode.F5, (int)Key.F5);
        sNoesisKeyCode.Add(KeyCode.F6, (int)Key.F6);
        sNoesisKeyCode.Add(KeyCode.F7, (int)Key.F7);
        sNoesisKeyCode.Add(KeyCode.F8, (int)Key.F8);
        sNoesisKeyCode.Add(KeyCode.F9, (int)Key.F9);
        sNoesisKeyCode.Add(KeyCode.F10, (int)Key.F10);
        sNoesisKeyCode.Add(KeyCode.F11, (int)Key.F11);
        sNoesisKeyCode.Add(KeyCode.F12, (int)Key.F12);
        sNoesisKeyCode.Add(KeyCode.F13, (int)Key.F13);
        sNoesisKeyCode.Add(KeyCode.F14, (int)Key.F14);
        sNoesisKeyCode.Add(KeyCode.F15, (int)Key.F15);
        
        sNoesisKeyCode.Add(KeyCode.Numlock, (int)Key.NumLock);
        sNoesisKeyCode.Add(KeyCode.ScrollLock, (int)Key.Scroll);
        
        sNoesisKeyCode.Add(KeyCode.Exclaim, 0);         // no match
        sNoesisKeyCode.Add(KeyCode.DoubleQuote, 0);     // no match
        sNoesisKeyCode.Add(KeyCode.Hash, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Dollar, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Ampersand, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.Quote, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.LeftParen, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.RightParen, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.Asterisk, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.Plus, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Comma, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Minus, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Period, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Slash, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Colon, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Semicolon, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.Less, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Equals, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Greater, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Question, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.At, 0);                // no match
        sNoesisKeyCode.Add(KeyCode.LeftBracket, 0);        // no match    
        sNoesisKeyCode.Add(KeyCode.RightBracket, 0);    // no match
        sNoesisKeyCode.Add(KeyCode.Caret, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Underscore, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.BackQuote, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.CapsLock, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.LeftApple, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.LeftWindows, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.RightApple, 0);        // no match
        sNoesisKeyCode.Add(KeyCode.RightWindows, 0);    // no match
        sNoesisKeyCode.Add(KeyCode.AltGr, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.SysReq, 0);            // no match
        sNoesisKeyCode.Add(KeyCode.Menu, 0);            // no match
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public static int Convert(KeyCode key)
    {
        if (sNoesisKeyCode == null)
        {
            Init();
        }
        
        int noesisKey;
        if (!sNoesisKeyCode.TryGetValue(key, out noesisKey))
        {
            noesisKey = 0;
        }
        
        return noesisKey;
    }
    
}
