using System;
using System.Linq;
using LiveSplit.ComponentUtil;

namespace TR456;

public readonly record struct AddressSignatureInfo
{
    public string Name { get; init; }
    public Func<IntPtr, MemoryWatcher> MemoryWatcherFactory { get; init; }
    public byte[] Signature { get; init; }
    public string[] SignatureWithMasks { get; init; }
    public (GameVersion? version, int offset)[] OffsetsToWriteInstruction { get; init; }
    public int WriteInstructionLength { get; init; }
    public int EffectiveAddressOffset { get; init; }

    public bool Equals(AddressSignatureInfo other)
        => Name                 == other.Name                                 &&
           MemoryWatcherFactory == other.MemoryWatcherFactory                 &&
           Equals(OffsetsToWriteInstruction, other.OffsetsToWriteInstruction) &&
           WriteInstructionLength == other.WriteInstructionLength             &&
           EffectiveAddressOffset == other.EffectiveAddressOffset             &&
           (
               Signature == other.Signature || // Reference
               (Signature != null && other.Signature != null && Signature.AsSpan().SequenceEqual(other.Signature)) // Member equality
           )                                                                  &&
           (
               SignatureWithMasks == other.SignatureWithMasks || // Reference
               (SignatureWithMasks != null && other.SignatureWithMasks != null && SignatureWithMasks.AsSpan().SequenceEqual(other.SignatureWithMasks)) // Member equality
           );

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Name);
        hash.Add(MemoryWatcherFactory);
        hash.Add(OffsetsToWriteInstruction);
        hash.Add(WriteInstructionLength);
        hash.Add(EffectiveAddressOffset);

        if (Signature == null)
            return hash.ToHashCode();

        foreach (byte b in Signature)
            hash.Add(b);

        return hash.ToHashCode();
    }

    public string BytesAsString()
    {
        bool signatureIsNullOrEmpty = Signature     == null || Signature.Length          == 0;
        bool maskIsNullOrEmpty = SignatureWithMasks == null || SignatureWithMasks.Length == 0;
        if (signatureIsNullOrEmpty && maskIsNullOrEmpty)
            return string.Empty;

        return signatureIsNullOrEmpty
            ? string.Join(" ", SignatureWithMasks)
            : string.Join(" ", Signature.Select(static b => b.ToString("X2")));
    }
}