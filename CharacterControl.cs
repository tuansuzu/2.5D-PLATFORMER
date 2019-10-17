﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace roundbeargames_tutorial
{
    public enum TransitionParameter
    {
        Move,
        Jump,
        ForceTransition,
        Grounded,
        Attack,
    }

    public class CharacterControl : MonoBehaviour
    {

        public Animator SkinnedMeshAnimator;
        public bool MoveRight;
        public bool MoveLeft;
        public bool Jump;
        public bool Attack;

        public GameObject ColliderEdgePrefab;
        public List<GameObject> BottomSpheres = new List<GameObject>();
        public List<GameObject> FrontSpheres = new List<GameObject>();
        public List<Collider> RagdollParts = new List<Collider>();
        public List<Collider> CollidingParts = new List<Collider>();

        public float GravityMultiplier;
        public float PullMultiplier;

        private Rigidbody rigid;
        public Rigidbody RIGID_BODY
        {
            get
            {
                if (rigid == null)
                {
                    rigid = GetComponent<Rigidbody>();
                }
                return rigid;
            }
        }
        public Material material;

        public void ChangeMaterial()
        {
            if (material == null)
            {
                Debug.LogError("No material specified");
            }

            Renderer[] arrMaterials = this.gameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer r in arrMaterials)
            {
                if (r.gameObject != this.gameObject)
                {
                    r.material = material;
                }
            }
        }

        private void Awake()
        {
            bool SwitchBack = false;

            if (!IsFacingForward())
            {
                SwitchBack = true;
            }

            FaceForward(true);
            SetRagdollParts();
            SetColliderSpheres();

            if (SwitchBack)
            {
                FaceForward(false);
            }
        }

        /*
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(5f);
            RIGID_BODY.AddForce(200f * Vector3.up);
            yield return new WaitForSeconds(0.5f);
            TurnOnRagdoll();
        }
        */

        private void OnTriggerEnter(Collider col)
        {
            if (RagdollParts.Contains(col))
            {
                return;
            }

            CharacterControl control = col.transform.root.GetComponent<CharacterControl>();

            if (control == null)
            {
                return;
            }

            if (col.gameObject == control.gameObject)
            {
                return;
            }

            if (!CollidingParts.Contains(col))
            {
                CollidingParts.Add(col);
            }
        }

        private void OnTriggerExit(Collider col)
        {
            if (CollidingParts.Contains(col))
            {
                CollidingParts.Remove(col);
            }
        }

        private void SetRagdollParts()
        {
            Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

            foreach (Collider c in colliders)
            {
                if (c.gameObject != this.gameObject)
                {
                    c.isTrigger = true;
                    RagdollParts.Add(c);
                }
               
            }
        }

        public void TurnOnRagdoll()
        {
            RIGID_BODY.useGravity = false;
            RIGID_BODY.velocity = Vector3.zero;      
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            SkinnedMeshAnimator.enabled = false;
            SkinnedMeshAnimator.avatar = null;

            foreach (Collider c in RagdollParts)
            {
                c.isTrigger = false;
                c.attachedRigidbody.velocity = Vector3.zero;
            }
        }

        private void SetColliderSpheres()
        {
            BoxCollider box = GetComponent<BoxCollider>();

            float bottom = box.bounds.center.y - box.bounds.extents.y;
            float top = box.bounds.center.y + box.bounds.extents.y;
            float front = box.bounds.center.z + box.bounds.extents.z;
            float back = box.bounds.center.z - box.bounds.extents.z;

            GameObject bottomFront = CreateEdgeSphere(new Vector3(0f, bottom, front));
            GameObject bottomBack = CreateEdgeSphere(new Vector3(0f, bottom, back));
            GameObject topFront = CreateEdgeSphere(new Vector3(0f, top, front));

            bottomFront.transform.parent = this.transform;
            bottomBack.transform.parent = this.transform;
            topFront.transform.parent = this.transform;

            BottomSpheres.Add(bottomFront);
            BottomSpheres.Add(bottomBack);

            FrontSpheres.Add(bottomFront);
            FrontSpheres.Add(topFront);

            float horSec = (bottomFront.transform.position - bottomBack.transform.position).magnitude / 5f;
            CreateMiddleSpheres(bottomFront, -this.transform.forward, horSec, 4, BottomSpheres);

            float verSec = (bottomFront.transform.position - topFront.transform.position).magnitude / 10f;
            CreateMiddleSpheres(bottomFront, this.transform.up, verSec, 9, FrontSpheres);
        }

        private void FixedUpdate()
        {
            if (RIGID_BODY.velocity.y < 0f)
            {
                RIGID_BODY.velocity += (-Vector3.up * GravityMultiplier);
            }

            if (RIGID_BODY.velocity.y > 0f && !Jump)
            {
                RIGID_BODY.velocity += (-Vector3.up * PullMultiplier);
            }
        }

        public void CreateMiddleSpheres(GameObject start, Vector3 dir, float sec, int interations, List<GameObject> spheresList)
        {
            for (int i = 0; i < interations; i++)
            {
                Vector3 pos = start.transform.position + (dir * sec * (i + 1));

                GameObject newObj = CreateEdgeSphere(pos);
                newObj.transform.parent = this.transform;
                spheresList.Add(newObj);
            }
        }

        public GameObject CreateEdgeSphere(Vector3 pos)
        {
            GameObject obj = Instantiate(ColliderEdgePrefab, pos, Quaternion.identity);
            return obj;
        }

        public void MoveForward(float Speed, float SpeedGraph)
        {
            transform.Translate(Vector3.forward * Speed * SpeedGraph * Time.deltaTime);
        }

        public void FaceForward(bool forward)
        {
            if (forward)
            {
                transform.rotation = Quaternion.Euler(0f,0f,0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f,180f,0f);
            }
        }

        public bool IsFacingForward()
        {
            Debug.Log(gameObject.name + " : "+ transform.forward);
            if (transform.forward.z > 0f)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}