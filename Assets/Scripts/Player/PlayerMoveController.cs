using UnityEngine;

namespace Player
{
    public class PlayerMoveController : MonoBehaviour
    {
        [SerializeField]
        private float movementSpeed = 3.0f;
        private Rigidbody2D rb;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            rb.velocity = new Vector2((Input.GetAxisRaw("Horizontal") * Time.fixedDeltaTime * movementSpeed), (Input.GetAxisRaw("Vertical") * Time.fixedDeltaTime * movementSpeed));
        }
    }
}
