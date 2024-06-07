﻿using System.Text;
using oren_client.Lib.Formats;

namespace oren_client.Lib.Utils;

public class FileReader : IDisposable
{
    private readonly BinaryReader _reader;
    private bool _disposed;
    
    public void Dispose()
    {
        if (_disposed) return;
        _reader.Dispose();
        _disposed = true;
    }
    
    public FileReader(Stream fileStream, bool leaveOpen = false)
    {
        _reader = new BinaryReader(fileStream, Encoding.UTF8, leaveOpen);
        Position = 0;
        Endianness = Endianness.BigEndian;
    }
    
    public long Position
    {
        get => _reader.BaseStream.Position;
        set => _reader.BaseStream.Position = value;
    }
    
    public Endianness Endianness { get; set; }

    #region public methods
    
    public void Skip(int count)
    {
        Position += count;
    }

    public byte[] ReadBytes(int length)
    {
        return ReadBytes(length, length);
    }
    
    public byte ReadByte(int length = 1)
    {
        byte[] bytes = ReadBytes(length, 1);
        return bytes[0];
    }
    
    public byte ReadByteAt(long position, int length = 1)
    {
        Position = position;
        return ReadByte(length);
    }
    
    public string ReadString(int length)
    {
        return ReadString(length, Encoding.UTF8);
    }
    
    public string ReadStringAt(long position, int length)
    {
        return ReadStringAt(position, length, Encoding.UTF8);
    }
    
    public string ReadString(int length, Encoding encoding)
    {
        byte[] bytes = ReadBytes(length, 0);
        return encoding.GetString(bytes).TrimEnd('\0');
    }
    
    public string ReadStringAt(long position, int length, Encoding encoding)
    {
        Position = position;
        return ReadString(length, encoding);
    }
    
    public short ReadInt16(int length = 2)
    {
        byte[] bytes = ReadBytes(length, 2, Endianness == Endianness.LittleEndian);
        return BitConverter.ToInt16(bytes, 0);
    }
    public short ReadInt16At(long position, int length = 2)
    {
        Position = position;
        return ReadInt16(length);
    }

    #endregion

    #region private methods

    private byte[] ReadBytes(int length, int padding, bool reversed = false)
    {
        if (length <= 0) return Array.Empty<byte>();

        var bytes = new byte[length > padding ? length : padding];
        _ = _reader.Read(bytes, reversed ? bytes.Length - length : 0, length);

        if (reversed) Array.Reverse(bytes);
        return bytes;
    }

    #endregion
}