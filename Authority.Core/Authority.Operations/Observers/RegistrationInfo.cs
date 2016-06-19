﻿using System;

namespace Authority.Operations.Observers
{
    public sealed class RegistrationInfo
    {
        public Guid ProductId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}