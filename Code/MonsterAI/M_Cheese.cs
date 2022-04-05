using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace K.Monster
{
    public class M_Cheese : MonoBehaviour
    {
        NormalMonster data;
        Rigidbody rigid;
        Transform pinPos;
        float springTime;
        [SerializeField]
        float springRate;
        [SerializeField]
        float springForce;
        void Awake()
        {
            //파라미터 부분은 파서 완성되는 대로 수정
            data = new NormalMonster(1f, 400f, MonsterBase.type.normal);
        }

        void Start()
        {
            rigid = this.GetComponent<Rigidbody>();
            pinPos = this.transform.parent.transform;
        }

        void Update()
        {
            if(springTime < springRate)
            {
                springTime += Time.deltaTime;
            }
            if(springTime >= springRate)
            {
                Debug.Log("호출");
                rigid.AddForce(new Vector3(0f, springForce, 0f), ForceMode.Impulse);
                wander();
                springTime = 0f;
                if(springRate >= 2f)
                    springRate += Random.Range(0f, 0.4f);
                if(springRate > 3f)
                    springRate = 2f;
            }

        }

        void FixedUpdate()
        {

        }

        void wander()
        {
            //배회할 중심 포지션 지정(빈오브젝트 등으로 핀 지정)
            //핀>몬스터로 계층구조로 관리
            //몬스터에서 핀까지의 벡터
            Vector3 dis = pinPos.position - this.transform.position;
            Debug.Log("ch: "+this.transform.position);
            Debug.Log("pin: "+pinPos.position);
            Debug.Log(dis.magnitude);
            if(dis.magnitude >= 15f-4f)
            {
                rigid.AddForce(new Vector3(dis.x/2, 0f, dis.z/2), ForceMode.Impulse);
            }
            if(dis.magnitude < 15f-4f)
            {
                rigid.AddForce(new Vector3(rand(), 0f, rand()), ForceMode.Impulse);
            }
        }

        float rand()
        {
            int MorP = Random.Range(0, 2);
            if(MorP  == 0)
                return Random.Range(-5f, -3f);
            if(MorP == 1)
                return Random.Range(3f, 5f);

            Debug.Log("M_Cheese/rand(): MorP is not 0 or 1");
            return 0f;
        }
    }
}

