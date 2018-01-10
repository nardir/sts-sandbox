using EnsureThat;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Menu
{
    public class MenuDomainOptions
    {
        public string ConnectionString { get; set; }
        public bool UseInMemoryDatabase { get; set; }
        public bool ProtectedSecrets { get; set; }
    }

    public class ConfigureMenuDomainOptions : IConfigureOptions<MenuDomainOptions>
    {
        IDataProtector _dataProtector;

        public ConfigureMenuDomainOptions(IDataProtectionProvider dataProtectionProvider)
        {
            EnsureArg.IsNotNull(dataProtectionProvider, nameof(dataProtectionProvider));

            _dataProtector = dataProtectionProvider.CreateProtector("Menus");
        }

        public void Configure(MenuDomainOptions options)
        {
            if (options.ProtectedSecrets)
                options.ConnectionString = _dataProtector.Unprotect(options.ConnectionString);
        }
    }
}
