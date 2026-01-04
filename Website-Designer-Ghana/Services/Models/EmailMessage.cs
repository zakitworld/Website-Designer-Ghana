namespace Website_Designer_Ghana.Services.Models;

public class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public string ToName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public List<string>? CcList { get; set; }
    public List<string>? BccList { get; set; }
}
