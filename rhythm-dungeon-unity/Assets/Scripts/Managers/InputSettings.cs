using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSettings
{
	public static readonly KeyCode PauseKey = KeyCode.Escape;

    public static readonly KeyCode[] MusicKeys = {
        KeyCode.UpArrow,
        KeyCode.LeftArrow,
        KeyCode.DownArrow,
        KeyCode.RightArrow,
        KeyCode.Q,
        KeyCode.E
        };
    public static readonly string[] NameOfMusicKeys = {
        "Up",
        "Left",
        "Down",
        "Right",
        "Q",
        "E"
        };
}