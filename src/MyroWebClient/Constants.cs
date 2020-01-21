using System;

namespace MyroWebClient
{
    public static class Constants
    {
        public static Uri BaseUri = new Uri("https://online.myro.be");
        public static Uri PhpSessionUri = new Uri(BaseUri, "");
        public static Uri LoginDoUri = new Uri(BaseUri, "loginDo.php");
        public static Uri IndexUri = new Uri(BaseUri, "index.php");
        public static Uri LogbookUri = new Uri(BaseUri, "logbook.php");
    }
}
