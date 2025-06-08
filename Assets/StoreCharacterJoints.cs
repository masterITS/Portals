using System;
using UnityEngine;
using System.Collections.Generic;

public class StoreCharacterJoints : MonoBehaviour
{
    private CharacterJoint[] _characterJoints;
    private List<GameObject> _jointGameObjects = new List<GameObject>();
    private List<Rigidbody> _initRigidBody = new List<Rigidbody>();

    private void Start()
    {
        // Scan all child GameObjects and store references to their CharacterJoint components
        _characterJoints = gameObject.GetComponentsInChildren<CharacterJoint>();

        foreach (var joint in _characterJoints)
        {
            _jointGameObjects.Add(joint.gameObject);
            _initRigidBody.Add(joint.connectedBody);
        }
    }

    // Optional: Method to get the stored CharacterJoint references
    public void ChangeCharacterActiveState(bool integer)
    {
        if (integer)
        {
            foreach (var characterJoint in _characterJoints) Destroy(characterJoint);
        }
        else
        {
            ConnectToParentRigidbody();
        }
    }

    private void ConnectToParentRigidbody()
    {
        for (int i = 0; i < _jointGameObjects.Count; i++)
        {
            var listOfChar = _jointGameObjects[i].AddComponent<CharacterJoint>();
            listOfChar.connectedBody = _initRigidBody[i];
        }
        _characterJoints = gameObject.GetComponentsInChildren<CharacterJoint>();
    }

}