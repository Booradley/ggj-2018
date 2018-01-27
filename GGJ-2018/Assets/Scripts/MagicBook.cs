using UnityEngine;
using VRTK;

public class MagicBook : MonoBehaviour
{
    [SerializeField]
    private Transform _bookImageTransform;

    [SerializeField]
    private BookImage _bookImage;

    private void FixedUpdate()
    {
        if (!_bookImage.IsGrabbed())
        {
            _bookImage.transform.position = _bookImageTransform.position;
            _bookImage.transform.rotation = _bookImageTransform.rotation;
        }
    }
}