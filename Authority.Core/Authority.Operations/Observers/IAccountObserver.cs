﻿using Authority.DomainModel;

namespace Authority.Operations.Observers
{
    public interface IAccountObserver
    {
        void OnInvited(InviteInfo inviteInfo);
        void OnRegistering(RegistrationInfo registrationInfo);
        void OnRegistered(User user);
        void OnActivated(User user);
        void OnLoggingIn(LoginInfo loginInfo);
        void LoggedIn(User user);
    }
}
