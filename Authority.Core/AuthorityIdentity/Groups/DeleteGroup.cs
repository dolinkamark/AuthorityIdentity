﻿using System;
using System.Data.Entity;
using System.Threading.Tasks;
using AuthorityIdentity.DomainModel;
using AuthorityIdentity.EntityFramework;

namespace AuthorityIdentity.Groups
{
    public sealed class DeleteGroup : OperationWithNoReturnAsync
    {
        private readonly Guid _groupId;

        public DeleteGroup(IAuthorityContext authorityContext, Guid groupId)
            : base(authorityContext)
        {
            _groupId = groupId;
        }

        public override async Task Do()
        {
            Group group = await Context.Groups.FirstOrDefaultAsync(g => g.Id == _groupId);

            Require(() => group != null, ErrorCodes.GroupNotFound);

            Context.Groups.Remove(group);
        }
    }
}
