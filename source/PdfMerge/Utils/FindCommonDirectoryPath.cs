﻿// From:
// https://stackoverflow.com/questions/24866683/find-common-parent-path-in-list-of-files-and-directories

namespace PdfMerge
{
    using System.Collections.Generic;

    public class FindCommonDirectoryPath
    {
        public static string FindCommonPath(List<string> paths)
        {
            string firstPath = paths[0];
            bool same = true;

            int i = 0;

            string commonPath = string.Empty;

            while (same && i < firstPath.Length)
            {
                for (int p = 1; p < paths.Count && same; p++)
                {
                    same = firstPath[i] == paths[p][i];
                }

                if (same)
                {
                    commonPath += firstPath[i];
                }

                i++;
            }

            return commonPath;
        }
    }
}