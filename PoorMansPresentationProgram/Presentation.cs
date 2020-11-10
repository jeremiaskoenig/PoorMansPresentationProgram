using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace PoorMansPresentationProgram
{
    class Presentation : IDisposable
    {
        private readonly string tempDir;
        private readonly Regex slideRegex = new Regex(@"\d+_?.*", RegexOptions.Compiled);

        public Presentation(FileInfo file)
        {
            var tempRootDir = Path.GetTempPath();
            var tempDirName = $"pmpp_{Guid.NewGuid():N}";

            while (Directory.Exists(Path.Combine(tempRootDir, tempDirName)))
            {
                tempDirName = $"pmpp_{Guid.NewGuid():N}";
            }
            tempDir = Path.Combine(tempRootDir, tempDirName);

            ZipFile.ExtractToDirectory(file.FullName, tempDir);


            var archive = new DirectoryInfo(tempDir);
            var masterFile = archive.GetFiles("master.png").FirstOrDefault();
            var files = archive.GetFiles().Where(x => slideRegex.IsMatch(x.Name)).OrderBy(x => x.Name);

            if (masterFile != null)
            {
                Master = ToMemory(Image.FromFile(masterFile.FullName));
            }

            foreach (var slideFile in files)
            {
                Slides.AddLast(() => ToMemory(Image.FromFile(slideFile.FullName)));
            }
            currentNode = Slides.First;
            UpdateSlide();
        }

        private Image ToMemory(Image image)
        {
            Image copy;
            using (var stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                copy = Image.FromStream(stream);
            }
            image.Dispose();
            return copy;
        }

        public Image Master { get; }

        public LinkedList<Func<Image>> Slides { get; } = new LinkedList<Func<Image>>();

        private LinkedListNode<Func<Image>> currentNode;

        public bool IsAtEndOfPresentation => currentNode == null;

        public Image CurrentSlide { get; private set; }

        public void Next()
        {
            if (currentNode != null)
            {
                currentNode = currentNode.Next;
            }
            UpdateSlide();
        }

        public void Previous()
        {
            if (currentNode != null)
            {
                currentNode = currentNode.Previous;
            }
            UpdateSlide();
        }

        void UpdateSlide()
        {
            if (CurrentSlide != null)
            {
                CurrentSlide.Dispose();
                CurrentSlide = null;
            }
            if (currentNode != null)
            {
                CurrentSlide = currentNode.Value();
            }
        }

        public void Dispose()
        {
            Directory.Delete(tempDir, true);
        }
    }
}
