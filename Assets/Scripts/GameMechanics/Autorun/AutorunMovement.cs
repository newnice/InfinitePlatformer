using System;
using System.Linq;
using Common;
using Player;
using UnityEngine;

namespace GameMechanics.Autorun {
    public class AutoRunMovement : MonoBehaviour {
        public bool Active { get; set; }
        public PlayerMovement Movement { get; set; }

        private BoxCollider2D _playerCollider;
        public bool isOnGroundLater = true;
        public bool isObstacle = false;
        public float moveSpeed = 1f;
        public bool isGroundUnderNow = true;
        public double raycastAngle = -Math.PI / 3;
        public float rayLengthGround = 15;
        public float rayLengthAngle = 10;

        public AutorunState autorunState;


        private void Start() {
            _playerCollider = GetComponent<BoxCollider2D>();
            autorunState = AutorunState.ON_GROUND;
        }

        private void FixedUpdate() {
            if (!Active) return;

            isObstacle = false;
            isOnGroundLater = false;


            switch (autorunState) {
                case AutorunState.ON_GROUND:
                    CheckGroundCollisions();
                    break;
                case AutorunState.IN_AIR:
                    CheckAirCollisions();
                    break;
            }
        }


        private bool CheckOnGround() {
            Vector2 size = _playerCollider.size * transform.localScale;
            var centerDownPoint = _playerCollider.transform.position + new Vector3(size.x/4+0.2f, -3f, 0);
            var overlaps = Physics2D.OverlapBoxAll(centerDownPoint, new Vector2(size.x/2-0.3f, 6f), 0);
            var filtered = overlaps.Where(c => !c.CompareTag(TagNames.PLAYER));
            isGroundUnderNow = filtered.Any(c=>c.CompareTag(TagNames.GROUND) || c.CompareTag(TagNames.ENEMY));

            ContactPoint2D[] cp = new ContactPoint2D[5];
            foreach (var c in filtered) {
                int cpCount = c.GetContacts(cp);
                for (var i = 0; i < cpCount; i++) {
                    var distance = Math.Abs(cp[i].point.y - _playerCollider.transform.position.y);
                    if (distance < 0.2f) return true;
                }
            }
            return false;
        }
        
        
        
        
        
        private void CheckAirCollisions() {
            Vector2 size = _playerCollider.size * transform.localScale;
            var raycastPos = _playerCollider.transform.position - new Vector3(size.x / 2, 0.1f, 0);
            
            
           /* var raycastGround = Physics2D.RaycastAll(raycastPos, rayLengthGround * Vector2.down);
            var ground = raycastGround.Where(r => r.collider.CompareTag(TagNames.GROUND)).ToArray();
            isGroundUnderNow = ground.Length > 0;
            var onGround = isGroundUnderNow && ground.Any(g => g.distance < 0.1f);*/
           
           var onGround = CheckOnGround();
            if (onGround)
                autorunState = AutorunState.ON_GROUND;


            var raycast = Physics2D.RaycastAll(raycastPos,
                rayLengthAngle * new Vector2((float) Math.Cos(raycastAngle), (float) Math.Sin(raycastAngle)));

            var onGroundLaterByRaycast = raycast.Any(r => r.collider.CompareTag(TagNames.GROUND) &&
                                                          Vector2.up.Equals(r.normal.normalized));
            if (!isGroundUnderNow || onGroundLaterByRaycast || onGround)
                moveSpeed = 1f;

            if (!onGround && isGroundUnderNow)
                moveSpeed = 0.5f;
            
            if (!onGroundLaterByRaycast && isGroundUnderNow)
                moveSpeed = 0f;

            Movement.Move(moveSpeed, false, false);
        }

        private void CheckGroundCollisions() {
            Vector2 size = _playerCollider.size * transform.localScale;
            var raycastPos = _playerCollider.transform.position + new Vector3(size.x / 2 + 0.1f, 0.1f, 0);

            var obstacleRaycast = Physics2D.RaycastAll(raycastPos, Vector2.right);
            isObstacle = obstacleRaycast.Any(r => r.distance < 0.1f &&
                                                  r.collider.CompareTag(TagNames.GROUND) ||
                                                  r.collider.CompareTag(TagNames.ENEMY)
            );

            var nextGroundRaycast = Physics2D.RaycastAll(raycastPos,
                2 * new Vector2((float) Math.Cos(Math.PI / 3), (float) -Math.Sin(Math.PI / 3)));

            isOnGroundLater = nextGroundRaycast.Any(r => r.collider.CompareTag(TagNames.GROUND) &&
                                                         r.distance < 0.5f);
            var needJump = isObstacle || !isOnGroundLater;
            if (needJump)
                Debug.Log("Ooops, need jump");
            Movement.Move(1, needJump, false);
            autorunState = needJump ? AutorunState.IN_AIR : AutorunState.ON_GROUND;
        }


        private void OnDrawGizmos() {
            if (_playerCollider == null) return;
            Gizmos.color = Color.blue;
            Vector2 size = _playerCollider.size * transform.localScale;
            var raycastPos = _playerCollider.transform.position + new Vector3(size.x / 2 + 0.1f, 0.1f, 0);
            Gizmos.DrawRay(raycastPos, Vector2.right);
            Gizmos.DrawRay(raycastPos,
                2 * new Vector2((float) Math.Cos(Math.PI / 3), (float) -Math.Sin(Math.PI / 3)));

            raycastPos = _playerCollider.transform.position - new Vector3(size.x / 2, 0.1f, 0);
            Gizmos.color = Color.green;
  //          Gizmos.DrawRay(raycastPos, rayLengthGround * Vector2.down);
            Gizmos.DrawRay(raycastPos,
                rayLengthAngle * new Vector2((float) Math.Cos(raycastAngle), (float) Math.Sin(raycastAngle)));
            
            
            var centerDownPoint = _playerCollider.transform.position + new Vector3(size.x/4+0.2f, -3f, 0);
            Gizmos.DrawWireCube(centerDownPoint, new Vector2(size.x/2-0.3f, 6f));
        }
    }
}