﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AuthorityIdentity.DomainModel;
using AuthorityIdentity.EntityFramework;

namespace AuthorityIdentity.Groups
{
    public sealed class AddUsersToGroup : OperationWithNoReturnAsync
    {
        private readonly IEnumerable<Guid> _userIdList;
        private readonly Guid _groupId;

        public AddUsersToGroup(IAuthorityContext authorityContext, Guid groupId, IEnumerable<Guid> userIdList) 
            : base(authorityContext)
        {
            _userIdList = userIdList;
            _groupId = groupId;
        }

        public override async Task Do()
        {
            IQueryable<User> users = Context.Users
                .Include(u => u.Groups)
                .Where(u => _userIdList.Contains(u.Id));

            Require(() => users.Count() == _userIdList.Count(), ErrorCodes.UserNotFound);

            Group group = await Context.Groups.FirstOrDefaultAsync(g => g.Id == _groupId);

            Require(() => group != null, ErrorCodes.GroupNotFound);
            Require(() => users.All(u => group.DomainId == u.DomainId), ErrorCodes.DomainNotFound);

            foreach (User user in users)
            {
                if (user.Groups.Any(g => g.Id == group.Id))
                {
                    continue;
                }

                user.Groups.Add(group);
            }
        }
    }
}
