namespace PdfService.Infrastructure.Data;

internal static class PdfBrandLayout
{
    public const string Brand500 = "#465fff";
    public const string Brand600 = "#3641f5";
    public const string Brand50 = "#ecf3ff";
    public const string Brand100 = "#dde9ff";
    public const string Brand200 = "#c2d6ff";
    public const string Brand25 = "#f2f7ff";
    public const string Gray25 = "#fcfcfd";
    public const string Gray50 = "#f9fafb";
    public const string Gray100 = "#f2f4f7";
    public const string Gray200 = "#e4e7ec";
    public const string Gray500 = "#667085";
    public const string Gray700 = "#344054";
    public const string Gray900 = "#101828";
    public const string Error50 = "#fef3f2";
    public const string Error500 = "#f04438";
    public const string Error700 = "#b42318";
    public const string Success50 = "#ecfdf3";
    public const string Success600 = "#039855";

    private const string IconSvg = """
        <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40" fill="none">
          <defs>
            <linearGradient id="nvIconBg" x1="8" y1="2" x2="34" y2="38" gradientUnits="userSpaceOnUse">
              <stop stop-color="#5B73FF"/>
              <stop offset="1" stop-color="#3641F5"/>
            </linearGradient>
          </defs>
          <rect width="40" height="40" rx="11" fill="url(#nvIconBg)"/>
          <circle cx="20" cy="14" r="1.7" fill="white"/>
          <circle cx="14.5" cy="17.5" r="1.35" fill="white"/>
          <circle cx="25.5" cy="17.5" r="1.35" fill="white"/>
          <circle cx="12.2" cy="23" r="1.2" fill="white"/>
          <circle cx="27.8" cy="23" r="1.2" fill="white"/>
          <circle cx="16.5" cy="27.2" r="1.2" fill="white"/>
          <circle cx="23.5" cy="27.2" r="1.2" fill="white"/>
          <circle cx="20" cy="21.2" r="1.85" fill="white"/>
          <path d="M20 14L14.5 17.5M20 14L25.5 17.5M14.5 17.5L12.2 23M25.5 17.5L27.8 23M12.2 23L16.5 27.2M27.8 23L23.5 27.2M16.5 27.2L20 21.2M23.5 27.2L20 21.2M14.5 17.5L20 21.2M25.5 17.5L20 21.2" stroke="white" stroke-width="1.15" stroke-linecap="round" opacity=".85"/>
          <circle cx="20" cy="14" r="4.2" stroke="white" stroke-width="1" fill="none" opacity=".75"/>
        </svg>
        """;

    public static string Document(
        string title,
        string kicker,
        string bodyHtml,
        string extraCss = "",
        bool wide = false)
    {
        var maxWidth = wide ? "820px" : "560px";

        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8" />
                <title>{{title}}</title>
                <style>
                    body {
                        margin: 0;
                        padding: 0;
                        font-family: Outfit, 'Segoe UI', Arial, sans-serif;
                        background: {{Brand25}};
                        color: {{Gray900}};
                    }
                    .wrapper { width: 100%; padding: 32px 16px; background: {{Brand25}}; }
                    .card {
                        max-width: {{maxWidth}};
                        margin: 0 auto;
                        background: #ffffff;
                        border: 1px solid {{Gray200}};
                        border-radius: 14px;
                        overflow: hidden;
                    }
                    .header {
                        background: {{Brand600}};
                        color: #ffffff;
                        padding: 22px 28px;
                    }
                    .header-table { width: 100%; border-collapse: collapse; }
                    .header-table td { vertical-align: middle; }
                    .brand-mark { width: 48px; }
                    .brand-name { font-size: 18px; font-weight: 700; letter-spacing: -0.04em; line-height: 1.1; }
                    .brand-name span { color: {{Brand200}}; }
                    .brand-kicker { margin-top: 4px; font-size: 12px; color: {{Brand100}}; }
                    .header-title { margin: 16px 0 0; font-size: 20px; font-weight: 700; }
                    .content { padding: 28px; }
                    .text { font-size: 14px; line-height: 1.6; color: {{Gray700}}; margin: 0 0 16px; }
                    .highlight { font-weight: 600; color: {{Gray900}}; }
                    .button {
                        display: inline-block;
                        padding: 12px 22px;
                        background: {{Brand500}};
                        color: #ffffff !important;
                        text-decoration: none;
                        border-radius: 8px;
                        font-weight: 600;
                        font-size: 14px;
                    }
                    .code-box {
                        margin: 20px 0;
                        padding: 20px 16px;
                        text-align: center;
                        background: {{Brand50}};
                        border: 1px solid {{Brand200}};
                        border-radius: 12px;
                    }
                    .code {
                        font-size: 32px;
                        font-weight: 700;
                        letter-spacing: 6px;
                        color: {{Brand600}};
                    }
                    .code-hint { margin-top: 8px; font-size: 12px; color: {{Gray500}}; }
                    .panel {
                        margin: 16px 0;
                        padding: 14px 16px;
                        background: {{Gray50}};
                        border: 1px solid {{Gray200}};
                        border-radius: 10px;
                        font-size: 14px;
                    }
                    .panel p { margin: 6px 0; }
                    .warning {
                        margin: 18px 0 0;
                        padding: 12px 14px;
                        background: {{Error50}};
                        border-left: 4px solid {{Error500}};
                        border-radius: 8px;
                        font-size: 12px;
                        color: {{Error700}};
                    }
                    .info {
                        margin-top: 16px;
                        font-size: 12px;
                        color: {{Gray500}};
                    }
                    .footer {
                        padding: 16px 28px;
                        background: {{Gray50}};
                        border-top: 1px solid {{Gray200}};
                        text-align: center;
                        font-size: 11px;
                        color: {{Gray500}};
                    }
                    {{extraCss}}
                </style>
            </head>
            <body>
                <div class="wrapper">
                    <div class="card">
                        <div class="header">
                            <table class="header-table">
                                <tr>
                                    <td class="brand-mark">{{IconSvg}}</td>
                                    <td>
                                        <div class="brand-name">Neuro<span>Vision</span>.AI</div>
                                        <div class="brand-kicker">{{kicker}}</div>
                                    </td>
                                </tr>
                            </table>
                            <div class="header-title">{{title}}</div>
                        </div>
                        <div class="content">
                            {{bodyHtml}}
                        </div>
                        <div class="footer">
                            NeuroVision.AI · AI-assisted neuroimaging diagnostics<br />
                            This document was generated automatically. Please do not reply.
                        </div>
                    </div>
                </div>
            </body>
            </html>
            """;
    }
}
