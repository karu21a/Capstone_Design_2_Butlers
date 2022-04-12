using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace K.Monster
{
    public class M_Rat : MonoBehaviour
    {
        NormalMonster data;
        Rigidbody rigid;
        Transform pinPos;
        float wanderTime;
        [SerializeField]
        float wanderRate;
        [SerializeField]
        float wanderForce;
        GameObject player;


        void Awake()
        {
            //파라미터 부분은 파서 완성되는 대로 수정
            data = new NormalMonster(1f, 400f, MonsterBase.type.normal);
        }

        void Start()
        {
            rigid = this.GetComponent<Rigidbody>();
            pinPos = this.transform.parent.transform;
            player = GameObject.FindWithTag("Player");
        }

        void Update()
        {
            if(wanderTime < wanderRate)
            {
                wanderTime += Time.deltaTime;
            }
            if(wanderTime >= wanderRate)
            {
                //Debug.Log("호출");
                //플레이어가 시야에 들어오면 쫒음
                float dotValue = Mathf.Cos(Mathf.Deg2Rad * (180/2));
                Vector3 dic = player.transform.position - this.transform.position;

                if(dic.magnitude < 20)
                {
                    if(Vector3.Dot(dic.normalized, this.transform.forward) > dotValue)
                    {
                        attack();
                        Debug.Log(this.gameObject.name + "col");
                        wanderTime = 0f;
                        if(wanderRate >= 1.5f)
                            wanderRate += Random.Range(0f, 0.5f);
                        if(wanderRate > 2f)
                            wanderRate = 1.5f;
                        return;
                    }
                    else
                        wander();
                }
                else
                {
                    wander();
                }

                wanderTime = 0f;
                if(wanderRate >= 1.5f)
                    wanderRate += Random.Range(0f, 0.4f);
                if(wanderRate > 3f)
                    wanderRate = 2f;
            }

        }

        void FixedUpdate()
        {

        }

        void attack()
        {
            Vector3 mtop = player.transform.position - this.transform.position;
            Vector3 dis = pinPos.position - this.transform.position;
            //경계 영역
            if(dis.magnitude >= 30f)
            {
                rigid.AddForce(new Vector3(dis.x*3, 0f, dis.z*3), ForceMode.Impulse);
                this.transform.rotation = Quaternion.LookRotation(dis.normalized);
            }
            if(dis.magnitude < 30f)
            {
                //rigid.AddForce((mtop.normalized)*40f, ForceMode.Impulse);
                StartCoroutine(AttackToPlayer(mtop));
                this.transform.rotation = Quaternion.LookRotation(mtop.normalized);
                
            }
        }
        IEnumerator AttackToPlayer(Vector3 mtop)
        {
            yield return new WaitForSeconds(0.2f);
            rigid.AddForce((mtop.normalized)*40f, ForceMode.Impulse);
        }
        void wander()
        {
            //배회할 중심 포지션 지정(빈오브젝트 등으로 핀 지정)
            //핀>몬스터로 계층구조로 관리
            //몬스터에서 핀까지의 벡터
            Vector3 dis = pinPos.position - this.transform.position;
            //Debug.Log("ch: "+this.transform.position);
            //Debug.Log("pin: "+pinPos.position);
            //Debug.Log(dis.magnitude);
            if(dis.magnitude >= 15f-4f)
            {
                rigid.AddForce(new Vector3(dis.x*3, 0f, dis.z*3), ForceMode.Impulse);
                this.transform.rotation = Quaternion.LookRotation(dis.normalized);
            }
            if(dis.magnitude < 15f-4f)
            {
                Vector3 v = new Vector3(rand(5f), 0f, rand(5f));
                rigid.AddForce(v, ForceMode.Impulse);
                this.transform.rotation = Quaternion.LookRotation(v.normalized);
                
            }
        }

        float rand(float weight)
        {
            int MorP = Random.Range(0, 2);
            if(MorP  == 0)
                return Random.Range(-5f *weight, -3f*weight);
            if(MorP == 1)
                return Random.Range(3f*weight, 5f*weight);

            Debug.Log("M_Cheese/rand(): MorP is not 0 or 1");
            return 0f;
        }
    }
}

