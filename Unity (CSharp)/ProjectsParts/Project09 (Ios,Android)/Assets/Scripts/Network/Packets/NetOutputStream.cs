using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface NetOutputStream
{
    
    void WriteC(byte value); //Byte
    
    /// <summary>
    /// Int32
    /// </summary>
    /// <param name="value">int</param>
    void WriteD(int value); //Int32
    
    /// <summary>
    /// Int16
    /// </summary>
    /// <param name="value">short</param>
    void WriteH(short value); //Int16
    
    /// <summary>
    /// Int64
    /// </summary>
    /// <param name="value">long</param>
    void WriteQ(long value);  //Int64
    
    /// <summary>
    /// String
    /// </summary>
    /// <param name="value">string</param>
    void WriteUTF8(string value); //String
    
    /// <summary>
    /// byte array
    /// </summary>
    /// <param name="value">byte[]</param>
    void writeB(byte[] value); //byte[]

    void WriteASCII(string value);

    void WriteBool(bool value);

    void WriteVec2(Vector2 vec);
    void WriteColor(Color32 vec);

    void WriteVec3(Vector3 vec);

    void WriteF(float _value);
}
