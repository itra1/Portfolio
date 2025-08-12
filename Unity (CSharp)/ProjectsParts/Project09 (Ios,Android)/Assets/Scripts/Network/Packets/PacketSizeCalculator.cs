using UnityEngine;

public class PacketSizeCalculator : NetOutputStream {
  private int packetSize;

  public int PacketSize {
    get { return packetSize; }
  }

  public void WriteC(byte value) {
    packetSize += 1;
  }

  /// <summary>
  /// Int32
  /// </summary>
  /// <param name="value">int</param>
  public void WriteD(int value) {
    packetSize += 4;
  }

  /// <summary>
  /// Int16
  /// </summary>
  /// <param name="value">short</param>
  public void WriteH(short value) {
    packetSize += 2;
  }

  /// <summary>
  /// Int64
  /// </summary>
  /// <param name="value">long</param>
  public void WriteQ(long value) {
    packetSize += 8;
  }

  /// <summary>
  /// String
  /// </summary>
  /// <param name="value">string</param>
  public void WriteUTF8(string value) {
    byte[] str = System.Text.UnicodeEncoding.UTF8.GetBytes(value);
    packetSize += str.Length + 2;
  }

  /// <summary>
  /// byte array
  /// </summary>
  /// <param name="value">byte[]</param>
  public void writeB(byte[] value) {
    packetSize += value.Length;
  }

  public void WriteASCII(string value) {
    packetSize += value.Length + 1;
  }

  /// <summary>
  /// String
  /// </summary>
  /// <param name="value">string</param>
  public void WriteUtf16(string value) {
    packetSize += value.Length * 2 + 2;
  }

  public void WriteBool(bool value) {
    packetSize += 1;
  }

  public void WriteVec2(Vector2 vec) {
    packetSize += 8;
  }

  public void WriteVec3(Vector3 vec) {
    packetSize += 12;
  }

  public void WriteF(float _value) {
    packetSize += 4;
  }

  public void WriteColor(Color32 value) {
    packetSize += 3;
  }
}
