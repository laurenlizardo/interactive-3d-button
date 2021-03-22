using UnityEngine;

/// <summary>
/// Settings based on the physical button.
/// </summary>
[CreateAssetMenu( fileName = "New Physical Button Settings", menuName = "Button Tunables/Physical Settings")]
public class PhysicalButtonSettings : ScriptableObject
{
    [Tooltip( "Diameter of the button in inches. " )]
    [Range( 1, 5 )]
    public float Diameter = 1;
    
    [Tooltip( "The amount of time in seconds the button should remain pressed." )]
    [Range( 1, 10 )]
    public float FreezeTime = 3;
    
    [Tooltip( "The distance in inches at which the button should be activated." )]
    [Range( 1, 2 )]
    public float ThrowDistance = 1;
}