using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MiniExcelLibs.Utilities
{
    public static class PathUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetRootPathInternal()
            => Path.Combine(Application.dataPath, "..");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetRootPath()
            => Path.GetFullPath(GetRootPathInternal()).ToUnixPath();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetRelativePath(string path)
            => string.IsNullOrEmpty(path) 
                ? string.Empty
                : Path.GetRelativePath(GetRootPath(), path).ToUnixPath();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAbsolutePath(string relativePath)
            => string.IsNullOrEmpty(relativePath)
                ? GetRootPath()
                : Path.GetFullPath(Path.Combine(GetRootPathInternal(), relativePath)).ToUnixPath();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToUnixPath(this string path)
            => path?.Replace(Path.DirectorySeparatorChar, '/') ?? string.Empty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToPlatformPath(this string path)
            => path?.Replace('/', Path.DirectorySeparatorChar) ?? string.Empty;
    }
}
