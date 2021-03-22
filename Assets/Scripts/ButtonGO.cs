using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles rendering and physics of the actual button.
/// </summary>
[RequireComponent( typeof ( Rigidbody ))]
[RequireComponent( typeof ( MeshRenderer ))]
[RequireComponent( typeof ( BoxCollider ))]
public class ButtonGO : MonoBehaviour
{
    [SerializeField] private ButtonController _buttonController;

    // Rendering
    private MaterialPropertyBlock _mpb;
    public MaterialPropertyBlock Mpb
    {
        get
        {
            if ( _mpb == null )
            {
                _mpb = new MaterialPropertyBlock();
            }
            return _mpb;
        }
    }
    private MeshRenderer _renderer => GetComponent<MeshRenderer>();
    
    // Physics
    private Rigidbody _rigidbody => GetComponent<Rigidbody>();
    
    // Read-only
    private readonly int _colorID = Shader.PropertyToID("_Color" );
    private readonly string _triggerTag = "Button Trigger";

    private Vector3 _startPosition;
    [SerializeField] private float _stopPosition;
    
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
        _stopPosition = _startPosition.y - _buttonController.GetThrowDistanceInMeters();
    }
    
    private void Update()
    {
        if ( transform.localPosition.y > _startPosition.y )
        {
            transform.localPosition = _startPosition;
        }

        if ( transform.localPosition.y < _stopPosition)
        {
            transform.localPosition = new Vector3( transform.localPosition.x, _stopPosition, transform.localPosition.z );
        }
        
        // Helps the button get unstuck
        // if ( _buttonController.CurrentButtonState == ButtonState.Unpressed )
        // {
        //     _rigidbody.isKinematic = false;
        // }
    }
    
    private void OnTriggerEnter( Collider collider )
    {
        if ( collider.gameObject.tag == _triggerTag )
        {
            _buttonController.CurrentButtonState = ButtonState.Pressed;
            _buttonController.OnButtonPressed?.Invoke();
        }
    }

    private void OnTriggerExit( Collider collider )
    {
        if ( collider.gameObject.tag == _triggerTag )
        {
            _buttonController.CurrentButtonState = ButtonState.Unpressed;
            _buttonController.OnButtonUnpressed?.Invoke();
        }
    }
#endregion

#region Public Methods
    public void ChangeColor( Color color )
    {
        if (_mpb == null)
        {
            _mpb = new MaterialPropertyBlock();
        }
        
        _mpb.SetColor( _colorID, color );
        _renderer.SetPropertyBlock( _mpb );
    }
#endregion

#region Event Listeners
public void FreezeButton()
    {
        StartCoroutine( "FreezeButtonCo" );
    }
#endregion Event Listeners
    
#region Coroutines
    private IEnumerator FreezeButtonCo()
    {
        _rigidbody.isKinematic = true;
        yield return new WaitForSeconds( _buttonController.GetFreezeTime() );
        _rigidbody.isKinematic = false;
    }
#endregion Coroutines
}