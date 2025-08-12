/* Packet.cs - базовый класс для всех пакетов. Определяет методы чтения разных типов данных из byte[], их запись, а также в соответсвии с ID пакета создаёт необходимого наследника, 
 * реализующего действия с данными и взаимодействуюещего с уровнем логики игры. Использован паттерн проектирования Полиморфизм (Polymorphism).
 * В методе CreatingPackage создаётся экземпляр класса-наследника Packet, принимающего byte[] данные для работы с ними. Тип класса-наследника определяется по словарю Constants.PacketID.  
 * Метод Dispose и наследование интерфейса IDisposable обусловлено тем, что NetworkManager всегда знает когда пакет ему больше не нужен.
 * Все классы-наследники (in_TypePacket, out_TypePacket) должны иметь параметрический конструктор, принимающий byte[], в котором переопределит ReciveData базового класса и сбросит значение Offset.
 * Метод GetData() возвращает byte[] для кодирования и отправки серверу, в этот byte[] добавление данных идёт методами Write.
 * */
using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Packet : IDisposable {
  /// <summary>
  /// Полученные данные
  /// </summary>
  public byte[] ReciveData;
  /// <summary>
  /// Offset
  /// </summary>
  public int Offset;
  //Для хранения byte[] на отправку
  private List<byte[]> heap = new List<byte[]>();
  /// <summary>
  /// Процесс обработки данных
  /// </summary>
  public virtual void Process() { isProcessedOverrided = false; }
  /// <summary>
  /// Процесс чтения данных из пакета
  /// </summary>
  public virtual void ReadImpl() { }

  private bool isProcessedOverrided = true;

  /// <summary>
  /// Базовый приоритет пакета для обработки клиентом
  /// </summary>
  public PacketPriority BasePacketPriority = PacketPriority.AlwaysTreated;

  public Packet() { }

  public enum PacketPriority : byte {
    SceneInitialize,
    AlwaysTreated,
    SyncAnimation,
    SyncMovement
  }

  public void Dispose() {
    // Debug.LogError("Packet Dispose");
    ReciveData = null;
    Offset = 0;
    heap = null;
  }

  public void setData(byte[] data, int offset) {
    ReciveData = data;
    Offset = offset;
  }

  /// <summary>
  /// Получение byte[] для кодирования и отправки
  /// </summary>
  /// <returns></returns>
  public byte[] GetData() {
    int size = 0;
    foreach(byte[] bin in heap) {
      size += bin.Length;
    }
    byte[] data = new byte[size + 2];
    size = 2;   // reserve for packet size
    foreach(byte[] bin in heap) {
      bin.CopyTo(data, size);
      size += bin.Length;
    }
    return data;
  }

  #region ===== === == = Чтение данных из пакета = == === =====

  /// <summary>
  /// Byte
  /// </summary>
  /// <returns>byte</returns>
  public byte ReadC() //Byte
  {
    byte result = Convert.ToByte(ReciveData[Offset++] & 0xff);
    return result;
  }
  /// <summary>
  /// Int16
  /// </summary>
  /// <returns>short</returns>
  public short ReadH() //Int16
  {
    return System.BitConverter.ToInt16(ReciveData, (Offset += 2) - 2);
  }

  public float ReadHFloat() //Int16
  {
    return ReadH() / 50f;
  }
  /// <summary>
  /// Int32
  /// </summary>
  /// <returns>int</returns>
  public int ReadD() //Int32
  {
    return System.BitConverter.ToInt32(ReciveData, (Offset += 4) - 4);
  }
  /// <summary>
  /// Int64
  /// </summary>
  /// <returns>long</returns>
  public long ReadQ() //Int64
  {
    return System.BitConverter.ToInt64(ReciveData, (Offset += 8) - 8);
  }

  /// <summary>
  /// float
  /// </summary>
  /// <returns>float</returns>
  public float ReadF() //Single
  {
    float retval = System.BitConverter.ToSingle(ReciveData, (Offset += 4) - 4);
    if(float.IsNaN(retval)) {
      Debug.LogError("Packet GetFloat is NaN");
      return 0f;
    }
    if(float.IsInfinity(retval)) {
      Debug.LogError("Packet GetFloat is Infinity");
      return 0f;
    }
    return retval;
  }


  /// <summary>
  /// String
  /// </summary>
  /// <returns>string</returns>
  public string ReadUTF8() //String
  {
    int size = ReadH();
    string str = System.Text.UnicodeEncoding.UTF8.GetString(ReciveData, Offset, size);
    Offset += size;
    return str;
  }

  public string ReadUtf16() //String
  {
    int pos = Offset;
    do {
      pos = System.Array.IndexOf<byte>(ReciveData, 0, pos) + 1;
    }
    while(ReciveData[pos] != 0);

    string str = System.Text.ASCIIEncoding.Unicode.GetString(ReciveData, Offset, pos - Offset - 1);
    Offset += pos + 1;
    return str;
  }

  /// <summary>
  /// byte array
  /// </summary>
  /// <param name="size">Размер массива</param>
  /// <returns>byte[]</returns>
  public byte[] ReadB(int size) //byte[]
  {
    byte[] bin = new byte[size];
    System.Array.Copy(ReciveData, Offset, bin, 0, size);
    Offset += size;
    return bin;
  }

  public bool ReadBool() {
    return ReadC() > 0;
  }

  /// <summary>
  /// String
  /// </summary>
  /// <returns>string</returns>
  public string ReadASCII() //String
  {
    int pos = System.Array.IndexOf<byte>(ReciveData, (byte)0x00, Offset);

    string str = System.Text.ASCIIEncoding.ASCII.GetString(ReciveData, Offset, pos - Offset);
    Offset = pos + 1;
    return str;
  }



  public Vector2 ReadVec2() {
    return new Vector2(ReadF(), ReadF());
  }

  public Vector2 ReadVec2Half() {
    return new Vector2(ReadH() / 50f, ReadH() / 50f);
  }

  public Vector3 ReadVec3() {
    return new Vector3(ReadF(), ReadF(), ReadF());
  }

  public Color32 ReadColor() {
    return new Color32(ReadC(), ReadC(), ReadC(), 255);
  }

  #endregion

  public override string ToString() {
    return " Type " + GetType();
  }

  private string ByteToString(byte[] data) {
    string s = "";
    foreach(byte a in data) {
      s += a.ToString() + " ";
    }
    return s;
  }

  public bool isProcessedByPacket() {
    return isProcessedOverrided;
  }
}