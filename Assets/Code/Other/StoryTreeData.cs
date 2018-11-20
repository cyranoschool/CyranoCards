using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
//Tree needs to get around artificial serialization depth limit
//In order to avoid this limit a list of serialized nodes must be used: https://docs.unity3d.com/Manual/script-Serialization-Custom.html
public class StoryTreeData : ISerializationCallbackReceiver
{
    // The root node used for runtime tree representation. Not serialized.
    StoryNode root = new StoryNode();
    public StoryNode Root { get { return root; } }
    // This is the field we give Unity to serialize.
    public List<StorySerializableNode> serializedNodes;
    public void OnBeforeSerialize()
    {
        // Unity is about to read the serializedNodes field's contents.
        // The correct data must now be written into that field "just in time".
        if (serializedNodes == null) serializedNodes = new List<StorySerializableNode>();
        if (root == null) root = new StoryNode();
        serializedNodes.Clear();
        AddNodeToSerializedNodes(root);
        // Now Unity is free to serialize this field, and we should get back the expected 
        // data when it is deserialized later.
    }
    //Recursive function
    void AddNodeToSerializedNodes(StoryNode n)
    {
        var serializedNode = new StorySerializableNode()
        {
            nodeData = n.nodeData,
            childCount = n.children.Count,
            indexOfFirstChild = serializedNodes.Count + 1
        };
        serializedNodes.Add(serializedNode);
        foreach (var child in n.children)
        {
            AddNodeToSerializedNodes(child);
        }
    }
    public void OnAfterDeserialize()
    {
        //Unity has just written new data into the serializedNodes field.
        //let's populate our actual runtime data with those new values.
        if (serializedNodes.Count > 0)
        {
            ReadNodeFromSerializedNodes(0, out root);
        }
        else
        {
            root = new StoryNode();
        }
    }
    int ReadNodeFromSerializedNodes(int index, out StoryNode node)
    {
        var serializedNode = serializedNodes[index];
        // Transfer the deserialized data into the internal Node class
        StoryNode newNode = new StoryNode()
        {
            nodeData = serializedNode.nodeData,
            children = new List<StoryNode>()
        }
        ;
        // The tree needs to be read in depth-first, since that's how we wrote it out.
        for (int i = 0; i < serializedNode.childCount; i++)
        {
            StoryNode childNode;
            index = ReadNodeFromSerializedNodes(++index, out childNode);
            newNode.children.Add(childNode);
        }
        node = newNode;
        return index;
    }
}

//The serialized and nonserialized story nodes should have the same serialized data class
// Node class that is used at runtime.
// This is internal to the BehaviourWithTree class and is not serialized.
//[NonSerialized]
public class StoryNode
{
    public StoryNodeData nodeData;
    public List<StoryNode> children = new List<StoryNode>();
}

// Node class that we will use for serialization.
[Serializable]
public struct StorySerializableNode
{
    public StoryNodeData nodeData;
    public int childCount;
    public int indexOfFirstChild;
}

[Serializable]
public class StoryNodeData
{
    public string CardUID = "";
    public string From = "";
    public string To = "";
}