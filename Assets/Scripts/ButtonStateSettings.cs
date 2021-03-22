using UnityEngine;

/// <summary>
/// Settings for the assigned state.
/// </summary>
[CreateAssetMenu( fileName = "New Button State Settings", menuName = "Button Tunables/State Settings")]
public class ButtonStateSettings : ScriptableObject
{
    public ButtonState ButtonState;
    public Color Color;
    public AudioClip Sound;
}