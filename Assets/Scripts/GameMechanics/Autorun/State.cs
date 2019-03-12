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


        public float GroundEdgePrecision = 0.5f;

        public const float GroundUnderPrecision = 0.1f;

        public State(BoxCollider2D player, PlayerMovement movement) {
            _player = player;
            _movement = movement;
        }

        public abstract State Move(bool isVerbose);
        protected Vector2 PlayerSize => _player.size * _player.transform.localScale;
        protected Vector2 PlayerPosition => _player.transform.position;
        protected Vector2 PlayerRightBottom => PlayerPosition + new Vector2(PlayerSize.x / 2, 0);

        protected Vector2 PlayerLeftBottom => PlayerPosition - new Vector2(PlayerSize.x / 2, 0);

        protected bool IsObstacle() {
            var raycastPos = PlayerRightBottom +
                             new Vector2(0, GroundUnderPrecision);

            var obstacleRaycast = Physics2D.RaycastAll(raycastPos, Vector2.right, 0.5f);
            return obstacleRaycast.Any(r => r.distance < GroundEdgePrecision &&
                                            r.collider.CompareTag(TagNames.GROUND) ||
                                            r.collider.CompareTag(TagNames.ENEMY)
            );
        }

        protected Vector2 GetColliderRightTop(Collider2D collider) {
            if (collider is BoxCollider2D) {
                var box = collider as BoxCollider2D;
                var position = box.transform.position;
                var size = box.size;

                return new Vector2(position.x + size.x / 2, position.y + size.y);
            }
            else if (collider is CircleCollider2D) {
                var circle = collider as CircleCollider2D;
                var circleTransform = circle.transform;
                return circleTransform.position + new Vector3(0, circle.radius * circleTransform.localScale.y, 0);
            }

            Debug.LogError($"Undefined collider type {collider}");
            return Vector2.negativeInfinity;
        }

        protected Vector2 GetColliderLeftTop(BoxCollider2D groundCollider) {
            var position = groundCollider.transform.position;
            var size = groundCollider.size;

            return new Vector2(position.x - size.x / 2, position.y + size.y);
        }

        protected Collider2D[] GetCollidersUnder() {
            var centerDownPoint = PlayerPosition + new Vector2(0, -5f);
            var overlaps = Physics2D.OverlapBoxAll(centerDownPoint, new Vector2(PlayerSize.x, 10f), 0);
            return overlaps.ToArray();
        }


        protected Vector2 CheckUpperGround() {
            var boxSize = new Vector2(3f, 5f);
            var centerDownPoint = PlayerRightBottom + new Vector2(boxSize.x / 2, boxSize.y / 2 + GroundUnderPrecision);

            var overlaps = Physics2D.OverlapBoxAll(centerDownPoint, boxSize, 0);
            var filtered = overlaps.Where(c => c.CompareTag(TagNames.GROUND));

            Vector2 min = Vector2.positiveInfinity;
            foreach (var c in filtered) {
                var point = GetColliderLeftTop((BoxCollider2D) c);
                if (min.x.FloatGreater(point.x) &&
                    (centerDownPoint.y + boxSize.y / 2).FloatGreater(point.y) &&
                    point.x.FloatGreater(PlayerRightBottom.x))
                    min = point;
            }


            return min;
        }

        protected bool IsGroundUnderNow(Collider2D[] filtered, out Collider2D groundUnder) {
            groundUnder = null;
            if (filtered == null || filtered.Length == 0) return false;

            ContactPoint2D[] cp = new ContactPoint2D[5];
            foreach (var c in filtered) {
                if (!c.CompareTag(TagNames.GROUND)) continue;
                int cpCount = c.GetContacts(cp);
                for (var i = 0; i < cpCount; i++) {
                    var distance = Math.Abs(cp[i].point.y - PlayerPosition.y);
                    if (GroundUnderPrecision.FloatGreater(distance) &&
                        Math.Abs(Vector2.Dot(cp[i].normal, Vector2.up)) > 0.9f) {
                        groundUnder = c;
                        return true;
                    }
                }
            }

            return false;
        }

        protected Vector2 CheckLongestGroundUnder(Collider2D[] filtered) {
            var groundCliff = Vector2.negativeInfinity;
            var cp = new ContactPoint2D[5];
            foreach (var c in filtered) {
                if (!c.CompareTag(TagNames.GROUND)) continue;
                var ground = GetColliderRightTop((BoxCollider2D) c);
                if (ground.x.FloatGreater(groundCliff.x)) {
                    groundCliff = ground;
                }
            }

            return groundCliff;
        }

        protected bool IsOnlyKillZone(Collider2D[] filtered) {
            return !filtered.Any(c => c.CompareTag(TagNames.GROUND));
        }

        protected bool IsEnemyUnderNow(Collider2D[] collidersUnder, out Vector2 enemyPosition) {
            enemyPosition = Vector2.negativeInfinity;
            for (var i = 0; i < collidersUnder.Length; i++) {
                var c = collidersUnder[i];
                if (!c.CompareTag(TagNames.ENEMY)) continue;
                var enemy = c as CircleCollider2D;
                var enemyTransform = enemy.transform;
                var enemyTop = enemyTransform.position + enemy.radius * enemyTransform.localScale;
                var distance = PlayerPosition.y - enemyTop.y;
                if (GroundUnderPrecision.FloatGreater(distance)) {
                    enemyPosition = enemyTop;
                    return true;
                }
            }

            return false;
        }

        protected bool IsEnemyUnderForFooting(Collider2D[] collidersUnder, out Vector2 enemyPosition) {
            enemyPosition = Vector2.negativeInfinity;
            for (var i = 0; i < collidersUnder.Length; i++) {
                var c = collidersUnder[i];
                if (!c.CompareTag(TagNames.ENEMY)) continue;
                var enemy = c as CircleCollider2D;
                var enemyTransform = enemy.transform;
                var enemyTop = enemyTransform.position + new Vector3(0, enemy.radius * enemyTransform.localScale.y, 0);

                enemyPosition = enemyTop;
                return true;
            }

            return false;
        }

        protected abstract bool IsGroundPassed(Vector2 ground);
    }
}