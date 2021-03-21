using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    public enum ButtonState
    {
        Unpressed,
        Pressed
    }
    [SerializeField] private ButtonState _currentButtonState;

    //==================================================================
    
    private MaterialPropertyBlock _mpb;
    private int _colorID = Shader.PropertyToID("_Color" );
    [SerializeField] private Color _unpressedColor;
    [SerializeField] private Color _pressedColor;
    private MeshRenderer _meshRenderer => GetComponent<MeshRenderer>();

    //==================================================================

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _unpressedSound;
    [SerializeField] private AudioClip _pressedSound;
    
    //==================================================================
    
    [SerializeField] private Collider _bottomTrigger;
    
    //==================================================================
    
    [SerializeField] public UnityEvent OnButtonPressed;
    [SerializeField] public UnityEvent OnButtonUnpressed;

    private readonly string _buttonTriggerTag = "Button Trigger";

    private void Awake()
    {
        if ( _mpb == null )
        {
            _mpb = new MaterialPropertyBlock();
        }
    }
    private void OnValidate()
    {
        ChangeColor();
    }

    public void ChangeToUnpressed()
    {
        _currentButtonState = ButtonState.Unpressed;
    }

    public void ChangeToPressed()
    {
        _currentButtonState = ButtonState.Pressed;
    }
    
    public void ChangeColor()
    {
        if ( _mpb == null )
        {
            _mpb = new MaterialPropertyBlock();
        }
        
        if ( _currentButtonState == ButtonState.Pressed )
        {
            _mpb.SetColor( _colorID, _pressedColor );
        }
        else
        {
            
            _mpb.SetColor( _colorID, _unpressedColor );
        }
        
        _meshRenderer.SetPropertyBlock( _mpb );
    }

    public void PlaySound()
    {
        if ( _currentButtonState == ButtonState.Pressed )
        {
            _audioSource.clip = _pressedSound;
            _audioSource.PlayOneShot( _pressedSound );
        }
        else
        {
            _audioSource.clip = _unpressedSound;
            _audioSource.PlayOneShot( _unpressedSound );
        }
    }
    
    private void OnTriggerEnter( Collider collider )
    {
        if ( collider.gameObject.name == _buttonTriggerTag )
        {
            OnButtonPressed?.Invoke();
        }
    }

    private void OnTriggerExit( Collider collider )
    {
        if ( collider.gameObject.name == _buttonTriggerTag )
        {
            OnButtonUnpressed?.Invoke();
        }
    }
}