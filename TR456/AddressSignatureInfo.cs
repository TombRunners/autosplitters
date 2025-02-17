using System;

namespace TR456;

public readonly record struct AddressSignatureInfo
{
    public string Name { get; init; }
    public Type DataType { get; init; }
    public byte[] Signature { get; init; }
    public int OffsetToWriteInstruction { get; init; }
    public int WriteInstructionLength { get; init; }
    public int EffectiveAddressOffset { get; init; }

    public bool Equals(AddressSignatureInfo other) =>
        Name == other.Name &&
        DataType == other.DataType &&
        OffsetToWriteInstruction == other.OffsetToWriteInstruction &&
        WriteInstructionLength == other.WriteInstructionLength &&
        EffectiveAddressOffset == other.EffectiveAddressOffset &&
        (
            Signature == other.Signature || // Reference
            (Signature != null && other.Signature != null && Signature.AsSpan().SequenceEqual(other.Signature)) // Member equality
        );

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Name);
        hash.Add(DataType);
        hash.Add(OffsetToWriteInstruction);
        hash.Add(WriteInstructionLength);
        hash.Add(EffectiveAddressOffset);

        if (Signature == null)
            return hash.ToHashCode();

        foreach (byte b in Signature)
            hash.Add(b);

        return hash.ToHashCode();
    }
};