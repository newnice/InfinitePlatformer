using System;
using System.Linq;
using UnityEngine;

namespace Player {
    public class AutoRunMovement : MonoBehaviour {
        public bool Active { get; set; }
        public PlayerMovement Movement { get; set; }


        private BoxCollider2D _playerCollider;
        public bool isOnGroundLater = true;
        public bool isObstacle = false;
        public bool needMove = true;
        public bool isGroundUnderNow = true;
        public double raycastAngle = -Math.PI / 3;
        public float rayLengthGround = 15;
        public float rayLengthAngle =10;


        private void Start() {
            _playerCollider = GetComponent<BoxCollider2D>();
        }

        private void FixedUpdate() {
            if (!Active) return;

            isObstacle = false;
            isOnGroundLater = false;

            Vector2 position = _playerCollider.transform.position;
            Vector2 size = _playerCollider.size * transform.localScale;
            var colliders = Physics2D
                .OverlapBoxAll(position+ new Vector2(0.1f, size.y/2), size, 0)
                .Where(c => c.gameObject != gameObject && !c.CompareTag("Player")).ToArray();
            Vector2 pos2 = new Vector2(position.x, position.y) + new Vector2(0, size.y / 2);

            
            // Debug.Log($"player position = {pos2}");
            foreach (var col in colliders) {
                ContactPoint2D[] points = new ContactPoint2D[5];

                var pCount = col.GetContacts(points);

                for (var k = 0; k < pCount; k++) {
                    var p = points[k];
                    if (!isOnGroundLater) {
                        var dot = Vector2.Dot(p.normal.normalized, Vector2.down);
                        isOnGroundLater = dot > 0.9;
                    }

                    if (!isObstacle) {
                        var dot = Vector2.Dot(p.normal.normalized, Vector2.right);
                        isObstacle = (dot > 0.9) && (p.point.x > pos2.x);
                    }
                }
            }

            needMove = true;
            if (colliders.Length == 0) {
                var raycastPos = _playerCollider.transform.position - new Vector3(size.x/2, 0.1f, 0);
                RaycastHit2D raycastGround = Physics2D.Raycast(raycastPos, rayLengthGround * Vector2.down);
                isGroundUnderNow = raycastGround.collider.CompareTag(TagNames.GROUND) /*&&
                              raycastGround.distance < 0.3f*/;


                RaycastHit2D raycast = Physics2D.Raycast(raycastPos,
                    rayLengthAngle * new Vector2((float) Math.Cos(raycastAngle), (float) Math.Sin(raycastAngle)));

                var onGroundLaterByRaycast = raycast.collider.CompareTag(TagNames.GROUND) && Vector2.up.Equals(raycast.normal.normalized);

                needMove = isGroundUnderNow || onGroundLaterByRaycast ||
                           raycastGround.collider.CompareTag(TagNames.KILLZONE);
                if (isGroundUnderNow && !onGroundLaterByRaycast)
                    needMove = false;
                Debug.Log(
                    $"Need move = {needMove} isGroundUnderNow = {isGroundUnderNow}; onGroundLaterByRaycast= {onGroundLaterByRaycast}");
            }


            Movement.Move(needMove ? 1 : 0, isObstacle || !isOnGroundLater, false);
        }


        private void OnDrawGizmos() {
            if (_playerCollider == null) return;
            Gizmos.color = Color.blue;
            Vector2 size = _playerCollider.size * transform.localScale;
            Vector2 position = _playerCollider.transform.position;
            Gizmos.DrawWireCube(position+ new Vector2(0.1f, size.y/2), /*new Vector2(0.4f, size.y)*/size);
            Gizmos.DrawRay(_playerCollider.transform.position  - new Vector3(size.x/2, 0.1f, 0), rayLengthGround * Vector3.down);
            Gizmos.DrawRay(_playerCollider.transform.position  - new Vector3(size.x/2, 0.1f, 0),
                rayLengthAngle * new Vector2((float) Math.Cos(raycastAngle), (float) Math.Sin(raycastAngle)));
        }
    }
}