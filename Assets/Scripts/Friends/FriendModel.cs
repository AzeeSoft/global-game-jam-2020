﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendModel : MonoBehaviour
{
    public NavMeshAgent navMeshAgent { get; private set; }
    public StateMachine<FriendModel> stateMachine;

    public IdleState.StateData idleStateData = new IdleState.StateData();
    public WanderingState.StateData wanderingStateData = new WanderingState.StateData();

    public float minVelocityToGetHit;

    public GameObject avatar;
    public Animator animator;
    public GameObject ragdollPrefab;

    public bool isHidden => gameObject.activeInHierarchy;
    public FriendRagdoll friendRagdoll = null;

    public bool enableRagdolling = true;

    private Dictionary<Renderer, Material[]> savedShaders = new Dictionary<Renderer, Material[]>();

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        stateMachine = new StateMachine<FriendModel>(this);
        stateMachine.SwitchState(IdleState.Instance);

        TaskManager.Instance.hideFriendsTask.AddFriend(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveShaders();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    void SaveShaders()
    {
        foreach (var component in GetComponentsInChildren<Renderer>())
        {
            savedShaders[component] = component.materials;
        }
    }

    void RestoreShaders()
    {
        foreach (var component in GetComponentsInChildren<Renderer>())
        {
            component.materials = savedShaders[component];
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!enableRagdolling)
        {
            return;
        }

        print(other.gameObject);

        if (other.rigidbody)
        {
            var pickable = other.rigidbody.GetComponent<Pickable>();
            if (pickable && other.rigidbody.velocity.magnitude >= minVelocityToGetHit)
            {
                print(other.rigidbody.velocity);

                // Gets hit
                // Instantiate ragdoll
                friendRagdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation)
                    .GetComponentInChildren<FriendRagdoll>();

                friendRagdoll.friendModelToRevive = this;

                gameObject.SetActive(false);
            }
        }
    }

    public void Revive()
    {
        friendRagdoll = null;

        gameObject.SetActive(true);
        stateMachine.SwitchState(IdleState.Instance);

        if (Random.Range(0f, 1f) < TaskManager.Instance.hideFriendsTask.godModeProbability)
        {
            GodMode();
        }
        else
        {
            RestoreShaders();
        }
    }

    void GodMode()
    {
        foreach (var component in GetComponentsInChildren<Renderer>())
        {
            var mats = new Material[savedShaders[component].Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = TaskManager.Instance.hideFriendsTask.fresnel;
            }

            component.materials = mats;
        }
    }
}