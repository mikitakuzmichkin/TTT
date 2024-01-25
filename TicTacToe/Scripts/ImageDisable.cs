using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe
{
    [RequireComponent(typeof(Image))]
    public class ImageDisable : MonoBehaviour
    {
        [SerializeField] private Color _disableColor = Color.gray;

        private Image _image;
        private Color _curColor;
        private bool _isEnable = true;

        public bool isEnable
        {
            set
            {
                if (value == _isEnable)
                {
                    return;
                }

                _isEnable = value;
                
                if (value)
                {
                    Image.color = _curColor;
                }
                else
                {
                    _curColor = Image.color;
                    Image.color = _disableColor;
                }
            }
        }

        private Image Image
        {
            get
            {
                if (_image == null)
                {
                    _image = GetComponent<Image>();
                    _curColor = _image.color;
                }

                return _image;
            }
        }
    }
}
