using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Helpers;

public class ButtonController : MonoBehaviour
{
    public ButtonStateSettings[] StateSettings;
    
    public static ButtonState CurrentButtonState;
    private ButtonStateSettings _currentStateSettings
    {
        get
        {
            return GetSettingsByState( CurrentButtonState );
        }
    }
    
    [SerializeField] private PhysicalButtonSettings _currentPhysicalSettings;

    [Header( "Events" )]
    [SerializeField] public UnityEvent OnButtonPressed;
    [SerializeField] public UnityEvent OnButtonUnpressed;

    [SerializeField] private ButtonGO _buttonGo;
    [SerializeField] private AudioSource _audioSource;

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
    
#region MonoBehaviour Methods
    private void OnValidate()
    {
        // ApplyCurrentStateSettings();
        // ApplyPhysicalButtonSettings();
    }
#endregion MonoBehaviour Methods
    
#region Event Listeners
public void ChangeColor()
    {
        Color color = GetSettingsByState( CurrentButtonState ).Color;
        _buttonGo.ChangeColor( color );
    }
    
    public void PlaySound()
    {
        AudioClip clip = GetSettingsByState( CurrentButtonState ).Sound;
        _audioSource.clip = clip;
        _audioSource.PlayOneShot( clip );
    }
#endregion Event Listeners

#region Private Methods
    private ButtonStateSettings GetSettingsByState( ButtonState state )
    {
        if ( StateSettings.Length > 0 )
        {
            foreach ( ButtonStateSettings settings in StateSettings )
            {
                if ( settings.ButtonState == state )
                {
                    return settings;
                }
            }
        }
        return null;
    }

    private void ApplyStateSettings()
    {
        ButtonStateSettings settings = _currentStateSettings;
        
        // State
        CurrentButtonState = settings.ButtonState;

        // Color
        Color color = settings.Color;
        _buttonGo.ChangeColor( color );

        // Sound
        AudioClip clip = settings.Sound;
        _audioSource.clip = clip;
        _audioSource.PlayOneShot( clip );
    }
    
    private void ApplyPhysicalSettings()
    {
        PhysicalButtonSettings settings = _currentPhysicalSettings;
        
        // Diameter
        _buttonTransform.localScale = new Vector3( settings.Diameter, _buttonTransform.localScale.y, settings.Diameter );
        _baseTransform.localScale = new Vector3( settings.Diameter, _baseTransform.localScale.y, settings.Diameter );
        
        // Trigger Distance
        float distance = GetThrowDistanceInMeters();
        float yPos = _buttonTransform.localPosition.y - distance;
        _triggerTransform.localPosition = new Vector3( _triggerTransform.localPosition.x, yPos, _triggerTransform.localPosition.z );
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