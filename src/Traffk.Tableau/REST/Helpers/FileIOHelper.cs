﻿using System;
using System.Collections.Generic;
using System.IO;
using Traffk.Tableau.REST.Models;
using ILogger = Serilog.ILogger;

/// <summary>
/// Helper for file io
/// </summary>
static class FileIOHelper
{

    /// <summary>
    /// Ensures the specified path exists 
    /// </summary>
    public static void CreatePathIfNeeded(string localPath)
    {
        if (Directory.Exists(localPath)) return;
        Directory.CreateDirectory(localPath);
    }

    /// <summary>
    /// Characters we will do search/replace on to make a Tableau Server content name safe for the Windows file system
    /// </summary>
    private static List<KeyValuePair<string, string>> s_generateSwapPairs;
    private static List<KeyValuePair<string, string>> GetFilenameSwapValues()
    {
        //Already exists? Return it.
        List<KeyValuePair<string, string>>  outValues = s_generateSwapPairs;
        if(outValues != null) return outValues;

        //Create a new one
        outValues = new List<KeyValuePair<string, string>>();
        outValues.Add(new KeyValuePair<string, string>("\\", "{{!x5c}}"));
        outValues.Add(new KeyValuePair<string, string>("/",  "{{!x2f}}"));
        outValues.Add(new KeyValuePair<string, string>("*",  "{{!x42}}"));
        outValues.Add(new KeyValuePair<string, string>("?",  "{{!x3f}}"));
        outValues.Add(new KeyValuePair<string, string>(":",  "{{!x3a}}"));
        outValues.Add(new KeyValuePair<string, string>("|",  "{{!x7c}}"));
        outValues.Add(new KeyValuePair<string, string>("\"", "{{!x22}}"));
        outValues.Add(new KeyValuePair<string, string>(">",  "{{!x3e}}"));
        outValues.Add(new KeyValuePair<string, string>("<",  "{{!x3c}}"));
        s_generateSwapPairs = outValues; //Store it so we don't need to create it again

        return outValues;
    }

    /// <summary>
    /// Take any text in an munge it into name that is valid on windows
    /// </summary>
    public static string GenerateWindowsSafeFilename(string fileNameIn)
    {
        string fileNameOut = fileNameIn;
        //"{{!x" -- Is an an uncommon sequence, but we should escape it, since this is what we use as our escape sequence for special chars
        fileNameOut = fileNameOut.Replace("{{!x", "{{!x}"); //Terminate every "{{!x" with "}" (so this is unique from any of our other escape sequences whic hahve hex-numbers)

        var specialCharsSet = GetFilenameSwapValues();
        foreach(var thisCharSwap in specialCharsSet)
        {
            fileNameOut = fileNameOut.Replace(thisCharSwap.Key, thisCharSwap.Value);
        }

        return fileNameOut;
    }

    /// <summary>
    /// Reverses 'GenerateWindowsSafeFilename_Reverse'
    /// </summary>
    public static string Undo_GenerateWindowsSafeFilename(string fileNameIn)
    {
        string fileNameOut = fileNameIn;
        //Small HACK: This assumes that no valid file name contains a "{{!x" -- which would seem an uncommon sequence
        var specialCharsSet = GetFilenameSwapValues();
        foreach (var thisCharSwap in specialCharsSet)
        {
            fileNameOut = fileNameOut.Replace(thisCharSwap.Value, thisCharSwap.Key);
        }

        //Undo the original escape sequence for "{{!x", which we changed to "{{!x}"
        fileNameOut = fileNameOut.Replace("{{!x}", "{{!x");

        return fileNameOut;
    }

    /// <summary>
    /// Creates a high-probabilty-unique path based on the current date-time
    /// </summary>
    public static string PathDateTimeSubdirectory(string basePath, bool createDirectory, string newDirectoryPrefix = "", Nullable<DateTime> when = null)
    {
        //Subdirectory name
        DateTime now;
        if(when.HasValue)
        {
            now = when.Value;
        }
        else
        {
            now = DateTime.Now;
        }

        string subDirectory = now.Year.ToString() + "-" + now.Month.ToString("00") + "-" + now.Day.ToString("00") + "-" + now.Hour.ToString("00") + now.Minute.ToString("00") + "-" + now.Second.ToString("00");
        if(!string.IsNullOrWhiteSpace(newDirectoryPrefix))
        {
            subDirectory = newDirectoryPrefix + subDirectory;
        }

        //Combined path
        string fullPathToDateTime = Path.Combine(basePath, subDirectory);
        //Create if specified
        if (createDirectory)
        {
            CreatePathIfNeeded(fullPathToDateTime);
        }
        return fullPathToDateTime;
    }


    /// <summary>
    /// Gives us a high probability unqique file name
    /// </summary>
    public static string FilenameWithDateTimeUnique(string baseName, Nullable<DateTime> when = null)
    {
        string rootName = Path.GetFileNameWithoutExtension(baseName);
        string extension = Path.GetExtension(baseName);

        //Subdirectory name

        DateTime now;
        if(when.HasValue)
        {
            now = when.Value;
        }
        else
        {
            now = DateTime.Now;
        }

        string subNameDateTime = now.Year.ToString() + "-" + now.Month.ToString("00") + "-" + now.Day.ToString("00") + "-" + now.Hour.ToString("00") + now.Minute.ToString("00") + "-" + now.Second.ToString("00");

        //Combined path
        return rootName + "_" + subNameDateTime + extension;
    }

    /// <summary>
    /// If we have Project Mapping information, generate a project based path for the download
    /// </summary>
    public static string EnsureProjectBasedPath(string basePath, IProjectsList projectList, IHasProjectId project, ILogger logger)
    {
        //If we have no project list to do lookups in then just return the base path
        if (projectList == null) return basePath;

        //Look up the project name
        var projWithId = projectList.FindProjectWithId(project.ProjectId);
        if (projWithId == null)
        {
            logger.Error("Project not found with id " + project.ProjectId);
            return basePath;
        }

        //Turn the project name into a directory name
        var safeDirectoryName = GenerateWindowsSafeFilename(projWithId.Name);

        var pathWithProject = Path.Combine(basePath, safeDirectoryName);
        //If needed, create the directory
        if (!Directory.Exists(pathWithProject))
        {
            Directory.CreateDirectory(pathWithProject);
        }

        return pathWithProject;
    }

}
