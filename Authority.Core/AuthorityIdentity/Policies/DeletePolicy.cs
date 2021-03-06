﻿using System;
using System.Data.Entity;
using System.Threading.Tasks;
using AuthorityIdentity.DomainModel;
using AuthorityIdentity.EntityFramework;

namespace AuthorityIdentity.Policies
{
    public sealed class DeletePolicy : OperationWithNoReturnAsync
    {
        private readonly Guid _policyId;

        public DeletePolicy(IAuthorityContext authorityContext, Guid policyId) 
            : base(authorityContext)
        {
            _policyId = policyId;
        }

        public override async Task Do()
        {
            Policy policy = await Context.Policies.FirstOrDefaultAsync(p => p.Id == _policyId);

            Require(() => policy != null, ErrorCodes.PolicyNotFound);

            Context.Policies.Remove(policy);
        }
    }
}
