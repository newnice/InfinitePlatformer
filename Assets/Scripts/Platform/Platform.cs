using UnityEngine;

public class Platform : MonoBehaviour {
    void Awake() {
        tag = TagNames.GROUND; // If you get an error here, create a Tag in Unity called "Ground".
        //See the GameplayConstants.cs file for other required Tags and Layers.

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            MatchColliderToSpriteSize(spriteRenderer);
            InitMiniMapObject(spriteRenderer);
        }
    }

    private void InitMiniMapObject(SpriteRenderer spriteRenderer) {
        var obj = new GameObject(gameObject.name + "_map");
        obj.transform.parent = spriteRenderer.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.layer = GameplayConstants.LAYER_MINI_MAP;
        
        var sprite = obj.AddComponent<SpriteRenderer>();
        sprite.sprite = spriteRenderer.sprite;
        sprite.drawMode = spriteRenderer.drawMode;
        sprite.material = spriteRenderer.material;
        sprite.size = spriteRenderer.size;
        sprite.color = Color.black;


    }

    private void MatchColliderToSpriteSize(SpriteRenderer spriteRenderer) {
        var coll = GetComponent<BoxCollider2D>();
        if (coll == null) {
            coll = gameObject.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
        }

        coll.size = spriteRenderer.size;
        coll.offset = 0.5f * spriteRenderer.size.y * Vector2.up;
    }
}