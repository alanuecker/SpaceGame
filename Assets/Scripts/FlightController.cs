using UnityEngine;
using System.Collections;
using TMPro;

public class FlightController : MonoBehaviour
{
    //speed stuff
    public int minSpeed;
    public int maxSpeed;
    public bool rapidDecelToggle = false;
    bool collisionToggle = false;
    public float collisionTimeout = 5.0f;
    public float timeLeft;

    public GameObject speedTextObj;
    public GameObject engineTextObj;
    TextMeshPro speedText;
    TextMeshPro engineText;

    Rigidbody shipRb;

    public class ShipRotation
    {
        public Vector3 rotation;
        private float resetTurnMultiplier = 5f;

        public ShipRotation()
        {
            rotation = new Vector3(0, 0, 0);
        }

        public float x
        {
            get { return rotation.x; }
            set
            {
                float nextValue = 0;
                if (value < 0) nextValue = Mathf.Clamp(rotation.x + value * Time.fixedDeltaTime, -10, 0);
                else nextValue = Mathf.Clamp(rotation.x + value * Time.fixedDeltaTime, 0, 10);
                rotation.x = nextValue;
            }
        }

        public float y
        {
            get { return rotation.y; }
            set
            {
                float nextValue = 0;
                if (value < 0) nextValue = Mathf.Clamp(rotation.y + value * Time.fixedDeltaTime, -10, 0);
                else nextValue = Mathf.Clamp(rotation.y + value * Time.fixedDeltaTime, 0, 10);
                rotation.y = nextValue;
            }
        }

        public float z
        {
            get { return rotation.z; }
            set
            {
                float nextValue = 0;
                if (value < 0) nextValue = Mathf.Clamp(rotation.z + value * Time.fixedDeltaTime, -10, 0);
                else nextValue = Mathf.Clamp(rotation.z + value * Time.fixedDeltaTime, 0, 10);
                rotation.z = nextValue;
            }
        }

        public void ResetX()
        {
            rotation.x = this.ResetTurnAngle(rotation.x);
        }

        public void ResetY()
        {
            rotation.y = this.ResetTurnAngle(rotation.y);
        }

        public void ResetZ()
        {
            rotation.z = this.ResetTurnAngle(rotation.z);
        }

        private float ResetTurnAngle(float angle)
        {
            float nextAngle = angle;
            float zero = Mathf.Abs(angle) * resetTurnMultiplier * Time.fixedDeltaTime;
            if (angle < 0) nextAngle = Mathf.Min(angle + zero, 0);
            else nextAngle = Mathf.Max(angle - zero, 0);

            return nextAngle;
        }
    }

    public class ShipSpeed
    {
        public float speed;
        private float accel;
        private float decel;
        private int minSpeed;
        private int maxSpeed;

        public ShipSpeed(int min, int max)
        {
            minSpeed = min;
            maxSpeed = max;
            speed = 0;
        }

        public void Accel()
        {
            accel = maxSpeed - speed;
            speed += accel * Time.fixedDeltaTime;
        }

        public void Decel()
        {
            decel = speed - minSpeed;
            speed -= decel * Time.fixedDeltaTime;
        }

        public void RapidDecel()
        {
            decel = speed - minSpeed;

            if (speed < 0)
            {
                float rapidAccel = Mathf.Min(0, Mathf.Abs(speed) * Time.fixedDeltaTime * 2);
                float speedDelta = speed + rapidAccel;
                speed = speedDelta > 0 ? 0 : speedDelta;
            }
            else
            {
                float rapidDecel = Mathf.Max(0, decel * Time.fixedDeltaTime * 2);
                float speedDelta = speed - rapidDecel;
                speed = speedDelta < 0 ? 0 : speedDelta;
            }
        }

        public void Stop(){
            speed = 0.0f;
        }
    }

    ShipSpeed shipSpeed;
    ShipRotation shipRotation;

    void Start()
    {
        shipSpeed = new ShipSpeed(minSpeed, maxSpeed);
        shipRotation = new ShipRotation();
        shipRb = GetComponent<Rigidbody>();
        speedText = speedTextObj.GetComponent<TextMeshPro>();
        if (speedText) speedText.text = shipSpeed.speed.ToString();

        engineText = engineTextObj.GetComponent<TextMeshPro>();
    }

    void FixedUpdate()
    {
        if (!collisionToggle)
        {
            // up and down movement
            if (Input.GetKey(KeyCode.E)) shipRotation.z = 5;
            else if (Input.GetKey(KeyCode.Q)) shipRotation.z = -5;
            else if (shipRotation.z != 0) shipRotation.ResetZ();

            // spin movement
            if (Input.GetKey(KeyCode.W)) shipRotation.x = -5;
            else if (Input.GetKey(KeyCode.S)) shipRotation.x = 5;
            else if (shipRotation.x != 0) shipRotation.ResetX();

            // right and left movement
            if (Input.GetKey(KeyCode.D)) shipRotation.y = 3;
            else if (Input.GetKey(KeyCode.A)) shipRotation.y = -3;
            else if (shipRotation.y != 0) shipRotation.ResetY();

            // apply rotation
            transform.Rotate(shipRotation.rotation);

            // set the speed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (rapidDecelToggle) rapidDecelToggle = false;
                shipSpeed.Accel();
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                if (rapidDecelToggle) rapidDecelToggle = false;
                shipSpeed.Decel();
            }
            else if (Input.GetKey(KeyCode.Z) && !rapidDecelToggle)
                rapidDecelToggle = true;

            // reduce the speed fast
            if (rapidDecelToggle && shipSpeed.speed != 0) shipSpeed.RapidDecel();

            if (shipRb.velocity.magnitude != 0.0f || shipRb.angularVelocity.magnitude != 0.0f)
            {
                shipRb.velocity = shipRb.velocity * 0.001f * Time.fixedDeltaTime;
                shipRb.angularVelocity = shipRb.angularVelocity * 0.001f * Time.fixedDeltaTime;
            }

            transform.Translate(-transform.forward * shipSpeed.speed * Time.fixedDeltaTime, Space.World);
        }
    }

    void Update()
    {
        int cleanSpeed = Mathf.FloorToInt(shipSpeed.speed);
        speedText.text = cleanSpeed.ToString();

        if (collisionToggle && timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            engineText.text = "Engine: " + timeLeft.ToString("F2");
        }
        else if (timeLeft < 0)
        {
            collisionToggle = false;
            engineText.text = "Engine: On";
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude != 0)
        {
            if (!collisionToggle)
            {
                collisionToggle = true;
                timeLeft = collisionTimeout;
            }
            shipSpeed.Stop();
        }
    }
}