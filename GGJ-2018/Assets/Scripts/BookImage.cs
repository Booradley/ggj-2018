using System;
using System.Collections;
using UnityEngine;
using VRTK;

public class BookImage : VRTK_InteractableObject
{
    [Header("Book Image Settings")]
    [SerializeField]
    private Transform _bookImageTransform;

    [SerializeField]
    private Collider _bookImageCollider;

    [SerializeField]
    private Transform _scaleContainer;

    [SerializeField]
    private float _scaleDistance;

    [SerializeField]
    private float _snapToHandSeconds;

    [SerializeField]
    private float _snapBackSeconds;

    [SerializeField]
    private GameObject _proxy;

    [SerializeField]
    private GameObject _actualObject;

    [SerializeField]
    private VRTK_InteractableObject[] _actualObjectInteractables;

    private Vector3 _initialLocalScale;
    private Vector3 _initialLocalPosition;
    private Quaternion _initialLocalRotation;
    private VRTK_ControllerReference _controllerReference;
    private Vector3 _grabOffset;
    private Vector3 _grabLocalPosition;
    private Quaternion _grabLocalRotation;
    private Vector3 _dropPosition;
    private Vector3 _dropRotation;
    private float _remainingSnapToHandSeconds = 0f;
    private float _remainingSnapBackSeconds = 0f;
    private bool _isSnappingToHand = false;
    private bool _isSnappedToHand = false;

    protected override void Awake()
    {
        base.Awake();
        
        _initialLocalScale = _scaleContainer.localScale;
        _initialLocalPosition = _proxy.transform.localPosition;
        _initialLocalRotation = _proxy.transform.localRotation;
        _proxy.SetActive(true);
        _actualObject.gameObject.SetActive(false);
    }

    public override void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)
    {
        base.Grabbed(currentGrabbingObject);
        
        _controllerReference = VRTK_ControllerReference.GetControllerReference(currentGrabbingObject.controllerEvents.gameObject);
        _grabOffset = _controllerReference.actual.transform.position - _bookImageTransform.position;
        _grabLocalPosition = this.transform.localPosition;
        _grabLocalRotation = this.transform.localRotation;
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
    {
        base.Ungrabbed(previousGrabbingObject);
        
        _controllerReference = null;
        _dropPosition = this.transform.position;
        _dropRotation = this.transform.rotation.eulerAngles;

        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
    }

    protected override void Update()
    {
        base.Update();

        if (_isSnappedToHand)
            return;
        
        if (IsGrabbed())
        {
            if (_isSnappingToHand)
            {
                if (_remainingSnapToHandSeconds > 0f)
                {
                    _remainingSnapToHandSeconds -= Time.deltaTime;

                    float ratio = _remainingSnapToHandSeconds / _snapToHandSeconds;
                    Vector3 currentPosition = Vector3.Lerp(_controllerReference.actual.transform.position, _bookImageTransform.position, ratio);
                    Quaternion currentRotation = Quaternion.Lerp(_controllerReference.actual.transform.rotation, _bookImageTransform.rotation, ratio);

                    this.transform.position = currentPosition;
                    this.transform.rotation = currentRotation;
                }
                else
                {
                    // Swap
                    StartCoroutine(SwapToObjectSequence());
                }
            }
            else
            {
                // Scale to the controllers position
                float distance = Vector3.Distance(_bookImageTransform.position, _controllerReference.actual.transform.position - _grabOffset);
                if (distance >= _scaleDistance)
                {
                    _isSnappingToHand = true;
                    _remainingSnapToHandSeconds = _snapToHandSeconds;
                }
                else
                {
                    float scaleRatio = Mathf.Clamp01(distance / _scaleDistance);

                    this.transform.position = _bookImageTransform.position;
                    this.transform.rotation = _bookImageTransform.rotation;

                    Vector3 localScale = Vector3.one;
                    localScale.y = Mathf.Max(scaleRatio, 0.01f);
                    _scaleContainer.localScale = localScale;
                }
            }
        }
        else
        {
            // Snap back into the page
            if (_remainingSnapBackSeconds > 0f)
            {
                _remainingSnapBackSeconds -= Time.deltaTime;

                float ratio = _remainingSnapBackSeconds / _snapBackSeconds;
                Vector3 currentPosition = Vector3.Lerp(_bookImageTransform.position, _dropPosition, ratio);
                Vector3 currentRotation = Vector3.Lerp(_bookImageTransform.rotation.eulerAngles, _dropRotation, ratio);

                this.transform.position = currentPosition;
                this.transform.rotation = Quaternion.Euler(currentRotation);
            }
            else
            {
                _remainingSnapBackSeconds = 0f;

                this.transform.position = _bookImageTransform.position;
                this.transform.rotation = _bookImageTransform.rotation;
                _scaleContainer.localScale = _initialLocalScale;
            }
        }
    }

    private IEnumerator SwapToObjectSequence()
    {
        _isSnappingToHand = false;
        _isSnappedToHand = true;

        _proxy.SetActive(false);
        _actualObject.gameObject.SetActive(true);

        for (int i = 0; i < _actualObjectInteractables.Length; i++)
        {
            _actualObjectInteractables[i].InteractableObjectUngrabbed += HandleObjectDropped;
        }

        _bookImageCollider.enabled = false;

        _actualObject.transform.position = _proxy.transform.position;
        _actualObject.transform.rotation = _proxy.transform.rotation;
        _actualObject.transform.SetParent(null);

        VRTK_InteractTouch touch = _controllerReference.actual.GetComponentInChildren<VRTK_InteractTouch>();
        VRTK_InteractGrab grab = _controllerReference.actual.GetComponentInChildren<VRTK_InteractGrab>();
        
        if (_actualObjectInteractables.Length > 0)
        {
            touch.ForceTouch(_actualObjectInteractables[0].gameObject);
        }
        
        grab.ForceRelease();

        yield return null;

        grab.AttemptGrab();
    }

    private void HandleObjectDropped(object sender, InteractableObjectEventArgs e)
    {
        float distance = Vector3.Distance(_bookImageTransform.position, _actualObject.transform.position);
        if (distance < _scaleDistance)
        {
            for (int i = 0; i < _actualObjectInteractables.Length; i++)
            {
                _actualObjectInteractables[i].InteractableObjectUngrabbed -= HandleObjectDropped;
            }

            _proxy.SetActive(true);
            _proxy.transform.localPosition = _initialLocalPosition;
            _proxy.transform.localRotation = _initialLocalRotation;
            _scaleContainer.localScale = _initialLocalScale;

            _actualObject.transform.SetParent(transform.parent);
            _actualObject.gameObject.SetActive(false);

            _bookImageCollider.enabled = true;

            _isSnappedToHand = false;
            _isSnappingToHand = false;
        }
    }
}