﻿
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using Google.ProtocolBuffers;

using UnityEngine;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

public class MemoryStream
{
    public const int BUFFER_MAX = 1460 * 4;

    public int rpos = 0;
    public int wpos = 0;
    private byte[] datas_ = new byte[BUFFER_MAX];

    private static System.Text.ASCIIEncoding _converter = new System.Text.ASCIIEncoding();


    [StructLayout(LayoutKind.Explicit, Size = 4)]
    struct PackFloatXType
    {
        [FieldOffset(0)]
        public float fv;

        [FieldOffset(0)]
        public UInt32 uv;

        [FieldOffset(0)]
        public Int32 iv;
    }

    public byte[] data()
    {
        return datas_;
    }

    public void setData(byte[] data)
    {
        datas_ = data;
    }

    //---------------------------------------------------------------------------------
    public SByte readInt8()
    {
        return (SByte)datas_[rpos++];
    }

    public Int16 readInt16()
    {
        rpos += 2;
        var ret = BitConverter.ToInt16(datas_, rpos - 2);
        return IPAddress.NetworkToHostOrder(ret);
    }

    public Int32 readInt32()
    {
        rpos += 4;
        var ret = BitConverter.ToInt32(datas_, rpos - 4);
        return IPAddress.NetworkToHostOrder(ret);
    }

    public Int64 readInt64()
    {
        rpos += 8;
        var ret = BitConverter.ToInt64(datas_, rpos - 8);
        return IPAddress.NetworkToHostOrder(ret);
    }

    public Byte readUint8()
    {
        return datas_[rpos++];
    }

    public UInt16 readUint16()
    {
        rpos += 2;
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(datas_, rpos - 2, 2);
        }
        return BitConverter.ToUInt16(datas_, rpos - 2);
    }

    public UInt32 readUint32()
    {
        rpos += 4;
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(datas_, rpos - 4, 4);
        }
        return BitConverter.ToUInt32(datas_, rpos - 4);
    }

    public UInt64 readUint64()
    {
        rpos += 8;
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(datas_, rpos - 8, 8);
        }
        return BitConverter.ToUInt64(datas_, rpos - 8);
    }


    //---------------------------------------------------------------------------------
    public void writeInt8(SByte v)
    {
        datas_[wpos++] = (Byte)v;
    }

    //big endian
    public void writeInt16(Int16 v)
    {
        writeInt8((SByte)(v >> 8 & 0xff));
        writeInt8((SByte)(v & 0xff));
    }

    public void writeInt32(Int32 v)
    {
        for (int i = 3; i >= 0; i--)
            writeInt8((SByte)(v >> i * 8 & 0xff));
    }

    public void writeInt64(Int64 v)
    {
        byte[] getdata = BitConverter.GetBytes(v);
        for (int i = getdata.Length - 1; i >= 0; i--)
        {
            datas_[wpos++] = getdata[i];
        }
    }

    public void writeUint8(Byte v)
    {
        datas_[wpos++] = v;
    }

    public void writeUint16(UInt16 v)
    {
        writeUint8((Byte)(v >> 8 & 0xff));
        writeUint8((Byte)(v & 0xff));
    }

    public void writeUint32(UInt32 v)
    {
        for (int i = 3; i >= 0; i--)
            writeUint8((Byte)(v >> i * 8 & 0xff));
    }

    public void writeUint64(UInt64 v)
    {
        byte[] getdata = BitConverter.GetBytes(v);
        for (int i = getdata.Length - 1; i >= 0; i--)
        {
            datas_[wpos++] = getdata[i];
        }
    }

    public void writeFloat(float v)
    {
        byte[] getdata = BitConverter.GetBytes(v);
        for (int i = getdata.Length - 1; i >= 0; i--)
        {
            datas_[wpos++] = getdata[i];
        }
    }

    public void writeDouble(double v)
    {
        byte[] getdata = BitConverter.GetBytes(v);
        for (int i = getdata.Length - 1; i >= 0; i++)
        {
            datas_[wpos++] = getdata[i];
        }
    }

    public void writePB(byte[] v)
    {
        UInt32 size = (UInt32)v.Length;
        if (size > fillfree())
        {
            Debug.LogError("memorystream::writeBlob: no free!");
            return;
        }

        //writeUint32(size);

        for (UInt32 i = 0; i < size; i++)
        {
            datas_[wpos++] = v[i];
        }
    }

    public void writeBlob(byte[] v)
    {
        UInt32 size = (UInt32)v.Length;
        if (size + 4 > fillfree())
        {
            Debug.LogError("memorystream::writeBlob: no free!");
            return;
        }

        writeUint32(size);

        for (UInt32 i = 0; i < size; i++)
        {
            datas_[wpos++] = v[i];
        }
    }

    public void writeString(string v)
    {
        if (v.Length > fillfree())
        {
            Debug.LogError("memorystream::writeString: no free!");
            return;
        }

        byte[] getdata = System.Text.Encoding.ASCII.GetBytes(v);
        for (int i = 0; i < getdata.Length; i++)
        {
            datas_[wpos++] = getdata[i];
        }

        datas_[wpos++] = 0;
    }

    //---------------------------------------------------------------------------------
    public void readSkip(UInt32 v)
    {
        rpos += (int)v;
    }

    //---------------------------------------------------------------------------------
    public UInt32 fillfree()
    {
        return (UInt32)(BUFFER_MAX - wpos);
    }

    //---------------------------------------------------------------------------------
    public UInt32 opsize()
    {
        return (UInt32)(wpos - rpos);
    }

    //---------------------------------------------------------------------------------
    public bool readEOF()
    {
        return (BUFFER_MAX - rpos) <= 0;
    }

    //---------------------------------------------------------------------------------
    public UInt32 totalsize()
    {
        return opsize();
    }

    //---------------------------------------------------------------------------------
    public void opfini()
    {
        rpos = wpos;
    }

    //---------------------------------------------------------------------------------
    public void clear()
    {
        rpos = wpos = 0;
    }

    //---------------------------------------------------------------------------------
    public byte[] getbuffer()
    {
        byte[] buf = new byte[opsize()];
        Array.Copy(data(), rpos, buf, 0, opsize());
        return buf;
    }
    public Google.ProtocolBuffers.ByteString getBytString()
    {
        ByteString inputString = ByteString.CopyFrom(data(), rpos, (int)opsize());
        return inputString;
    }
    //---------------------------------------------------------------------------------
    public string toString()
    {
        string s = "";
        int ii = 0;
        byte[] buf = getbuffer();

        for (int i = 0; i < buf.Length; i++)
        {
            ii += 1;
            if (ii >= 200)
            {
                // MyDebug.Dbg.Log(s);
                s = "";
                ii = 0;
            }

            s += buf[i];
            s += " ";
        }

        // MyDebug.Dbg.Log(s);
        return s;
    }
}

