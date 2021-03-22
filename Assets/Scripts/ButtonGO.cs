using System.Collections;
using UnityEngine;

/// <summary>
/// Handles rendering and physics of the actual button game object.
/// </summary>
[RequireComponent( typeof ( Rigidbody ))]
[RequireComponent( typeof ( MeshRenderer ))]
[RequireComponent( typeof ( BoxCollider ))]
public class ButtonGO : MonoBehaviour
{
    [SerializeField] private ButtonController _buttonController;
    private MeshRenderer _renderer => GetComponent<MeshRenderer>();
    private Rigidbody _rigidbody => GetComponent<Rigidbody>();

    /// <summary>
    /// A reference to this game object's material property block.
    /// This is used to change the color of the material without altering the material itself.
    /// </summary>
    private MaterialPropertyBlock _mpb;
    /// <summary>
    /// The name of the shader property converted to an integer.
    /// </summary>
    private readonly int _colorID = Shader.PropertyToID("_Color" );

    /// <summary>
    /// The start position of the button.
    /// </summary>
    private Vector3 _startPosition;
    /// <summary>
    /// The max distance the button could be away from its start position.
    /// </summary>
    private float _stopDistance;
    
#region MonoBehaviour Methods
    private void Start()
    {
        _startPosition = transform.localPosition;
        _stopDistance = _startPosition.y - _buttonController.GetThrowDistanceInMeters();
    }
    
    private void Update()
    {
        if ( transform.localPosition.y > _startPosition.y )
        {
            transform.localPosition = _startPosition;
        }

        if ( transform.localPosition.y < _stopDistance)
        {
            transform.localPosition = new Vector3( transform.localPosition.x, _stopDistance, transform.localPosition.z );
            _buttonController.CurrentButtonState = ButtonState.Pressed;
            _buttonController.OnButtonPressed?.Invoke();
        }
    }
#endregion

#region Public Methods
    /// <summary>
    /// Applies a color to the material property block.
    /// </summary>
    public void ChangeColor( Color color )
    {
        // Initialize the material property block if null.
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
        StartCoroutine( nameof( FreezeButtonCo ) );
    }
#endregion Event Listeners
    
#region Coroutines
    /// <summary>
    /// Toggles the rigidbody's isKinematic for a given amount of time.
    /// </summary>
    private IEnumerator FreezeButtonCo()
    {
        _rigidbody.isKinematic = true;
        
        yield return new WaitForSeconds( _buttonController.GetFreezeTime() );
        
        _rigidbody.isKinematic = false;
        _buttonController.CurrentButtonState = ButtonState.Unpressed;
        _buttonController.OnButtonUnpressed?.Invoke();
    }
#endregion Coroutines
}