using UnityEngine;
using System.Runtime.Serialization;

public class QuaternionSurrogate : ISerializationSurrogate
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
        Quaternion v = (Quaternion)obj;
        info.AddValue("v", new float[] { v.x, v.y, v.z, v.w }, typeof(float[]));
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
        return new Quaternion(values[0], values[1], values[2], values[3]);
    }

    #endregion
}
