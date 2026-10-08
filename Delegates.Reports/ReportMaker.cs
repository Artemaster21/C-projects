using System.Text;

namespace Delegates.Reports;

public class ReportMaker
{
    private readonly Func<string, string> _makeCaption;
    private readonly Func<string> _beginList;
    private readonly Func<string> _endList;
    private readonly Func<string, string, string> _makeItem;
    private readonly Func<IEnumerable<double>, object> _makeStatistics;
    private readonly string _caption;

    public ReportMaker(
        string caption,
        Func<string, string> makeCaption,
        Func<string> beginList,
        Func<string> endList,
        Func<string, string, string> makeItem,
        Func<IEnumerable<double>, object> makeStatistics)
    {
        _caption = caption;
        _makeCaption = makeCaption;
        _beginList = beginList;
        _endList = endList;
        _makeItem = makeItem;
        _makeStatistics = makeStatistics;
    }

    public string MakeReport(IEnumerable<Measurement> measurements)
    {
        var data = measurements.ToList();
        var result = new StringBuilder();
        result.Append(_makeCaption(_caption));
        result.Append(_beginList());
        result.Append(_makeItem("Temperature", _makeStatistics(data.Select(z => z.Temperature)).ToString()));
        result.Append(_makeItem("Humidity", _makeStatistics(data.Select(z => z.Humidity)).ToString()));
        result.Append(_endList());
        return result.ToString();
    }
}

public static class ReportMakerHelper
{
    private static readonly Func<string, string> HtmlCaption = caption => $"<h1>{caption}</h1>";
    private static readonly Func<string> HtmlBeginList = () => "<ul>";
    private static readonly Func<string> HtmlEndList = () => "</ul>";
    private static readonly Func<string, string, string> HtmlItem = (type, entry) => $"<li><b>{type}</b>: {entry}";
    private static readonly Func<string, string> MarkdownCaption = caption => $"## {caption}\n\n";
    private static readonly Func<string> MarkdownBeginList = () => "";
    private static readonly Func<string> MarkdownEndList = () => "";
    private static readonly Func<string, string, string> MarkdownItem = (type, entry) => $" * **{type}**: {entry}\n\n";

    private static readonly Func<IEnumerable<double>, object> MeanAndStd = data =>
    {
        var list = data.ToList();
        var mean = list.Average();
        var std = Math.Sqrt(list.Select(z => Math.Pow(z - mean, 2)).Sum() / (list.Count - 1));
        return new MeanAndStd { Mean = mean, Std = std };
    };

    private static readonly Func<IEnumerable<double>, object> Median = data =>
    {
        var list = data.OrderBy(z => z).ToList();
        if (list.Count % 2 == 0)
            return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
        return list[list.Count / 2];
    };

    public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data) =>
        new ReportMaker("Mean and Std", HtmlCaption, HtmlBeginList, HtmlEndList, HtmlItem, MeanAndStd).MakeReport(data);

    public static string MedianMarkdownReport(IEnumerable<Measurement> data) =>
        new ReportMaker("Median", MarkdownCaption, MarkdownBeginList, MarkdownEndList, MarkdownItem, Median).MakeReport(data);

    public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> data) =>
        new ReportMaker("Mean and Std", MarkdownCaption, MarkdownBeginList, MarkdownEndList, MarkdownItem, MeanAndStd).MakeReport(data);

    public static string MedianHtmlReport(IEnumerable<Measurement> data) =>
        new ReportMaker("Median", HtmlCaption, HtmlBeginList, HtmlEndList, HtmlItem, Median).MakeReport(data);
}