using UnityEngine;
using System.Runtime.Serialization;

public class Vector2Surrogate : ISerializationSurrogate
{
    #region ISerializationSurrogate implementation

    /// <summary>
    /// Serialize a unity component into binary data
    /// </summary>
    /// <param name="obj">Object to be serialized</param>
    /// <param name="info">Data needed to serialze the object</param>
    /// <param name="context">Goves information regarding locations of the data</param>
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector2 v = (Vector2)obj;
        info.AddValue("v", new float[] { v.x, v.y}, typeof(float[]));
    }

    /// <summary>
    /// Create object out of the binary data
    /// </summary>
    /// <param name="obj">Object to be serialized</param>
    /// <param name="info">Data needed to serialze the object</param>
    /// <param name="context">Gives information regarding locations of the data</param>
    /// <param name="selector">Gives the method the proper serializer to utilize</param>
    /// <returns></returns>
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        float[] values = (float[])info.GetValue("v", typeof(float[]));
        return new Vector2(values[0], values[1]);
    }

    #endregion
}
