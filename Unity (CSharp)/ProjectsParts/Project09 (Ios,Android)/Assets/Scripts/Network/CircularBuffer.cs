using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCollections {

  public class CircularBuffer {
    private byte[] m_Data;
    private int m_Offset = 0;
    private int m_Count  = 0;

    public int Capacity {
      get { return m_Data.Length; }
    }

    public int Offset {
      get { return m_Offset; }
    }

    public int Count {
      get { return m_Count; }
    }

    public int NextEmptyOffset {
      get { return (m_Offset + m_Count) % m_Data.Length; }
    }

    public CircularBuffer(int _Capacity) {
      m_Data = new byte[_Capacity];
    }

    public void clear() {
      m_Offset = 0;
      m_Count = 0;
    }

    public void put(byte value) {
      //if (m_Count >= m_Data.Length)
      //    throw new BufferOverflowException();

      m_Data[(m_Offset + m_Count) % m_Data.Length] = value;
      m_Count++;
    }

    public byte get() {
      //if (m_Count <= 0)
      //    throw new BufferEmptyException();
      byte Result = m_Data[m_Offset];
      m_Offset = (m_Offset + 1) % m_Data.Length;
      m_Count--;
      return Result;
    }

    public byte peek(int i) {
      //if (i >= m_Count)
      //    throw new BufferEmptyException();
      return m_Data[(m_Offset + i) % m_Data.Length];
    }

    public short peekShort() {
      //if (m_Count < 2)
      //    throw new BufferEmptyException();
      int Offset, Size;
      byte[] Data = peek(2, out Offset, out Size);
      if(Size == 2) {
        short Result = BitConverter.ToInt16(Data, Offset);
        return Result;
      } else {
        int hByte = 0xFF & (int) peek(1);
        int lByte = 0xFF & (int) peek(0);
        return (short)((hByte << 8) + lByte);
      }
    }

    public ushort peekUShort() {
      //if (m_Count < 2)
      //    throw new BufferEmptyException();
      int Offset, Size;
      byte[] Data = peek(2, out Offset, out Size);
      if(Size == 2) {
        ushort Result = BitConverter.ToUInt16(Data, Offset);
        return Result;
      } else {
        int hByte = 0xFF & (int) peek(1);
        int lByte = 0xFF & (int) peek(0);
        return (ushort)((hByte << 8) + lByte);
      }
    }

    public void remove(int _Count) {
      int CountToRemove = Math.Min(m_Count, _Count);
      m_Offset = (m_Offset + CountToRemove) % m_Data.Length;
      m_Count -= CountToRemove;
    }

    public void put(byte[] _Data, int _Offset, int _DataSize) {
      if(_DataSize <= 0 || _Offset < 0 || _Offset >= _Data.Length)
        return;

      if(m_Count + _DataSize >= m_Data.Length) {
        Debug.Log(m_Count + _DataSize + " " + m_Data.Length);

        //throw new BufferOverflowException();
      }
      int WriteIndexStart = (m_Offset + m_Count) % m_Data.Length;
      int WriteIndexEnd = (m_Offset + m_Count + _DataSize) % m_Data.Length;
      if(WriteIndexStart < WriteIndexEnd) {
        Array.Copy(_Data, _Offset, m_Data, WriteIndexStart, _DataSize);
      } else {
        int OverboundSize = m_Offset + m_Count + _DataSize - m_Data.Length;
        Array.Copy(_Data, _Offset, m_Data, WriteIndexStart, _DataSize - OverboundSize);
        Array.Copy(_Data, _Offset + _DataSize - OverboundSize, m_Data, 0, OverboundSize);
      }
      m_Count += _DataSize;
    }

    public void put(byte[] _Data) {
      put(_Data, 0, _Data.Length);
    }

    public void set(byte _Value, int _Offset) {
      m_Data[_Offset] = _Value;
    }

    public byte[] peek(int _MaxSize, out int _Begin, out int _Size) {
      int LockSize = Math.Min(_MaxSize, m_Count);
      if(LockSize <= 0) {
        _Begin = -1;
        _Size = 0;
        return null;
      }
      if(m_Offset + LockSize < m_Data.Length) {
        _Begin = m_Offset;
        _Size = LockSize;
      } else {
        _Begin = m_Offset;
        _Size = m_Data.Length - m_Offset;
      }
      return m_Data;
    }

    public void get(byte[] buffer, int count) {
      int LockSize = Math.Min(count, m_Count);
      for(int i = 0; i < LockSize; ++i) {
        buffer[i] = get();
      }
    }

    public static byte[] ReverseBytes(byte[] _InArray, int _Offset, int _Size) {
      byte temp;
      int highCtr = _Size- 1;

      for(int ctr = 0; ctr < _Size / 2; ctr++) {
        temp = _InArray[_Offset + ctr];
        _InArray[_Offset + ctr] = _InArray[_Offset + highCtr];
        _InArray[_Offset + highCtr] = temp;
        highCtr -= 1;
      }
      return _InArray;
    }
  }


}
