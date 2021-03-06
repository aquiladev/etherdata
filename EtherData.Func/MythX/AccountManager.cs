﻿using EtherData.Func.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace EtherData.Func.MythX
{
    public class AccountManager
    {
        private IDictionary<string, string> _accounts = new Dictionary<string, string>();
        private IList<string> _addresses = new List<string>();

        public AccountManager(IConfigurationRoot config)
        {
            var section = config.GetSection("MythX:Account");

            foreach (var setting in section.GetChildren())
            {
                _addresses.Add(setting.Key);
                _accounts.Add(setting.Key, setting.Value);
            }
        }

        public int Count()
        {
            return _addresses.Count;
        }

        public int IndexOf(string address)
        {
            return _addresses.IndexOf(address);
        }

        public string GetPassword(string key)
        {
            return _accounts[key];
        }

        public Account Get(int index = 0)
        {
            if (_addresses.Count == 0)
            {
                throw new Exception("No credentials");
            }

            var password = _accounts[_addresses[index]];
            return new Account(_addresses[index], password);
        }

        public Account Next(string address)
        {
            var index = _addresses.IndexOf(address) + 1;
            if (index >= _addresses.Count)
            {
                index = 0;
            }

            return Get(index);
        }
    }
}
