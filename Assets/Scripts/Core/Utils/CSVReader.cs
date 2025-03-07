﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Utils {
    public class CSVReader {
        private const string SplitRe = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        private const string LineSplitRe = @"\r\n|\n\r|\n|\r";
        private static readonly char[] TrimChars = { '\"' };

        [CanBeNull]
        public static List<Dictionary<string, object>> Read(TextAsset data) {
            var list = new List<Dictionary<string, object>>();
            // var data = Resources.Load (file) as TextAsset;
            if (data == null) {
                return null;
            }

            var lines = Regex.Split(data.text, LineSplitRe);
            if(lines.Length <= 1) {
                return list;
            }

            var header = Regex.Split(lines[0], SplitRe);
            for(var i = 1; i < lines.Length; i++) {

                var values = Regex.Split(lines[i], SplitRe);
                if(values.Length == 0) continue;

                var entry = new Dictionary<string, object>();
                for(var j = 0; j < header.Length && j < values.Length; j++ ) {
                    var value = values[j];
                    value = value.TrimStart(TrimChars).TrimEnd(TrimChars).Replace("\\", string.Empty);
                    
                    // combined cells return empty values for all but first row
                    // var finalValue = value == string.Empty && list.Count > 0 ? list[i - 2][header[j]] : value;
                    
                    if(int.TryParse(value, out var n)) {
                        // finalValue = n;
                        entry[header[j]] = n;
                    } else if (float.TryParse(value, out var f)) {
                        // finalValue = f;
                        entry[header[j]] = f;
                    } else {
                        entry[header[j]] = value;
                    }
                }
                list.Add(entry);
            }
            return list;
        }


        public static IEnumerable<Dictionary<string, object>> ReadIterative(TextAsset data, bool leaveCellsEmpty = false) {
            var history = new List<Dictionary<string, object>>();
            // var data = Resources.Load (file) as TextAsset;
            if (data == null) {
                yield break;
            }

            var lines = Regex.Split(data.text, LineSplitRe);
            if(lines.Length <= 1) {
                yield break;
            }

            var header = Regex.Split(lines[0], SplitRe);
            for(var i = 1; i < lines.Length; i++) {

                var values = Regex.Split(lines[i], SplitRe);
                if(values.Length == 0) continue;

                var entry = new Dictionary<string, object>();
                for(var j = 0; j < header.Length && j < values.Length; j++ ) {
                    var value = values[j];
                    value = value.TrimStart(TrimChars).TrimEnd(TrimChars).Replace("\\", string.Empty);
                    
                    // combined cells return empty values for all but the first row
                    var finalValue = value == string.Empty && history.Count > 0 && !leaveCellsEmpty ? 
                        history[i - 2][header[j]] : 
                        value;
                    
                    if(int.TryParse(value, out var n)) {
                        finalValue = n;
                    } else if (float.TryParse(value, out var f)) {
                        finalValue = f;
                    }
                    entry[header[j]] = finalValue;
                }
                history.Add(entry);
                yield return entry;
            }
        }

        public static string[] GetHeader(TextAsset csv) {
            var lines = Regex.Split(csv.text, LineSplitRe);
            return lines.Length < 1 ? null : Regex.Split(lines[0], SplitRe);
        }
    }
}