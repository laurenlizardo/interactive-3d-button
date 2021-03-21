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
    
    [Header( "Transforms" )]
    [SerializeField] private Transform _buttonTransform;
    [SerializeField] private Transform _triggerTransform;
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private Transform _bottomTransform;

    [Header( "Rendering" )]
    [SerializeField] private MeshRenderer _buttonRenderer;
    [SerializeField] private Color _unpressedColor;
    [SerializeField] private Color _pressedColor;
    private MaterialPropertyBlock _mpb;
    
    [Header("Colliders")]
    [SerializeField] private Collider _buttonTrigger;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _unpressedSound;
    [SerializeField] private AudioClip _pressedSound;
    
    [Header( "Events" )]
    [SerializeField] public UnityEvent OnButtonPressed;
    [SerializeField] public UnityEvent OnButtonUnpressed;
    
    [Header( "Tunables" )]
    [Tooltip( "Diameter of the button in inches. " )]
    [Range( 1, 5 )]
    [SerializeField]
    private float _diameter = 1;
    
    [Tooltip( "The height of the button in inches." )]
    [Range( 1, 5 )]
    [SerializeField]
    private float _buttonHeight = 1;
    
    [Tooltip( "The height of the base of the button in inches." )]
    [Range( 2, 6 )]
    [SerializeField]
    private float _baseHeight = 2;
    
    [Tooltip( "The amount of time in seconds the button should remain pressed." )]
    [Range( 1, 10 )]
    [SerializeField]
    private float _freezeTime = 3;
    
    [Tooltip( "The distance between the button and the trigger in inches." )]
    [Range( 1, 2 )]
    [SerializeField]
    private float _throwDistance = 1;
    
    private readonly int _colorID = Shader.PropertyToID("_Color" );
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
        ApplyNewDiameter();
        ApplyNewButtonHeight();
        ApplyNewBaseHeight();
        ApplyNewTriggerDistance();
    }

    private void OnTriggerEnter( Collider collider )
    {
        if ( collider.gameObject.tag == _buttonTriggerTag )
        {
            OnButtonPressed?.Invoke();
        }
    }

    private void OnTriggerExit( Collider collider )
    {
        if ( collider.gameObject.tag == _buttonTriggerTag )
        {
            OnButtonUnpressed?.Invoke();
        }
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
        
        _buttonRenderer.SetPropertyBlock( _mpb );
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
    
    

    private void ApplyNewDiameter()
    {
        _buttonTransform.localScale = new Vector3( _diameter, _buttonTransform.localScale.y, _diameter );
        _triggerTransform.localScale = new Vector3( _diameter, _triggerTransform.localScale.y, _diameter );
        _baseTransform.localScale = new Vector3( _diameter, _baseTransform.localScale.y, _diameter );
    }

    private void ApplyNewButtonHeight()
    {
        _buttonTransform.localScale = new Vector3( _buttonTransform.localScale.x, _buttonHeight, _buttonTransform.localScale.z );
    }

    private void ApplyNewBaseHeight()
    {
        _baseTransform.localScale = new Vector3( _baseTransform.localScale.x, _baseHeight, _baseTransform.localScale.z );
    }

    private IEnumerator FreezeButtonCo()
    {
        _buttonTransform.GetComponent<Collider>().enabled = false;
        _buttonTransform.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds( _freezeTime );
        _buttonTransform.GetComponent<Collider>().enabled = true;
        _buttonTransform.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void FreezeButton()
    {
        StartCoroutine( "FreezeButtonCo" );
    }

    public void ApplyNewTriggerDistance()
    {
        float convertedDist = _throwDistance * .0254f;
        float yPos = _buttonTransform.localPosition.y - convertedDist;
        _triggerTransform.localPosition = new Vector3( _triggerTransform.localPosition.x, yPos, _triggerTransform.localPosition.z );
    }
}