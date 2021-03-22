using UnityEngine;
using UnityEngine.Events;
using Helpers;

/// <summary>
/// Holds references to its child objects and controls the overall behaviour of the button.
/// </summary>
public class ButtonController : MonoBehaviour
{
    /// <summary>
    /// An extensible collection of settings for each button state.
    /// Populate the collection with one Scriptable Object per button state.
    /// </summary>
    [SerializeField] private ButtonStateSettings[] _availableStateSettings;
    /// <summary>
    /// The physical settings to apply to the button.
    /// Includes diameter, throw distance, and freeze time.
    /// </summary>
    [SerializeField] private PhysicalButtonSettings _currentPhysicalSettings;
    
    /// <summary>
    /// The current set button state.
    /// Dictates what settings are applied to the button.
    /// </summary>
    [SerializeField] private ButtonState _currentButtonState;
    public ButtonState CurrentButtonState
    {
        get => _currentButtonState;
        set => _currentButtonState = value;
    }
    private ButtonStateSettings _currentStateSettings => GetSettingsByState( _currentButtonState );

    /// <summary>
    /// The component attached the button game object.
    /// </summary>
    [SerializeField] private ButtonGO _buttonGo;
    /// <summary>
    /// The empty parent transform of the button game object.
    /// </summary>
    [SerializeField] private Transform _buttonTransform;
    /// <summary>
    /// The empty parent transform of the trigger collider.
    /// </summary>
    [SerializeField] private Transform _triggerTransform;
    /// <summary>
    /// The empty parent transform of the base group.
    /// </summary>
    [SerializeField] private Transform _baseTransform;
    /// <summary>
    /// The audio source used to play audio clips.
    /// </summary>
    [SerializeField] private AudioSource _audioSource;
    
    
    [Header( "Events" )]
    [SerializeField] public UnityEvent OnButtonPressed;
    [SerializeField] public UnityEvent OnButtonUnpressed;
    
#region MonoBehaviour Methods
    private void Start()
    {
        _currentButtonState = ButtonState.Unpressed;
        
        ApplyStateSettings();
        ApplyPhysicalSettings();
    }

    private void OnValidate()
    {
        ApplyStateSettings();
        ApplyPhysicalSettings();
    }
#endregion MonoBehaviour Methods
    
#region Event Listeners
    /// <summary>
    /// Changes the color based on the settings of the current state.
    /// </summary>
    public void ChangeColor()
    {
        Color color = _currentStateSettings.Color;
        _buttonGo.ChangeColor( color );
    }
    
    /// <summary>
    /// Plays the sounds based on the settings of the current state.
    /// </summary>
    public void PlaySound()
    {
        if ( _audioSource.isPlaying )
        {
            _audioSource.Stop();
        }
        
        AudioClip clip = _currentStateSettings.Sound;
        _audioSource.clip = clip;
        _audioSource.Play();
    }
#endregion Event Listeners

#region Private Methods
    /// <summary>
    /// Returns the state setting in the collection whose state matches the argument.
    /// </summary>
    private ButtonStateSettings GetSettingsByState( ButtonState state )
    {
        if ( _availableStateSettings.Length > 0 )
        {
            foreach ( ButtonStateSettings settings in _availableStateSettings )
            {
                if ( settings.ButtonState == state )
                {
                    return settings;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// Applies the relevant state settings from the ScriptableObject.
    /// </summary>
    private void ApplyStateSettings()
    {
        ChangeColor();
    }
    
    /// <summary>
    /// Applies the physical settings from the ScriptableObject.
    /// </summary>
    private void ApplyPhysicalSettings()
    {
        PhysicalButtonSettings settings = _currentPhysicalSettings;
        
        // Diameter
        _buttonTransform.localScale = new Vector3( settings.Diameter, _buttonTransform.localScale.y, settings.Diameter );
        _baseTransform.localScale = new Vector3( settings.Diameter, _baseTransform.localScale.y, settings.Diameter );
        
        // Trigger Distance - TODO: Scrap this.
        float distance = GetThrowDistanceInMeters();
        float yOffset = Mathf.Abs( _buttonGo.transform.localPosition.y );    // Distance from the center of the button game object to the parent transform
        float triggerPos = _buttonGo.transform.localPosition.y - yOffset - distance;    // The y value of where the parent transform of the trigger game object should be.
        _triggerTransform.localPosition = new Vector3( _triggerTransform.localPosition.x, triggerPos, _triggerTransform.localPosition.z );
    }

    public float GetThrowDistanceInMeters()
    {
        return Convert.InchToMeter( _currentPhysicalSettings.ThrowDistance );
    }

    public float GetFreezeTime()
    {
        return _currentPhysicalSettings.FreezeTime;
    }
#endregion Private Methods
}