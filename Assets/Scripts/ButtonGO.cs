using System.Collections;
using UnityEngine;

/// <summary>
/// Handles rendering and physics of the actual button game object.
/// </summary>
[RequireComponent( typeof ( Rigidbody ))]
[RequireComponent( typeof ( MeshRenderer ))]
[RequireComponent( typeof ( Collider ))]
public class ButtonGO : MonoBehaviour
{
    [Tooltip("The ButtonController parent.")]
    [SerializeField] private ButtonController _buttonController;

    [Tooltip( "A reference to this game object's material property block. This is used to change the color of the material without altering the material itself." )]
    private MaterialPropertyBlock _mpb;
    
    [Tooltip("The name of the shader property converted to an integer.")]
    private readonly int _colorID = Shader.PropertyToID("_Color" );

    [Tooltip( "The start position of the button." )]
    private Vector3 _startPosition;
    
    [Tooltip( "The max distance the button could be away from its start position." )]
    private float _maxDistance;
    
    private MeshRenderer _renderer => GetComponent<MeshRenderer>();
    private Rigidbody _rigidbody => GetComponent<Rigidbody>();
    
#region MonoBehaviour Methods
    private void Start()
    {
        /*
         * Initialize the start position and max distance.
         */
        
        _startPosition = transform.localPosition;
        _maxDistance = _startPosition.y - _buttonController.ThrowDistanceInMeters;
    }
    
    private void Update()
    {
        /*
         * Stop the button from going above its start position.
         * Stop the button from going below its stop distance.
         * While the button is at its lowest point, set the current state to pressed and invoke the OnButtonPressed UnityEvent.
         */

        if ( transform.localPosition.y > _startPosition.y )
        {
            transform.localPosition = _startPosition;
        }

        if ( transform.localPosition.y < _maxDistance)
        {
            transform.localPosition = new Vector3( transform.localPosition.x, _maxDistance, transform.localPosition.z );
            
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
        /*
         * Initialize the material property block if null.
         * Set the material property block's color
         *  Use that same material property block to set as the renderer's material property block.
         */
        
        if (_mpb == null)
        {
            _mpb = new MaterialPropertyBlock();
        }
        
        _mpb.SetColor( _colorID, color );
        _renderer.SetPropertyBlock( _mpb );
    }
#endregion

#region Event Listeners
    /// <summary>
    /// The coroutine nested in a void method. Made this way to allow the method to be added as a listener to events.
    /// </summary>
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
        /*
         * Set the rigidbody's isKinematic to true to stop the button from moving.
         * Hold for the time set in the physical button settings.
         * Set the rigidbody's isKinematic back to false to allow movement.
         * Set the current button state to true.
         * Invoke the unpressed UnityEvent.
         */
        
        _rigidbody.isKinematic = true;
        
        yield return new WaitForSeconds( _buttonController.FreezeTime );
        
        _rigidbody.isKinematic = false;
        _buttonController.CurrentButtonState = ButtonState.Unpressed;
        _buttonController.OnButtonUnpressed?.Invoke();
    }
#endregion Coroutines
}