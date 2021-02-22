﻿using System.Text;
using System.Collections;
using System;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class WeatherIconProvider
    {
        private readonly Hashtable _mappings;

        public WeatherIconProvider()
        {
            _mappings = new Hashtable()
            {
                { "200", "\uf01e"},
                { "201", "\uf01e"},
                { "202", "\uf01e"},
                { "210", "\uf016"},
                { "211", "\uf016"},
                { "212", "\uf016"},
                { "221", "\uf016"},
                { "230", "\uf01e"},
                { "231", "\uf01e"},
                { "232", "\uf01e"},
                { "300", "\uf01c"},
                { "301", "\uf01c"},
                { "302", "\uf019"},
                { "310", "\uf017"},
                { "311", "\uf019"},
                { "312", "\uf019"},
                { "313", "\uf01a"},
                { "314", "\uf019"},
                { "321", "\uf01c"},
                { "500", "\uf01c"},
                { "501", "\uf019"},
                { "502", "\uf019"},
                { "503", "\uf019"},
                { "504", "\uf019"},
                { "511", "\uf017"},
                { "520", "\uf01a"},
                { "521", "\uf01a"},
                { "522", "\uf01a"},
                { "531", "\uf01d"},
                { "600", "\uf01b"},
                { "601", "\uf01b"},
                { "602", "\uf0b5"},
                { "611", "\uf017"},
                { "612", "\uf017"},
                { "615", "\uf017"},
                { "616", "\uf017"},
                { "620", "\uf017"},
                { "621", "\uf01b"},
                { "622", "\uf01b"},
                { "701", "\uf01a"},
                { "711", "\uf062"},
                { "721", "\uf0b6"},
                { "731", "\uf063"},
                { "741", "\uf014"},
                { "761", "\uf063"},
                { "762", "\uf063"},
                { "771", "\uf011"},
                { "781", "\uf056"},
                { "800", "\uf00d"},
                { "801", "\uf011"},
                { "802", "\uf011"},
                { "803", "\uf012"},
                { "804", "\uf013"},
                { "900", "\uf056"},
                { "901", "\uf01d"},
                { "902", "\uf073"},
                { "903", "\uf076"},
                { "904", "\uf072"},
                { "905", "\uf021"},
                { "906", "\uf015"},
                { "957", "\uf050"},
                { "d200", "\uf010"},
                { "d201", "\uf010"},
                { "d202", "\uf010"},
                { "d210", "\uf005"},
                { "d211", "\uf005"},
                { "d212", "\uf005"},
                { "d221", "\uf005"},
                { "d230", "\uf010"},
                { "d231", "\uf010"},
                { "d232", "\uf010"},
                { "d300", "\uf00b"},
                { "d301", "\uf00b"},
                { "d302", "\uf008"},
                { "d310", "\uf008"},
                { "d311", "\uf008"},
                { "d312", "\uf008"},
                { "d313", "\uf008"},
                { "d314", "\uf008"},
                { "d321", "\uf00b"},
                { "d500", "\uf00b"},
                { "d501", "\uf008"},
                { "d502", "\uf008"},
                { "d503", "\uf008"},
                { "d504", "\uf008"},
                { "d511", "\uf006"},
                { "d520", "\uf009"},
                { "d521", "\uf009"},
                { "d522", "\uf009"},
                { "d531", "\uf00e"},
                { "d600", "\uf00a"},
                { "d601", "\uf0b2"},
                { "d602", "\uf00a"},
                { "d611", "\uf006"},
                { "d612", "\uf006"},
                { "d615", "\uf006"},
                { "d616", "\uf006"},
                { "d620", "\uf006"},
                { "d621", "\uf00a"},
                { "d622", "\uf00a"},
                { "d701", "\uf009"},
                { "d711", "\uf062"},
                { "d721", "\uf0b6"},
                { "d731", "\uf063"},
                { "d741", "\uf003"},
                { "d761", "\uf063"},
                { "d762", "\uf063"},
                { "d781", "\uf056"},
                { "d800", "\uf00d"},
                { "d801", "\uf000"},
                { "d802", "\uf000"},
                { "d803", "\uf000"},
                { "d804", "\uf00c"},
                { "d900", "\uf056"},
                { "d902", "\uf073"},
                { "d903", "\uf076"},
                { "d904", "\uf072"},
                { "d906", "\uf004"},
                { "d957", "\uf050"},
                { "n200", "\uf02d"},
                { "n201", "\uf02d"},
                { "n202", "\uf02d"},
                { "n210", "\uf025"},
                { "n211", "\uf025"},
                { "n212", "\uf025"},
                { "n221", "\uf025"},
                { "n230", "\uf02d"},
                { "n231", "\uf02d"},
                { "n232", "\uf02d"},
                { "n300", "\uf02b"},
                { "n301", "\uf02b"},
                { "n302", "\uf028"},
                { "n310", "\uf028"},
                { "n311", "\uf028"},
                { "n312", "\uf028"},
                { "n313", "\uf028"},
                { "n314", "\uf028"},
                { "n321", "\uf02b"},
                { "n500", "\uf02b"},
                { "n501", "\uf028"},
                { "n502", "\uf028"},
                { "n503", "\uf028"},
                { "n504", "\uf028"},
                { "n511", "\uf026"},
                { "n520", "\uf029"},
                { "n521", "\uf029"},
                { "n522", "\uf029"},
                { "n531", "\uf02c"},
                { "n600", "\uf02a"},
                { "n601", "\uf0b4"},
                { "n602", "\uf02a"},
                { "n611", "\uf026"},
                { "n612", "\uf026"},
                { "n615", "\uf026"},
                { "n616", "\uf026"},
                { "n620", "\uf026"},
                { "n621", "\uf02a"},
                { "n622", "\uf02a"},
                { "n701", "\uf029"},
                { "n711", "\uf062"},
                { "n721", "\uf0b6"},
                { "n731", "\uf063"},
                { "n741", "\uf04a"},
                { "n761", "\uf063"},
                { "n762", "\uf063"},
                { "n781", "\uf056"},
                { "n800", "\uf02e"},
                { "n801", "\uf022"},
                { "n802", "\uf022"},
                { "n803", "\uf022"},
                { "n804", "\uf086"},
                { "n900", "\uf056"},
                { "n902", "\uf073"},
                { "n903", "\uf076"},
                { "n904", "\uf072"},
                { "n906", "\uf024"},
                { "n957", "\uf050"},
            };
        }

        //public string GetImportRange()
        //{
        //    var sb = new StringBuilder();
        //    foreach(DictionaryEntry icon in _mappings)
        //    {                
        //        var value = Convert.ToInt32(icon.Value.ToString(), 16);
        //        sb.AppendLine($"ImportRange {value} {value}");
        //    }

        //    return sb.ToString();
        //}

        public string GetIconUnicode(string id, string icon)
        {
            var searchKey = id.ToLower();

            if (!string.IsNullOrEmpty(icon))
            {
                if (icon.Contains("d"))
                {
                    searchKey = $"d{id}";
                };

                if (icon.Contains("n"))
                {
                    searchKey = $"n{id}";
                }
            }

            if (_mappings.Contains(searchKey))
            {
                return (string) _mappings[searchKey];
            }

            return "\uf07b"; // default n/a glyphs;
        }
    }
}