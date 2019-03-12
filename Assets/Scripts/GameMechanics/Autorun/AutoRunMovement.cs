using Player;
using UnityEngine;

namespace GameMechanics.Autorun {
    public class AutoRunMovement : MonoBehaviour {
        private PlayerMovement _movement;
        private BoxCollider2D _playerCollider;
        private PlayerCharacter _character;
        private State _autorunState;
        private float _totalTime;
        [SerializeField] private HUD _hud = null;
        [SerializeField] private bool _verbose = false;


        private void Start() {
            _playerCollider = GetComponent<BoxCollider2D>();
            _movement = GetComponent<PlayerMovement>();
            _autorunState = new OnGroundState(_playerCollider, _movement);
            _character = GetComponent<PlayerCharacter>();
        }

        private void FixedUpdate() {
            if (enabled && _character.Lives > 0) {
                _autorunState = _autorunState.Move(_verbose);
                _totalTime += Time.fixedDeltaTime;
                _hud.UpdateTotalAutorunTime(_totalTime);
            }
        }

        private void OnDrawGizmos() {
            if(!_verbose) return;
            if (_playerCollider == null) return;
            Gizmos.color = Color.blue;
            Vector2 PlayerSize = _playerCollider.size * _playerCollider.transform.localScale;
            Vector2 PlayerPosition = _playerCollider.transform.position;
            var centerDownPoint = PlayerPosition + new Vector2(0, -5f);
            Gizmos.DrawWireCube(centerDownPoint, new Vector2(PlayerSize.x, 10f));

            /* Gizmos.color = Color.green;
             var raycastPos = PlayerPosition + new Vector2(PlayerSize.x / 2, 0) +new Vector2(0, State.GroundUnderPrecision);
             Gizmos.DrawRay(raycastPos, Vector3.right*0.5f);*/

            Gizmos.color = Color.yellow;
            centerDownPoint = PlayerPosition + new Vector2(PlayerSize.x / 2, 0) +
                              new Vector2(1.5f, 2.5f + State.GroundUnderPrecision);
            Gizmos.DrawWireCube(centerDownPoint, new Vector2(3f, 5f));
        }
    }
}