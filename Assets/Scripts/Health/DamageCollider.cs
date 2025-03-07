﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRJammies.Framework.Core.Health
{
    public class DamageCollider : MonoBehaviour 
    {
        // How much damage to apply to the Damageable object
        public int Damage = 1;

        // What type of damage to apply. 
        [SerializeField]
        protected DamageForm _damageForm;

        // Used to define the collider which can deal damage
        public Collider DamagingCollider;

        // Used to determine velocity of this collider
        public Rigidbody ColliderRigidbody;

        // Minimum Amount of force necessary to do damage. Expressed as relativeVelocity.magnitude
        protected float _minForce = 0f;

        // Our previous frame's last relative velocity value
        //private float _lastRelativeVelocity = 0;

        // How much impulse force was applied last onCollision enter
        public float _lastDamageForce = 0;

        // Destroy this object after dealing damage
        [SerializeField]
        protected bool _destroyOnDamaging = true;

        protected GameObject CraftingSocketGO;
        [SerializeField]
        protected Material CraftingSocketMat;


        protected virtual void Start() 
        {
            // Initiliaze values and null checks

            if (ColliderRigidbody == null) {
                ColliderRigidbody = transform.root.GetComponentInChildren<Rigidbody>();
            }

            if (_damageForm == DamageForm.NoType)
            {
                Debug.Log("No Damage Type or Tier for this collider determined: "+this.name);
                Damage = 0;
            }

            if (DamagingCollider == null) 
            {
                Debug.Log($"No specific Damage collider defined on {gameObject.name}, will try to find one on this as default");
                DamagingCollider = GetComponentInChildren<Collider>();
            }
        }

        // Call the custom public collision event in case this object can have collision events
        protected virtual void OnCollisionEnter(Collision collision) 
        {
            if (!this.isActiveAndEnabled) {
                return;
            }

            OnCollisionEvent(collision);
        }

        // Public event to pass collision data from the parent to his script
        public virtual void OnCollisionEvent(Collision collision) {
            _lastDamageForce = collision.impulse.magnitude;
            //_lastRelativeVelocity = collision.relativeVelocity.magnitude;

            if (_lastDamageForce >= _minForce) {    
                // Can we damage what we hit?
                Damageable d = collision.GetContact(0).otherCollider.gameObject.GetComponentInParent<Damageable>();
                //Damageable d = collision.collider.transform.root.GetComponentInChildren<Damageable>();
                if (d && collision.GetContact(0).thisCollider == DamagingCollider) {

                    d.DealDamage(Damage, _damageForm, this);
                }
            }
        }

        public void OnDestroyThis()
        {
            ReactivateCraftingSocket();
            Destroy(this.gameObject);
        }

        public void SetCraftingSocket(GameObject craftingSocket) 
        {
            CraftingSocketGO = craftingSocket;
        }

        public void SetDamageType(DamageForm damageForm) 
        {
            _damageForm = damageForm;
        }

        protected void ReactivateCraftingSocket()
        {
            if (CraftingSocketGO)
            {
                CraftingSocketGO.GetComponent<XRSocketInteractor>().socketActive = false;
                CraftingSocketGO.SetActive(true);
            }
            else Debug.Log(name+" has no crafting socket assigned to reactivate");

            if (CraftingSocketMat) 
            {
                CraftingSocketGO.GetComponent<MeshRenderer>().material = CraftingSocketMat;
            }
        }

        public bool ShouldDestroy() 
        {
            return _destroyOnDamaging;
        }
    }
}