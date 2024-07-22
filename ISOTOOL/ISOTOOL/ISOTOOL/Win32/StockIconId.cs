﻿namespace Win32
{
    public enum StockIconId
    {
        DocNoAssoc = 0,          // document (blank page), no associated program
        DocAssoc = 1,            // document with an associated program
        Application = 2,         // generic application with no custom icon
        Folder = 3,              // folder (closed)
        FolderOpen = 4,          // folder (open)
        Drive525 = 5,            // 5.25" floppy disk drive
        Drive35 = 6,             // 3.5" floppy disk drive
        DriveRemove = 7,         // removable drive
        DriveFixed = 8,          // fixed (hard disk) drive
        DriveNet = 9,            // network drive
        DriveNetDisabled = 10,   // disconnected network drive
        DriveCd = 11,            // CD drive
        DriveRam = 12,           // RAM disk drive
        World = 13,              // entire network
        Server = 15,             // a computer on the network
        Printer = 16,            // printer
        MyNetwork = 17,          // My network places
        Find = 22,               // Find
        Help = 23,               // Help
        Share = 28,              // overlay for shared items
        Link = 29,               // overlay for shortcuts to items
        SlowFile = 30,           // overlay for slow items
        Recycler = 31,           // empty recycle bin
        RecyclerFull = 32,       // full recycle bin
        MediaCdAudio = 40,       // Audio CD Media
        Lock = 47,               // Security lock
        AutoList = 49,           // AutoList
        PrinterNet = 50,         // Network printer
        ServerShare = 51,        // Server share
        PrinterFax = 52,         // Fax printer
        PrinterFaxNet = 53,      // Networked Fax Printer
        PrinterFile = 54,        // Print to File
        Stack = 55,              // Stack
        MediaSvcd = 56,          // SVCD Media
        StuffedFolder = 57,      // Folder containing other items
        DriveUnknown = 58,       // Unknown drive
        DriveDvd = 59,           // DVD Drive
        MediaDvd = 60,           // DVD Media
        MediaDvdRam = 61,        // DVD-RAM Media
        MediaDvdRw = 62,         // DVD-RW Media
        MediaDvdR = 63,          // DVD-R Media
        MediaDvdRom = 64,        // DVD-ROM Media
        MediaCdAudioPlus = 65,   // CD+ (Enhanced CD) Media
        MediaCdRw = 66,          // CD-RW Media
        MediaCdR = 67,           // CD-R Media
        MediaCdBurn = 68,        // Burning CD
        MediaBlankCd = 69,       // Blank CD Media
        MediaCdRom = 70,         // CD-ROM Media
        AudioFiles = 71,         // Audio files
        ImageFiles = 72,         // Image files
        VideoFiles = 73,         // Video files
        MixedFiles = 74,         // Mixed files
        FolderBack = 75,         // Folder back
        FolderFront = 76,        // Folder front
        Shield = 77,             // Security shield. Use for UAC prompts only.
        Warning = 78,            // Warning
        Info = 79,               // Informational
        Error = 80,              // Error
        Key = 81,                // Key / Secure
        Software = 82,           // Software
        Rename = 83,             // Rename
        Delete = 84,             // Delete
        MediaAudioDvd = 85,      // Audio DVD Media
        MediaMovieDvd = 86,      // Movie DVD Media
        MediaEnhancedCd = 87,    // Enhanced CD Media
        MediaEnhancedDvd = 88,   // Enhanced DVD Media
        MediaHdDvd = 89,         // HD-DVD Media
        MediaBluRay = 90,        // BluRay Media
        MediaVcd = 91,           // VCD Media
        MediaDvdPlusR = 92,      // DVD+R Media
        MediaDvdPlusRw = 93,     // DVD+RW Media
        DesktopPc = 94,          // desktop computer
        MobilePc = 95,           // mobile computer (laptop/notebook)
        Users = 96,              // users
        MediaSmartMedia = 97,    // Smart Media
        MediaCompactFlash = 98,  // Compact Flash
        DeviceCellPhone = 99,    // Cell phone
        DeviceCamera = 100,      // Camera
        DeviceVideoCamera = 101, // Video camera
        DeviceAudioPlayer = 102, // Audio player
        NetworkConnect = 103,    // Connect to network
        Internet = 104,          // Internet
        ZipFile = 105,           // ZIP file
        Settings = 106,          // Settings
                                 // 107-131 are internal Vista RTM icons
                                 // 132-159 for SP1 icons
        DriveHdDvd = 132,        // HDDVD Drive (all types)
        DriveBd = 133,           // BluRay Drive (all types)
        MediaHdDvdRom = 134,     // HDDVD-ROM Media
        MediaHdDvdR = 135,       // HDDVD-R Media
        MediaHdDvdRam = 136,     // HDDVD-RAM Media
        MediaBdRom = 137,        // BluRay ROM Media
        MediaBdR = 138,          // BluRay R Media
        MediaBdRe = 139,         // BluRay RE Media (Rewriable and RAM)
        ClusteredDrive = 140,    // Clustered disk
                                 // 160+ are for Windows 7 icons
        MaxIcons = 181
    }
}