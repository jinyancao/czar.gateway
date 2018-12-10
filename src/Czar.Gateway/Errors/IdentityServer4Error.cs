using Ocelot.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Czar.Gateway.Errors
{
    public class IdentityServer4Error:Error
    {
        public IdentityServer4Error(string message) : base(message, OcelotErrorCode.UnableToCreateAuthenticationHandlerError)
        {

        }
    }
}
