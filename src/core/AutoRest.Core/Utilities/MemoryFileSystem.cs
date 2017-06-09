﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.


using AutoRest.Core.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoRest.Core.Utilities
{
    public class MemoryFileSystem : IFileSystem, IDisposable
    {
        private const string FolderKey = "Folder";

        public Dictionary<string, StringBuilder> VirtualStore { get; } = new Dictionary<string, StringBuilder>();

        public bool IsCompletePath(string path)
           => Uri.IsWellFormedUriString(path, UriKind.Relative);

        public string MakePathRooted(Uri rootPath, string relativePath)
        {
            return (new Uri(Path.Combine(rootPath.ToString(), relativePath).ToString(), UriKind.Relative)).ToString();
        }

        public Uri GetParentDir(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new Exception(Resources.PathCannotBeNullOrEmpty);
            }
            if (IsCompletePath(path))
            {
                return new Uri(new Uri(path), ".");
            }
            else
            {
                return new Uri(Path.GetDirectoryName(path), UriKind.Relative);
            }
        }
        
        public void WriteAllText(string path, string contents)
            => VirtualStore[path] = new StringBuilder(contents);

        public string ReadAllText(string path)
        {
            if (VirtualStore.ContainsKey(path))
            {
                return VirtualStore[path].ToString();
            }
            else if (VirtualStore.ContainsKey(path.Replace("\\", "/")))
            {
                return VirtualStore[path.Replace("\\", "/")].ToString();
            }
            else if (VirtualStore.ContainsKey(path.Replace("/", "\\")))
            {
                return VirtualStore[path.Replace("/", "\\")].ToString();
            }
            
            throw new IOException("File not found: " + path);
        }

        public TextWriter GetTextWriter(string path)
        {
            if (path.IsNullOrEmpty())
            {
                throw new ArgumentException("path cannot be null.", nameof(path));
            }
            var directory = Path.GetDirectoryName(path);
            if (!VirtualStore.ContainsKey(directory))
            {
                throw new IOException(string.Format(CultureInfo.InvariantCulture, "Directory {0} does not exist.", directory));
            }

            var stringBuilder = VirtualStore.ContainsKey(path) ? VirtualStore[path] : new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
            VirtualStore[path] = stringBuilder;

            return stringWriter;
        }

        public bool FileExists(string path)
            => VirtualStore.ContainsKey(path) 
            || VirtualStore.ContainsKey(path.Replace("/", "\\")) 
            || VirtualStore.ContainsKey(path.Replace("\\", "/")); // TODO: uniform treatment

        public void DeleteFile(string path)
        {
            if (VirtualStore.ContainsKey(path))
            {
                VirtualStore.Remove(path);
            }
        }

        public bool DirectoryExists(string path)
            => VirtualStore.Keys.Any(key => key.StartsWith(path, StringComparison.Ordinal));

        public void CreateDirectory(string path)
            => VirtualStore[path] = new StringBuilder(FolderKey);

        public string[] GetDirectories(string startDirectory, string filePattern, SearchOption options)
        {
            HashSet<string> dirs = new HashSet<string>();
            foreach (var key in VirtualStore.Keys.ToArray())
            {
                if (key.StartsWith(startDirectory, StringComparison.Ordinal) &&
                    Regex.IsMatch(key, WildcardToRegex(filePattern)))
                {
                    var directoryName = Path.GetDirectoryName(key);
                    if (!dirs.Contains(directoryName))
                    {
                        dirs.Add(directoryName);
                    }
                }
            }
            return dirs.ToArray();
        }

        public string[] GetFiles(string startDirectory, string filePattern, SearchOption options)
        {
            HashSet<string> files = new HashSet<string>();
            foreach (var key in VirtualStore.Keys.ToArray())
            {
                if (key.StartsWith(startDirectory, StringComparison.Ordinal) &&
                    VirtualStore[key].ToString() != FolderKey &&
                    Regex.IsMatch(key, WildcardToRegex(filePattern)))
                {
                    if (!files.Contains(key))
                    {
                        files.Add(key);
                    }
                }
            }
            return files.ToArray();
        }

        /// <summary>
        /// Converts unix asterisk based file pattern to regex
        /// </summary>
        /// <param name="wildcard">Asterisk based pattern</param>
        /// <returns>Regeular expression of null is empty</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        private static string WildcardToRegex(string wildcard)
        {
            if (string.IsNullOrEmpty(wildcard)) return wildcard;

            var sb = new StringBuilder();

            var chars = wildcard.ToCharArray();
            foreach (var ch in chars)
            {
                if (ch == '*')
                    sb.Append(".*");
                else if (ch == '?')
                    sb.Append(".");
                else if ("+()^$.{}|\\".IndexOf(ch) != -1)
                    sb.Append('\\').Append(ch); // prefix all metacharacters with backslash
                else
                    sb.Append(ch);
            }
            return sb.ToString().ToLowerInvariant();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                VirtualStore?.Clear();
            }
        }

        public void CommitToDisk(string targetDirectory)
        {
            var fs = new FileSystem();
            foreach (var entry in VirtualStore)
            {
                if (entry.Value.ToString() == FolderKey)
                {
                    var targetDirName = Path.Combine(targetDirectory, entry.Key);
                    fs.CreateDirectory(targetDirName);
                }
                else
                {
                    var targetFileName = Path.Combine(targetDirectory, entry.Key);
                    var targetFileDir = Path.GetDirectoryName(targetFileName);
                    fs.CreateDirectory(targetFileDir);
                    fs.WriteAllText(targetFileName, entry.Value.ToString());
                }
            }
        }

        public string CurrentDirectory
        {
            get
            {
                return "";
            }
        }
    }
}
