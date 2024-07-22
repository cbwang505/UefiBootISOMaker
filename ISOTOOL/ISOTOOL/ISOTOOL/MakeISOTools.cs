using IMAPI2.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using Win32;

namespace ISOTOOL
{
    internal class FileItem
    {
        private const long SECTOR_SIZE = 2048;

        private readonly long fileLength = 0;

        public FileItem(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The file added to FileItem was not found!", path);
            }

            Path = path;

            var fileInfo = new FileInfo(Path);
            DisplayName = fileInfo.Name;
            fileLength = fileInfo.Length;
        }

        public long SizeOnDisc => (fileLength > 0) ? ((fileLength / SECTOR_SIZE) + 1) * SECTOR_SIZE : 0;

        //public string FriendlySize => Utilities.GetBytesReadable(SizeOnDisc);

        public long FileCount => 1;

        public string Type => "File";

        public string Path { get; }


        public string DisplayName { get; }

        public bool AddToFileSystem(IFsiDirectoryItem rootItem, string basePath = "")
        {


            try
            {
                var stream = new ManagedIStream(File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read));
                rootItem.AddFile(System.IO.Path.Combine(basePath, DisplayName), stream);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public class MakeISOTools
    {


        // private CancellationTokenSource tokenSource;

        private readonly object fileListLockObject = new object();
        private readonly List<FileItem> fileList = new List<FileItem>();
        // public ICollectionView FileList { get; }


        private long totalBytesWritten;
        public long TotalBytesWritten
        {
            get => totalBytesWritten;
            set => totalBytesWritten = value;
        }



        private bool addingFiles;
        public bool AddingFiles
        {
            get => addingFiles;
            set
            {
                addingFiles = value;

            }
        }

        private bool writingIso;
        public bool WritingIso
        {
            get => writingIso;
            set
            {
                writingIso = value;

            }
        }

        private long fileCount;
        public long FileCount
        {
            get => fileCount;
            set => fileCount = value;
        }

        private long filesStaged;
        public long FilesStaged
        {
            get => filesStaged;
            set => filesStaged = value;
        }

        private string volumeName;
        public string VolumeName
        {
            get => volumeName;
            set => volumeName = value;
        }

        private string biosBootFile;
        public string BiosBootFile
        {
            get => biosBootFile;
            set => biosBootFile = value;
        }

        private string uefiBootFile;
        public string UefiBootFile
        {
            get => uefiBootFile;
            set => uefiBootFile = value;
        }

        public MakeISOTools()
        {
            BiosBootFile = "";
            UefiBootFile = "";


        }

        /*private bool addFolder(string path)
        {
            if (isDuplicate(path, true))
            {
                return false;
            }

            var item = new DirectoryItem(path);

            fileList.Add(item);
            FileCount += item.FileCount;
            //TotalSpaceRequired += item.SizeOnDisc;

            return true;
        }*/

        private bool addFile(string path)
        {
            if (isDuplicate(path, false))
            {
                return false;
            }

            var item = new FileItem(path);

            fileList.Add(item);
            FileCount += item.FileCount;
            //  TotalSpaceRequired += item.SizeOnDisc;

            return true;
        }

        private List<string> addFiles(IEnumerable<string> paths)
        {
            var duplicates = new List<string>();

            foreach (var path in paths)
            {
                if (!addFile(path))
                {
                    duplicates.Add(path);
                }
            }

            return duplicates;
        }

        public void writeIso(string path,string BootFile)
        {
            ManagedIStream uefiBootFilestm = null;
            ManagedIStream biosBootFilestm = null;
            FilesStaged = 0;
            TotalBytesWritten = 0;
            // WriterStatus = WriterStatus.Staging;
            Console.WriteLine(path);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var iso = new MsftFileSystemImage();
            iso.Update += isoUpdate;
            iso.ChooseImageDefaultsForMediaType(IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DISK);
            //iso.FileSystemsToCreate = FsiFileSystems.FsiFileSystemUDF;
            iso.FileSystemsToCreate = FsiFileSystems.FsiFileSystemISO9660;
            iso.UDFRevision = 0x250;
            if (!string.IsNullOrWhiteSpace(VolumeName))
            {
                iso.VolumeName = VolumeName;
            }

            try
            {
                var bootImageOptions = new List<object>();

                if (File.Exists(BiosBootFile))
                {
                    var biosBootOptions = new BootOptions
                    {
                        Manufacturer = "Herohtar",
                        PlatformId = PlatformId.PlatformX86,
                        Emulation = EmulationType.EmulationNone
                    };

                    biosBootFilestm = new ManagedIStream(File.OpenRead(BiosBootFile));
                    biosBootOptions.AssignBootImage(biosBootFilestm);
                    bootImageOptions.Add(biosBootOptions);
                }

               
                 UefiBootFile = BootFile;
                if (File.Exists(UefiBootFile))
                {
                    var uefiBootOptions = new BootOptions
                    {
                        Manufacturer = "Herohtar",
                        PlatformId = PlatformId.PlatformEFI,
                        Emulation = EmulationType.EmulationHardDisk
                    };

                    uefiBootFilestm = new ManagedIStream(File.OpenRead(UefiBootFile));
                    uefiBootOptions.AssignBootImage(uefiBootFilestm);
                    bootImageOptions.Add(uefiBootOptions);
                }

                if (bootImageOptions.Count > 0)
                {
                    iso.UDFRevision = 0x150; // Boot images don't work with later revisions
                    ((IFileSystemImage2)iso).BootImageOptionsArray = bootImageOptions.ToArray();
                }


                /*foreach (var item in fileList)
                {
                    item.AddToFileSystem(iso.Root, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                }*/


                var resultImage = iso.CreateResultImage();
                var imageStream = resultImage.ImageStream;

                imageStream.Stat(out var stat, 0x01);
                long TotalBytesToWrite = stat.cbSize;
                // WriterStatus = WriterStatus.Writing;

                var bytesReadPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(long)));
                var bytesRead = 0L;
                Marshal.WriteInt64(bytesReadPtr, bytesRead);

                try
                {
                    using (var outStream = File.Create(path))
                    {
                        var buffer = new byte[1024 * 1024];

                        do
                        {


                            imageStream.Read(buffer, buffer.Length, bytesReadPtr);
                            bytesRead = Marshal.ReadInt64(bytesReadPtr);
                            TotalBytesWritten += bytesRead;
                            outStream.Write(buffer, 0, (int)bytesRead);
                        } while (bytesRead > 0);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(bytesReadPtr);
                }
            }
            finally
            {
                // For any file >= 128 kB, the streams are not automatically cleaned up, meaning their handles will remain open indefinitely
                // This results in all files that were added to the ISO being locked if you try to modify them while the program is still running
                disposeStreams(iso.Root);

                iso.Update -= isoUpdate;
                if (uefiBootFilestm != null)
                {
                    uefiBootFilestm.Dispose();
                }
                if (biosBootFilestm != null)
                {
                    biosBootFilestm.Dispose();
                }
                Console.WriteLine("CreateResultImage :=>"+path);
            }
        }

        private void disposeStreams(IFsiDirectoryItem directory)
        {
            // Go through all the file items and dispose their streams
            var itemEnumerator = directory.GetEnumerator();
            while (itemEnumerator.MoveNext())
            {
                var currentItem = itemEnumerator.Current;
                if (currentItem is IFsiFileItem fileItem)
                {
                    // All files are added using ManagedIStream; however, any file under 128 kB is treated differently by the file system image
                    // The small files are changed to be added using some COM implementation of IStream which gets cleaned up automatically
                    if (fileItem.Data is ManagedIStream dataStream)
                    {
                        dataStream.Dispose();
                    }
                }
                else if (currentItem is IFsiDirectoryItem directoryItem)
                {
                    disposeStreams(directoryItem);
                }
            }
        }

        private bool isDuplicate(string path, bool isFolder)
        {
            var name = Path.GetFileName(path);
            return fileList.Where(item => item is FileItem).Any(item => item.DisplayName.Equals(name));

        }


        private void isoUpdate(object sender, string currentFile, int copiedSectors, int totalSectors)
        {
            if (copiedSectors == totalSectors)
            {
                FilesStaged++;
            }

            Console.WriteLine("currentFile:=>" + currentFile + ",copiedSectors:=>" + copiedSectors + ",totalSectors:=>" + totalSectors);
        }


    }
}