using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI
{
    public enum ComicFormat
    {
        Comic,
        Magazine,
        TradePaperback,
        Hardcover,
        Digest,
        GraphicNovel,
        DigitalComic,
        InfiniteComic
    }

    public enum ComicFormatType
    {
        Comic,
        Collection
    }

    public enum SeriesType 
    {
        Collection,
        OneShot,
        Limited,
        Ongoing
    }

    public enum DateDescriptor
    {
        LastWeek,
        ThisWeek,
        NextWeek,
        ThisMonth
    }

    public enum OrderBy
    {
        FocDate,
        FocDateDesc,
        OnSaleDate,
        OnSaleDateDesc,
        Title,
        TitleDesc,
        IssueNumber,
        IssueNumberDesc,
        Modified,
        ModifiedDesc,
        Id,
        IdDesc,
        Name,
        NameDesc,
        StartDate,
        StartDateDesc,
        FirstName,
        FirstNameDesc,
        MiddleName,
        MiddleNameDesc,
        LastName,
        LastNameDesc,
        Suffix,
        SuffixDesc,
        StartYear,
        StartYearDesc
    }

    public static class EnumExtensions
    {
        public static string ToParameter(this ComicFormat Format)
        {
            switch (Format)
            {
                case ComicFormat.Comic:
                    return "comic";
                case ComicFormat.Digest:
                    return "digest";
                case ComicFormat.DigitalComic:
                    return "digital comic";
                case ComicFormat.GraphicNovel:
                    return "graphic novel";
                case ComicFormat.Hardcover:
                    return "hardcover";
                case ComicFormat.InfiniteComic:
                    return "infinite comic";
                case ComicFormat.Magazine:
                    return "magazine";
                case ComicFormat.TradePaperback:
                    return "trade paperback";
                default:
                    return String.Empty;
            }
        }

        public static string ToParameter(this ComicFormatType FormatType)
        {
            switch (FormatType)
            {
                case ComicFormatType.Collection:
                    return "collection";
                case ComicFormatType.Comic:
                    return "comic";
                default:
                    return String.Empty;
            }
        }

        public static string ToParameter(this SeriesType Type)
        {
            switch (Type)
            {
                case SeriesType.Collection:
                    return "collection";
                case SeriesType.OneShot:
                    return "one shot";
                case SeriesType.Limited:
                    return "limited";
                case SeriesType.Ongoing:
                    return "ongoing";
                default:
                    return String.Empty;
            }
        }

        public static string ToParameter(this DateDescriptor Descriptor)
        {
            switch (Descriptor)
            {
                case DateDescriptor.LastWeek:
                    return "lastWeek";
                case DateDescriptor.NextWeek:
                    return "nextWeek";
                case DateDescriptor.ThisMonth:
                    return "thisMonth";
                case DateDescriptor.ThisWeek:
                    return "thisWeek";
                default:
                    return String.Empty;
            }
        }

        public static string ToParameter(this OrderBy Order)
        {
            switch(Order)
            {
                case OrderBy.FocDate:
                    return "focDate";
                case OrderBy.FocDateDesc:
                    return "-focDate";
                case OrderBy.IssueNumber:
                    return "issueNumber";
                case OrderBy.IssueNumberDesc:
                    return "-issueNumber";
                case OrderBy.Modified:
                    return "modified";
                case OrderBy.ModifiedDesc:
                    return "-modified";
                case OrderBy.OnSaleDate:
                    return "onsaleDate";
                case OrderBy.OnSaleDateDesc:
                    return "-onsaleDate";
                case OrderBy.Title:
                    return "title";
                case OrderBy.TitleDesc:
                    return "-title";
                case OrderBy.Id:
                    return "id";
                case OrderBy.IdDesc:
                    return "-id";
                case OrderBy.Name:
                    return "name";
                case OrderBy.NameDesc:
                    return "-name";
                case OrderBy.StartDate:
                    return "startDate";
                case OrderBy.StartDateDesc:
                    return "-startDate";
                case OrderBy.FirstName:
                    return "firstName";
                case OrderBy.FirstNameDesc:
                    return "-firstName";
                case OrderBy.MiddleName:
                    return "middleName";
                case OrderBy.MiddleNameDesc:
                    return "-middleName";
                case OrderBy.LastName:
                    return "lastName";
                case OrderBy.LastNameDesc:
                    return "-lastName";
                case OrderBy.Suffix:
                    return "suffix";
                case OrderBy.SuffixDesc:
                    return "-suffix";
                case OrderBy.StartYear:
                    return "startYear";
                case OrderBy.StartYearDesc:
                    return "-startYear";
                default:
                    return String.Empty;
            }
        }
    }
}
