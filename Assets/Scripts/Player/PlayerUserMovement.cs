﻿using Common;
using UnityEngine;

namespace Player {
    public class PlayerUserMovement : MonoBehaviour {
        private PlayerMovement _movement;
        private bool _isJump;
        private float _moveDir = 1f;


        // Start is called before the first frame update
        void Start() {
            _movement = GetComponent<PlayerMovement>();
        }

        // Update is called once per frame
        void Update() {
            if (!_isJump)
                _isJump = Input.GetButtonDown("Jump") ;
        }

        private void FixedUpdate() {
            float x = Input.GetAxis("Horizontal");
            _movement.Move(x, _isJump, IsChangeDirection(x));
            _isJump = false;
        }


        private bool IsChangeDirection(float xShift) {
            if (xShift.FloatEquals(0f)) return false;
            var newDir = Mathf.Sign(xShift);
            if (newDir.FloatEquals(_moveDir)) return false;
            _moveDir = newDir;
            return true;
        }
    }
}