namespace TR456;

public class SignatureScanInfo(uint maxRetries = 6)
{
    public SignatureScanStatus Status = SignatureScanStatus.NotTriedYet;
    public readonly uint MaxRetries = maxRetries;
    public uint RetryCount;

    public bool IsSuccess => Status is SignatureScanStatus.Success;
    public bool MaxRetriesReached => RetryCount >= MaxRetries;

    public void AddRetry() => RetryCount++;
    public void ResetCount() => RetryCount = 0;

    public void SetStatus(SignatureScanStatus status) => Status = status;
}