namespace FPP.BlazorOidcAuthenticationHelper.Contracts;

public interface IAuthenticationLock
{
    bool IsLocked { get; }
    void Lock();
    void Unlock();

    event EventHandler Locked;
    event EventHandler Unlocked;
}
