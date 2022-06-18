using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using playerWeapon;
using UnityEngine.UI;
using FirstGearGames.SmoothCameraShaker;

namespace K.Monster
{
    public class M_Rat : MonoBehaviour
    {
        BossMonster data;
        GameObject player;
        public BossMonster DATA
        {
            get{
                return data;
            }
            set{
                data = value;
            }
        }
        bool BigAttack;
        float CloseAtkDistance = 10f;
        float FarAtkDistance = 20f;
        bool AttackEnd;
        bool isAnimating;
        float dashAttackTimer;
        float lookTimer;
        Vector3 lookVector;

        float testtimer;

        enum AttackState
        {
            none,
            dash,
            wave,
            bigattack,
            attacked,
            dead
        }

        AttackState nowState;
        public ShakeData MyShake;
        [SerializeField]
        GameObject deadEffect;
        [SerializeField]
        GameObject fish;
        [SerializeField]
        AudioClip hit;
        [SerializeField]
        AudioClip dead;

        void Awake()
        {
            data = new BossMonster(1f, 200f, MonsterBase.type.boss);
            BigAttack = false;
            AttackEnd = true;
            isAnimating = false;
            nowState = 0;
            dashAttackTimer = 0.3f;
            lookVector = new Vector3();
            testtimer = 0f;
        }

        void Start()
        {
            player = GameObject.FindWithTag("Player");
        }
        IEnumerator CallEffect()
        {
            yield return new WaitForSeconds(4.5f);
            this.GetComponent<AudioSource>().clip = dead;
            this.GetComponent<AudioSource>().Play();
            Vector3 pos = this.transform.position + new Vector3(0, 2, 0);
            Instantiate(deadEffect, this.transform) ;
            Instantiate(fish, pos, Quaternion.identity);
        }

        void Update()
        {
            Vector3 dis = player.transform.position - this.transform.position;
            if(dis.magnitude <100f)
            {
                this.transform.GetChild(4).GetComponent<Renderer>().material.color = Color.red;
                GameObject.Find("Canvas").transform.GetChild(9).gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.GetChild(10).gameObject.SetActive(true);
                if (data.FullHp <= 0f)
                {   
                    this.transform.GetChild(4).GetComponent<Renderer>().material.color = Color.white;
                    Dead();
                }
                //공격 모션 실행 중이 아닐 때 attack 실행
                //if(isAnimating == false)
                Attack();

                //피격 시 공격 모션 무시하고 피격 모션
                //if(검 등 충돌 시)
                //Damaged();
            }

        }

        //hp에 따라 공격패턴 변화, 70%, 40% 10%
        //미구현

        void Attacked()
        {
            CameraShakerHandler.Shake(MyShake);
            this.GetComponent<AudioSource>().clip = hit;
            this.GetComponent<AudioSource>().Play();
            nowState = AttackState.attacked;
             gameObject.GetComponent<Animator>().SetTrigger("attacked");
             StartCoroutine("AttackedDelayTimer", 1f);
        }

        void Dead()
        {
            if(nowState == AttackState.none)
            {
                nowState = AttackState.dead;
                gameObject.GetComponent<Animator>().SetTrigger("dead");
                StartCoroutine(CallEffect());
                //this.GetComponent<Rigidbody>().isKinematic = true;
                StartCoroutine("DeadDelayTimer", 5f);
            }
        }

        void OnTriggerEnter(Collider collision)
        {
            if(collision.gameObject.tag == "hitBox")
            {
                data.FullHp -= collision.gameObject.transform.parent.GetComponent<weaponController>().plDemage.demage;
                if (data.FullHp < 0)
                {
                    data.FullHp = 0;
                }
                //Debug.Log(collision.gameObject.GetComponent<weaponController>().plDemage.demage);
                GameObject.Find("Canvas").transform.GetChild(10).gameObject.GetComponent<Image>().fillAmount = data.FullHp / data.Hp;
                this.transform.GetChild(4).gameObject.transform.localScale = new Vector3(2 * (data.FullHp / data.Hp),
                this.transform.GetChild(4).gameObject.transform.localScale.y,
                this.transform.GetChild(4).gameObject.transform.localScale.z);
                Attacked();
            }

            if (collision.gameObject.tag == "fire")
            {
                Debug.Log(player.GetComponent<weaponController>().plDemage.demage);
                data.FullHp -= player.GetComponent<weaponController>().plDemage.demage;
                if (data.FullHp < 0)
                {
                    data.FullHp = 0;
                }
                Debug.Log(player.GetComponent<weaponController>().plDemage.demage);
                GameObject.Find("Canvas").transform.GetChild(10).gameObject.GetComponent<Image>().fillAmount = data.FullHp / data.Hp;
                this.transform.GetChild(2).gameObject.transform.localScale = new Vector3(1 * (data.FullHp / data.Hp),
                this.transform.GetChild(2).gameObject.transform.localScale.y,
                this.transform.GetChild(2).gameObject.transform.localScale.z);
                //Attacked();
            }
        }

        //공격 함수
        void Attack()
        {
            /*
            //fullhp는 인스턴스 hp
            if(data.Hp*0.4f < data.FullHp && data.FullHp <= data.Hp*0.7f)
            {
                BigAttack = true;
            }

            if(data.Hp*0.1f < data.FullHp && data.FullHp <= data.Hp*0.4f)
            {
                BigAttack = true;
            }

            if(data.Hp*0.0f < data.FullHp && data.FullHp <= data.Hp*0.1f)
            {
                BigAttack = true;
            }

            if(BigAttack == true)
            {
                Atk_BigAttack(3f);
                return;
            }
            */

            //근접 공격 호출, 근거리 범위 안
            if(Vector3.Distance(this.transform.position, player.transform.position) <= CloseAtkDistance)
            {
                if(nowState == AttackState.none)
                {
                    nowState = AttackState.wave;
                }
            }
            
            if(nowState == AttackState.wave)
            {
                if(lookTimer < 1f)
                {
                    //돌아보기
                    Look();
                }
                lookTimer += Time.deltaTime;
                //공격
                Atk_Wave(2.0f);
                return;
            }
            
            //원거리 공격 호출, 원거리 범위 안
            if(Vector3.Distance(this.transform.position, player.transform.position) <= FarAtkDistance &&
            Vector3.Distance(this.transform.position, player.transform.position) > CloseAtkDistance)
            {
                if(nowState == AttackState.none)
                {
                    nowState = AttackState.dash;
                }
            }

            if(nowState == AttackState.dash)
            {
                if(lookTimer < 1f)
                {
                    //돌아보기
                    Look();
                }
                lookTimer += Time.deltaTime;
                //공격
                Atk_Dash(2f);
                return;
            }

            //달리기, 원거리 범위보다 먼 경우
            if(Vector3.Distance(this.transform.position, player.transform.position) > FarAtkDistance)
            {
                if(nowState == AttackState.none)
                {
                    //돌아보기
                    Look();
                    //이동(스피드 업)
                    Move(1.5f);
                    return;
                }
            }
        }

        //피격 함수
        void Damaged()
        {
            //피격 정보, 애니메이션
            //GameObject.Find("...").GetComponent<playerAttack>().damage
        }

        void Look()
        {
            //플레이어를 향해
            lookVector = player.transform.position - this.transform.position;
            this.gameObject.transform.rotation = Quaternion.LookRotation(lookVector);
           //this.gameObject.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.LookRotation(look), Time.deltaTime * 0.5f);
           //Debug.Log("Look for player");
        }
        void Move(float speedRate)
        {   
            gameObject.GetComponent<Animator>().SetFloat("walkSpeed", speedRate);
            gameObject.GetComponent<Animator>().SetBool("isWalking", true);
            lookVector = player.transform.position - this.transform.position;
            //speed 처리
            this.transform.position += lookVector.normalized * (speedRate * 10f) * Time.deltaTime;
            //Debug.Log("Move to player");
        }
        void Atk_BigAttack(float delaytime)
        {
            gameObject.GetComponent<Animator>().SetBool("isWalking", false);
            //gameObject.GetComponent<Animator>().SetTrigger("isAttacking");
            isAnimating = true;
            BigAttack = false;
            //Debug.Log("Big Attack!");
            StartCoroutine(DelayTimer(delaytime));
        }
        //근거리 저격 공격(충격파)
        void Atk_Wave(float delaytime)
        {
            if(isAnimating == false)
            {
                isAnimating = true;
                gameObject.GetComponent<Animator>().SetBool("isWalking", false);
                gameObject.GetComponent<Animator>().SetBool("isAttacking", true);
                gameObject.GetComponent<Animator>().SetTrigger("attack_wave");
                StartCoroutine(DelayTimer(delaytime));
                StartCoroutine(Hitbox(delaytime - 0.5f, 0.2f, 5));
            }
            //Debug.Log("Wave Attack!");
            
        }
        //원거리 저격 공격(돌진)
        void Atk_Dash(float delaytime)
        {
            if(isAnimating == false)
            {
                isAnimating = true;
                gameObject.GetComponent<Animator>().SetBool("isWalking", false);
                gameObject.GetComponent<Animator>().SetBool("isAttacking", true);
                gameObject.GetComponent<Animator>().SetTrigger("attack_dash");
                StartCoroutine(DelayTimer(delaytime));
                StartCoroutine(Hitbox(delaytime - 1f, 0.6f, 6));
            }
            else
            {
                if(dashAttackTimer <= 0f)
                {
                    //Vector3 look = player.transform.position - this.transform.position;
                    this.transform.position += lookVector.normalized * (20f) * Time.deltaTime;
                }
                dashAttackTimer -= Time.deltaTime;
                //this.gameObject.GetComponent<Rigidbody>().AddForce(look.normalized*10f, ForceMode.VelocityChange);
                //isAnimating = true;
                //Debug.Log("Dash Attack!");
            }

        }
        
        IEnumerator DelayTimer(float time)
        {
            yield return new WaitForSeconds(time);
            nowState = AttackState.none;
            isAnimating = false;
            gameObject.GetComponent<Animator>().SetBool("isAttacking", false);
            dashAttackTimer = 0.8f;
            lookTimer = 0f;
            //this.gameObject.GetComponent<Rigidbody>()
        }
        IEnumerator Hitbox(float starttime, float endtime, int childnum)
        {
            yield return new WaitForSeconds(starttime);
            //hitbox
            this.transform.GetChild(childnum).gameObject.GetComponent<CapsuleCollider>().enabled = true;
            yield return new WaitForSeconds(endtime);
            this.transform.GetChild(childnum).gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }

        IEnumerator AttackedDelayTimer(float time)
        {
            yield return new WaitForSeconds(time);
            nowState = AttackState.none;
            //gameObject.GetComponent<Animator>().SetBool("attacked", false);

        }
        IEnumerator DeadDelayTimer(float time)
        {
            yield return new WaitForSeconds(time);
            nowState = AttackState.none;
            Destroy(this.gameObject);
        }
    }
}

