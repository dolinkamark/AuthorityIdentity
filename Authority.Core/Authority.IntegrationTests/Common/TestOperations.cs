﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Authority.DomainModel;
using Authority.EntityFramework;
using Authority.Operations.Account;
using Authority.Operations.Claims;
using Authority.Operations.Policies;
using Authority.Operations.Products;

namespace Authority.IntegrationTests.Common
{
    public static class TestOperations
    {
        public static async Task<Domain> CreateDomain(AuthorityContext context)
        {
            CreateDomain operation = new CreateDomain(context, RandomData.RandomString());
            Guid productId = await operation.Do();
            await operation.CommitAsync();

            Domain product = await context.Domains.FirstOrDefaultAsync(p => p.Id == productId);

            return product;
        }

        public static async Task<Policy> CreatePolicy(
            AuthorityContext context, 
            Guid domainId, 
            string name,
            bool defaultPolicy = false)
        {
            CreatePolicy operation = new CreatePolicy(context, domainId, name, defaultPolicy);

            Policy policy = await operation.Do();
            await operation.CommitAsync();

            return policy;
        }

        public static async Task<AuthorityClaim> CreateClaim(AuthorityContext context, Guid domainId,
            string friendlyName, string issuer, string type, string value)
        {
            CreateClaim create = new CreateClaim(context, domainId, friendlyName, issuer, type, value);
            AuthorityClaim claim = await create.Do();
            await create.CommitAsync();

            return claim;
        }

        public static async Task<User> RegisterUser(AuthorityContext context, Guid domainId, string password = "")
        {
            string email = RandomData.Email();
            string username = RandomData.RandomString();
            password = string.IsNullOrEmpty(password) ? RandomData.RandomString(12, true) : password;

            RegisterUser operation = new RegisterUser(context, domainId, email, username, password);
            User user = await operation.Do();
            await operation.CommitAsync();

            return user;
        }

        public static async Task<User> RegisterAndActivateUser(AuthorityContext context, Guid domainId, string password)
        {
            User user = await RegisterUser(context, domainId, password);

            ActivateUser activation = new ActivateUser(context, user.PendingRegistrationId);
            await activation.Do();
            await activation.CommitAsync();

            return user;
        }

        public static async Task<List<User>> CreateUsers(AuthorityContext context, Guid domainId, int numberOfUsers = 10)
        {
            List<User> users = new List<User>();

            for (int i = 0; i < numberOfUsers; ++i)
            {
                User user = await RegisterAndActivateUser(context, domainId, RandomData.RandomString());
                users.Add(user);
            }

            return users;
        }

        public static async Task CreateComplexSetup(AuthorityContext context, Guid domainId)
        {
            Random random = new Random();

            List<AuthorityClaim> claims = new List<AuthorityClaim>();
            for (int i = 0; i < 30; ++i)
            {
                AuthorityClaim claim = await CreateClaim(context, domainId, RandomData.RandomString(), RandomData.RandomString(),
                            RandomData.RandomString(), RandomData.RandomString());
                claims.Add(claim);
            }

            List<Policy> policies = new List<Policy>();
            for (int i = 0; i < 20; ++i)
            {
                Policy policy = await CreatePolicy(context, domainId, RandomData.RandomString(), false);
                policies.Add(policy);

                List<AuthorityClaim> claimsToAdd = new List<AuthorityClaim>();
                for (int j = 0; j < 10; ++j)
                {
                    claimsToAdd.Add(claims[random.Next(0, claims.Count)]);
                }

                AddClaimsToPolicy addClaims = new AddClaimsToPolicy(context, policy.Id, claimsToAdd.Select(c => c.Id));
                await addClaims.Do();
                await addClaims.CommitAsync();
            }

            List<User> users = await CreateUsers(context, domainId, 20);
            for (int i = 0; i < users.Count; ++i)
            {
                Policy policyToAdd = policies[random.Next(0, policies.Count)];
                AddUserToPolicy addToPolicy = new AddUserToPolicy(context, users[i].Id, policyToAdd.Id);
                await addToPolicy.Do();
                await addToPolicy.CommitAsync();
            }
        }
    }
}
