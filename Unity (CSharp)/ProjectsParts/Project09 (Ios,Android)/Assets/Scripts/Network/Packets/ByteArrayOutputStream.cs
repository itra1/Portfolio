using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ByteArrayOutputStream : NetOutputStream {
  private byte[] data;

  private int offset;

  public ByteArrayOutputStream(int size) {
    data = new byte[size + 2];
    offset = 2;
  }

  public byte[] GetData() {
    return data;
  }

  #region ===== === == = Запись данных в пакет = == === =====

  /// <summary>
  /// Byte
  /// </summary>
  /// <param name="value">byte</param>
  public void WriteC(byte value) {
    data[offset] = value;
    offset += 1;
  }

  /// <summary>
  /// Int32
  /// </summary>
  /// <param name="value">int</param>
  public void WriteD(int value) {
    System.Array.Copy(System.BitConverter.GetBytes(value), 0, data, offset, 4);
    offset += 4;
  }
  /// <summary>
  /// Int16
  /// </summary>
  /// <param name="value">short</param>
  public void WriteH(short value) {
    System.Array.Copy(System.BitConverter.GetBytes(value), 0, data, offset, 2);
    offset += 2;
  }
  /// <summary>
  /// Int64
  /// </summary>
  /// <param name="value">long</param>
  public void WriteQ(long value) {
    System.Array.Copy(System.BitConverter.GetBytes(value), 0, data, offset, 8);
    offset += 8;
  }

  /// <summary>
  /// String
  /// </summary>
  /// <param name="value">string</param>
  public void WriteUTF8(string value) {
    byte[] str = System.Text.UnicodeEncoding.UTF8.GetBytes(value);
    WriteH((short)str.Length);
    System.Array.Copy(str, 0, data, offset, str.Length);
    offset += str.Length;
  }

  /// <summary>
  /// String в формате ASCII
  /// </summary>
  /// <param name="value"></param>
  public void WriteASCII(string value) //String
  {
    byte[] str = System.Text.ASCIIEncoding.UTF8.GetBytes(value);
    System.Array.Copy(str, 0, data, offset, str.Length);
    offset += str.Length + 1;
  }

  /// <summary>
  /// byte array
  /// </summary>
  /// <param name="value">byte[]</param>
  public void writeB(byte[] value) //byte[]
  {
    System.Array.Copy(value, 0, data, offset, value.Length);
    offset += value.Length;
  }

  public void WriteBool(bool value) {
    WriteC((byte)(value ? 1 : 0));
  }

  public void WriteVec2(Vector2 vec) {
    WriteF(vec.x);
    WriteF(vec.y);
  }

  public void WriteVec3(Vector3 vec) {
    WriteF(vec.x);
    WriteF(vec.y);
    WriteF(vec.z);
  }

  public void WriteF(float _value) {
    System.Array.Copy(System.BitConverter.GetBytes(_value), 0, data, offset, 4);
    offset += 4;
  }

  public void WriteColor(Color32 color) {
    WriteC(color.r);
    WriteC(color.g);
    WriteC(color.b);
  }
  #endregion

}

