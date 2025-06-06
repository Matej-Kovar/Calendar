using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar.Resources;

namespace Calendar
{
    [AcceptEmptyServiceProvider]
    [ContentProperty(nameof(Key))]
    internal class TranslationExtension : IMarkupExtension
    {
        public string? Key { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Key == null)
                return "";

            return Strings.ResourceManager.GetString(Key, CultureInfo.CurrentUICulture) ?? Key;
        }
    }
}
