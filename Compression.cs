using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Crypto.AES;

namespace Compression{
public class HuffmanNode
{
    public char Character { get; set; }
    public int Frequency { get; set; }
    public HuffmanNode? Left { get; set; }
    public HuffmanNode? Right { get; set; }
}

public class Huffman
{
    public HuffmanNode? BuildHuffmanTree(string input)
    {
        var frequencies = new Dictionary<char, int>();
        foreach (var chr in input)
        {
            if (!frequencies.ContainsKey(chr))
                frequencies[chr] = 0;
            frequencies[chr]++;
        }

        var priorityQueue = new List<HuffmanNode>();
        foreach (var frequency in frequencies)
        {
            priorityQueue.Add(new HuffmanNode { Character = frequency.Key, Frequency = frequency.Value });
        }

        while (priorityQueue.Count > 1)
        {
            var sorted = priorityQueue.OrderBy(node => node.Frequency).ToList();
            var left = sorted[0];
            var right = sorted[1];

            var newNode = new HuffmanNode
            {
                Frequency = left.Frequency + right.Frequency,
                Left = left,
                Right = right
            };

            priorityQueue.Remove(left);
            priorityQueue.Remove(right);
            priorityQueue.Add(newNode);
        }

        return priorityQueue.FirstOrDefault();
    }

    public void GenerateCodes(HuffmanNode? node, string code, Dictionary<char, string> codes)
    {
        if (node == null)
            return;

        if (node.Left == null && node.Right == null)
        {
            codes[node.Character] = code;
        }

        GenerateCodes(node.Left, code + "0", codes);
        GenerateCodes(node.Right, code + "1", codes);
    }

    public byte[] Compress(string input, Dictionary<char, string> codes)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var character in input)
        {
            stringBuilder.Append(codes[character]);
        }

        int numOfBytes = (stringBuilder.Length + 7) / 8;
        byte[] compressedData = new byte[numOfBytes];
        for (int i = 0; i < stringBuilder.Length; i++)
        {
            if (stringBuilder[i] == '1')
            {
                compressedData[i / 8] |= (byte)(1 << (7 - (i % 8)));
            }
        }

        return compressedData;
    }

    public string Decompress(byte[] input, HuffmanNode? root)
    {
        HuffmanNode? current = root;
        StringBuilder decoded = new StringBuilder();
        foreach (byte byteValue in input)
        {
            for (int bitPosition = 7; bitPosition >= 0; bitPosition--)
            {
                bool isBitSet = (byteValue & (1 << bitPosition)) != 0;
                current = isBitSet ? current?.Right : current?.Left;

                if (current?.Left == null && current?.Right == null)
                {
                    decoded.Append(current?.Character);
                    current = root;
                }
            }
        }

        return decoded.ToString();
    }
}
}