﻿using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Authority.DomainModel;
using Authority.EntityFramework;

namespace Authority.Operations.Account
{
    public sealed class SetUserStatus : OperationWithNoReturnAsync
    {
        private readonly Guid _userId;
        private readonly bool _isActive;

        public SetUserStatus(IAuthorityContext authorityContext, Guid userId, bool isActive) 
            : base(authorityContext)
        {
            _userId = userId;
            _isActive = isActive;
        }

        public override async Task Do()
        {
            User user = await Context.Users.FirstOrDefaultAsync(u => u.Id == _userId);

            Require(() => user != null, AccountErrorCodes.UserNotFound);

            user.IsActive = _isActive;
        }
    }
}
