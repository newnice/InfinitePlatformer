using System;
using System.Linq;
using Common;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace GameMechanics.Autorun {
    public abstract class State {
        private readonly BoxCollider2D _player;
        private readonly PlayerMovement _movement;

        protected PlayerMovement Movement => _movement;
        protected BoxCollider2D PlayerCollider => _player;


        public const float GroundEdgePrecision = 0.5f;

        public const float GroundUnderPrecision = 0.1f;

        public State(BoxCollider2D player, PlayerMovement movement) {
            _player = player;
            _movement = movement;
        }

        public abstract State Move();
        protected Vector2 PlayerSize => _player.size * _player.transform.localScale;
        protected Vector2 PlayerPosition => _player.transform.position;
        protected Vector2 PlayerRightBottom => PlayerPosition + new Vector2(PlayerSize.x / 2, 0);

        protected Vector2 PlayerLeftBottom => PlayerPosition - new Vector2(PlayerSize.x / 2, 0);

        protected bool CheckObstacle() {
            var raycastPos = PlayerRightBottom +
                             new Vector2(0, GroundUnderPrecision);

            var obstacleRaycast = Physics2D.RaycastAll(raycastPos, Vector2.right, 0.5f);
            return obstacleRaycast.Any(r => r.distance < GroundEdgePrecision &&
                                            r.collider.CompareTag(TagNames.GROUND) ||
                                            r.collider.CompareTag(TagNames.ENEMY)
            );
        }

        protected Vector2 GetColliderRightTop(BoxCollider2D groundCollider) {
            var position = groundCollider.transform.position;
            var size = groundCollider.size;

            return new Vector2(position.x + size.x / 2, position.y + size.y);
        }

        protected Vector2 GetColliderLeftTop(BoxCollider2D groundCollider) {
            var position = groundCollider.transform.position;
            var size = groundCollider.size;

            return new Vector2(position.x - size.x / 2, position.y + size.y);
        }

        protected Collider2D[] CheckGround() {
            var centerDownPoint = PlayerPosition + new Vector2(0, -5f);
            var overlaps = Physics2D.OverlapBoxAll(centerDownPoint, new Vector2(PlayerSize.x, 10f), 0);
            var filtered = overlaps.Where(c => c.CompareTag(TagNames.GROUND)).ToArray();

            return filtered;
        }


        protected Vector2 CheckUpperGround() {
            var centerDownPoint = PlayerRightBottom + new Vector2(1.5f, 2.5f + GroundUnderPrecision);
            var overlaps = Physics2D.OverlapBoxAll(centerDownPoint, new Vector2(3f, 5f), 0);
            var filtered = overlaps.Where(c => c.CompareTag(TagNames.GROUND) /*|| c.CompareTag(TagNames.ENEMY)*/);

            Vector2 min = Vector2.positiveInfinity;
            foreach (var c in filtered) {
                var point = GetColliderLeftTop((BoxCollider2D) c);
                if (point.x < min.x)
                    min = point;
            }


            return min;
        }

        protected BoxCollider2D CheckGroundNow(Collider2D[] filtered) {
            ContactPoint2D[] cp = new ContactPoint2D[5];
            foreach (var c in filtered) {
                if (!c.CompareTag(TagNames.GROUND)) continue;
                int cpCount = c.GetContacts(cp);
                for (var i = 0; i < cpCount; i++) {
                    var distance = Math.Abs(cp[i].point.y - PlayerPosition.y);
                    if (distance < GroundUnderPrecision && Math.Abs(Vector2.Dot(cp[i].normal, Vector2.up)) > 0.9f)
                        return (BoxCollider2D) c;
                }
            }

            return null;
        }

        protected Vector2 CheckLongestGroundUnder(Collider2D[] filtered) {
            var groundCliff = Vector2.negativeInfinity;
            var cp = new ContactPoint2D[5];
            foreach (var c in filtered) {
                if (!c.CompareTag(TagNames.GROUND)) continue;
                var ground = GetColliderRightTop((BoxCollider2D) c);
                if (groundCliff.x < ground.x) {
                    groundCliff = ground;
                }
            }

            return groundCliff;
        }

        protected bool IsOnlyKillZone(Collider2D[] filtered) {
            return filtered.All(c => c.CompareTag(TagNames.KILLZONE) || c.CompareTag(TagNames.UNTAGGED));
        }

        protected abstract bool IsGroundPassed(Vector2 ground);
    }

    public class OnGroundState : State {
        private Vector2 _groundEnd = Vector2.negativeInfinity;

        protected override bool IsGroundPassed(Vector2 ground) {
            return PlayerLeftBottom.x + GroundEdgePrecision >= ground.x;
        }

        public override State Move() {
            var grounds = CheckGround();
            if (Vector2.negativeInfinity.Equals(_groundEnd) || IsGroundPassed(_groundEnd)) {
                var groundCollider = CheckGroundNow(grounds);
                if (groundCollider != null) {
                    _groundEnd = GetColliderRightTop(groundCollider);
                }
            }

            var obstacle = CheckObstacle();


            if (IsGroundPassed(_groundEnd)) {
                Vector2 next = CheckLongestGroundUnder(grounds);
                if (!Vector2.negativeInfinity.Equals(next) && _groundEnd.x < next.x) {
                    Movement.Move(1, false, false);
                    Debug.Log($"Change state on Air in soft cliff point {PlayerPosition}");
                    return new InAirState(PlayerCollider, Movement, false, PlayerLeftBottom);
                }
            }


            var nearestUpper = CheckUpperGround();

            if (!Vector2.positiveInfinity.Equals(nearestUpper)) {
                Movement.Move(1, true, false);
                Debug.Log($"Jump for upper ground {nearestUpper}");
                return new InAirState(PlayerCollider, Movement, nearestUpper);
            }


            if (!obstacle && !IsGroundPassed(_groundEnd)) {
                Movement.Move(1, false, false);
                return this;
            }

            Movement.Move(1, true, false);
            Debug.Log($"Change state on Air in cliff point {PlayerPosition}");
            return new InAirState(PlayerCollider, Movement, true, PlayerLeftBottom);
        }

        public OnGroundState(BoxCollider2D player, PlayerMovement movement) : base(player, movement) { }
    }

    
    
    
    
    public class InAirState : State {
        private Vector2 _groundUnderEnd = Vector2.negativeInfinity;
        private Vector2 _cliffPoint;
        private Vector2 _requiredPointToFly = Vector2.negativeInfinity;

        private bool _cliff;

        public InAirState(BoxCollider2D player, PlayerMovement movement, bool cliff, Vector2 cliffPoint) : base(
            player, movement) {
            _cliff = cliff;
            _cliffPoint = cliffPoint;
        }

        public InAirState(BoxCollider2D player, PlayerMovement movement, Vector2 requiredPointToFly) : base(
            player, movement) {
            _requiredPointToFly = requiredPointToFly;
        }

        protected override bool IsGroundPassed(Vector2 ground) {
            return PlayerRightBottom.x - GroundEdgePrecision - 0.1f >= ground.x;
        }


        public override State Move() {
            var grounds = CheckGround();
            var currentGround = CheckGroundNow(grounds);
            var groundPoint = Vector2.negativeInfinity;
            if (currentGround != null)
                groundPoint = GetColliderRightTop(currentGround);

            
            
            if (groundPoint.x + GroundEdgePrecision > _cliffPoint.x) {
                Movement.Move(1, false, false);
                Debug.Log($"Change state on OnGroundState in point {groundPoint} ");
                return new OnGroundState(PlayerCollider, Movement);
            }

           
            if (!Vector2.negativeInfinity.Equals(_requiredPointToFly)) {
                var distance = _requiredPointToFly - PlayerRightBottom;
                if (distance.x > 0) {
                    if (distance.x > distance.y)
                        Movement.Move(1, false, false);
                    else {
                        Movement.Move(0, false, false);
                    }

                    return this;
                }
            }

            if (IsOnlyKillZone(grounds)) {
                Movement.Move(1, false, false);
                _cliff = false;
                return this;
            }

            var longestGround = CheckLongestGroundUnder(grounds);
            if (!IsGroundPassed(longestGround) && longestGround.x > _groundUnderEnd.x) {
                _groundUnderEnd = longestGround;
                _cliff = false;
            }

            if (_cliff) {
                Movement.Move(1, false, false);
                return this;
            }

            if (!IsGroundPassed(_groundUnderEnd)) {
                Movement.Move(1, false, false);
                _cliff = false;
                return this;
            }


            Movement.Move(0, false, false);
            return this;
        }
    }

    public class AutoRunMovement : MonoBehaviour {
        private PlayerMovement _movement;
        private BoxCollider2D _playerCollider;
        private PlayerCharacter _character;
        private State _autorunState;
        private float _totalTime;
        [SerializeField] private HUD _hud = null;


        private void Start() {
            _playerCollider = GetComponent<BoxCollider2D>();
            _movement = GetComponent<PlayerMovement>();
            _autorunState = new OnGroundState(_playerCollider, _movement);
            _character = GetComponent<PlayerCharacter>();
        }

        private void FixedUpdate() {
            if (enabled && _character.Lives > 0) {
                _autorunState = _autorunState.Move();
                _totalTime += Time.fixedDeltaTime;
                _hud.UpdateTotalAutorunTime(_totalTime);
            }
        }

        private void OnDrawGizmos() {
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