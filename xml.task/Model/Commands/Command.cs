﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using xml.task.Model.RastrManager;
using System.Diagnostics;

namespace xml.task.Model.Commands
{
    public abstract class Command
    {
        public string Name;
        public bool Success;
        public string Summary;
        public string ResultMessage;

        protected Command(XElement xElement)
        {
            Name = xElement?.Attribute(@"name")?.Value;
        }

        public abstract void Perform();
    }

    public class DynamicStabilityCommand : Command
    {
        public string Rst;
        public string Scn;
        public string Folder;

        public DynamicStabilityCommand(XElement xElement) : base(xElement)
        {
            Rst = xElement?.Attribute(@"rst")?.Value;
            Scn = xElement?.Attribute(@"scn")?.Value;
            Folder = xElement?.Attribute(@"folder")?.Value;
        }

        public override void Perform()
        {
            if (Rst != null && Scn != null)
            {
                var rastr = new RastrOperations();
                rastr.Load(Rst, Scn);
                var result = rastr.RunDynamic();
                Success = result.IsSuccess;
                ResultMessage = $@"{Rst}{(char)9}{Scn}{(char)9}";
                var stabilityString = result.IsStable ? @"Устойчиво" : "Неустойчиво";
                ResultMessage += Success == false
                    ? @"Ошибка расчета"
                    : $@"{stabilityString}. Время расчета: {result.TimeReached}. {result.ResultMessage}";
            }
            if (Folder != null)
            {
                var rstFiles = Directory.GetFiles(Folder, @"*.rst");
                var scnFiles = Directory.GetFiles(Folder, @"*.scn");
                foreach (var rst in rstFiles)
                {
                    foreach (var scn in scnFiles)
                    {
                        var rastr = new RastrOperations();
                        rastr.Load(rst, scn);
                        var result = rastr.RunDynamic();
                        Console.WriteLine($@"{rst}   {scn}");
                    }
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
