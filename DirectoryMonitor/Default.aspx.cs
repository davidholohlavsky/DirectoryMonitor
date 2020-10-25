using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Security.Permissions;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace DirectoryMonitor
{
  public partial class Default : System.Web.UI.Page
  {

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected void Page_Load(object sender, EventArgs e)
    {
      try
      {
        if (IsPostBack)
        {
          List<string> fileEntries = new List<string>();
          List<string> subdirectoryEntries = new List<string>();
          ProcessDirectory(TextBox1.Text, fileEntries, subdirectoryEntries);
          //string[] fileEntries = Directory.GetFiles(TextBox1.Text);

          List<string> newFilesNamesList = new List<string>();
          List<string> newFilesLastWriteTimeList = new List<string>();
          List<int> newVersionOfFileList = new List<int>();
          List<string> newSubDirectoryNamesList = new List<string>();

          foreach (string filePath in fileEntries)
          {
            FileInfo fileInfo = new FileInfo(filePath);
            string fileName = filePath.Replace(TextBox1.Text, "");
            newFilesNamesList.Add($"{fileName}");
            newFilesLastWriteTimeList.Add($"{fileInfo.LastWriteTime}");
            newVersionOfFileList.Add(1);
          }
          foreach (string subDirectoryPath in subdirectoryEntries)
          {
            string subDirectoryName = subDirectoryPath.Replace(TextBox1.Text, "");
            newSubDirectoryNamesList.Add($"{subDirectoryName}");
          }
          if (ViewState["filesNames"] == null)
          {
            Label1.Text = "nový adresář, žádné změny";
          }
          else
          {
            string oldFilesNames = ViewState["filesNames"].ToString();
            string oldFilesLastWriteTime = ViewState["filesLastWriteTime"].ToString();
            string oldFileVersion = ViewState["versionOfFiles"].ToString();
            string oldSubDirectoryNames = ViewState["subDirectoryNames"].ToString();
            List<string> oldFilesNamesList = oldFilesNames.Split(';').ToList();
            List<string> oldFilesLastWriteTimeList = oldFilesLastWriteTime.Split(';').ToList();
            List<int> oldFileVersionList = oldFileVersion.Split(';').ToList().ConvertAll(int.Parse);
            List<string> oldSubDirectoryNamesList = oldSubDirectoryNames.Split(';').ToList();
            StringBuilder sb = new StringBuilder();
            foreach (string oldFile in oldFilesNamesList)
            {
              if (newFilesNamesList.Contains(oldFile))
              {
                int indexNameNew = newFilesNamesList.IndexOf(oldFile);
                int indexNameOld = oldFilesNamesList.IndexOf(oldFile);
                // no changes
                if (newFilesLastWriteTimeList[indexNameNew].Equals(oldFilesLastWriteTimeList[indexNameOld]))
                {
                  Label1.Text = "žádná změna";
                  newVersionOfFileList = oldFileVersionList;
                }
                else
                {
                  int newVersionOfFile = oldFileVersionList[indexNameOld] + 1;
                  newVersionOfFileList[indexNameNew] = newVersionOfFile;
                  sb.Append($"[M] {newFilesNamesList[indexNameNew]} (ve verzi {newVersionOfFile})<br />");
                }
              }
              else
              {
                sb.Append($"[D] {oldFile}<br />");
              }
            }
            foreach (string newFile in newFilesNamesList)
            {
              if (!oldFilesNamesList.Contains(newFile))
              {
                sb.Append($"[A] {newFile}<br />");
              }
            }
            foreach (string oldSubDirectory in oldSubDirectoryNamesList)
            {
              if (!newSubDirectoryNamesList.Contains(oldSubDirectory))
              {
                sb.Append($"[D] {oldSubDirectory}(directory)<br />");
              }
            }
            foreach (string newDirectory in newSubDirectoryNamesList)
            {
              if (!oldSubDirectoryNamesList.Contains(newDirectory))
              {
                sb.Append($"[A] {newDirectory}(directory)<br />");
              }
            }
            if (!String.IsNullOrEmpty(sb.ToString()))
            {
              sb.Append("<br />");
              sb.Append("[A] = added (nový soubor)<br />");
              sb.Append("[M] = modified (změněný soubor)<br />");
              sb.Append("[D] = deleted (odstraněný soubor)<br />");
              Label1.Text = sb.ToString();
            }
          }
          string filesNames = String.Join(";", newFilesNamesList);
          string filesLastWriteTime = String.Join(";", newFilesLastWriteTimeList);
          string versionOfFile = String.Join(";", newVersionOfFileList);
          string subDirectoryNames = String.Join(";", newSubDirectoryNamesList);
          ViewState["filesNames"] = filesNames;
          ViewState["filesLastWriteTime"] = filesLastWriteTime;
          ViewState["versionOfFiles"] = versionOfFile;
          ViewState["subDirectoryNames"] = subDirectoryNames;
        }
      }
      catch (Exception ex)
      {
        log.Error("This is my error", ex);
      }
    }
    public static void ProcessDirectory(string targetDirectory, List<string> fileEntries, List<string> subdirectoryEntries)
    {
      //system.stackoverflowexception
      fileEntries.AddRange(Directory.GetFiles(targetDirectory).ToList());
      subdirectoryEntries.AddRange(Directory.GetDirectories(targetDirectory).ToList());
      foreach(string subdirectory in subdirectoryEntries)
      {
        ProcessDirectory(subdirectory, fileEntries, subdirectoryEntries);
      }
    }
  }
}