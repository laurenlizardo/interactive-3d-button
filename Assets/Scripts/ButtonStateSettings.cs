using UnityEngine;

/// <summary>
/// Settings for the assigned state.
/// </summary>
[CreateAssetMenu( fileName = "New Button State Settings", menuName = "Button Tunables/State Settings")]
public class ButtonStateSettings : ScriptableObject
{
    [Tooltip( "The assigned button state." )]
    public ButtonState ButtonState;
    [Tooltip( "The color to apply to the button based on its state." )]
    public Color Color;
    [Tooltip( "The sound to play when the button is in its assigned state." )]
    public AudioClip Sound;
}