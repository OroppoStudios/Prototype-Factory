using UnityEngine;

public class GrapplingGun : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, cam, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    private bool Pulling = false;

    void Awake()
    {
       
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
            StartGrapple();       
        else if (Input.GetMouseButtonUp(2))
            StopGrapple();

        if (Input.GetMouseButtonDown(1))
            StartPullGrapple();
        else if (Input.GetMouseButtonUp(1))
            StopPullGrapple();

    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, whatIsGrappleable))
        {
            player.GetComponent<CharacterMovement>().IsDecellerating = false; 
            player.GetComponent<CharacterMovement>().SlowCurrentSpeed = 0.01f;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = 500f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
            player.GetComponent<CharacterMovement>().InAir = true;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
        player.GetComponent<CharacterMovement>().InAir = false;
    }
    private void StartPullGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, whatIsGrappleable))
        {
            player.GetComponent<CharacterMovement>().IsDecellerating = false;
            player.GetComponent<CharacterMovement>().SlowCurrentSpeed = 0.01f;
            player.GetComponent<CharacterMovement>().InAir = true;
            Pulling = true;
            grapplePoint = hit.point;
            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
            player.GetComponent<CharacterMovement>().RB.useGravity = false;
         player.GetComponent<CharacterMovement>().RB.velocity=(grapplePoint - player.position).normalized* player.GetComponent<CharacterMovement>().GrappleSpeed/3.6f;
        }
    }
    private void StopPullGrapple()
    {
        lr.positionCount = 0;
        Pulling = false;
        player.GetComponent<CharacterMovement>().RB.useGravity = true;
        player.GetComponent<CharacterMovement>().InAir = false;
    }
    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint&&!Pulling) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}