﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoHelper.Application.Common.Extensions;
public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        if (!string.IsNullOrEmpty(str) && str.Length > 1)
        {
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
        return str.ToLowerInvariant();
    }
}
