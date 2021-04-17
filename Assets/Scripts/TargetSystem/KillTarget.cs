﻿using System.Collections;
using People;
using People.NPC;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.AI;

namespace TargetSystem
{
    public class KillTarget : MonoBehaviour
    {
        // Start is called before the first frame update
        private SelectedTarget _selectedTarget;
        private Animator _animator;

        private bool _killClick;
        private CastTarget _casttarget;

        [SerializeField] private Material dissolved;

        void Start()
        {
            if (PhotonNetwork.IsConnected && !gameObject.GetPhotonView().IsMine)
                enabled = false;
            _animator = GetComponent<Animator>();
            _selectedTarget = GetComponent<SelectedTarget>();
            _casttarget = GetComponent<CastTarget>();
        }

        /// <summary>
        /// Méthode appelée quand un joueur tue la cible verouillée 
        /// </summary>
        /// <param name="target"></param>
        void Kill(GameObject target)
        {
            target.GetComponent<Human.Human>().Death();
            _casttarget.SetAiming(false);
            Debug.Log($"killed {target.name}");

            _animator.Play("sword kill");
            _selectedTarget.UpdateSelectedTarget(target, target.GetComponentInChildren<Outline>());

            // StartCoroutine(WaitForDeathAnim(target));
        }

        IEnumerator WaitForDeathAnim(GameObject target)
        {
            yield return new WaitForSeconds(5);
            SkinnedMeshRenderer meshRenderer = target.transform.GetComponentInChildren<SkinnedMeshRenderer>();
            Texture oldTexture = meshRenderer.sharedMaterial.mainTexture;
            meshRenderer.sharedMaterial = dissolved;
            meshRenderer.sharedMaterial.SetTexture("Texture2D_C902C618", oldTexture);
            meshRenderer.sharedMaterial.SetFloat("Vector1_203537A2", Time.time);


            float timeElapsed = 0f;
            float phase = 3;
            float targetPhase = 5.5f;
            while (timeElapsed <= 3)
            {
                timeElapsed += Time.deltaTime;
                meshRenderer.sharedMaterial.SetFloat("Vector1_203537A2",
                    Mathf.Lerp(phase, targetPhase, timeElapsed / 3));
                yield return new WaitForEndOfFrame();
            }

            Destroy(target);
        }

        public void OnAttack(InputValue value)
        {
            if (value.isPressed)
            {
                GameObject target = _selectedTarget.GetTarget();
                // Si le joueur est à moins d'un mètre et demi.
                if (target != null && Vector3.Distance(transform.position, target.transform.position) < 1.5f)
                    Kill(target);
            }
        }
    }
}