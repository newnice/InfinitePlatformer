using System;
using System.Linq;
using Common;
using Player;
using UnityEngine;

namespace GameMechanics.Autorun {
    public abstract class State {
        private readonly BoxCollider2D _player;
        private readonly PlayerMovement _movement;

        protected PlayerMovement Movement => _movement;
        protected BoxCollider2D PlayerCollider => _player;


        public float groundEdgePrecision = 0.3f;

        public float groundUnderPrecision = 0.1f;

        public State(BoxCollider2D player, PlayerMovement movement) {
            _player = player;
            _movement = movement;
        }

        public abstract State Move();
        protected Vector2 PlayerSize => _player.size * _player.transform.localScale;
        protected Vector2 PlayerPosition => _player.transform.position;
        protected Vector2 PlayerRightBottom => PlayerPosition + new Vector2(PlayerSize.x / 2, 0);

        protected bool CheckObstacle() {
            var raycastPos = PlayerPosition +
                             new Vector2(PlayerSize.x / 2 + groundEdgePrecision, groundUnderPrecision);

            var obstacleRaycast = Physics2D.RaycastAll(raycastPos, Vector2.right);
            return obstacleRaycast.Any(r => r.distance < groundEdgePrecision &&
                                            r.collider.CompareTag(TagNames.GROUND) ||
                                            r.collider.CompareTag(TagNames.ENEMY)
            );
        }

        protected Vector2 GetColliderRightTop(BoxCollider2D groundCollider) {
            var position = groundCollider.transform.position;
            var size = groundCollider.size;

            return new Vector2(position.x + size.x / 2, position.y + size.y);
        }

        protected Collider2D[] CheckGround() {
            var centerDownPoint = PlayerPosition + new Vector2(0, -5f);
            var overlaps = Physics2D.OverlapBoxAll(centerDownPoint, new Vector2(PlayerSize.x, 10f), 0);
            var filtered = overlaps.Where(c => !c.CompareTag(TagNames.PLAYER)).ToArray();

            return filtered;
        }

        protected BoxCollider2D CheckGroundNow(Collider2D[] filtered) {
            ContactPoint2D[] cp = new ContactPoint2D[5];
            foreach (var c in filtered) {
                if (!c.CompareTag(TagNames.GROUND)) continue;
                int cpCount = c.GetContacts(cp);
                for (var i = 0; i < cpCount; i++) {
                    var distance = Math.Abs(cp[i].point.y - PlayerPosition.y);
                    if (distance < groundUnderPrecision && Math.Abs(Vector2.Dot(cp[i].normal, Vector2.up)) > 0.9f)
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
        protected bool IsGroundPassed(Vector2 ground) {
            return PlayerRightBottom.x + groundEdgePrecision >= ground.x;
        }
    }

    public class OnGroundState : State {
        private Vector2 groundEnd = Vector2.negativeInfinity;


        public override State Move() {
            if (Vector2.negativeInfinity.Equals(groundEnd) || IsGroundPassed(groundEnd)) {
                var colliders = CheckGround();
                var groundCollider = CheckGroundNow(colliders);
                if (groundCollider != null) {
                    groundEnd = GetColliderRightTop(groundCollider);
                }
            }

            var obstacle = CheckObstacle();

            if (!obstacle && !IsGroundPassed(groundEnd)) {
                Movement.Move(1, false, false);
                return this;
            }

            if (IsGroundPassed(groundEnd)) {
                var grounds = CheckGround();
                Vector2 next = CheckLongestGroundUnder(grounds);
                if (!Vector2.negativeInfinity.Equals(next) && groundEnd.x < next.x) {
                    groundEnd = next;
                  //  Movement.Move(0.5f, false, false);
                    Debug.Log($"Don't jump, found new ground without cliff");
                    //return this;
                    return new InAirState(PlayerCollider, Movement);
                }
            }


            Movement.Move(1, true, false);
            Debug.Log(
                $"Jump obstacle={obstacle} groundEnd.x={groundEnd.x} PlayerRightBottom.x ={PlayerRightBottom.x + groundEdgePrecision}");
            return new InAirState(PlayerCollider, Movement, true);
        }

        public OnGroundState(BoxCollider2D player, PlayerMovement movement) : base(player, movement) { }
    }

    public class InAirState : State {
        private Vector2 groundUnderEnd = Vector2.negativeInfinity;

        private bool _cliff;

        public InAirState(BoxCollider2D player, PlayerMovement movement, bool cliff = false) : base(player, movement) {
            _cliff = cliff;
        }

        public override State Move() {
            var filtered = CheckGround();
            if (CheckGroundNow(filtered) != null) {
                Movement.Move(1, false, false);
                Debug.Log("Change state on OnGroundState ");
                return new OnGroundState(PlayerCollider, Movement);
            }


            if (IsOnlyKillZone(filtered)) {
                Movement.Move(1, false, false);
                Debug.Log("Cliff skipped because of kill zone");
                _cliff = false;
                return this;
            }

            var longestGround = CheckLongestGroundUnder(filtered);
            if (Vector2.negativeInfinity.Equals(longestGround) ||!IsGroundPassed(longestGround)) {
                Debug.Log("Cliff skipped because of new longestGround");
                Debug.Log($"old={groundUnderEnd} new = {longestGround}");
                groundUnderEnd = longestGround;
                _cliff = false;
            }

            if (!IsGroundPassed(groundUnderEnd)) {
                Movement.Move(1, false, false);
                _cliff = false;
                Debug.Log(
                    $"Cliff skipped because of ground exist groundUnderEnd.x ={groundUnderEnd.x} PlayerRightBottom.x ={PlayerRightBottom.x + groundEdgePrecision}");
                return this;
            }

            if (_cliff) {
                Movement.Move(1, false, false);
                return this;
            }


            Movement.Move(0, false, false);
            return this;
        }
    }

    public class AutoRunMovement : MonoBehaviour {
        public bool Active { get; set; }
        public PlayerMovement Movement { get; set; }

        private BoxCollider2D _playerCollider;


        public State autorunState;


        private void Start() {
            _playerCollider = GetComponent<BoxCollider2D>();
            autorunState = new OnGroundState(_playerCollider, Movement);
        }

        private void FixedUpdate() {
            if (!Active) return;
            autorunState = autorunState.Move();
        }

        private void OnDrawGizmos() {
            if (_playerCollider == null) return;
            Gizmos.color = Color.blue;
            Vector2 PlayerSize = _playerCollider.size * _playerCollider.transform.localScale;
            Vector2 PlayerPosition = _playerCollider.transform.position;
            var centerDownPoint = PlayerPosition + new Vector2(0, -5f);
            Gizmos.DrawWireCube(centerDownPoint, new Vector2(PlayerSize.x, 10f));
        }
    }
}