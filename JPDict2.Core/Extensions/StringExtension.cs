using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Core.Extensions;
public static class StringExtension
{
    public static bool ContainsUnicodeCharacter(this string input)
    {
        const int MaxAnsiCode = 255;
        return input.Any(c => c > MaxAnsiCode);
    }
}
