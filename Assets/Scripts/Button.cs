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
    
    // [Tooltip( "The height of the button in inches." )]
    // [Range( 1, 5 )]
    // [SerializeField]
    // private float _buttonHeight = 1;
    
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
    
    //===========

    private Vector3 _startPosition;
    private Vector3 _triggerPosition;
    
#region MonoBehaviour Methods
    private void Awake()
    {
        if ( _mpb == null )
        {
            _mpb = new MaterialPropertyBlock();
        }
    }

    private void Start()
    {
        _startPosition = transform.localPosition;
        _triggerTransform.localPosition = new Vector3( _startPosition.x, ConvertToInches( -_throwDistance ), _startPosition.z );
    }

    private void FixedUpdate()
    {
        if ( transform.localPosition.y > _startPosition.y )
        {
            transform.localPosition = _startPosition;
        }

        if ( transform.localPosition.y < _triggerTransform.localPosition.y )
        {
            transform.localPosition = _triggerTransform.localPosition;
        }
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
    
    private void OnValidate()
    {
        ChangeColor();
        ApplyNewDiameter();
        ApplyNewBaseHeight();
        ApplyNewTriggerDistance();
    }
#endregion MonoBehaviour Methods

#region Event Listeners
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
    
    public void FreezeButton()
    {
        StartCoroutine( "FreezeButtonCo" );
    }
#endregion Event Listeners
    
#region Private Methods
    private void ApplyNewDiameter()
    {
        _buttonTransform.localScale = new Vector3( _diameter, _buttonTransform.localScale.y, _diameter );
        _baseTransform.localScale = new Vector3( _diameter, _baseTransform.localScale.y, _diameter );
    }

    private void ApplyNewBaseHeight()
    {
        _baseTransform.localScale = new Vector3( _baseTransform.localScale.x, _baseHeight, _baseTransform.localScale.z );
    }
    
    
    private void ApplyNewTriggerDistance()
    {
        float convertedDist = _throwDistance * .0254f;
        float yPos = _buttonTransform.localPosition.y - convertedDist;
        _triggerTransform.localPosition = new Vector3( _triggerTransform.localPosition.x, yPos, _triggerTransform.localPosition.z );
    }

    private float ConvertToInches( float meter )
    {
        float inchToMeters = .0254f;
        return meter * inchToMeters;
    }
#endregion Private Methods
    
#region Coroutines
    private IEnumerator FreezeButtonCo()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds( _freezeTime );
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Collider>().enabled = true;
    }
#endregion Coroutines
}