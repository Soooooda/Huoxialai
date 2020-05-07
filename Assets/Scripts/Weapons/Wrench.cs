using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrench : MonoBehaviour
{
   
    //throw star
    //-0.00077
    //0.00049
    //-0.00076
    //90 0 0
    //0.03*3
    public float X_bias;
    public float Y_bias;
    //public float rotate_bias;
    public float size = 1;
    public float ATK = 0.5f;
    public string weaponName;
    public string weaponAnimation;
    //public Transform rotat;
   
    private GameObject role;
    private GameObject roleWeapon;// Weapon that the player's holding
    private GameObject roleRightHand;
    private Animator roleAnimator;
    private GameObject rot;

    private bool isThrown;
    private Vector3 g = new Vector3(0, -5f,0);
    private Vector3 v;


    private bool isHold = false;
    private bool attacking = false;
    private GameObject owner = null;

    private GameObject player;
    private Vector3 playerPos;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("Player");
        isThrown = false;
        size = 1;
        ATK = 0.5f;
}



    // Update is called once per frame
    void Update()
    {
        if (isThrown)
        {

            transform.position += Time.fixedDeltaTime * v;
            v = v + Time.fixedDeltaTime * g;
            if (transform.position.x < 0.0f)
                Destroy(gameObject);
            transform.Rotate(new Vector3(0, 0, 30.0f));
            return;
        }
        playerPos = player.transform.position;
        //Debug.Log(transform.parent.name);
        if (!role) return;
        if (role.name.Contains("Player"))
        {
            //attacking = false;
            if (Input.GetKeyDown(KeyCode.Return) == true)
            {
                //attacking = true;
                StartCoroutine("PlayAnimation");

            }
        }

        if(owner != null && owner.transform.name.Contains("NPC")) {
            if(Vector3.Distance(transform.position, playerPos) <= 3) {
                Debug.Log("NPC wanted to attack");
                StartCoroutine("PlayAnimation");
            }
        }
        
    }



    private void OnTriggerEnter(Collider other)
    {
        if (owner == null) return;

        if (owner.transform.name.Equals("Player") && attacking)
        {
            if (other.transform.name.Contains("NPC"))
            {
                Debug.Log(owner.transform.name + " attack " + other.transform.name);
                //other.GetComponent<NPCMovement>().health -= ATK;
                other.SendMessage("getHurt", ATK);
            }
        }

        if(owner.transform.name.Contains("NPC") && attacking) {
            if(other.transform.name.Equals("Player")) {
                Debug.Log(owner.transform.name + " attack " + other.transform.name);
                //other.GetComponent<PlayerMovement>().health -= ATK;
                other.SendMessage("getHurt", ATK);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //When throwing, the collision is detected here, don't know why, just a bug
        if (isThrown)
        {
            Debug.Log("撞到" + other.transform.name + "啦！");
            if (owner.transform.name.Equals("Player") && attacking)
            {
                if (other.transform.name.Contains("NPC"))
                {
                    Debug.Log(owner.transform.name + " attack " + other.transform.name);
                    other.transform.SendMessage("getHurt", ATK);
                    //other.transform.GetComponent<NPCMovement>().health -= ATK;
                }
            }

            if (owner.transform.name.Contains("NPC") && attacking)
            {
                if (other.transform.name.Equals("Player"))
                {
                    Debug.Log(owner.transform.name + " attack " + other.transform.name);
                    //other.transform.GetComponent<PlayerMovement>().health -= ATK;
                    other.transform.SendMessage("getHurt", ATK);
                }
            }
        }

        //if (other.transform.name != "Player" || !other.transform.name.Contains("NPC"))
        //    return;
        if (!other.transform.tag.Equals("Player")) return;
        if (isThrown) return;

        GetComponent<Rigidbody>().useGravity = false;

        

        if (isHold == false)// Is on the ground
        {
            Debug.Log(other.transform.name+" picked up a "+transform.name);

            role = other.gameObject;
            
            
            role.SendMessage("increaseWeaponNum");

            roleRightHand = other.transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand").gameObject; //roleRightHand.transform.GetChild(5).gameObject;
            rot = other.transform.Find("Throwing hand").gameObject; //roleRightHand.transform.GetChild(5).gameObject;

            Transform tmp = roleRightHand.transform.Find("Weapon");
            
            roleAnimator = other.collider.GetComponent<Animator>();

            // Put the weapon on the player's hand
            
            transform.rotation = roleRightHand.transform.rotation;
            transform.parent = roleRightHand.transform;
            transform.name = "Weapon";

            //Vector3 scale = transform.localScale;

            //scale.Set(2f,2f,2f);//(scale*size).x, (scale * size).y, (scale * size).z

            transform.localScale *= size;

            float x = roleRightHand.transform.position.x + X_bias;
            float y = roleRightHand.transform.position.y + Y_bias;
            float z = roleRightHand.transform.position.z;
            //transform.rotate;
            transform.position = new Vector3(x, y, z);


            // Destroy the original weapon in hand
            if(tmp!=null)
            {
                roleWeapon = tmp.gameObject;
                Destroy(roleWeapon);
            }
            

            // Destory the weapon's rigidbody property
            Destroy(transform.GetComponent<Rigidbody>());

            // If the weapon is hold, then it starts to function
            isHold = true;

            owner = other.gameObject;

            if (owner.name.Contains("NPC")) owner.GetComponent<NPCMovement>().currentWeapon = gameObject;
            if (owner.name.Equals("Player")) owner.GetComponent<PlayerMovement>().currentWeapon = gameObject;

        }
        else // Is holding by the role
        {
        }
    }

    public IEnumerator PlayAnimation()
    {
        roleAnimator.SetBool(weaponAnimation,true);
        yield return new WaitForSeconds(Time.deltaTime * 10);
        
        attacking = true;
        roleAnimator.SetBool(weaponAnimation, false);
        yield return new WaitForSeconds(Time.deltaTime * 40);
        attacking = false;
        if (gameObject != null && weaponName == "proj")
        {
            role.SendMessage("decreaseWeaponNum");
            Destroy(gameObject);
        }
    }

    public void ThrowMe()
    {
        
        //if (role.name.Contains("Player") && gameObject != null && weaponName == "proj")
        //{
        Debug.Log("Throwlalallala!");
        isThrown = true;
        gameObject.transform.SetParent(null);
        gameObject.transform.rotation = rot.transform.rotation;
        v = g+transform.forward*30.0f/1.0f;
    }
}
